//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	ゲーム管理関係
//
//	Copyright(C)2016 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//← List<XXX> 使うときに要る




/// 駒の種類
public	enum	PieceKind {
	 SideA		//A側の駒
	,SideB		//B側の駒
	,EnumMax
}
/// <summary>
/// グリッド情報
/// </summary>
public class GridData {
	public	GridBase	grid		= null;					//グリッドクラス
	public	int			x			= 0;					//座標X
	public	int			y			= 0;					//座標Y
	public	PieceKind	pieceKind	= PieceKind.EnumMax;	//どっちのプレーヤーのものか
	public	int			prio		= -1;					//優先度
	public	int			getNum		= 0;					//取得できる枚数
}




/// <summary>
/// ゲーム管理クラス
/// </summary>
public class Game : MonoBehaviour {



	public					GameObject		gridObj			= null;								//グリッドのプレハブ

	public	static readonly	int				fieldGridMaxX	= 8;								//マス目
	public	static readonly	int				fieldGridMaxY	= 8;								//マス目

	private	static readonly	Vector3			fieldPos		= new Vector3( -3.5f, 0,  3.5f);	//配置時の開始位置
	private	static readonly	Vector3			fieldOffset		= new Vector3(  1.0f, 0, -1.0f);	//配置時の位置のずれ

	private					GridData[][]	gridDataArray	= null;								//グリッド情報の配列


	public					GameObject		scoreObj		= null;								//スコアのプレハブ
	private					ScoreUI			scoreUI			= null;								//スコアのスクリプト



	//ゲームの状態遷移ステップ
	public	enum	MainStep {
		 First			//ターンの最初
		,CheckAIPrio	//AIの優先度確認
		,BeforeWait		//操作待機
		,PutPiece		//駒を置く
		,AfterWait		//操作待機
		,ChangePlayer	//プレーヤー交代
		,Last			//ターンの最後

		,GameEnd		//最後
	}
	private					MainStep		mainStep		= MainStep.CheckAIPrio;				//ステップ

	private					PieceKind		nowPlayer		= PieceKind.SideA;					//現在の手番

	private					float			waitSec_Now		= 0.0f;								//待機時間
	private					float			waitSec_ViewPrio= 1.0f;								//待機時間	優先度表示
	private					float			waitSec_ViewSet	= 1.0f;								//待機時間	置いた駒を見せる

	private					PlayerBase[]	playerArray		= null;								//プレーヤーの一覧
	private					List<GridData>	enableHandList	= null;								//打てる手のリスト

	private					int				passedCount		= 0;								//パスしたユーザーの数



	/// <summary>
	/// 起動時の処理
	/// </summary>
	private	void Awake() {
		Initialize();
	}
	/// <summary>
	/// 初期化
	/// </summary>
	private	void Initialize() {

		//フィールド生成
		{
			GameObject	tempObj;
			int i, j;
			//X軸
			gridDataArray	= new GridData[ fieldGridMaxX][];
			for( i=0; i<fieldGridMaxX; i++) {

				//Y軸(配置はXZ平面なのでZ軸)
				gridDataArray[ i]	= new GridData[ fieldGridMaxY];
				for( j=0; j<fieldGridMaxY; j++) {

					//プレハブを生成(ヒエラルキーにコピーを生成)
					tempObj						= Instantiate( gridObj,		(fieldPos	+new Vector3( (fieldOffset.x *i), 0, (fieldOffset.z *j))),	Quaternion.identity)		as GameObject;
					//ヒエラルキー上で自分の子供にする
					tempObj.name				= "GRID("+ i +","+ j +")";
					tempObj.transform.parent	= this.transform;

					//グリッド情報を生成
					gridDataArray[ i][ j]		= new GridData();
					//情報を格納
					gridDataArray[ i][ j].x		= i;
					gridDataArray[ i][ j].y		= j;
					//GridBaseを取得
					gridDataArray[ i][ j].grid	= tempObj.GetComponent<GridBase>();
				}
			}
		}

		//スコア表示
		{
			//プレハブを生成(ヒエラルキーにコピーを生成)
			GameObject	tempObj	= Instantiate( scoreObj)		as GameObject;
			//ヒエラルキー上で自分の子供にする
			tempObj.transform.parent	= this.transform;
			scoreUI		= tempObj.GetComponent<ScoreUI>();
		}

		//AIをロード
		{
			playerArray		= new PlayerBase[ (int)PieceKind.EnumMax];
			playerArray[ (int)PieceKind.SideA]	= new PlayerAI_SideA();
			playerArray[ (int)PieceKind.SideB]	= new PlayerAI_SideB();
		}

		//打てる手のリスト
		{
			enableHandList	= new List<GridData>();
			enableHandList.Clear();
		}

		//最初の配置
		{
			//中心
			int	centerX	= ((fieldGridMaxX /2) -1);
			int	centerY	= ((fieldGridMaxY /2) -1);
			//ここに駒を置く
			PutPiecce( PieceKind.SideA, (centerX +0), (centerY +0));
			PutPiecce( PieceKind.SideB, (centerX +1), (centerY +0));
			PutPiecce( PieceKind.SideB, (centerX +0), (centerY +1));
			PutPiecce( PieceKind.SideA, (centerX +1), (centerY +1));
		}

		//最初の手番はランダム
		nowPlayer		= (PieceKind)Random.Range( 0, (int)PieceKind.EnumMax);
	}




	/// <summary>
	/// 毎フレーム呼ばれる更新処理
	/// </summary>
	private	void Update() {

		//ステップで実行していく
		switch( mainStep) {
		case	MainStep.First:
			mainStep++;
			break;
		case	MainStep.CheckAIPrio:
			//現在の手番のプレーヤーから優先度データを取得
			{
				//打てる手を取得
				UpdateEnableHandList();

				//取れるものある？
				if( enableHandList.Count > 0) {
					//現在の手番のプレーヤーに優先度情報を追記してもらう
					enableHandList	= playerArray[ (int)nowPlayer].GetPrioTable( enableHandList);
				}

				//表示に反映
				UpdatePrioText();

				//取れるものがない？
				if( enableHandList.Count <= 0) {
					//パス
					passedCount++;
					if( passedCount >= (int)PieceKind.EnumMax) {
						//全員パスした？
						mainStep	= MainStep.GameEnd;
					} else {
						//手番交代
						mainStep	= MainStep.ChangePlayer;
					}
				} else {
					//操作OK
					passedCount	= 0;

					//待機時間
					waitSec_Now	= waitSec_ViewPrio;
					mainStep++;
				}
			}
			break;

		case	MainStep.BeforeWait:
			//操作前の待機時間
			if( waitSec_Now > 0) {
				//待機
				waitSec_Now	-= Time.deltaTime;
			} else {
				//次へ
				mainStep++;
			}
			break;
		case	MainStep.PutPiece:
			//操作前の待機時間
			{
				//現在の手番のプレーヤーに優先度情報を追記してもらう
				GridData	nextHand	= playerArray[ (int)nowPlayer].GetNextHand( enableHandList);
				if( null==nextHand) {
					//有効な手を打つまで待機する
					break;
				}

				//駒を置く
				SetNewPiece( nextHand.x, nextHand.y);

				//待機時間
				waitSec_Now	= waitSec_ViewSet;
				mainStep++;
			}
			break;

		case	MainStep.AfterWait:
			//操作後の待機時間
			if( waitSec_Now > 0) {
				//待機
				waitSec_Now	-= Time.deltaTime;
			} else {
				//次へ
				mainStep++;
			}
			break;
		case	MainStep.ChangePlayer:
			//攻守交代
			{
				//今 Aだった？
				if( PieceKind.SideA==nowPlayer) {
					//次はB
					nowPlayer	= PieceKind.SideB;
				} else {
					//次はA
					nowPlayer	= PieceKind.SideA;
				}
				mainStep++;
			}
			break;
		case	MainStep.Last:
			//最初に戻る
			mainStep	= MainStep.First;
			break;
		}
	}







	//チェックの方向
	public	enum	CheckDir {
		 X
		,Y
	}
	//取れるかどうかのチェックで検索しに行く方向
	private	static readonly	int[,]			checkDirArray		= new int[,] {
		//		x		y
		 {		-1,		-1		}
		,{		 0,		-1		}
		,{		 1,		-1		}
		,{		-1,		 0		}
		,{		 1,		 0		}
		,{		-1,		 1		}
		,{		 0,		 1		}
		,{		 1,		 1		}
	};

	/// <summary>
	/// 現在手を打てる場所を取得
	/// </summary>
	private	void UpdateEnableHandList() {
		//リストをクリア
		enableHandList.Clear();

		//初期化
		int	i, j;
		for( i=0; i<fieldGridMaxX; i++) {
			for( j=0; j<fieldGridMaxY; j++) {
				//優先度を-1にしておく
				gridDataArray[ i][ j].prio		= -1;
				gridDataArray[ i][ j].getNum	= 0;
				gridDataArray[ i][ j].grid.SetPlate( false);
			}
		}
		//取得できるかチェック
		for( i=0; i<fieldGridMaxX; i++) {
			for( j=0; j<fieldGridMaxY; j++) {
				//すでに置かれている？
				if( PieceKind.EnumMax!=gridDataArray[ i][ j].pieceKind) {
					//無視
					continue;
				}

				//チェック開始
				CheckPiece( i, j, false);

				//何枚取れる？
				if( gridDataArray[ i][ j].getNum > 0) {
					//1枚以上なら登録
					enableHandList.Add( gridDataArray[ i][ j]);
					//色も変える
					gridDataArray[ i][ j].grid.SetPlate( true);
				}
			}
		}
	}
	/// <summary>
	/// 現在 手を打てる場所かどうかチェック
	/// </summary>
	private	void CheckPiece( int x0, int y0, bool update) {

		//打つ予定の場所
		GridData	rootGrid	= gridDataArray[ x0][ y0];
		//チェックする場所
		GridData	checkGrid;

		int		x1, y1;
		int		x2, y2;
		int		i;
		int		num;

		//縦横ナナメ８方向のいずれかの隣に 自分以外の駒があり、その直線上に 自分の駒があるか？
		int	dir;
		for( dir=0; dir<checkDirArray.GetLength( 0); dir++) {
			//チェックする座標
			x1			= (x0	+checkDirArray[ dir, (int)CheckDir.X]);
			y1			= (y0	+checkDirArray[ dir, (int)CheckDir.Y]);
			//グリッドの範囲外
			if( (x1 < 0) || (x1 >= fieldGridMaxX)		||(y1 < 0) || (y1 >= fieldGridMaxY)) {
				//場外は無視
				continue;
			}
			//このグリッドを調べる
			checkGrid	= gridDataArray[ x1][ y1];

			//空欄
			if( PieceKind.EnumMax==checkGrid.pieceKind) {
				//無視
				continue;
			}
			//自分
			if( nowPlayer==checkGrid.pieceKind) {
				//無視
				continue;
			}

			//★★★	打つ予定の場所の隣が、自分以外の駒


			//元の駒から何枚離れているか
			num		= 1;
			//無限ループ
			for(;;) {
				//チェックする場所 +1
				num++;

				//チェックする座標
				x2			= (x0	+(checkDirArray[ dir, (int)CheckDir.X] *num));
				y2			= (y0	+(checkDirArray[ dir, (int)CheckDir.Y] *num));
				//グリッドの範囲外
				if( (x2 < 0) || (x2 >= fieldGridMaxX)		||(y2 < 0) || (y2 >= fieldGridMaxY)) {
					//場外に行ったら中断
					break;
				}
				//このグリッドを調べる
				checkGrid	= gridDataArray[ x2][ y2];

				//空欄
				if( PieceKind.EnumMax==checkGrid.pieceKind) {
					//だめだった
					break;
				}
				//自分以外
				if( nowPlayer!=checkGrid.pieceKind) {
					//続行
					continue;
				}
				//自分がもう一度出てきた

				//★★★	打つ予定の場所の隣が自分以外の駒で、その先に自分の駒がある

				//この場所に自分が駒を打つと、この枚数 取れる
				rootGrid.getNum		+= (num -1);

				//反映フラグ
				if( update) {
					//塗りつぶす
					for( i=0; i<num; i++) {
						//チェックする座標
						x2			= (x0	+(checkDirArray[ dir, (int)CheckDir.X] *i));
						y2			= (y0	+(checkDirArray[ dir, (int)CheckDir.Y] *i));
						//反映
						PutPiecce( nowPlayer, x2, y2);
					}
				}

				//終わり
				break;
			}
		}
	}
	/// <summary>
	/// 駒を置く
	/// </summary>
	private	void PutPiecce( PieceKind kind, int x, int y) {
		//どちらの駒かを更新して描画に反映
		gridDataArray[ x][ y].pieceKind		= kind;
		gridDataArray[ x][ y].grid.SetPieceKind( kind);
	}

	/// <summary>
	/// 優先度情報をテキストに反映
	/// </summary>
	private	void UpdatePrioText() {

		//お互い何枚ずつあるか
		int	numA	= 0;
		int	numB	= 0;

		//更新処理
		int	i, j;
		for( i=0; i<fieldGridMaxX; i++) {
			for( j=0; j<fieldGridMaxY; j++) {
				//優先度の文字を更新
				gridDataArray[ i][ j].grid.SetPrio( gridDataArray[ i][ j].prio);

				//お互い何枚ずつ持っているか
				if( PieceKind.SideA==gridDataArray[ i][ j].pieceKind) {
					numA++;
				} else
				if( PieceKind.SideB==gridDataArray[ i][ j].pieceKind) {
					numB++;
				}
			}
		}

		//何枚ずつあるか通知
		if( null!=scoreUI) {
			scoreUI.SetScore( numA, numB);
		}
	}

	/// <summary>
	/// 駒を置いた場所を光らせる
	/// </summary>
	private	void SetNewPiece( int x, int y) {

		//駒を置く
		CheckPiece( x, y, true);

		int	i, j;
		for( i=0; i<fieldGridMaxX; i++) {
			for( j=0; j<fieldGridMaxY; j++) {
				//表示全部消す
				gridDataArray[ i][ j].grid.SetPlate( false);
			}
		}
		//置いた場所を光らせる
		gridDataArray[ x][ y].grid.SetPlate( true);
	}


}

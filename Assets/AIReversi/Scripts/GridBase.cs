//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	グリッド管理関係
//
//	Copyright(C)2016 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;




/// <summary>
/// グリッドクラス
/// </summary>
public class GridBase : MonoBehaviour {





	public		GameObject		pieceObj		= null;					/// 駒のオブジェクト

	public		GameObject		plateObj		= null;					/// 台紙のオブジェクト

	public		TextMesh		textMesh		= null;					/// 優先度を表示する3Dテキスト




	/// <summary>
	/// 起動時の処理
	/// </summary>
	private	void	Awake() {

		//最初は表示物を消しておく
		pieceObj.SetActive( false);
		plateObj.SetActive( false);
	}



	/// <summary>
	/// この駒を置く
	/// </summary>
	public	void	SetPieceKind( PieceKind kind) {

		//駒の種類を取得
		pieceObj.transform.localRotation	= Quaternion.Euler( new Vector3( 0, 0, (180 *(int)kind)));

		//駒を表示
		pieceObj.SetActive( true);
	}



	/// <summary>
	/// 台紙を表示するかどうか
	/// </summary>
	public	void	SetPlate( bool enabled) {
		if( null!=plateObj) {
			//表示更新
			plateObj.SetActive( enabled);
		}
	}
	/// <summary>
	/// 優先度情報を通知
	/// </summary>
	public	void	SetPrio( int prio) {
		if( null!=textMesh) {
			//文字を書き換える
			string	updateText	= "";
			//優先度が0以上なら書き換え
			if( prio >= 0) {
				updateText	= ""+ prio;
			}
			textMesh.text		= ""+ updateText;
		}
	}



}

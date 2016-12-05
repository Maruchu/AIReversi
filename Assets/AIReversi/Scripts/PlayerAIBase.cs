//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	プレーヤーAI基底
//
//	Copyright(C)2016 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//← List<XXX> 使うときに要る





/// <summary>
/// プレーヤー基底クラス
/// </summary>
public class PlayerAIBase : PlayerBase {		//← PlayerBase のソースをすべて引き継いでいる



	protected	int[,]			prioTable		= null;				/// 優先度情報(ここをAとBのAIが個別に書き換える)



	/// <summary>
	/// 取れる手を渡して優先度情報を書いて返してもらう
	/// </summary>
	public		override	List<GridData>	GetPrioTable( List<GridData> enableHandList) {
		//リストの情報をすべてチェック
		foreach( GridData data in enableHandList) {
			//優先度テーブルから情報を取得して追記
			data.prio	= prioTable[ data.x, data.y];
		}
		//追記したデータを返す
		return	enableHandList;
	}



	/// <summary>
	/// 取れる手の中からどれかを選ぶ
	/// </summary>
	public		override	GridData		GetNextHand( List<GridData> enableHandList) {
		//自分のAIの優先度情報を取得
		enableHandList		= GetPrioTable( enableHandList);

		//優先度の一番高いもののリストを生成して、その中からランダムに一つ選ぶ
		List<GridData>	highestList		= new List<GridData>();
		//リストの中身を空にする
		highestList.Clear();

		//現在 一番高い優先度はこれ
		int				highestPrio		= (enableHandList[ 0].prio -1);

		//リストの情報をすべてチェック
		foreach( GridData data in enableHandList) {
			//優先度テーブルから情報を取得して追記
			if( data.prio > highestPrio) {
				//今の最高より高い！

				//最高値更新
				highestPrio	= data.prio;
				//リストの中身を空にする
				highestList.Clear();
			}
			//今の最高値と同じ
			if( data.prio==highestPrio) {
				//記録
				highestList.Add( data);
			}
		}

		//抽出されたものの中からランダムに一つ選ぶ
		return	highestList[ Random.Range( 0, highestList.Count)];
	}



}

//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	プレーヤー関係
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
public class PlayerBase {				//← 何も継承してないことに注意(MonoBehaviour を継承してないので Awake や Update は使えない)




	/// <summary>
	/// コンストラクタ
	/// </summary>
	protected		PlayerBase() {
		Initialize();
	}
	/// <summary>
	/// 初期化
	/// </summary>
	protected	virtual		void	Initialize() {
	}




	/// <summary>
	/// 取れる手を渡して優先度情報を書いて返してもらう
	/// </summary>
	public		virtual		List<GridData>	GetPrioTable( List<GridData> enableHandList) {
		//PlayerBase ではなにもしない、ここを PlayerAIBase が override して改造する
		return	enableHandList;
	}
	/// <summary>
	/// 取れる手の中からどれかを選ぶ
	/// </summary>
	public		virtual		GridData		GetNextHand( List<GridData> enableHandList) {
		//最初の手をとりあえず選ぶ(xとyの情報だけ入っていればOK)
		return	enableHandList[ 0];
	}



}

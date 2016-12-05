//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	プレーヤーAI
//
//	Copyright(C)2016 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;





/// <summary>
/// プレーヤーAIクラス
/// </summary>
public class PlayerAI_SideA : PlayerAIBase {		//← PlayerAIBase のソースをすべて引き継いでいる



	/// <summary>
	/// 初期化
	/// </summary>
	protected	override	void	Initialize() {
		//優先度テーブルを A のもので上書き
		prioTable	= AI_PrioTable.prioTable_SideA;
	}



}

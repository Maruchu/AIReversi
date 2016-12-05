//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
//
//	スコアの表示物
//
//	Copyright(C)2016 Maruchu
//	http://maruchu.nobody.jp/
//
//	=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
using UnityEngine;
using System.Collections;




/// <summary>
/// スコア表示クラス
/// </summary>
public class ScoreUI : MonoBehaviour {





	public		TextMesh		textMesh_SideA		= null;					/// スコアの3Dテキスト
	public		TextMesh		textMesh_SideB		= null;					/// スコアの3Dテキスト



	/// <summary>
	/// スコアを通知
	/// </summary>
	public	void	SetScore( int scoreA, int scoreB) {
		if( null!=textMesh_SideA) {
			textMesh_SideA.text		= ""+ scoreA;
		}
		if( null!=textMesh_SideB) {
			textMesh_SideB.text		= ""+ scoreB;
		}
	}



}

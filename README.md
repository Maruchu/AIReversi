# AIReversi
優先度テーブル型AIリバーシ<br>

<img src="http://many.chu.jp/Unity/2016/AIReversi/Sample1.gif" alt="AI対戦" width="256px"> <img src="http://many.chu.jp/Unity/2016/AIReversi/Sample2.gif" alt="AI対戦" width="256px">

Copyright(C)[Maruchu](http://maruchu.nobody.jp/ "Maruchu") 2016


## 注意
※ Unityの対応バージョンは 5.3.4 以降です。


## 使い方
AIの学習の初歩として、優先度に従って駒を置くだけのシンプルなリバーシ用AIがあると便利ですよね。<br>
というわけで あらかじめ設定した優先度に従ってリバーシを遊ぶAIのサンプルプロジェクトです。

AIReversi のフォルダに Main.scene というシーンが入っているので開いて実行してみてください。

SideA(白い方)とSideB(赤い方)の2つのAIが自動的に対戦してくれます。

<br>


## 優先度テーブルの変え方
優先度のデータは Assets/Scripts/AI_PrioTable.cs の中に入っています。

・ 8x8 の配列に 0以上の整数で優先度を入れてください

・ 0に近いほうが優先度が低く(駒を置きにくく)、数字が大きい方が優先度が高く(駒を置きやすく)なります<br>
　(※ 極端に大きい数字はコンピューターが使えないので、0 ～ 9999 あたりで入力してください)

・ AIは、現在 取得できる駒のリストの中で、一番優先度の高い場所に駒を置きます<br>
　一番優先度の高い場所が複数ある場合は、その中からランダムに一つを選びます




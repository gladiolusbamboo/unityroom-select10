using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hananoki;


[CreateAssetMenu( menuName = "TweenParams" )]
[Serializable]
public class TweenParams : ScriptableObject {

	[Serializable]
	public class Title {
		public DOTweenFloat ロゴフェード;
		public DOTweenFloat ボタン出現スケール;
		public DOTweenFloat ボタングローフェード;
		public DOTweenFloat スタート文字移動;

		public DOTweenFloat タイトルグループフェードアウト;
		public DOTweenFloat スタートクリック;
		public float startWait;
	}
	[Header( "タイトル" )]
	public Title title;

	[Serializable]
	public class Game {
		public DOTweenFloat グループフェードイン;
		public DOTweenFloat グループフェードアウト;
		public DOTweenFloat ボタン出現;
		public DOTweenFloat ボタングロー;

		public float タイムバーながさ;
		public DOTweenEaseTime タイムバーのび;

		public DOTweenFloat かくだい;

		public DOTweenFloat 足すスコア移動;
		public DOTweenEaseTime 足すスコアふぇーど;
		public float 足すスコア継続時間;

		public DOTweenFloat うに移動;
		public float うに縦ランダム;
		public DOTweenJump うにジャンプ;
		public DOTweenJump ゆにジャンプ;
		public float ゆに横ランダム;
	}
	[Header( "ゲーム部" )]
	public Game game;

	[Serializable]
	public class BG {
		public float 回転時間;
	}
	[Header( "BG" )]
	public BG bg;

	public float 画面下;
}

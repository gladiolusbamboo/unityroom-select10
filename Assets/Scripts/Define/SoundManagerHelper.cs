/* エディタースクリプト側でコンパイルしてください */
using UnityEngine;
using GJW;

public static partial class sound {
	public static void スタート音( int c = 0 ) { SoundManager.PlaySound( 0, c ); } 

	public static void ピンポーン( int c = 0 ) { SoundManager.PlaySound( 1, c ); } 

	public static void ブブー( int c = 0 ) { SoundManager.PlaySound( 2, c ); } 

	public static void コインゲット( int c = 0 ) { SoundManager.PlaySound( 3, c ); } 

	public static void どどん( int c = 0 ) { SoundManager.PlaySound( 4, c ); } 

	public static void キッ( int c = 0 ) { SoundManager.PlaySound( 5, c ); } 

	public static void コッ( int c = 0 ) { SoundManager.PlaySound( 6, c ); } 

	public static void 警告( int c = 0 ) { SoundManager.PlaySound( 7, c ); } 

	public static void スタート矢印( int c = 0 ) { SoundManager.PlaySound( 8, c ); } 

	public static void スタートボタン登場( int c = 0 ) { SoundManager.PlaySound( 9, c ); } 

	public static void ぽにゅ( int c = 0 ) { SoundManager.PlaySound( 10, c ); } 

	public static void アイテムゲット( int c = 0 ) { SoundManager.PlaySound( 11, c ); } 

	public static void ブロック破壊( int c = 0 ) { SoundManager.PlaySound( 12, c ); } 

	public static void bgm_intro_music_gjt( int c = 0 ) { SoundManager.PlayBgm( 0 ); } 

	public static void bgm_ゲーム中( int c = 0 ) { SoundManager.PlayBgm( 1 ); } 

	public static void bgm_タイムアップ( int c = 0 ) { SoundManager.PlayBgm( 2 ); } 

	public static void bgm_クリア( int c = 0 ) { SoundManager.PlayBgm( 3 ); } 

	public static void bgm_チャレンジ( int c = 0 ) { SoundManager.PlayBgm( 4 ); } 

	public static void bgm_チャレンジ6( int c = 0 ) { SoundManager.PlayBgm( 5 ); } 

	public static void voice_ああ( int c = 0 ) { SoundManager.PlayVoice( 0 ); } 

	public static void voice_それ( int c = 0 ) { SoundManager.PlayVoice( 1 ); } 

	public static void voice_そんなー( int c = 0 ) { SoundManager.PlayVoice( 2 ); } 

	public static void voice_や( int c = 0 ) { SoundManager.PlayVoice( 3 ); } 

	public static void voice_やったー( int c = 0 ) { SoundManager.PlayVoice( 4 ); } 

}

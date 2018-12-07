using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Serialization;

using TMPro;
using Hananoki;
using Hananoki.Extensions;
using DG.Tweening;

#if UNITY_EDITOR
using Hananoki;
#endif

using UnityObject = UnityEngine.Object;
using UnityRandom = UnityEngine.Random;

namespace GJW {
	public class GameController : MonoBehaviour {

		public static GameController instance;
#if UNITY_EDITOR
		public SessionStateBool m_Debug = new SessionStateBool( "gjw.debug", "デバック" );
#endif

		public CanvasGroup m_titleGroup;
		public CanvasGroup[] m_gameGroup;
		public Transform[] m_showRoot;

		public TweenParams m_params;
		public LevelSetting m_levelSetting;
		public PtclPool m_ptclPool;

		public Action m_stepFunc;
		bool m_timeUp;
		public Transform m_trsRoot;

		UniqueRandom ur;
		UniqueRandom urpos;

		public BGAnim m_bgAnim;

		public Image[] m_uniListImage;

		[Header( "タイトル部" )]
		[FormerlySerializedAs( "m_タイトルロゴ" )]
		public CanvasGroup タイトルロゴグループ;
		[FormerlySerializedAs( "m_タイトルボタン背景グロー" )]
		public Image タイトルボタン背景グローイメージ;
		[FormerlySerializedAs( "m_タイトルボタン" )]
		public Transform タイトルボタン;
		[FormerlySerializedAs( "m_タイトルスタート" )]
		public Transform タイトルスタート文字;

		public bool m_startOn;

		[Header( "ゲーム部" )]
		public Transform m_timeBar;
		public NumButton[] m_buttons;
		public NumButton m_buttonPrefabs;
		public TextMeshProUGUI スコア;
		public ParticleSystem コインパーティクル;
		public NumButton 正解のボタン;
		public ParticleSystem コインゲットパーティクル;

		public Animator m_unityChanAnimator;

		public TextMeshProUGUI レート;
		public TextMeshProUGUI 足すスコア;
		public Image うに;
		public Image どっどゆにてぃ;
		bool itemOff;
		public int uniGet;
		public int yuniDotGet;
		bool IsExtraGame() {
			if( ( uniGet + yuniDotGet ) == 0 ) return false;
			return true;
		}
		public int m_selectScene;
		public int m_buttonNum;
		int m_score;
		public int GetScore() => m_score;
		float m_timer;
		public int scoreAddPt;
		Tween twTime;
		Tween twどっどゆにてぃ;
		Tween twうに;

		List<NumButton> ランダムで選んだボタン;
		public bool 正解選んだ;

		public bool runkingClose;
		//[Header( "リザルト部" )]
		//public VerticalLayoutGroup[] m_resultRoot;
		//public TextMeshProUGUI[] m_resultText;

		//public TextMeshProUGUI[] m_resultT;
		//public TextMeshProUGUI[] m_resultLv;
		//public TextMeshProUGUI[] m_resultScore;

		public ParticleSystem mFrame;
		public ParticleSystem mFrame2;

#if UNITY_EDITOR
		/// <summary>
		/// ホットリロードイベント処理
		/// </summary>
		void OnHotReload() {

		}
#endif


		void Initialized() {
			m_showRoot.SetActiveArray( true );
			m_gameGroup.SetActiveArray( false );

			m_startOn = false;
			コインパーティクル.SetActive( false );
			足すスコア.SetActive( false );
			SoundManager.StopAll();

			List<int> aa = new List<int>();
			for( int i = 11; i < 100; i++ ) {
				aa.Add( i );
			}

			ur = new UniqueRandom( aa.ToArray() );

		}


		void SetScore( int s ) {
			m_score = s;
			スコア.SetText( m_score.ToString() );
		}


		public void AddScore() {
			int bakScore = m_score;
			SetScore( m_score + scoreAddPt );

			if( itemOff ) return;
			int ii = bakScore / 1000;
			int jj = m_score / 1000;
			if(  ii < jj ) {
				int n = m_score / 1000;
				if( (n % 3 ) == 0 ) {
					if( !どっどゆにてぃ.IsActive() ) {
						どっどゆにてぃ.SetActive( true );
						どっどゆにてぃ.TwLocalJump( m_params.game.ゆにジャンプ, () => { どっどゆにてぃ.SetActive( false ); } );
						どっどゆにてぃ.SetLocalPosX( ( UnityRandom.value * m_params.game.ゆに横ランダム * 2 ) - m_params.game.ゆに横ランダム );
					}
				}
				else {
					if( !うに.IsActive() ) {
						うに.SetActive( true );
						うに.SetLocalPosY( ( UnityRandom.value * m_params.game.うに縦ランダム * 2 ) - m_params.game.うに縦ランダム );
						うに.TwLocalMoveX( m_params.game.うに移動, ()=> { うに.SetActive( false ); } );
					}
				}
			}
		}


		public void OnうにClick() {
			sound.ぽにゅ(1);
			m_uniListImage[ uniGet ].SetActive(true);
			uniGet++;
			twうに.Kill();
			うに.SetActive( false );
		}

		public void OnどっどゆにてぃClick() {
			sound.アイテムゲット(1);
			yuniDotGet++;
			scoreAddPt++;
			レート.SetText( "+" + scoreAddPt );
			twどっどゆにてぃ.Kill();
			どっどゆにてぃ.SetActive( false );
		}

		/// <summary>
		/// この関数はオブジェクトが有効/アクティブになった時に呼び出されます
		/// </summary>
		void OnEnable() {
			if( instance == null ) {
				instance = this;
			}
		}

		void Awake() {
			//if( instance != null ) {
			//	Destroy( gameObject );
			//	return;
			//}
			//DontDestroyOnLoad( gameObject );
			instance = this;
		}


		// Start is called before the first frame update
		void Start() {
			Initialized();

			m_buttons = new NumButton[ m_buttonNum ];
			for( int i = 0; i < m_buttonNum; i++ ) {
				m_buttons[ i ] = Instantiate( m_buttonPrefabs, m_trsRoot ).GetComponent<NumButton>();
				m_buttons[ i ].SetText( "10" );
			}
			m_buttonPrefabs.SetActive( false );

			if( m_selectScene == 0 )
				StartCoroutine( _Title() );
			else if( m_selectScene == 1 )
				StartCoroutine( _Main() );
			else
				StartCoroutine( _Result() );

			mFrame.SetActive(false);
			mFrame2.SetActive( false );
		}

		// Update is called once per frame
		void Update() {
			if( Input.GetKey( KeyCode.Escape ) ) {
				//m_timeUp = true;
				//m_timer = 0;
				SceneManager.LoadScene( SceneManager.GetActiveScene().name );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator _Title() {
			sound.bgm_intro_music_gjt();
			m_gameGroup.SetActiveArray( false );
			mFrame.Stop();
			mFrame2.Stop();
			itemOff = false;

			m_bgAnim.Reset();
			uniGet = 0;
			m_uniListImage.SetActiveArray(false);
			yuniDotGet = 0;
			SetScore( 0 );
			m_timeBar.SetLocalSclX( 0.00f );
			m_titleGroup.SetActive( true );
			m_titleGroup.SetAlpha( 1.00f );
			タイトルロゴグループ.SetActive( true );
			タイトルロゴグループ.SetAlpha( 0.00f );
			タイトルロゴグループ.SetAlpha( 0.00f );
			タイトルボタン背景グローイメージ.SetActive( false );
			タイトルボタン.SetActive( false );
			タイトルスタート文字.SetActive( false );
			scoreAddPt = 1;
			レート.SetText( "+" + scoreAddPt );
			レート.DOFade( 1.00f, 0.50f );
			うに.SetActive( false );
			どっどゆにてぃ.SetActive( false );
			yield return new WaitForSeconds( 0.50f );
			タイトルロゴグループ.TwFade( m_params.title.ロゴフェード );
			yield return new WaitForSeconds( m_params.title.ロゴフェード.time );

			sound.スタートボタン登場();
			タイトルボタン.SetActive( true );
			タイトルボタン.TwScale( m_params.title.ボタン出現スケール );
			yield return new WaitForSeconds( m_params.title.ボタン出現スケール.time );

			//タイトルボタン背景グローイメージ.SetActive( true );
			//タイトルボタン背景グローイメージ.TwFade( m_params.title.ボタングローフェード );
			//yield return new WaitForSeconds( m_params.title.ボタングローフェード.time );

			sound.スタート矢印();
			タイトルスタート文字.SetActive( true );
			タイトルスタート文字.TwLocalMoveX( m_params.title.スタート文字移動 );

			StartCoroutine( _TitleWait() );
			yield break;
		}

		IEnumerator _TitleWait() {
			while( !m_startOn ) {
				yield return null;
			}
			m_startOn = false;
			SoundManager.StopBgm();
			yield return new WaitForSeconds( m_params.title.startWait );

			m_titleGroup.TwFade( m_params.title.タイトルグループフェードアウト );
			yield return new WaitForSeconds( m_params.title.タイトルグループフェードアウト.time );

			StartCoroutine( _Main() );
			yield break;
		}

		void DisableButtons() {
			foreach( var a in m_buttons ) {
				a.ResetHide();
			}
		}

		void MakeButtons( int lv, bool block ) {
			int num = m_levelSetting.m_data[ lv ].num;
			ランダムで選んだボタン = new List<NumButton>();

			urpos = new UniqueRandom( m_buttonNum );

			DisableButtons();

			for( int i = 0; i < num; i++ ) {
				ランダムで選んだボタン.Add( m_buttons[ urpos.Get() ] );
			}
			Debug.Log( $"ランダムで選んだボタン: {ランダムで選んだボタン.Count}" );

			foreach( var a in ランダムで選んだボタン ) {
				a.SetAlpha( 1.00f );
			}

			ランダムで選んだボタン[ 0 ].SetText( 10 );
			ランダムで選んだボタン[ 0 ].atari = true;
			ランダムで選んだボタン[ 0 ].問題初期化( block );
			ランダムで選んだボタン[ 0 ].button.enabled = true;
			///ランダムで選んだボタン[ 0 ].image.color = Color.red;
			for( int i = 1; i < num; i++ ) {
				ランダムで選んだボタン[ i ].SetText( ur.Get() );
				ランダムで選んだボタン[ i ].問題初期化( block );
			}


#if UNITY_EDITOR
			if( m_Debug.Value ) {
				var aa = ランダムで選んだボタン[ 0 ].button.colors;
				aa.normalColor = Color.red;
				ランダムで選んだボタン[ 0 ].button.colors = aa;
			}
#endif
		}

		void Animせいかい( int lv ) {
			int num = m_levelSetting.m_data[ lv ].num;

			ランダムで選んだボタン[ 0 ].transform.DOScale( 1.5f, 0.25f );
			///ランダムで選んだボタン[ 0 ].image.color = Color.red;
			for( int i = 1; i < num; i++ ) {
				ランダムで選んだボタン[ i ].transform.DOScale( 0.5f, 0.25f );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator _Main() {
			Debug.Log( "_Main" );

			DisableButtons();

			m_gameGroup.SetActiveArray( true );
			m_gameGroup[ 0 ].TwFade( m_params.game.グループフェードイン );
			m_gameGroup[ 1 ].TwFade( m_params.game.グループフェードイン );
			yield return new WaitForSeconds( m_params.game.グループフェードイン.time );


			for( int i = 0; i < m_levelSetting.m_data.Length; i++ ) {
				//i = 9;
				if( i == 5 ) {
					m_bgAnim.フィーバー();
				}
				正解選んだ = false;
				Debug.Log( $"{i + 1 }問目: {m_levelSetting.m_data[ i ].num}" );
				MakeButtons( i, false );
				m_timeBar.TwScaleX( m_params.game.タイムバーながさ, m_params.game.タイムバーのび );
				foreach( var p in ランダムで選んだボタン ) {
					p.ShowButtonGroup();
				}
				yield return new WaitForSeconds( m_params.game.タイムバーのび.time );

				foreach( var p in ランダムで選んだボタン ) {
					p.ShowText();
				}
				yield return new WaitForSeconds( 0.50f );

				m_levelSetting.制限時間 = 10;
				sound.bgm_ゲーム中();

				var co = StartCoroutine( _GameMain( i,false ) );

				yield return co;
				if( co != null ) {
					StopCoroutine( co );
				}
				if( m_timeUp ) {
					break;
				}
			}

			itemOff = true;

			if( m_timeUp == false && IsExtraGame() ) {
				foreach( var p in ランダムで選んだボタン ) p.HideButtonGroup();
				while( コインパーティクル.IsAlive() ) {
					yield return null;
				}

				SoundManager.StopBgm();
				mFrame.SetActive(true);
				mFrame.Play();
				if( 4 <= scoreAddPt && 6<= uniGet ) {
					sound.bgm_チャレンジ();
					mFrame2.SetActive( true );
					mFrame2.Play();
				}
				else {
					sound.bgm_チャレンジ();
				}
				レート.DOFade( 0.00f, 0.50f );
				m_bgAnim.フィーバー();

				int n = uniGet;
				for( int i = 0; i < n; i++ ) {
					正解選んだ = false;
					MakeButtons( m_levelSetting.ブロックレベル, true );
					m_timeBar.TwScaleX( m_params.game.タイムバーながさ, m_params.game.タイムバーのび );
					foreach( var p in ランダムで選んだボタン ) {
						p.ShowButtonGroup();
					}
					yield return new WaitForSeconds( m_params.game.タイムバーのび.time );
					foreach( var p in ランダムで選んだボタン ) {
						p.ShowText();
					}
					yield return new WaitForSeconds( 0.50f );
					m_levelSetting.制限時間 = 10;
					var co = StartCoroutine( _GameMain( 9, true ) );

					yield return co;
					if( co != null ) {
						StopCoroutine( co );
					}
					m_uniListImage[ i ].SetActive( false );

					if( m_timeUp ) {
						break;
					}
				}
			}

			foreach( var p in m_buttons ) {
				p.button.enabled = false;
			}
			StartCoroutine( _Result() );
			yield break;
		}

		IEnumerator _ScoreAddAnim( int cnt ) {
			足すスコア.SetActive( true );
			足すスコア.SetAlpha( 0.00f );
			足すスコア.TwFade( 1.00f, m_params.game.足すスコアふぇーど );
			足すスコア.TwLocalMoveY( m_params.game.足すスコア移動 );
			足すスコア.SetText( "+" + cnt );
			yield return new WaitForSeconds( m_params.game.足すスコアふぇーど.time );
			yield return new WaitForSeconds( m_params.game.足すスコア継続時間 );

			足すスコア.TwFade( 0.00f, m_params.game.足すスコアふぇーど, () => { 足すスコア.SetActive( false ); } );

			yield break;
		}

		IEnumerator _GameMain( int lv, bool bolck ) {
			m_timer = m_levelSetting.制限時間;
			m_timeUp = false;
			twTime = m_timeBar.transform.DOScaleX( 0.00f, m_levelSetting.制限時間 );
			while( 0.00f < m_timer ) {
				if( 正解選んだ ) {
					コインパーティクル.transform.position = 正解のボタン.transform.position;
					コインパーティクル.SetActive( false );
					int cnt = 500;
					if( bolck ) {
						scoreAddPt = m_score / m_levelSetting.割る数;
						StartCoroutine( _ScoreAddAnim( scoreAddPt * 500 ) );
					}
					else {
						cnt = (int) ( m_timer * 100 );
						StartCoroutine( _ScoreAddAnim( cnt* scoreAddPt ) );
					}
					コインパーティクル.maxParticles = cnt;
					コインパーティクル.SetActive( true );
					m_unityChanAnimator.Play( AnimHash.Jump );
					twTime.Kill();
					//Animせいかい( lv );
					yield return new WaitForSeconds( 0.25f );
					break;
				}

				m_timer -= Time.deltaTime;
				//スコア.SetText( m_timer.ToString( "F2" ) );
				if( m_timer < 0.00f ) {
					m_timer = 0.00f;
					m_timeUp = true;
				}
				yield return null;
			}

			twTime.Kill();
			yield break;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator _Result() {
			if( !m_timeUp ) {
				foreach( var p in ランダムで選んだボタン ) p.HideButtonGroup();
			}

			while( コインパーティクル.IsAlive() ) {
				yield return null;
			}

			SoundManager.StopBgm();
			if( m_timeUp ) {
				var aa = ランダムで選んだボタン[ 0 ].button.colors;
				aa.normalColor = Color.red;
				ランダムで選んだボタン[ 0 ].button.colors = aa;
				//ランダムで選んだボタン[ 0 ].transform.DOScale( 1.5f, 0.25f );
				m_unityChanAnimator.Play( AnimHash.Dmg );
				sound.警告(1);
				yield return new WaitForSeconds( 0.750f );
			}

			if( m_timeUp ) {
				sound.bgm_タイムアップ();
			}
			else {
				sound.bgm_クリア();
			}
			naichilab.RankingLoader.Instance.SendScoreAndShowRanking( m_score );

			while( !runkingClose ) {
				yield return null;
			}
			runkingClose = false;
			SoundManager.StopBgm();
			mFrame.Stop();
			mFrame2.Stop();
			m_timeBar.DOScaleX( 0.00f, m_params.game.グループフェードアウト.time );
			m_gameGroup[ 0 ].TwFade( 0.00f, m_params.game.グループフェードアウト );
			m_gameGroup[ 1 ].TwFade( 0.00f, m_params.game.グループフェードアウト );
			yield return new WaitForSeconds( m_params.game.グループフェードアウト.time );


			StartCoroutine( _Title() );
			yield break;
		}


		public void OnReset() {
			SceneManager.LoadScene( Game.Scene.Title );
		}


		[InspectorGUI]
		void InspectorGUI() {
			if( GUILayout.Button( "ranking" ) ) {
				// Type == Time の場合
				//var millsec = 5000;
				//var timeScore = new System.TimeSpan( 0, 0, 0, 0, millsec );
				//naichilab.RankingLoader.Instance.SendScoreAndShowRanking( timeScore );

				// Type == Number の場合
				naichilab.RankingLoader.Instance.SendScoreAndShowRanking( 100 );
			}
		}

#if UNITY_EDITOR
		//[InspectorGUI]
		//void InspectorGUI() {
		//	if( GUILayout.Button("回収") ) {
		//		m_buttons
		//	}
		//}
#endif
	}
}

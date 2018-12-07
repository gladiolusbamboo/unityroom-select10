#pragma warning disable 0649 // 常に既定値
#pragma warning disable 0169

//#define BUILD_MODE
//#define DEBUG_LOG
#if UNITY_STANDALONE
#else
#define LOCAL_SWITCH
#endif

using System.Collections;

using System.IO;
using UnityEngine;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

//using UniRx;
//using UniRx.Triggers;

using UnityEngine.SceneManagement;
using UnityScene = UnityEngine.SceneManagement.Scene;

using UnityObject = UnityEngine.Object;

#if !UNITY_EDITOR
#if !DEVELOP_MODE
public static class Debug {
	[System.Diagnostics.Conditional( "UNITY_ASSERTIONS" )]
	public static void Assert( bool condition ) { UnityEngine.Debug.Assert( condition ); }
	[System.Diagnostics.Conditional( "UNITY_ASSERTIONS" )]
	public static void Assert( bool condition, object message ) { UnityEngine.Debug.Assert( condition, message ); }
	[System.Diagnostics.Conditional( "UNITY_ASSERTIONS" )]
	public static void Assert( bool condition, UnityObject context ) { UnityEngine.Debug.Assert( condition, context ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void Log( object message ) { UnityEngine.Debug.Log( message ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void Log( object message, UnityObject context ) { UnityEngine.Debug.Log( message, context ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogAssertion( object message ) { UnityEngine.Debug.LogAssertion( message ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogAssertion( object message, UnityObject context ) { UnityEngine.Debug.LogAssertion( message, context ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogAssertionFormat( string format, params object[] args ) { UnityEngine.Debug.LogAssertionFormat( format, args ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogAssertionFormat( UnityObject context, string format, params object[] args ) { UnityEngine.Debug.LogAssertionFormat( context, format, args ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogError( object message ) { UnityEngine.Debug.LogError( message ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogError( object message, UnityObject context ) { UnityEngine.Debug.LogError( message, context ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogErrorFormat( string format, params object[] args ) { UnityEngine.Debug.LogErrorFormat( format, args ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogErrorFormat( UnityObject context, string format, params object[] args ) { UnityEngine.Debug.LogErrorFormat( context, format, args ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogException( Exception exception ) { UnityEngine.Debug.LogException( exception ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogException( Exception exception, UnityObject context ) { UnityEngine.Debug.LogException( exception, context ); }

	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogFormat( string format, params object[] args ) { UnityEngine.Debug.LogFormat( format, args ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogFormat( UnityObject context, string format, params object[] args ) { UnityEngine.Debug.LogFormat( context, format, args ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogWarning( object message ) { UnityEngine.Debug.LogWarning( message ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogWarning( object message, UnityObject context ) { UnityEngine.Debug.LogWarning( message, context ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogWarningFormat( string format, params object[] args ) { UnityEngine.Debug.LogWarningFormat( format, args ); }
	[System.Diagnostics.Conditional( "DEBUG_LOG" )]
	public static void LogWarningFormat( UnityObject context, string format, params object[] args ) { UnityEngine.Debug.LogWarningFormat( context, format, args ); }
}
#endif
#endif

namespace GJW {

	public partial class GameSys : MonoBehaviour {

		public static GameSys instance;

		[RuntimeInitializeOnLoadMethod()]
		static void Init() {
			var a = new GameObject( "GameSys" );
			var gs = a.AddComponent<GameSys>();
		}

		public SoundManager m_SoundManager;

		//[HideInInspector]
		//public AssetManager m_AssetManager;
		//[HideInInspector]
		//public TimeManager m_TimeManager;
		//[HideInInspector]
		//public SaveManager m_SaveManager;

		//public GameWork m_Gamework;

		//public SaveWork m_SaveWork;

		string m_entryNextSceneName = null;
		int m_loadCnt;
		AsyncOperation m_asynOpScene;
		bool m_changeScene;
		bool m_autoFadeIn;
		public float m_loadProgress;

		public static event Action m_SoftResetCallback;  ///< ソフトリセット時に処理を追加したい時等
		public static Action<float,bool> cbLoadingPercent;
		public static Action s_initComplete;
		public static bool inited;

		static bool pauseFlag;
		

		int m_frame_counts;
		float m_start_time;
		float m_fps;

		//PrefabList m_prefabList;

		//public static TimeManager TimeManager { get { return instance.m_TimeManager; } }
		//public static SaveWork SaveWork { get { return instance.m_SaveWork; } }


		public static void Pause( bool flag ) {
			if( flag ) {
				Time.timeScale = 0.0f;
				pauseFlag = true;
			}
			else {
				Time.timeScale = 1.0f;
				pauseFlag = false;
			}
			//Joypad.trg = 0;
			//Joypad.lev = 0;
		}

		public static bool IsPause() {
			return pauseFlag;
		}

		public static void EnablePause() {
			GameSys.Pause( true );
			//UIHelpGuide1P.Pause( true );
			//UIHelpGuide2P.Pause( true );
			//SoundManager.PauseAll( true, 0x1000 );
		}

		public static void DisablePause() {
			GameSys.Pause( false );
			//UIHelpGuide1P.Pause( false );
			//UIHelpGuide2P.Pause( false );
			//SoundManager.PauseAll( false, 0x1000 );
		}

		public static void TogglePause() {
			GameSys.Pause( !pauseFlag );
			Debug.Log( "TogglePause" );
		}

#if UNITY_EDITOR
		/// <summary>
		/// リロードが入ったら全ゲームオブジェクトにリロードメッセージを送る
		/// </summary>
		[DidReloadScripts]
		static void DidReloadScripts() {
			if( !EditorApplication.isPlaying ) return;

			//foreach( GameObject obj in Resources.FindObjectsOfTypeAll( typeof( GameObject ) )
			//	.Where( a => AssetDatabase.GetAssetOrScenePath( a ).Contains( ".unity" ) ).ToArray() ) {
			foreach( GameObject obj in Resources.FindObjectsOfTypeAll( typeof( GameObject ) ) ){
				if( obj.name == "[DOTween]" ) {
					GameObject.Destroy( obj );
				}
				try {
					obj.SendMessage( "OnHotReload", SendMessageOptions.DontRequireReceiver );
					//Debug.Log(obj.name);
				}
				catch( System.Exception e ) {
					Debug.LogError( e );
				}
			}
		}


		/// <summary>
		/// ホットリロードイベント処理
		/// </summary>
		void OnHotReload() {
			Debug.Log( "GameSys: OnHotReload" );
			InitRuntime();
			inited = true;
		}
#endif


		/// <summary>
		/// シーンが読み込まれた際の通知を受け取るデリゲート内容です
		/// </summary>
		/// <param name="s"></param>
		/// <param name="mode"></param>
		void OnSceneLoaded( UnityScene scene, LoadSceneMode mode ) {
			} // OnSceneLoaded


		/// <summary>
		/// この関数はオブジェクトが有効/アクティブになった時に呼び出されます
		/// </summary>
		void OnEnable() {
			if( instance == null ) {
				instance = this;
			}
		}


		//[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
		public static void Spawn( Action func = null ) {
#if UNITY_EDITOR
			//既に存在する場合false
			if( isAlive() ) return;

			var a = new GameObject( "GameSys" );
			var gs = a.AddComponent<GameSys>();
			//gs.m_Gamework = Resources.Load( "Gamework" ) as GameWork;

			//gs.m_Gamework.m_buildSceneNames = EditorBuildSettings.scenes
			//			.Where( it => it.enabled )
			//			.Select( it => Path.GetFileNameWithoutExtension( it.path ) )
			//			.ToArray();
			//gs.m_SaveWork = gs.m_Gamework.m_SaveWork;
//#if UNITY_SWITCH
//			var c = a.AddComponent<HrJoyConAxis>();
//#endif

			//gs.m_prefabList = Resources.Load( "systemprefabs" ) as PrefabList;
			//if( gs.m_prefabList != null ) {
			//	foreach( var p in gs.m_prefabList.m_Data ) {
			//		if( p.prefab == null ) continue;
			//		var instance = GameObject.Instantiate( p.prefab );
			//		instance.name = p.prefab.name;
			//		//instance.transform.parent = a.transform;
			//	}
			//}
			inited = true;
#else
			Debug.Assert( false, "GameSys.Spawnは UNITY_EDITORのみ使用可能");

#endif
			InitRuntime();
		}


		/// <summary>
		/// initで呼ばれる初期化用
		/// </summary>
//		public static void SpawnAsync( GameWork gmwork, Action func = null ) {
//			Debug.Assert( gmwork != null, "gmworkがnull" );

//			//既に存在する場合false
//			if( isAlive() ) return;

//			var a = new GameObject( "GameSys" );
//			var b = a.AddComponent<GameSys>();
			
//			b.m_Gamework = gmwork;
//			b.m_SaveWork = gmwork.m_SaveWork;
//			GameWork.Init();

//#if UNITY_EDITOR
//			b.m_Gamework.m_buildSceneNames = EditorBuildSettings.scenes
//						.Where( it => it.enabled )
//						.Select( it => Path.GetFileNameWithoutExtension( it.path ) )
//						.ToArray();
//#endif

//#if LOCAL_SWITCH
//		UnityEngine.Switch.Notification.SetResumeNotificationEnabled(true);
//		UnityEngine.Switch.Notification.notificationMessageReceived += Resume;
//#endif

//			s_initComplete = func;

//			InitRuntime();
//		}



		/// <summary>
		/// GameSystemが存在するかチェックします
		/// </summary>
		/// <returns>存在していたらtrue</returns>
		public static bool isAlive() {
			return ( instance != null ) ? true : false;
		}

		//scene用(読み込み終了)
		public static bool isLoadSceneComplete() {
			_checkInstance();
			if( instance.m_loadProgress >= 0.9f ) return true;
			return false;
		}

		//player用
		public static bool isLoading() {
			_checkInstance();
			if( instance.m_entryNextSceneName != null ) return true;
			return 0 < instance.m_loadCnt ? true : false;
		}


		/// <summary>
		/// シーンの事前読み込み
		/// </summary>
		/// <param name="sceneName"></param>
		/// <returns></returns>
		public static bool PreLoadScene( string sceneName ) {
//#if UNITY_EDITOR
//			if( GameWork.i.m_preloadOff ) return false;
//#endif
			_checkInstance();
			return GameSys.ChangeScene( sceneName , false, false );
		}


		/// <summary>
		/// シーンの切り替えを指定する
		/// 非同期読み込みなので注意
		/// </summary>
		/// <param name="sceneName">シーン名</param>
		/// <param name="autoChange">読み込み完了時にシーン切り替えを行う</param>
		/// <param name="autoFadeIn">自動フェードイン、あまりオススメしない</param>
		public static bool ChangeScene( string sceneName, bool autoChange = true, bool autoFadeIn = false ) {
			_checkInstance();

			if( string.IsNullOrEmpty( sceneName ) ) {
				Debug.LogErrorFormat( "sceneName = {0}", sceneName );
				return false;
			}
#if DEBUG_MODE
			int idx = Array.IndexOf( GameWork.buildSceneNames, sceneName );
			if( idx < 0 ) {
				Debug.LogErrorFormat( "sceneName = {0} がビルドリストに登録されていません", sceneName );
				return false;
			}
#endif
			if( isAlive() == false ) {
				Debug.LogErrorFormat( "GameSys: isAlive == false" );
				return false;
			}

			instance.m_entryNextSceneName = sceneName;
			//self.m_loadType = 0;
			instance.m_changeScene = autoChange;
			instance.m_autoFadeIn = autoFadeIn;
			instance.m_loadProgress = 0.00f;
			return true;
		}


		/// <summary>
		/// シーンの切り替えを許可する
		/// </summary>
		/// <param name="enable"></param>
		public static void ChangeSceneEnable( bool enable = true ) {
			_checkInstance();
			instance.m_changeScene = enable;
		}


		public static void ResetNextScene() {
			instance.m_asynOpScene = null;
			instance.m_entryNextSceneName = null;
		}

		static void InitRuntime() {

			// ソフトリセット時の処理
		//	m_SoftResetCallback += () => {
		//		Debug.Log( "SoftResetCallback" );
		//		SoundManager.StopAll( 0.75f );
		//		ResetNextScene();
		//		SceneManager.LoadScene( Game.Scene.DebugMenu );
		//	};
		}


		/// <summary>
		/// シーンを非同期で読み込んで遷移します
		/// </summary>
		/// <param name="sceneName"></param>
		/// <returns></returns>
		IEnumerator nextSceneAsync( string sceneName ) {

			m_asynOpScene = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );

			if( m_asynOpScene == null ) {
				Debug.LogErrorFormat( "{0}: シーンが見つからない", sceneName );
				goto exit;
			}
			m_asynOpScene.allowSceneActivation = false;
			m_loadProgress = 0.00f;
			Debug.LogFormat( "{0}: LoadSceneAsync", sceneName );
			while( m_asynOpScene.progress < 0.9f ) {

				m_loadProgress = m_asynOpScene.progress;
				if( cbLoadingPercent != null ) {
					cbLoadingPercent( m_asynOpScene.progress, false );
				}
				yield return null;
			}
			Debug.LogFormat( "{0}: complete", sceneName );
			m_loadProgress = 0.95f;


			//Fadeが動作中の場合はシーン遷移を待つ
			//while( GameDisp.isFadeDrv() == true ) {
			//	yield return null;
			//}
			while( m_changeScene == false ) {
				yield return null;
			}

		exit:
			if( cbLoadingPercent != null ) {
				cbLoadingPercent( 1.00f, true );
			}

			//ブロック対策でシーン遷移前に１度抜けておく
			yield return null;

			if( m_asynOpScene != null ) {
				m_asynOpScene.allowSceneActivation = true;
			}
			ResetNextScene();

			yield break;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		//bool IsSoftResetKey() {
		//	if( joycon.lev.has( joycon.Minus ) && joycon.trg.has( joycon.Plus ) ) {
		//		return true;
		//	}
		//	if( Input.GetKey( KeyCode.Space ) ) {
		//		return true;
		//	}
		//	return false;
		//}


		//[System.Diagnostics.Conditional( "DEVELOP_MODE" )]
		//void ActionSoftReset() {
		//	if( joycon.trg.Has( joycon.Minus ) ) {
		//		GameObject.Instantiate( GameWork.i.m_DebugConsolePrefab );
		//	}
		//	if( IsSoftResetKey() || Input.GetKey( KeyCode.Escape ) ) {
		//		bool b = true;
		//		// 非同期読み込み有でローディング中は処理を行わない
		//		if( m_asynOpScene != null ) {
		//			if( !isLoadSceneComplete() ) {
		//				b = false;
		//			}
		//		}
		//		if( b ) {
		//			if( Input.GetKey( KeyCode.Escape ) ) {
		//				SoundManager.StopAll( 0.75f );
		//				ResetNextScene();
		//				SceneManager.LoadScene( SceneManager.GetActiveScene().name );
		//			}
		//			else if( m_SoftResetCallback != null ) {
		//				m_SoftResetCallback();
		//			}
		//		}
		//	}
		//}




		[System.Diagnostics.Conditional( "DEBUG_CHECK" )]
		static void _checkInstance() {
			Debug.Assert( GameSys.instance != null, "GameSys: instance が null で呼び出しがありました" );
		}

		//public static void SpawnTimeLagPrefabs( Action comp = null) {
		//	instance.StartCoroutine( instance._SpawnTimeLagPrefabs( comp ) );
		//}

		//IEnumerator _SpawnTimeLagPrefabs( Action comp ) {
		//	foreach( var p in m_prefabList.m_Data ) {
		//		if( p.prefab == null ) continue;

		//		var instance = GameObject.Instantiate( p.prefab );
		//		instance.name = p.prefab.name;
		//		//if( instance.name == "SoundManager" ) {
		//		//	Debug.Log();
		//		//}
		//		yield return null;
		//	}
		//	comp.Call();
		//	yield break;
		//}



		/// <summary>
		/// スクリプトのインスタンスがロードされたときに呼び出されます
		/// </summary>
		void Awake() {
			if( instance != null ) {
				Destroy( gameObject );
				return;
			}
			DontDestroyOnLoad( gameObject );
			instance = this;

			//GameObject.Instantiate( m_SoundManager );
			 
			//m_AssetManager = gameObject.AddComponent<AssetManager>();
			//m_TimeManager = gameObject.AddComponent<TimeManager>();
			//m_SaveManager = gameObject.AddComponent<SaveManager>();

			SceneManager.sceneLoaded += OnSceneLoaded;
		}


		// Use this for initialization
		void Start() {
			InitRuntime();
			

			
			// 起動時の必要なプレハブの読み込み
			//AssetManager.LoadAsync( 0, "data_common", ( uobj01 ) => {
			//AssetManager.LoadAsync( 0, "systemprefabs", ( uobj ) => {
			//	m_prefabList = uobj as PrefabList;
			//	//if( lst == null ) Debug.LogError( "OnSystemPrefabs: error" );

			//	//try {
			//	//	foreach( var p in lst.m_Data ) {
			//	//		if( p.prefab == null ) continue;
			//	//		var instance = GameObject.Instantiate( p.prefab );
			//	//		instance.name = p.prefab.name;
			//	//		//instance.transform.parent = gameObject.transform;
			//	//	}
			//	//}
			//	//catch( System.Exception e ) {
			//	//	Debug.Log( e );
			//	//}
			//	inited = true;
			//	if( s_initComplete != null ) {
			//		s_initComplete();
			//	}
			//} );
			//} );

			//Joypad.init();
			//account.init();
			//SaveFs.Init();
#if UNITY_SWITCH
			gameObject.AddComponent<JoyConVib>();
#endif
			//#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE
			//			touchScreen.init();
			//#endif

			//DOTween.Init();    // ← コレないと効かない
			//DOTween.defaultEaseType = Ease.Linear;
		}


		/// <summary>
		/// Update is called once per frame
		/// </summary>
		void Update() {
			//DebugPanel.Clear();

			if( !inited ) return;
			//Joypad.trg = Joypad.lev = 0;
			// パッドの更新
			//Joypad.update();
			//#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE
			//			touchScreen.update();
			//#endif
			//DebugMonitor.update();

			//{
			//	++m_frame_counts;
			//	float currentTime = Time.time;
			//	float diffTime = currentTime - m_start_time;
			//	m_fps = m_frame_counts / diffTime;

			//	var monoUsed = Profiler.GetMonoUsedSizeLong();
			//	var monoSize = Profiler.GetMonoHeapSizeLong();
			//	var totalUsed = Profiler.GetTotalAllocatedMemoryLong(); // == Profiler.usedHeapSize
			//	var totalSize = Profiler.GetTotalReservedMemoryLong();
			//	//string text = string.Format(
			//	//		"mono: {0}/{1} kb({2:f1}%)\n" +
			//	//		"total:{3}/{4} kb({5:f1}%)\n",
			//	//		monoUsed / 1024, monoSize / 1024, 100.0 * monoUsed / monoSize,
			//	//		totalUsed / 1024, totalSize / 1024, 100.0 * totalUsed / totalSize );
			//	const uint mega = 1024 * 1024;
			//	string memory = string.Format( "Memory: {0:####.0} / {1}.0MB GCCount: {2}", Profiler.usedHeapSizeLong / (float) mega, SystemInfo.systemMemorySize, System.GC.CollectionCount( 0 ) );
			//	string performance = string.Format( "Performance: {0:#0.#}fps", m_fps );
			//	DebugPanel.SetText( memory + System.Environment.NewLine + performance );
			//}


			// ソフトリセット処理
			//ActionSoftReset();

			// シーン切り替えがあった場合に
			// コルーチンを起動して非同期読み込みを開始する
			if( !string.IsNullOrEmpty( m_entryNextSceneName ) && m_asynOpScene == null ) {
				StartCoroutine( nextSceneAsync( m_entryNextSceneName ) );
			}
		}

	} // class GameSys
} // namespace SwKids

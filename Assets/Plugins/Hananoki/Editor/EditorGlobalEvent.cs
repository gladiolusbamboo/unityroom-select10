
#if LOCAL_TEST

using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using System.Reflection;
using UnityEditor.SceneManagement;

namespace HananokiEditor {

	[InitializeOnLoad]
	public partial class EditorGlobalEvent {

		public static EditorGlobalEvent instance;
		
		static EditorGlobalEvent() {
			instance = new EditorGlobalEvent();
			//Debug.Log( "EditorGlobalEvent Constract" );
		}


		/// <summary>
		/// 
		/// </summary>
		public EditorGlobalEvent() {
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			EditorApplication.update += OnUpdate;

			FieldInfo globalEventHandlerInfo = typeof( EditorApplication ).GetField( "globalEventHandler", BindingFlags.NonPublic | BindingFlags.Static );
			globalEventHandlerInfo.SetValue( this, Delegate.CreateDelegate( globalEventHandlerInfo.FieldType, this, "EventHandler" ) );

			//// シーンを開いたらCanvasにworldCamera入れる
			//EditorSceneManager.sceneOpened += ( scene, mode ) => {
			//	if( BuildPipeline.isBuildingPlayer ) return;
			//	if( Application.isPlaying ) return;

			//	var lst = Resources.FindObjectsOfTypeAll( typeof( Canvas ) ).Where( ( a ) => AssetDatabase.GetAssetOrScenePath( a ).Contains( ".unity" ) ).ToArray();
			//	foreach( var l in lst ){
			//		var canvas = (Canvas) l;
			//		canvas.worldCamera = Camera.main;
			//	}
			//};

			//// シーン保存完了
			//EditorSceneManager.sceneSaved += ( scene ) => {
			//	Debug.Log( "sceneSaved : " + scene.name );
			//};
		}


		/// <summary>
		/// globalEventHandler
		/// </summary>
		public void EventHandler() {
			switch( Event.current.type ) {
				case EventType.KeyDown:
					//if( Event.current.keyCode == KeyCode.F1 ) {
					//	//Debug.Log("ファンクションき～");
					//	//EdNotes.NoteWindow.ShowNoteWindow();
					//	EditorApplication.ExecuteMenuItem( "GameObject/UNotes/Add Note" );
					//	Event.current.Use();
					//}
#if false
					if( Event.current.keyCode == KeyCode.L ) {
						SceneView.lastActiveSceneView.m_SceneLighting = !SceneView.lastActiveSceneView.m_SceneLighting;
						SceneView.lastActiveSceneView.Repaint();
						Event.current.Use();
					}
					if( Event.current.keyCode == KeyCode.Space ) {
						EditorApplication.isPaused = !EditorApplication.isPaused;
					}
					if( Event.current.keyCode == KeyCode.N ) {
						EditorApplication.Step();
					}
#endif
					if( Event.current.keyCode == KeyCode.Escape ) {
						try {
							//SoundManager.StopAllBGM( 1.00f );
						}
						catch( System.Exception ) {
						}

						if( Application.isPlaying ) {
							EditorSceneManager.LoadScene( EditorSceneManager.GetActiveScene().name );
						}
						else {
							EditorSceneManager.OpenScene( EditorSceneManager.GetActiveScene().path );
						}
						Event.current.Use();
					}
					break;
			}
		}


		/// <summary>
		/// プレイモード状態の変更を捕捉します
		/// </summary>
		static void OnPlayModeStateChanged( PlayModeStateChange playModeStateChange ) {
			if( PlayModeStateChange.ExitingEditMode == playModeStateChange ) {
				//EditorPrefs.SetBool( "kAutoRefresh", false );
				Time.timeScale = 1.00f;
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			if( PlayModeStateChange.ExitingPlayMode == playModeStateChange ) {
				//EditorPrefs.SetBool( "kAutoRefresh", true );
				Time.timeScale = 1.00f;
				AssetDatabase.Refresh();
			}
		}


		static int ii = 0;

		/// <summary>
		/// 
		/// </summary>
		static void OnUpdate() {
			
			if( SceneView.lastActiveSceneView ) {
				if( EditorApplication.isCompiling ) {

					GUIContent guiContent = new GUIContent();
					guiContent.text = "コンパイル中";
					guiContent.image = EditorGUIUtility.FindTexture( string.Format("WaitSpin{0:D2}",ii) );
					ii++;
					if(11<ii){
						ii=0;
					}
//					EditorGUIUtility.FindTexture( "WaitSpin00" )
					//UnityEditor.SceneView.currentDrawingSceneView.ShowNotification( guiContent );
					SceneView.lastActiveSceneView.ShowNotification( guiContent );
					SceneView.RepaintAll();
				}
				else {
					SceneView.lastActiveSceneView.RemoveNotification();
				}
			}
		}
	}
}

#endif

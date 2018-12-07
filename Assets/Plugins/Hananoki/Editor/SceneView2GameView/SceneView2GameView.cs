
#if !DISABLE_SCENEVIEW2GAMEVIEW

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Linq;

namespace HananokiEditor {

	[InitializeOnLoad]
	internal class SceneView2GameView {

		static int selected = 0;
		static bool gameview = false;

		static Rect rcPop = new Rect( 8, 0, 160, 24 );
		static Rect rcRad = new Rect( 12, 0, 160-4, 16 );
		static Rect rcToggle = new Rect( rcPop.max.x, 0, 16, 16 );

		public static void menu_ToggleSceneView2GameView() {

			if( PreferenceSettings.i.EnableSceneView2GameView.Value ) {
				SceneView.onSceneGUIDelegate += SceneView2GameView.onSceneGUI;
			}
			else {
				SceneView.onSceneGUIDelegate -= SceneView2GameView.onSceneGUI;
			}

			if( SceneView.lastActiveSceneView != null ) {
				SceneView.lastActiveSceneView.Repaint();
			}
		}
		
		static SceneView2GameView() {

			if( PreferenceSettings.i.EnableSceneView2GameView.Value ) {
				SceneView.onSceneGUIDelegate += onSceneGUI;
			}
		}

		public static void onSceneGUI( SceneView sceneView ) {
			if( !PreferenceSettings.i.EnableSceneView2GameView.Value ) {
				selected = 0;
				gameview = false;
				return;
			}


			Handles.BeginGUI();

			var cameras = UnityEngine.Object.FindObjectsOfType<Camera>();
			string[] displayNames = new string[] { "None", "" };
			ArrayUtility.AddRange( ref displayNames, cameras.Select<Camera, string>( c => c.name ).ToArray() );


			rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );
			//rcBak.y = rcPop.y - 16 - 4;
			//GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );
			//GUI.Label( rcBak, "", "HelpBox" );

			GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );
			selected = EditorGUI.Popup( rcPop, selected, displayNames, (GUIStyle) "Popup" );

			rcToggle.y = rcPop.y;
			sceneView.orthographic = GUI.Toggle( rcToggle, sceneView.orthographic, "" );
			
			int index = selected - 2;

			if( index >= 0 && index < cameras.Length ) {
				var camera = cameras[ index ];
				if( gameview ) {
					var a = camera.transform.rotation * Vector3.forward;
					a *= 25.0f;
					sceneView.pivot = camera.transform.position + a;
					sceneView.rotation = camera.transform.rotation;
					sceneView.size = 25.0f;

					gameview = false;
				}
				else {
					camera.transform.position = sceneView.camera.transform.position;
					camera.transform.rotation = sceneView.camera.transform.rotation;
				}
			}
			else {
				rcRad.y = rcPop.y - 16;
				GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );
				gameview = GUI.Toggle( rcRad, gameview, "Align Game View" );
			}
			Handles.EndGUI();
		}
	}
}


#endif // #if UNITY_EDITOR

#endif // #if !DISABLE_SCENEVIEW2GAMEVIEW

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using CallbackFunction = UnityEditor.EditorApplication.CallbackFunction;

namespace HananokiEditor {
	[InitializeOnLoad]
	public partial class EditorRuntime {

		const int WIDTH = 16;

		public class UI {
			public Texture2D PrefabNormal;
			public Texture2D PrefabModel;
			public Texture2D MissingPrefabInstance;
			public Texture2D DisconnectedPrefab;
			public Texture2D DisconnectedModelPrefab;
			public GUIStyle ControlLabel;

			public UI() {
				PrefabNormal            = getTex( "720c94b91e4c27a43accf4f16f1010c0" );
				PrefabModel             = getTex( "c7c5f92f6b55b8d478feb4d9bde48ded" );
				MissingPrefabInstance   = getTex( "e446dcdef6208e243bd1cd6abfa8f253" );
				DisconnectedPrefab      = getTex( "a67c78c4e56cec740a8bd4657e9d0777" );
				DisconnectedModelPrefab = getTex( "03b83c5634fd2854080c71e6ea74aacc" );
				ControlLabel = (GUIStyle) "ControlLabel";
			}

			Texture2D getTex( string guid ) {
				return AssetDatabase.LoadAssetAtPath<Texture2D>( AssetDatabase.GUIDToAssetPath( guid ) );
			}
		}

		public static UI s_UI;


		/// <summary>
		/// コンストラクタ
		/// </summary>
		static EditorRuntime() {
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;

			EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;

			if( PreferenceSettings.i.EnableObjectStat.Value ) {
				ObjectStat.Enable();
			}
		}


		/// <summary>
		/// エディタ上でシーン遷移した時にライトが暗くなる対応処置
		/// </summary>
		public static void SceneUpdateEnvironment() {
			if( UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.Iterative ) return;

			foreach( GameObject obj in Resources.FindObjectsOfTypeAll( typeof( GameObject ) ) ) {
				string path = UnityEditor.AssetDatabase.GetAssetOrScenePath( obj );
				bool isScene = path.Contains( ".unity" );

				if( !isScene ) continue;

				var r = obj.GetComponent<Renderer>();
				if( r == null ) continue;

				r.UpdateGIMaterials();
			}

			DynamicGI.UpdateEnvironment();
			Debug.Log( "DynamicGI.UpdateEnvironment" );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="selectionRect"></param>
		static void ProjectWindowItemCallback( string guid, Rect selectionRect ) {
			if( !PreferenceSettings.i.EnableProjectItem.Value ) return;

			drawBackColor( selectionRect, 0x00 );

			//showContextMenu( ContextTargetWindow.Project );
		}


		static void drawBackColor( Rect selectionRect, int mask ) {
			//if( _SimaSima == false ) return;

			var index = ( (int) selectionRect.y ) >> 4;

			if( ( index & 0x01 ) == mask ) {
				return;
			}

			var pos = selectionRect;
			pos.x = 0;
			pos.xMax = selectionRect.xMax;

			var color = GUI.color;
			//GUI.color = new Color( 0, 0, 0, 0.05f );
			GUI.color = PreferenceSettings.i.m_color;
			GUI.Box( pos, string.Empty );
			GUI.color = color;
		}


		/// <summary>
		/// アクティブボタン
		/// </summary>
		/// <param name="go"></param>
		/// <param name="rc"></param>
		static void DrawActiveButtonGUI( GameObject go, Rect rc ) {
			rc.width = WIDTH;
			var _b1 = go.activeSelf;
			var _b2 = GUI.Toggle( rc, _b1, string.Empty );
			if( _b1 != _b2 ) {
				if( 1 < Selection.gameObjects.Length ) {
					Undo.RecordObjects( Selection.gameObjects, "Selection.gameObjects" );
					foreach( var goo in Selection.gameObjects ) {
						goo.SetActive( _b2 );
					}
				}
				else {
					Undo.RecordObject( go, "Selection.gameObjects" );
					go.SetActive( _b2 );
				}
				//Debug.LogFormat( "Selection.gameObjects {0}", Selection.gameObjects.Length );
			}
		}


		//static readonly Vector2 iconSize = new Vector2( 32, 32 );
		static void DrawPrefabIconGUI( Rect rc ,GameObject go ) {
			
			//EditorGUIUtility.SetIconSize( iconSize );
			rc.width = WIDTH;
			var type = PrefabUtility.GetPrefabType( go );
			if( type == PrefabType.PrefabInstance ) {
				if( GUI.Button( rc, s_UI.PrefabNormal, s_UI.ControlLabel ) ) {
					//console.log( "PrefabType.PrefabInstance" );
				}
				//GUI.DrawTexture( pos, EditorGUIUtility.FindTexture( "PrefabNormal Icon" ), ScaleMode.ScaleToFit, true );
			}
			else if( type == PrefabType.ModelPrefabInstance ) {
				if( GUI.Button( rc, s_UI.PrefabModel, s_UI.ControlLabel ) ) {
					//console.log( "PrefabType.ModelPrefabInstance" );
				}
			}
			else if( type == PrefabType.DisconnectedPrefabInstance ) {
				if( GUI.Button( rc, s_UI.DisconnectedPrefab, s_UI.ControlLabel ) ) {
					//Debug.Log( "PrefabType.DisconnectedPrefabInstance" );
				}
			}
			else if( type == PrefabType.DisconnectedModelPrefabInstance ) {
				if( GUI.Button( rc, s_UI.DisconnectedModelPrefab, s_UI.ControlLabel ) ) {
					//Debug.Log( "PrefabType.DisconnectedModelPrefabInstance" );
				}
			}
			else if( type == PrefabType.MissingPrefabInstance ) {
				if( GUI.Button( rc, s_UI.MissingPrefabInstance, s_UI.ControlLabel ) ) {
					//Debug.Log( "PrefabType.MissingPrefabInstance" );
				}
			}
			//EditorGUIUtility.SetIconSize( Vector2.zero );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="selectionRect"></param>
		static void HierarchyWindowItemCallback( int instanceID, Rect selectionRect ) {

			if( !PreferenceSettings.i.EnableHierarchyItem.Value ) return;

			if( s_UI == null ) {
				s_UI = new UI();
			}

			var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
			if( go == null ) return;

			if( go.layer != LayerMask.NameToLayer( "Category" ) ) {
				drawBackColor( selectionRect, 0x01 );
			}

			//showContextMenu( ContextTargetWindow.Hierarchy );
			//EditorGUIUtility.FindTexture( "winbtn_mac_min_h" )
			

			var rcL = selectionRect;
			var rc = selectionRect;
			var max = rc.xMax;

			rcL.x = 0;
			


			rc.x = max;
			if( PreferenceSettings.i.HierarchyActive ) {
				rc.x = rc.x - WIDTH;
				DrawActiveButtonGUI( go, rc );
			}
			if( PreferenceSettings.i.HierarchyPrefab ) {
				rc.x = rc.x - WIDTH;
				DrawPrefabIconGUI( rc, go );
			}
 #if false
			if( PreferenceSettings.i.EnableHierarchyItem ) {
				rc.x = rc.x - WIDTH;
				DrawAnimationMonitor( rc, go );
			}
#endif     

			//if( _RinkakuChange ) {
			//	pos.x = pos.x - WIDTH;
			//	pos.width = WIDTH;
			//	if( GUI.Button( pos, EditorGUIUtility.FindTexture( "UnityEditor.SceneHierarchyWindow" ), (GUIStyle) "ControlLabel" ) ) {
			//		Event evt = Event.current;
			//		Vector2 mousePos = evt.mousePosition;
			//		Rect contextRect = new Rect( 0, 0, Screen.width, Screen.height );
			//		if( contextRect.Contains( mousePos ) ) {
			//			// Now create the menu, add items and show it
			//			GenericMenu menu = new GenericMenu();

			//			menu.AddItem( new GUIContent( "Hidden" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Hidden ); }, go );
			//			menu.AddItem( new GUIContent( "Wireframe" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Wireframe ); }, go );
			//			menu.AddItem( new GUIContent( "Highlight" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Highlight ); }, go );
			//			menu.ShowAsContext();
			//			evt.Use();
			//		}
			//	}
			//}


			
		}

#if false
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="go"></param>
		static void DrawAnimationMonitor( Rect rc, GameObject go ) {
			if( PreferenceSettings.i.HierarchyAnim == false ) return;

			var anim = go.GetComponent<Animator>();

			if( anim == null ) return;

			if( Monitor4AnimationCurve._monitorTransform == go.transform ) {
				GUI.Label( rc, EditorGUIUtility.IconContent( "lightMeter/redLight" ) );
				//GUI.Toggle( rcL, true, "", (GUIStyle) "Radio" );
			}
			else {
				GUI.Label( rc, EditorGUIUtility.IconContent( "lightMeter/lightRim" ) );
				//GUI.Toggle( rcL, false, "", (GUIStyle) "Radio" );
			}
			if( Event.current.type == EventType.MouseDown ) {
				if( rc.Contains( Event.current.mousePosition ) ) {
					Event.current.Use();
					if( Monitor4AnimationCurve._monitorTransform == go.transform ) {
						//var menu = new GenericMenu();
						//menu.AddItem( new GUIContent( "Set Monitor" ), true, () => {
						//Monitor4AnimationCurve.SetEnable( go.transform );
						Monitor4AnimationCurve.Disable();
						//Debug.Log( "Set Monitor" );

						//} );
						//menu.ShowAsContext();
					}
					else {
						//var menu = new GenericMenu();
						//menu.AddItem( new GUIContent( "Set Monitor" ), false, () => {
						//Monitor4AnimationCurve.SetEnable( go.transform );
						Monitor4AnimationCurve.SetEnable( go.transform );
						Debug.Log( "Set Monitor" );

						//} );
						//menu.ShowAsContext();
					}

				}
			}

		} // DrawAnimationMonitor
#endif


		
	}


}

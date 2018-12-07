

using System.IO;

using UnityEngine;
using UnityEditor;

using System.Reflection;
using HananokiEditor;
using UnityReflection;

using UnityObject = UnityEngine.Object;


namespace Hananoki {

	public class ShelfToolbar : EditorWindow {
		const string className = "ShelfToolbar";

		[MenuItem( "Window/" + Hananokia.className + "/" + className )]
		static void open() {
			GetWindow<ShelfToolbar>().titleContent = new GUIContent( "Shelf", Icon.Get( "winbtn_mac_inact" ) );
		}

		class Styles {
			public GUIStyle ToolbarPopup;
		}


		Styles m_styles;
		GUIStyle toolbarSearchField;
		GUIStyle toolbarSearchFieldCancelButton;
		GUIStyle toolbarSearchFieldCancelButtonEmpty;
		static int searchFieldHash = "SearchBoxTestWindow_SearchField".GetHashCode();

		LocalSettings m_localSettings;

		//RenderSettingsList rsl;

		string m_guid;
		bool m_guidEdit;
		//bool m_focus;
		bool m_focusLost;

		string m_guidTextEdit = "";

		GUIContent[] m_searchFilterHierarchy;
		GUIContent[] m_searchFilterProject;
		int m_searchFilterMode;
		string[] m_searchFilterModeName;



		/// <summary>
		/// Add copy-paste functionality to any text field
		/// Returns changed text or NULL.
		/// Usage: text = HandleCopyPaste (controlID) ?? text;
		/// </summary>
		public static string HandleCopyPaste( int controlID ) {
			if( controlID == GUIUtility.keyboardControl ) {
				if( Event.current.type == EventType.KeyUp && ( Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command ) ) {
					if( Event.current.keyCode == KeyCode.C ) {
						Event.current.Use();
						TextEditor editor = (TextEditor) GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
						editor.Copy();
					}
					else if( Event.current.keyCode == KeyCode.V ) {
						Event.current.Use();
						TextEditor editor = (TextEditor) GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
						editor.Paste();
						return editor.text;
					}
				}
			}
			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		void onSelectionChanged() {

			if( Selection.assetGUIDs.Length == 0 ) {
				m_guid = "---";
			}
			else {
				m_guid = Selection.assetGUIDs[ 0 ];
			}
			this.Repaint();
		}


		void OnFocus() {
			//m_focus = true;
		}
		void OnLostFocus() {
			//m_focus = false;
			m_focusLost = true;
			
		}

		/// <summary>
		/// ScriptableObject クラスのオブジェクトがスコープを外れるとき、この関数は呼び出されます。
		/// </summary>
		void OnDisable() {
			Selection.selectionChanged -= onSelectionChanged;
		}


		/// <summary>
		/// オブジェクトがロードされたとき、この関数は呼び出されます。
		/// </summary>
		public void OnEnable() {
			//EditorSceneManager.sceneLoaded += onLevelWasLoaded;
			Selection.selectionChanged += onSelectionChanged;


			//rsl = (RenderSettingsList) AssetDatabase.LoadAssetAtPath( "Assets/Unipo/RenderSettingsList.asset", typeof( RenderSettingsList ) );
			//if( rsl ) {
			//	popupNames = rsl.m_lst.Select( x => x.sceneName ).ToArray();
			//}

#if UNITY_2018
			minSize = new Vector2( 100, 14 );
#else
			minSize = new Vector2( 100, 17 );
#endif

			m_searchFilterModeName = new string[] { "SerachFilter（Hierarchy）", "SerachFilter（Project）" };
			m_searchFilterHierarchy = new GUIContent[] {
				new GUIContent("Camera"),
				new GUIContent("Light"),
				new GUIContent("CharacterController"),
				new GUIContent("Terrain"),
				new GUIContent("WindZone"),
				new GUIContent("ShinyEffectForUGUI"),
				new GUIContent("Animator"),
				new GUIContent("SpringCollider"),
				new GUIContent("ParticleSystem"),
			};
			m_searchFilterProject = new GUIContent[] {
				new GUIContent("t:Scene"),
				new GUIContent("t:SpriteAtlas"),
			};
		}


		void onLoadScene( object userData ) {
			string sceneName = (string) userData;

			EditorUtils.OpenScene( sceneName );
		}


		void onSelectObject( object userData ) {
			string path = (string) userData;

			UnityEditorProjectBrowser.SelectionChangedLockProjectWindow( path );
		}

		void onShowFolderConternts( object userData ) {
			string path = (string) userData;

			UnityEditorProjectBrowser.ShowFolderContents( path, true );
		}

		void onHierarchy_SetSearchFilter( object userData ) {
			string filter = (string) userData;

			UnityEditorHierarchy.SetSearchFilter( filter, UnityEditorHierarchy.SearchMode.Type, true );
		}

		void onProjectBrowser_SetSearch( object userData ) {
			string filter = (string) userData;

			UnityEditorProjectBrowser.SetSearch( filter );
		}


		/// <summary>
		/// ここに独自のエディターの GUI を実装します
		/// </summary>
		void OnGUI() {
			if( toolbarSearchField == null ) { toolbarSearchField = new GUIStyle( "ToolbarSeachTextField" ); }
			if( toolbarSearchFieldCancelButton == null ) { toolbarSearchFieldCancelButton = new GUIStyle( "ToolbarSeachCancelButton" ); }
			if( toolbarSearchFieldCancelButtonEmpty == null ) { toolbarSearchFieldCancelButtonEmpty = new GUIStyle( "ToolbarSeachCancelButtonEmpty" ); }

			//toolbarSearchField.fixedHeight = 18;
			//toolbarSearchFieldCancelButton.fixedHeight = 18;
			//toolbarSearchFieldCancelButtonEmpty.fixedHeight = 18;
			if( m_styles == null ) {
				m_styles = new Styles();
			}
			m_styles.ToolbarPopup = new GUIStyle( EditorStyles.toolbarPopup );
			m_styles.ToolbarPopup.alignment = TextAnchor.MiddleCenter;

			if( m_localSettings == null ) {
				m_localSettings = LocalSettings.i;
			}

			using( new GUILayout.HorizontalScope( EditorStyles.toolbar ) ) {
				GUILayout.Label( Icon.Get( "Animation.PrevKey" ), EditorStyles.toolbarButton );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//EditorApplication.ExecuteMenuItem( "Assets/Navigate Backward History %-" );
					UnityEditorProjectBrowser.lockOnce();
					//SelectionHistory.Backward();
				}
				GUILayout.Label( Icon.Get( "Animation.NextKey" ), EditorStyles.toolbarButton );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//EditorApplication.ExecuteMenuItem( "Assets/Navigate Forward History %#-" );
					UnityEditorProjectBrowser.lockOnce();
					//SelectionHistory.Forward();
				}

				GUILayout.Space( 6 );


				GUILayout.Label( Icon.Get( "SceneAsset Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					var optionsMenu = new GenericMenu();
					foreach( var e in m_localSettings.sceneList ) {
						var path = AssetDatabase.GUIDToAssetPath( e );
						if( File.Exists( path ) ) {
							optionsMenu.AddItem( new GUIContent( path.fileNameWithoutExtension() ), false, onLoadScene, path );
						}
						else {
							optionsMenu.AddDisabledItem( new GUIContent( path.fileNameWithoutExtension( ) + ": File not found" ) );
						}
					}
					optionsMenu.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}
				GUILayout.Label( Icon.Get( "SceneAsset Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					var optionsMenu = new GenericMenu();
					foreach( var e in m_localSettings.sceneList2 ) {
						var path = AssetDatabase.GUIDToAssetPath( e );
						if( File.Exists( path ) ) {
							optionsMenu.AddItem( new GUIContent( path.fileNameWithoutExtension(  ) ), false, onLoadScene, path );
						}
						else {
							optionsMenu.AddDisabledItem( new GUIContent( path.fileNameWithoutExtension(  ) + ": File not found" ) );
						}
					}
					optionsMenu.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}


				GUILayout.Label( Icon.Get( "GameManager Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					var optionsMenu = new GenericMenu();
					//optionsMenu.AddItem( new GUIContent( "ScriptingDefineSymbols" ), false, onSelectObject, "Assets/Game/ScriptingDefineSymbols.asset" );
					foreach( var e in m_localSettings.scriptableObjectList ) {
						var path = AssetDatabase.GUIDToAssetPath( e );
						if( File.Exists( path ) ) {
							optionsMenu.AddItem( new GUIContent( path.fileNameWithoutExtension(  ) ), false, onSelectObject, path );
						}
						else {
							optionsMenu.AddDisabledItem( new GUIContent( path.fileNameWithoutExtension(  ) + ": File not found" ) );
						}
					}
					optionsMenu.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}

				GUILayout.Label( Icon.Get( "Folder Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					var optionsMenu = new GenericMenu();
					optionsMenu.AddItem( new GUIContent( "Local" ), false, onShowFolderConternts, "Assets/Local" );
					optionsMenu.AddItem( new GUIContent( "CutScene" ), false, onShowFolderConternts, "Assets/Grp/CutScene" );
					optionsMenu.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}

				


				GUILayout.Space( 12 );
#if false
				GUILayout.Label( Icon.Get( "BuildSettings.Metro.Small" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( ut.hasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorApplication.ExecuteMenuItem( "Window/" + Hananokia.className + "/" + BuildWindow.className );
				}
#endif
				GUILayout.Label( Icon.Get( "GameManager Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorApplication.ExecuteMenuItem( "Window/ProjectSettingWindow" );
				}

#if UNITY_2017
				GUILayout.Label( Icon.Get( "Animation Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
#endif
#if UNITY_2018
				GUILayout.Label( Icon.Get( "UnityEditor.AnimationWindow" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
#endif
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorUtils.AnimationWindow();
				}

#if UNITY_2017
				GUILayout.Label( Icon.Get( "Animator Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
#endif
#if UNITY_2018
				GUILayout.Label( Icon.Get( "UnityEditor.Graphs.AnimatorControllerTool" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
#endif
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorUtils.AnimatorWindow();
				}
				GUILayout.Label( Icon.Get( "ViewToolZoom" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//builtinWindow.AnimatorWindow();
					EditorApplication.ExecuteMenuItem( "Window/Frame Debugger" );
				}
#if UNITY_2017
				GUILayout.Label( Icon.Get( "TimelineAsset Icon" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( ut.hasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//builtinWindow.AnimatorWindow();
					EditorApplication.ExecuteMenuItem( "Window/Timeline" );
				}
#endif
#if UNITY_2018
				GUILayout.Label( Icon.Get( "TimelineSelector" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorApplication.ExecuteMenuItem( "Window/Sequencing/Timeline" );
				}
#endif

#if UNITY_2017
				GUILayout.Label( icon.get( "Lighting" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( ut.hasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorApplication.ExecuteMenuItem( "Window/Lighting/Light Explorer" );
				}
#endif
#if UNITY_2018
				GUILayout.Label( Icon.Get( "Lighting" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorApplication.ExecuteMenuItem( "Window/Rendering/Light Explorer" );
				}
#endif
				GUILayout.Label( Icon.Get( "UnityEditor.ConsoleWindow" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//builtinWindow.ConsoleWindow();
					EditorApplication.ExecuteMenuItem( "Window/Console Pro 3" );
				}

				GUILayout.Label( Icon.Get( "WelcomeScreen.AssetStoreLogo" ), EditorStyles.toolbarButton, GUILayout.Width( 26 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					EditorUtils.AssetStoreWindow();
				}


				GUILayout.Space( 6 );
				GUILayout.Label( m_searchFilterModeName[ m_searchFilterMode ], m_styles.ToolbarPopup, GUILayout.Width( 140 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect(), EventMouseButton.R ) ) {
					m_searchFilterMode = 1 - m_searchFilterMode;
					this.Repaint();
				}
				else if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect(), EventMouseButton.L ) ) {
					var m = new GenericMenu();

					if( m_searchFilterMode == 0 ) {
						for( int i = 0; i < m_searchFilterHierarchy.Length; i++ ) {
							m.AddItem( m_searchFilterHierarchy[ i ], false, onHierarchy_SetSearchFilter, m_searchFilterHierarchy[ i ].text );
						}
					}
					else if( m_searchFilterMode == 1 ) {
						for( int i = 0; i < m_searchFilterProject.Length; i++ ) {
							m.AddItem( m_searchFilterProject[ i ], false, onProjectBrowser_SetSearch, m_searchFilterProject[ i ].text );
						}
					}

					m.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}

				//if( m_guidEdit == false ) {
				GUILayout.Label( m_guid, EditorStyles.toolbarButton, GUILayout.Width( 220 ) );
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect(), EventMouseButton.R ) ) {
					m_guidEdit = !m_guidEdit;
					Repaint();
				}
				if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
					//var path = AssetDatabase.GUIDToAssetPath( m_guid );
					//if( !string.IsNullOrEmpty( path ) ){
					//	var obj = AssetDatabase.LoadAssetAtPath<UnityObject>( path );
					//	Selection.activeObject = obj;
					//}
					var m = new GenericMenu();
					m.AddItem( new GUIContent( "クリップボードにコピー" ), false, () => { GUIUtility.systemCopyBuffer = m_guid; } );
					m.DropDown( EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ) );
				}

				if( m_focusLost  ) {
					GUIUtility.keyboardControl = 0;
					m_focusLost = false;
				}

				int textFieldID = GUIUtility.GetControlID( "TextField".GetHashCode(), FocusType.Keyboard ) + 1;
				if( textFieldID == 0 ) { }
				else {
					
					// Handle custom copy-paste
					string guid = HandleCopyPaste( textFieldID ) ?? "";
					bool bb = false;
					if( !string.IsNullOrEmpty( guid ) ) {
						m_guidTextEdit = guid;
						bb = true;
					}
					GUI.changed = false;

					//Rect rect = GUILayoutUtility.GetRect( 220f, GUILayoutUtility.GetLastRect().y );
					//rect.x += 4f;
					//rect.y += 2f;

					 // 検索ボックスを表示
					//int controlID = GUIUtility.GetControlID( searchFieldHash, FocusType.Passive, rect );
					//m_guidTextEdit = ToolbarSearchField( controlID, rect, m_guidTextEdit, false );
					m_guidTextEdit = GUILayout.TextField( m_guidTextEdit, toolbarSearchField, GUILayout.Width( 220 ) );
					if( GUI.changed || bb ) {
						var path = AssetDatabase.GUIDToAssetPath( m_guidTextEdit );
						if( !string.IsNullOrEmpty( path ) ) {
							var obj = AssetDatabase.LoadAssetAtPath<UnityObject>( path );
							EditorGUIUtility.PingObject( obj );
						}
					}
					GUILayout.Button( GUIContent.none, toolbarSearchFieldCancelButtonEmpty );
				}

				//}
				//else {
				//	int textFieldID = GUIUtility.GetControlID( "TextField".GetHashCode(), FocusType.Keyboard ) + 1;
				//	if( textFieldID == 0 ) { }
				//	else {
				//		// Handle custom copy-paste
				//		string guid = HandleCopyPaste( textFieldID ) ?? m_guid;

				//		guid = GUILayout.TextField( guid, EditorStyles.toolbarTextField, GUILayout.Width( 220 ) );
				//		if( m_guid != guid ) {
				//			//Event.current.Use();
				//			var path = AssetDatabase.GUIDToAssetPath( guid );
				//			if( !string.IsNullOrEmpty( path ) ) {
				//				var obj = AssetDatabase.LoadAssetAtPath<UnityObject>( path );
				//				EditorGUIUtility.PingObject( obj );
				//			}
				//		}
				//		m_guid = guid;
				//	}
				//	//if( ut.hasMouseClick( GUILayoutUtility.GetLastRect(), EventMouseButton.R ) ) {
				//	//	m_guidEdit = !m_guidEdit;
				//	//	Repaint();
				//	//}
				//}

				GUILayout.FlexibleSpace();
			}
		}

		string ToolbarSearchField( int id, Rect pos, string text, bool showWithPopupArrow ) {
			Rect pos2 = pos;
			pos2.width -= 14f;
			Rect pos3 = pos;
			pos3.x += pos.width;
			pos3.width = 14f;

			text = EditorGUI.TextField( pos2, text, toolbarSearchField );
			if( text == "" ) {
				GUI.Button( pos3, GUIContent.none, toolbarSearchFieldCancelButtonEmpty );
			}
			else {
				if( GUI.Button( pos3, GUIContent.none, toolbarSearchFieldCancelButton ) ) {
					text = "";
					GUIUtility.keyboardControl = 0;
				}
			}
			return text;
		}
	}
}


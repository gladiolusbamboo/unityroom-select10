
#define DEBUG_LOCAL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Linq;

using UnityEditorInternal;
using Hananoki;

namespace HananokiEditor {

	public class ObjectBrowserPreference : EditorWindow {

		public ObjectBrowser m_parentWindow;

		GUIStyle sectionHeader;

		static ReorderableList m_rList;
		static List<string> m_searchPathList;

		static string BASE = Hananokia.className + ".ObjectBrowser.ObjectType";

		static string GetPrefString( string prefName ) {
			return EditorPrefs.GetString( prefName, "" );
		}

		static void SetPrefString( string prefName, string value ) {
			EditorPrefs.SetString( prefName, value );
		}

		public static string[] GetTypeNameList() {
			return GetPrefString( BASE ).Split( ';' );
		}

		static ReorderableList MakeReorderableList() {
			m_searchPathList = new List<string>( GetTypeNameList() );
			
			//m_searchPathList.AddRange( pref.searchPath.Split( ';' ) );

			var r = new ReorderableList( m_searchPathList, typeof( string ) );

			r.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, "ObjectType" );
			};

			r.onAddCallback = ( rl ) => {
				if( m_searchPathList.Count == 0 ) {
					m_searchPathList.Add( "" );
				}
				else {
					m_searchPathList.Add( m_searchPathList[ rl.count - 1 ] );
				}
			};

			r.onRemoveCallback = ( rl ) => {
				m_searchPathList.RemoveAt( rl.index );
			};

			r.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				var p = m_searchPathList[ index ];

				rect.y += 2;
				rect.height -= 4;
				GUI.changed = false;
				p = EditorGUI.TextField( rect, p );
				if( GUI.changed ) {
					m_searchPathList[ index ] = p;
				}
			};

			return r;
		}


		void OnDestroy() {
			SetPrefString( BASE, string.Join( ";", m_searchPathList.ToArray() ) );

			m_parentWindow.OnEnable();
			m_rList = null;
		}


		/// <summary>
		/// 
		/// </summary>
		static void DrawGUI(  ) {
			if( m_rList == null ) {
				m_rList = MakeReorderableList();
			}

			m_rList.DoLayoutList();
		}

		/// <summary>
		/// Implement your own editor GUI here.
		/// </summary>
		void OnGUI() {
			try {
				var w = position.width - 10;
				var h = position.height;
				using( new EditorGUI.DisabledGroupScope( EditorApplication.isCompiling ) ) {
					using( new GUILayout.AreaScope( new Rect( 10, 10, w - 10, h ) ) ) {
						if( sectionHeader == null ) {
							sectionHeader = new GUIStyle( EditorStyles.largeLabel );
							sectionHeader.fontStyle = FontStyle.Bold;
							sectionHeader.fontSize = 18;
							sectionHeader.margin.top = 10;
							sectionHeader.margin.left++;
							if( !EditorGUIUtility.isProSkin ) {
								sectionHeader.normal.textColor = new Color( 0.4f, 0.4f, 0.4f, 1f );
							}
							else {
								sectionHeader.normal.textColor = new Color( 0.7f, 0.7f, 0.7f, 1f );
							}
						}
						GUILayout.Label( ObjectBrowser.className, sectionHeader );
						GUILayout.Space( 10f );
						DrawGUI();
					}
				}
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}


		[PreferenceItem( "ObjectBrowser" )]
		public static void PreferencesGUI() {
			DrawGUI();
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public class ObjectBrowser : EditorWindow {
		public static string className { get { return typeof( ObjectBrowser ).Name; } }

		[MenuItem( "Window/" + Hananokia.className + "/ObjectBrowser" )]
		static void open() {
			var window = EditorWindow.GetWindow<ObjectBrowser>();
			window.SetTitle( new GUIContent( "ObjectBrowser", Icon.Get( "winbtn_mac_inact" ) ));
		}

		class Styles {
			public GUIStyle toolbarbutton;
			public GUIStyle ToolbarDropDown;
			public GUIStyle ToolbarPopup;
			public Styles() {
				toolbarbutton = new GUIStyle( "toolbarbutton" );
				ToolbarDropDown = new GUIStyle( "ToolbarDropDown" );
				ToolbarPopup = new GUIStyle( "ToolbarPopup" );
				toolbarbutton.alignment = TextAnchor.MiddleCenter;

				ToolbarDropDown.alignment = TextAnchor.MiddleCenter;

				ToolbarPopup.alignment = TextAnchor.MiddleCenter;
			}
		}

		static Styles m_style;

		System.Type m_findType;

		Object[] m_objList;
		Vector2 m_ScrollPos;

		bool m_hideFlag;


		[System.Diagnostics.Conditional("DEBUG")]
		public static void log( string fmtmsg, params object[] args ) {
			UnityEngine.Debug.Log( string.Format( fmtmsg, args ) );
		}


		GUIContent[] m_contentTypeList;
		int m_selectTypeList;

		int m_selectObjIndex;



		public void OnEnable() {
			//m_contentTypeList = new GUIContent[]{
			//	new GUIContent("GameObject"),
			//	new GUIContent("Material"),
			//	new GUIContent("Collider"),
			//	new GUIContent("Texture"),
			//	new GUIContent("EditorWindow"),
			
			//	new GUIContent("GUISkin"),
			//	new GUIContent("Font"),
			//};
			m_contentTypeList = new GUIContent[ 0 ];
			foreach( var s in ObjectBrowserPreference.GetTypeNameList() ) {
				ArrayUtility.Add( ref m_contentTypeList, new GUIContent( s ) );
			}
			m_objList = null;
			log( "OnEnable" );
		}


		public void OnFocus() {
			//Selection.activeTransform = camera.transform;
			log( "OnFocus" );
			onButton_SearchStart();
		}


		void onButton_SearchStart() {
			m_findType = System.Reflection.Assembly.Load( "UnityEngine.dll" ).GetType( "UnityEngine." + m_contentTypeList[ m_selectTypeList ].text ); ;// Types.GetType( m_contentTypeList[ m_selectTypeList ].text, "UnityEngine.dll" );
			//m_findType = gutEditorHelper.GetType( m_contentTypeList[ m_selectTypeList ].text );
			if( m_findType == null ) {
				if( m_contentTypeList[ m_selectTypeList ].text == "EditorWindow" ) {
					m_findType = typeof( EditorWindow );
				}
			}
			try {
				var a = Resources.FindObjectsOfTypeAll( m_findType );
				if( m_hideFlag ) {
					m_objList = a.Where( _ => _.hideFlags != HideFlags.None ).ToArray();
				}
				else {
					m_objList = a.Where( _ => _.hideFlags == HideFlags.None ).ToArray();
				}
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}


		void selectMenuItemFunction( object userData, string[] options, int selected ) {
			m_selectTypeList = selected;
			onButton_SearchStart();
		}

		void updateSearchList() {
		}


		void forcusObject( int index ) {
			m_selectObjIndex = index;
			Selection.activeObject = m_objList[ index ];
		}

		void clampObjList() {
			if( m_objList == null ) {
				m_selectObjIndex = -1;
			}
			else {
				m_selectObjIndex = Mathf.Clamp( m_selectObjIndex, 0, m_objList.Length );
				if( m_objList.Length <= m_selectObjIndex ) {
					m_selectObjIndex = m_objList.Length - 1;
				}
			}
		}


		static void drawBackColor( Rect selectionRect, int mask ) {
			//if( _SimaSima == false ) return;

			var index =  ( (int) selectionRect.y ) >> 4;

			if( ( index & 0x01 ) == mask ) {
				return;
			}

			var pos     = selectionRect;
			pos.x = 0;
			pos.xMax = selectionRect.xMax;

			var color = GUI.color;
			GUI.color = new Color( 0, 0, 0, 0.05f );
			GUI.Box( pos, string.Empty );
			GUI.color = color;
		}






		/// <summary>
		/// 
		/// </summary>
		void OnDrawGUI() {

			EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );

			GUILayout.Label( EditorGUIUtility.IconContent( "_Popup" ), EditorStyles.toolbarButton, GUILayout.Width( 27 ) );
			if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
				var window = EditorWindow.GetWindow<ObjectBrowserPreference>( true );
				window.SetTitle( new GUIContent( "Preference" ) );
				window.m_parentWindow = this;
			}

			GUILayout.Label( m_contentTypeList[ m_selectTypeList ], m_style.ToolbarPopup, GUILayout.Width( 160 ) );
			if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
				EditorUtility.DisplayCustomMenu(
							EditorHelper.PopupRect( GUILayoutUtility.GetLastRect() ),
							m_contentTypeList,
							-1,
							selectMenuItemFunction,
							Selection.objects );
			}

			var _b = GUILayout.Toggle( m_hideFlag, "hideFlags", m_style.toolbarbutton, GUILayout.Width( 80 ) );
			if( _b != m_hideFlag ) {
				m_hideFlag = _b;
				onButton_SearchStart();
			}
			

			GUILayout.FlexibleSpace();

			EditorGUILayout.EndHorizontal();
			
			GUIStyle styleListElement = new GUIStyle( "PreferencesKeysElement" );
			

			bool _select = false;
			if( m_objList != null ) {
				m_ScrollPos = EditorGUILayout.BeginScrollView( m_ScrollPos );

				m_objList = m_objList.Where( x => x != null ).ToArray();

				for( int i =0; i < m_objList.Length; ++i ) {
					var obj  = m_objList[ i ];
					var r = GUILayoutUtility.GetRect( position.width + 1, styleListElement.fixedHeight );

					if( EditorHelper.HasMouseClick( r ) ) {
						//Debug.LogFormat( "hit {0}: {1}: {2}", obj.name, r, m_ScrollPos );
						m_selectObjIndex = i;

						_select = true;
						Repaint();
					}
					var col = styleListElement.normal.textColor;
					if( m_selectObjIndex == i ) {
						styleListElement.normal.background = styleListElement.onNormal.background;
						styleListElement.normal.textColor = styleListElement.onNormal.textColor;
					}
					else {
						styleListElement.normal.background = null;

					}

					if( obj != null ) {
						var rofs = r;
						GUI.Label( r, "", styleListElement );
						drawBackColor( r, 0x00 );
						rofs.x += 16;
						GUI.Label( rofs, obj.name, styleListElement );

						if( AssetDatabase.GetAssetOrScenePath( obj ).Contains( ".unity" ) ) {
							rofs.x -= 16;
							rofs.width = 16;
							GUI.Label( rofs, "S", styleListElement );
						}
					}

					styleListElement.normal.textColor = col;
				}

				EditorGUILayout.EndScrollView();
			}

			if( _select ) {
				forcusObject( m_selectObjIndex );

				//Selection.activeObject.hideFlags = HideFlags.None;
			}
		}




		// Windowのクライアント領域のGUI処理を記述
		void OnGUI() {
			if( m_style == null ) {
				m_style = new Styles();
			}

			var ev = Event.current;
			if( ev.type == EventType.KeyDown && ev.keyCode == KeyCode.UpArrow ) {
				m_selectObjIndex = m_selectObjIndex - 1;
				clampObjList();
				forcusObject( m_selectObjIndex );
				Repaint();
			}
			if( ev.type == EventType.KeyDown && ev.keyCode == KeyCode.DownArrow ) {
				m_selectObjIndex = m_selectObjIndex + 1;
				clampObjList();
				forcusObject( m_selectObjIndex );
				Repaint();
			}

			try {
				OnDrawGUI();
			}
			catch( MissingReferenceException me ) {
				Debug.LogError( me );
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}
		
	}
}

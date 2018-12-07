
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

using UnityEditor;
using UnityEngine;
using UnityEditorInternal;


//[Serializable]
//public class GUIStyleJson {
//	public GUIStyle[] styles;
//	public GUIStyleJson() {
//		styles = new GUIStyle[ 0 ];
//		ArrayUtility.Add( ref styles, new GUIStyle() );
//	}

//	public GUIStyle this[ int i ] {
//		set { this.styles[ i ] = value; }
//		get { return this.styles[ i ]; }
//	}
//}

namespace Harmony.Toolkit {
	public class EditorStyleEditor : EditorWindow {

		//[MenuItem( "Window/EditorStyleEditor" )]
		//public static void Init() {
		//	EditorStyleEditor w = EditorWindow.GetWindow<EditorStyleEditor>();

		//}

		public GUIStyleObject m_GUIStyleList;
		public int m_EditIndex;

		SerializedObject serializedObject;
		SerializedObject serializedObject2;

		public string m_guid;
		public string m_styleName;
		public int m_builtInStyleIndex;

		public Color m_backColor = Color.white;

		public Rect m_testRect;
		public string m_testString = "ABC --";


		ReorderableList m_reorderableList;

		Vector2 posLeft;
		Vector2 posRight;
		GUIStyle styleLeftView;
		GUIStyle styleRightView;
		public float splitterPos;
		Rect splitterRect;
		Vector2 dragStartPos;
		bool dragging;
		float splitterWidth = 5;

		public int m_uiType;
		public bool m_uiToggle;


		void OnEnable() {

			//m_GUIStyleJson = JsonUtility.FromJson( File.ReadAllText( "Assets/hoge.json" ), typeof( GUIStyleJson ) ) as GUIStyleJson;
			//if( m_GUIStyleJson == null ) {
			//	m_GUIStyleJson = new GUIStyleJson();
			//}

			serializedObject = new SerializedObject( this );
			m_styleName = "";


			m_testRect = new Rect( 8, 8, 100, 32 );
		}


		void OnDisable() {
			//m_GUIStyleJson.styles[ 0 ] = m_style;
			//string s = JsonUtility.ToJson( m_GUIStyleList,true );

			//File.WriteAllText( "Assets/hoge.json", s );
			//AssetDatabase.SaveAssets();
			//AssetDatabase.Refresh();
		}


		public ReorderableList makeReorderableList() {
			ReorderableList rlist;

			serializedObject2 = new SerializedObject( m_GUIStyleList );
			//var sp = serializedObject.GetIterator();
			//// 最初にNext(true)をする
			//while( sp.Next( true ) ) {
			//	Debug.Log( sp.propertyPath );
			//}

			rlist = new ReorderableList( serializedObject2, serializedObject2.FindProperty( "styles" ) );
			//rlist = new ReorderableList( m_GUIStyleJson.styles, typeof(GUIStyle) );

			return rlist;
		}


		void exportTexturePng( Texture2D m_tex ) {
			Texture2D texCopy = new Texture2D( m_tex.width, m_tex.height, m_tex.format, m_tex.mipmapCount > 1 );
			texCopy.LoadRawTextureData( m_tex.GetRawTextureData() );
			texCopy.Apply();

			byte[] pngData = texCopy.EncodeToPNG();   // pngのバイト情報を取得.

			// ファイルダイアログの表示.
			string filePath = EditorUtility.SaveFilePanel( "Save Texture", "", m_tex.name + ".png", "png" );

			if( filePath.Length > 0 ) {
				// pngファイル保存.
				File.WriteAllBytes( filePath, pngData );
			}
		}


		void drawGUILeft() {
			GUIStyle style = m_GUIStyleList[ m_reorderableList.index ];

			GUILayout.Space( 64 );
			var oldindex = m_builtInStyleIndex;
			using( new GUILayout.HorizontalScope() ) {
				int oldIndex = m_builtInStyleIndex;
				var newIndex = EditorGUILayout.Popup( m_builtInStyleIndex, GUI.skin.customStyles.Select( x => new GUIContent( x.name ) ).ToArray(), "Popup" );
				if( GUILayout.Button( "+", GUILayout.ExpandWidth( false ) ) ) {
					newIndex++;
				}
				if( GUILayout.Button( "-", GUILayout.ExpandWidth( false ) ) ) {
					newIndex--;
				}
				if( GUILayout.Button( "<", GUILayout.ExpandWidth( false ) ) ) {
					m_GUIStyleList[ m_reorderableList.index ] = new GUIStyle( EditorStyles.largeLabel );
					style = m_GUIStyleList[ m_EditIndex ];
				}
				if( GUILayout.Button( "w", GUILayout.ExpandWidth( false ) ) ) {
					m_GUIStyleList[ m_reorderableList.index ] = new GUIStyle( "window" );
					style = m_GUIStyleList[ m_EditIndex ];
				}

				if( GUILayout.Button( "background export", GUILayout.ExpandWidth( false ) ) ) {
					exportTexturePng( style.normal.background );
				}

				//if( oldIndex != newIndex ) {
				//	m_builtInStyleIndex = newIndex;
				//	GUI.skin.customStyles[ newIndex ];
				//}
				if( newIndex != oldindex ) {
					m_builtInStyleIndex = newIndex;
					m_GUIStyleList[ m_reorderableList.index ] = new GUIStyle( GUI.skin.customStyles[ m_builtInStyleIndex ] );
					style = m_GUIStyleList[ m_EditIndex ];
				}
			}


			m_backColor = EditorGUILayout.ColorField( m_backColor );

			if( style != null ) {
				var backgroundColor = GUI.backgroundColor;

				//float f = 222.0f / 255.0f;
				GUI.backgroundColor = m_backColor;

				if( m_uiType == 0 ) {
					GUI.Label( m_testRect, m_testString, style );
				}
				if( m_uiType == 1 ) {
					GUI.Button( m_testRect, m_testString, style );
				}
				if( m_uiType == 2 ) {
					m_uiToggle = GUI.Toggle( m_testRect, m_uiToggle, m_testString, style );
				}

				GUI.backgroundColor = backgroundColor;
			}

			GUILayout.Space( 16 );
			serializedObject.Update();
			//var prop = serializedObject.FindProperty( "m_style" );
			//if( prop != null ) {
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "splitterPos" ), true );

			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_uiType" ), true );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_testString" ), true );
			m_testString = EditorGUILayout.TextArea( m_testString );

			EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_testRect" ), true );
			//EditorGUILayout.PropertyField( prop, true );
			//}

			serializedObject.ApplyModifiedProperties();

			serializedObject2.Update();
			EditorGUILayout.PropertyField( serializedObject2.FindProperty( "styles.Array.data[" + m_reorderableList.index + "]" ), true );
			serializedObject2.ApplyModifiedProperties();
		}


		void drawGUIRight() {
			serializedObject2.Update();
			m_reorderableList.DoLayoutList();
			serializedObject2.ApplyModifiedProperties();
		}


		void OnGUI() {
			if( m_GUIStyleList == null ) {
				//	m_GUIStyleList = ScriptableObject.CreateInstance<GUIStyleList>();
				//	ProjectWindowUtil.CreateAsset( m_GUIStyleList, "New ToolbarLaunchSettings.asset" );
				return;
			}
			if( m_reorderableList == null ) {
				m_reorderableList = makeReorderableList();
				m_reorderableList.index = 0;

				position = new Rect( position.x, position.y, 1000, position.height );
				splitterPos = position.width * 0.65f;
				Repaint();
			}

			if( styleLeftView == null ) {
				styleLeftView = new GUIStyle( GUI.skin.box );
			}
			if( styleRightView == null ) {
				styleRightView = new GUIStyle( GUI.skin.button );
			}

			GUILayout.BeginHorizontal();

			// Left view
			posLeft = GUILayout.BeginScrollView( posLeft, GUILayout.Width( splitterPos ), GUILayout.MaxWidth( splitterPos ), GUILayout.MinWidth( splitterPos ) );

			drawGUILeft();
			//GUILayout.Box( "Left View",
			//				styleLeftView,
			//				GUILayout.ExpandWidth( true ),
			//				GUILayout.ExpandHeight( true ) );
			GUILayout.EndScrollView();

			// Splitter
			GUILayout.Box( "", GUILayout.Width( splitterWidth ), GUILayout.MaxWidth( splitterWidth ), GUILayout.MinWidth( splitterWidth ), GUILayout.ExpandHeight( true ) );
			splitterRect = GUILayoutUtility.GetLastRect();


			// Right view
			posRight = GUILayout.BeginScrollView( posRight, GUILayout.ExpandWidth( true ) );
			//GUILayout.Box( "Right View",
			//styleRightView,
			//GUILayout.ExpandWidth( true ),
			//GUILayout.ExpandHeight( true ) );
			drawGUIRight();

			GUILayout.EndScrollView();

			GUILayout.EndHorizontal();

			// Splitter events
			if( Event.current != null ) {
				switch( Event.current.rawType ) {
					case EventType.MouseDown:
						if( splitterRect.Contains( Event.current.mousePosition ) ) {
							//Debug.Log( "Start dragging" );
							dragging = true;
						}
						break;
					case EventType.MouseDrag:
						if( dragging ) {
							//Debug.Log( "moving splitter" );
							splitterPos += Event.current.delta.x;
							Repaint();
						}
						break;
					case EventType.MouseUp:
						if( dragging ) {
							//Debug.Log( "Done dragging" );
							dragging = false;
						}
						break;
				}
			}
			return;


		}
	}
}



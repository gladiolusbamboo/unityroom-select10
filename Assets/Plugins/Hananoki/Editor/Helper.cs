
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

using System.Reflection;
using System;
using System.Linq;
using UnityEditor.Animations;


namespace HananokiEditor {
	public static class Helper {

		private static GUIContent s_Text = new GUIContent();
		public static GUIContent TempContent( string t ) {
			s_Text.text = t;
			return s_Text;
		}
		private static GUIContent s_TextTool = new GUIContent();
		public static GUIContent TempContent( string t, string t2 ) {
			s_TextTool.text = t;
			s_TextTool.tooltip = t2;
			return s_TextTool;
		}
		private static GUIContent s_TextImage = new GUIContent();
		public static GUIContent TempContent( string t, Texture i ) {
			s_TextImage.image = i;
			s_TextImage.text = t;
			return s_TextImage;
		}

		private static GUIContent s_ContentImage = new GUIContent();
		public static GUIContent TempContent( Texture image, string tooltip = "" ) {
			s_ContentImage.image = image;
			s_ContentImage.tooltip = tooltip;
			return s_ContentImage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="type">0=左 1=右</param>
		/// <returns></returns>
		//public static bool hasMouseClick( Rect rc, int type = 0 ) {
		//	var ev = Event.current;
		//	var pos = ev.mousePosition;
		//	if( ev.type == EventType.MouseDown && ev.button == type ) {
		//		if( rc.x < pos.x && pos.x < rc.max.x && rc.y < pos.y && pos.y < rc.max.y ) {
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		public static void SetBoldFont( SerializedProperty prop ) {
			if( prop.prefabOverride ) {
				GUI.skin.font = EditorStyles.boldFont;
			}
			else {
				GUI.skin.font = EditorStyles.standardFont;
			}
		}

		public static void ForceReloadInspectors() {
			var _ForceReloadInspectors = typeof( UnityEditor.EditorUtility ).GetMethod( "ForceReloadInspectors", BindingFlags.NonPublic | BindingFlags.Static );
			_ForceReloadInspectors.Invoke( null, null );
		}


		internal static void SetPrefabOverride( object userData ) {
			SerializedProperty serializedProperty = (SerializedProperty) userData;

			serializedProperty.serializedObject.Update();
			serializedProperty.prefabOverride = false;
			serializedProperty.serializedObject.ApplyModifiedProperties();

			//Assembly assembly = typeof( UnityEditor.EditorUtility ).Assembly;

			ForceReloadInspectors();
			//EditorUtility.ForceReloadInspectors();
			//Debug.Log(aaa);
			GUI.FocusControl( "" );

			serializedProperty.serializedObject.Update();
			serializedProperty.prefabOverride = false;
			serializedProperty.serializedObject.ApplyModifiedProperties();
			ForceReloadInspectors();
		}


		private static Type typeFoldoutTitlebar;
		private static MethodInfo methodInfoFoldoutTitlebar;
		private static MethodInfo EditorGUI_FoldoutTitlebar;

		public static bool FoldoutTitlebar( Rect rect, bool foldout, GUIContent label, bool skipIconSpacing ) {
			if( EditorGUI_FoldoutTitlebar == null ) {
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017_1_OR_NEWER
				var t = System.Reflection.Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.EditorGUI" );
#else
				typeFoldoutTitlebar = Types.GetType( "UnityEditor.EditorGUILayout", "UnityEditor.dll" );
#endif
				EditorGUI_FoldoutTitlebar = t.GetMethod( "FoldoutTitlebar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static );
			}

			var obj = EditorGUI_FoldoutTitlebar.Invoke( null, new object[] { rect, label, foldout, skipIconSpacing } );
			return Convert.ToBoolean( obj );
		}

		public static bool FoldoutTitlebar( Rect rect, bool foldout, string label, bool skipIconSpacing ) {
			return FoldoutTitlebar( rect, foldout, Helper.TempContent( label ), skipIconSpacing );
		}



		public static bool FoldoutTitlebar( bool foldout, GUIContent label, bool skipIconSpacing ) {
			if( methodInfoFoldoutTitlebar == null ) {
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017_1_OR_NEWER
				typeFoldoutTitlebar = System.Reflection.Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.EditorGUILayout" );
#else
				typeFoldoutTitlebar = Types.GetType( "UnityEditor.EditorGUILayout", "UnityEditor.dll" );
#endif
				methodInfoFoldoutTitlebar = typeFoldoutTitlebar.GetMethod( "FoldoutTitlebar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static );
			}

			var obj = methodInfoFoldoutTitlebar.Invoke( null, new object[] { foldout, label, skipIconSpacing } );
			return Convert.ToBoolean( obj );
		}
		public static bool FoldoutTitlebar( bool foldout, string label, bool skipIconSpacing ) {
			return FoldoutTitlebar( foldout, Helper.TempContent( label ), skipIconSpacing );
		}



		public static bool AnimationControllerIsRegistered( UnityEditor.Animations.AnimatorController controller, AnimationClip clip ) {
			var st = controller.layers[ 0 ].stateMachine.states;

			if( 0 <= System.Array.FindIndex( controller.animationClips, ( c ) => { return c.name == clip.name; } ) ) {
				int i = System.Array.FindIndex( st, c => c.state.motion == clip );
				if( 0 <= i ) {
					if( st[ i ].state.name != clip.name ) {
						//console.log( st[ i ].state.name );
						st[ i ].state.name = clip.name;
					}
				}
				return true;
			}
			return false;
		}
	}


	public static class Extensions {

		/// <summary>
		/// スネークケースをアッパーキャメル(パスカル)ケースに変換します
		/// 例) quoted_printable_encode → QuotedPrintableEncode
		/// </summary>
		public static string SnakeToUpperCamel( this string self ) {
			if( string.IsNullOrEmpty( self ) ) return self;

			return self
					.Split( new[] { '_' }, StringSplitOptions.RemoveEmptyEntries )
					.Select( s => char.ToUpperInvariant( s[ 0 ] ) + s.Substring( 1, s.Length - 1 ) )
					.Aggregate( string.Empty, ( s1, s2 ) => s1 + s2 )
			;
		}

		/// <summary>
		/// スネークケースをローワーキャメル(キャメル)ケースに変換します
		/// 例) quoted_printable_encode → quotedPrintableEncode
		/// </summary>
		//public static string SnakeToLowerCamel( this string self ) {
		//	if( string.IsNullOrEmpty( self ) ) return self;

		//	return self
		//			.SnakeToUpperCamel()
		//			.Insert( 0, char.ToLowerInvariant( self[ 0 ] ).ToString() ).Remove( 1, 1 )
		//	;
		//}

		public static void SetTitle( this EditorWindow wnd, GUIContent cont ) {
			var property = typeof( EditorWindow ).GetProperty( "titleContent" );

			if( property != null ) {
				// インスタンスの値を取得
				var beforeName = property.GetValue( wnd, null );

				// インスタンスに値を設定
				property.SetValue( wnd, cont, null );
			}
			else {
#if UNITY_5_0
				wnd.title = cont.text;
#else
				wnd.titleContent = cont;
#endif
			}
		}


		public static string colorTag( this string s, string color ) {
			return "<color=" + s + ">" + s + "</color>";
		}

		public static string quote( this string s ) {
			return '"' + s + '"';
		}

		public static string GUID2Path( this string guid )  {
			return AssetDatabase.GUIDToAssetPath( guid );
		}

		/// <summary>
		/// 指定したアセットのGUIDを返します
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string toGUID( UnityEngine.Object obj ) {
			var a = AssetDatabase.GetAssetPath( obj );
			var b = AssetDatabase.AssetPathToGUID( a );
			return b;
		}


		public static T GUID2Asset<T>( this string guid ) where T : UnityEngine.Object {
			var a = AssetDatabase.GUIDToAssetPath( guid );
			T b = AssetDatabase.LoadAssetAtPath( a, typeof( T ) ) as T;
			return b;
		}
		public static UnityEngine.Object GUID2Asset( this string guid ) {
			var a = AssetDatabase.GUIDToAssetPath( guid );
			var b = (UnityEngine.Object) AssetDatabase.LoadAssetAtPath( a, typeof( UnityEngine.Object ) );
			return b;
		}
		public static UnityEngine.Object pathAsset( this string path ) {
			var b = (UnityEngine.Object) AssetDatabase.LoadAssetAtPath( path, typeof( UnityEngine.Object ) );
			return b;
		}

		public static int toInt( this string s ) {
			return int.Parse( s );
		}


		public static float toRealTime( this AnimationClip clip ) {
			return clip.frameRate * clip.length;
		}



		public static string fileNameWithoutExtension( this string s ) {
			return Path.GetFileNameWithoutExtension( s );
		}
		
		public static string directoryName( this string s ) {
			return Path.GetDirectoryName( s );
		}
		
		public static string fileName( this string s ) {
			return Path.GetFileName( s );
		}
		
		public static string extension( this string s ) {
			return Path.GetExtension( s );
		}



		public static class EditorHelper {
			public static void showNotification( string text ) {
				if( SceneView.lastActiveSceneView ) {
					GUIContent guiContent = new GUIContent();
					guiContent.text = text;
					guiContent.image = EditorGUIUtility.FindTexture( "SceneAsset Icon" );

					//UnityEditor.SceneView.currentDrawingSceneView.ShowNotification( guiContent );
					SceneView.lastActiveSceneView.ShowNotification( guiContent );
					SceneView.RepaintAll();
				}
			}
		}
	}
}

#endif

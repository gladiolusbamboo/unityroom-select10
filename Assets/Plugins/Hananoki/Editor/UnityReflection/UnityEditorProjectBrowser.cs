
using UnityEngine;
using UnityEditor;

using System.Reflection;
using System;

namespace UnityReflection {
	public static class UnityEditorProjectBrowser {

		static EditorWindow s_ProjectBrowserWindow;
		static Type s_ProjectBrowserType;
		static MethodInfo s_ProjectBrowser_SetSearch;
		//static MethodInfo m_ProjectBrowser_FrameObject;
		static MethodInfo m_ProjectBrowser_ShowFolderContents;

		static FieldInfo s_ProjectBrowser_m_IsLocked;

		static MethodInfo m_ProjectBrowser_ValidateCreateNewAssetPath;

		/// <summary>
		/// 
		/// </summary>
		static void init() {

			if( s_ProjectBrowserType == null ) {
				s_ProjectBrowserType = typeof( UnityEditor.EditorWindow ).Assembly.GetType( "UnityEditor.ProjectBrowser" );
			}

			if( s_ProjectBrowserWindow == null ) {
				s_ProjectBrowserWindow = EditorWindow.GetWindow( s_ProjectBrowserType, false, "Project", false );
			}

			if( s_ProjectBrowserType != null ) {
				foreach( MethodInfo mi in s_ProjectBrowserType.GetMethods( BindingFlags.Public | BindingFlags.Instance ) ) {
					if( mi.Name == "SetSearch" ) {
						var para = mi.GetParameters();
						if( para[ 0 ].Name == "searchString" ) {
							s_ProjectBrowser_SetSearch = mi;
							break;
						}
					}
				}

				//m_ProjectBrowser_FrameObject = m_ProjectBrowserType.GetMethod( "FrameObject", BindingFlags.Public | BindingFlags.Instance );

				m_ProjectBrowser_ShowFolderContents = s_ProjectBrowserType.GetMethod( "ShowFolderContents", BindingFlags.NonPublic | BindingFlags.Instance );
				//if( m_ProjectBrowser_ShowFolderContents == null ) {
				//	Debug.Log( "m_ProjectBrowser_ShowFolderContents が GetMethod 失敗" );
				//}
				//else {
				//	Debug.Log( "m_ProjectBrowser_ShowFolderContents が ok" );
				//}
				s_ProjectBrowser_m_IsLocked = s_ProjectBrowserType.GetField( "m_IsLocked", BindingFlags.NonPublic | BindingFlags.Instance );

				m_ProjectBrowser_ValidateCreateNewAssetPath = s_ProjectBrowserType.GetMethod( "ValidateCreateNewAssetPath", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static );
			}
		}



		public static EditorWindow Window() {
			init();
			return s_ProjectBrowserWindow;
		}

		public static Type Type() {
			init();
			return s_ProjectBrowserType;
		}

		/// <summary>
		/// プロジェクトブラウザの検索文字列を指定します
		/// </summary>
		/// <param name="searchString"></param>
		public static void SetSearch( string searchString ) {
			init();

			s_ProjectBrowser_SetSearch.Invoke( s_ProjectBrowserWindow, new object[] { searchString } );
		}



		public static bool m_IsLocked {
			get {
				init();
				if( s_ProjectBrowser_m_IsLocked != null ) {
					return (bool) s_ProjectBrowser_m_IsLocked.GetValue( s_ProjectBrowserWindow );
				}
				else {
					return false;
				}
			}
			set {
				init();
				if( s_ProjectBrowser_m_IsLocked != null ) {
					s_ProjectBrowser_m_IsLocked.SetValue( s_ProjectBrowserWindow, value );
				}
			}
		}

		//static void m_ProjectBrowser_m_IsLocked_SetValue( bool b ) {
		//	Project_InitHelper();
		//	m_ProjectBrowser_m_IsLocked.SetValue( m_ProjectBrowserWindow, b );
		//}



		static void unlock() {
			m_IsLocked = false;
			Selection.selectionChanged -= unlock;
		}

		/// <summary>
		/// プロジェクトブラウザの表示状態を維持してオブジェクトの選択を行います
		/// </summary>
		/// <param name="obj"></param>
		public static void SelectionChangedLockProjectWindow( UnityEngine.Object obj ) {
			if( obj == null ) return;
			SelectionChangedLockProjectWindow( AssetDatabase.GetAssetPath( obj ) );
		}

		public static void SelectionChangedLockProjectWindow( string path ) {
			if( string.IsNullOrEmpty( path ) ) return;

			UnityEditorProjectBrowser.m_IsLocked = true;
			Selection.selectionChanged += unlock;

			Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( path );
		}


		public static void lockOnce() {
			UnityEditorProjectBrowser.m_IsLocked = true;
			Selection.selectionChanged += unlock;
		}

		public static void ShowFolderContents( string assetPath, bool revealAndFrameInFolderTree ) {
			var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( assetPath );
			if( obj == null ) return;

			UnityEditorProjectBrowser.ShowFolderContents( obj.GetInstanceID(), true );
		}

		public static void ShowFolderContents( int folderInstanceID, bool revealAndFrameInFolderTree ) {
			init();
			
			if( m_ProjectBrowser_ShowFolderContents == null ) {
				Debug.LogError( "m_ProjectBrowser_ShowFolderContents is null." );
				return;
			}

			m_ProjectBrowser_ShowFolderContents.Invoke( s_ProjectBrowserWindow, new object[] { folderInstanceID, revealAndFrameInFolderTree } );
		}


		public static string ValidateCreateNewAssetPath( string pathName ) {
			init();
			if( m_ProjectBrowser_ValidateCreateNewAssetPath == null ) {
				Debug.LogError( "m_ProjectBrowser_ValidateCreateNewAssetPath is null." );
				return null;
			}

			var obj = m_ProjectBrowser_ValidateCreateNewAssetPath.Invoke( s_ProjectBrowserWindow, new object[] { pathName } );
			return obj as string;
		}
	}
}

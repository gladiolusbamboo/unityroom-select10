
using UnityEditor;
using System.Reflection;
using System;

namespace UnityReflection {
	public static class UnityEditorHierarchy {

		public enum SearchMode {
			All,
			Name,
			Type,
			Label,
			AssetBundleName
		}

		static EditorWindow s_SceneHierarchyWindow;
		static Type s_SceneHierarchyWindowType;
		static MethodInfo s_SceneHierarchyWindowr_SetSearchFilter;

		static void init() {
			if( s_SceneHierarchyWindow == null ) {
				Assembly assembly = typeof( EditorWindow ).Assembly;
				s_SceneHierarchyWindow = EditorWindow.GetWindow( assembly.GetType( "UnityEditor.SceneHierarchyWindow" ), false, "Hierarchy", false );
			}

			if( s_SceneHierarchyWindow == null ) return;

			s_SceneHierarchyWindowType = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.SceneHierarchyWindow" );
			foreach( MethodInfo mi in s_SceneHierarchyWindowType.GetMethods( BindingFlags.NonPublic | BindingFlags.Instance ) ) {
				if( mi.Name == "SetSearchFilter" ) {
					var para = mi.GetParameters();
					if( para[ 0 ].Name == "searchFilter" ) {
						s_SceneHierarchyWindowr_SetSearchFilter = mi;
						break;
					}
				}
			}
		}


		public static EditorWindow GetWindow() {
			init();
			return s_SceneHierarchyWindow;
		}

		public static void SetSearchFilter( string searchFilter, SearchMode mode = SearchMode.All, bool setAll = false ) {
			init();
			s_SceneHierarchyWindowr_SetSearchFilter.Invoke( s_SceneHierarchyWindow, new object[] { searchFilter, (int) mode, setAll } );
		}
	}
}

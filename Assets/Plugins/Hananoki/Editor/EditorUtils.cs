
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Reflection;

namespace HananokiEditor {
	public static class EditorUtils  {

		public static void RepaintEditorWindow() {
			foreach( EditorWindow a in Resources.FindObjectsOfTypeAll( typeof( EditorWindow ) ) ) {
				a.Repaint();
			}
		}

		public static EditorWindow InspectorWindow() {
			System.Reflection.Assembly assembly = typeof( UnityEditor.EditorWindow ).Assembly;
			return EditorWindow.GetWindow( assembly.GetType( "UnityEditor.InspectorWindow" ) );
		}

		public static EditorWindow ProjectBrowser() {
			var t = typeof( UnityEditor.EditorWindow ).Assembly.GetType( "UnityEditor.ProjectBrowser" );

			return EditorWindow.GetWindow( t, false, "Project", false );
		}
		public static EditorWindow SceneHierarchyWindow() {
			var t = typeof( UnityEditor.EditorWindow ).Assembly.GetType( "UnityEditor.SceneHierarchyWindow" );

			return EditorWindow.GetWindow( t, false, "Hierarchy", false );
		}

		public static EditorWindow AnimationWindow() {
			//アニメーションウィンドウ
			var asm = Assembly.Load( "UnityEditor" );
			var typeAnimWindow = asm.GetType( "UnityEditor.AnimationWindow" );
			return EditorWindow.GetWindow( typeAnimWindow, true, "Animation", false );
		}

		public static EditorWindow AnimatorWindow() {
			var asm2 = Assembly.Load( "UnityEditor.Graphs" );
			Module editorGraphModule = asm2.GetModule( "UnityEditor.Graphs.dll" );
			var typeAnimatorWindow = editorGraphModule.GetType( "UnityEditor.Graphs.AnimatorControllerTool" );
			return EditorWindow.GetWindow( typeAnimatorWindow, true, "Animator", false );
		}

		public static EditorWindow ConsoleWindow() {
			var asm = Assembly.Load( "UnityEditor" );
			var typeConslWindow = asm.GetType( "UnityEditor.ConsoleWindow" );
			return EditorWindow.GetWindow( typeConslWindow, true, "Console", false );
		}

		public static EditorWindow AssetStoreWindow() {
			//アセットストアウィンドウ
			var asm = Assembly.Load( "UnityEditor" );
			var typeAssetWindow = asm.GetType( "UnityEditor.AssetStoreWindow" );
			return EditorWindow.GetWindow( typeAssetWindow, true, "Asset Store", false );
		}

		/// <summary>
		/// 変更点があるかチェックしてからシーンを開きます
		/// </summary>
		/// <param name="sceneName"></param>
		public static void OpenScene( string sceneName ) {
			if( EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() ) {
				EditorSceneManager.OpenScene( sceneName );
			}
		}
	}
}

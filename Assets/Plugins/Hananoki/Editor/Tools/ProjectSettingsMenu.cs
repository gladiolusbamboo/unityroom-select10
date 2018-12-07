
using UnityEditor;
using UnityEngine;

using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEditorTools {
	public static class ProjectSettingsMenu {
		const string NAME = "プロジェクト設定/";

		//[MenuItem( NAME + "SaveAssets ＆ Refresh", false, 10 )]
		//public static void Open_Refresh() {
		//	AssetDatabase.SaveAssets();
		//	AssetDatabase.Refresh();
		//}
#if UNITY_2018_3_OR_NEWER
		[MenuItem( NAME + "プリファレンス", false, 33 )]
		public static void Open_Preference() {
			EditorApplication.ExecuteMenuItem( "Edit/Preferences..." );
		}

		[MenuItem( NAME + "プロジェクト" )]
		public static void Open_Input() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings..." );
		}

#else
		[MenuItem( NAME + "プリファレンス", false, 33 )]
		public static void Open_Preference() {
			//var asm = Assembly.GetAssembly( typeof( EditorWindow ) );
			//var T = asm.GetType( "UnityEditor.PreferencesWindow" );
			//var M = T.GetMethod( "ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static );
			//M.Invoke( null, null );

			var asm = Assembly.Load( "UnityEditor" );
			var pref_win_type = asm.GetType( "UnityEditor.PreferencesWindow" );
			var wind = EditorWindow.GetWindow( pref_win_type, false, "Preferences" );
			//var ico = GameEditorUtility.loadIcon( "Icon_25-005" );
			//wind.titleContent = new GUIContent( "Preferences", ico );
		}

		[MenuItem( NAME + "入力" )]
		public static void Open_Input() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Input" );
		}

		[MenuItem( NAME + "タグとレイヤー" )]
		public static void Open_Tags_and_Layers() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Tags and Layers" );
		}

		[MenuItem( NAME + "時間" )]
		public static void Open_Time() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Time" );
		}

		[MenuItem( NAME + "プレイヤー" )]
		public static void Open_Player() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Player" );
		}

		[MenuItem( NAME + "物理" )]
		public static void Open_Physics() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Physics" );
		}

		[MenuItem( NAME + "物理2D" )]
		public static void Open_Physics_2D() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Physics 2D" );
		}

		[MenuItem( NAME + "クオリティ" )]
		public static void Open_Quality() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Quality" );
		}

		[MenuItem( NAME + "グラフィック" )]
		public static void Open_Graphics() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Graphics" );
		}

		[MenuItem( NAME + "ネットワーク" )]
		public static void Open_Network() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Network" );
		}

		[MenuItem( NAME + "エディタ" )]
		public static void Open_Editor() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Editor" );
		}

		[MenuItem( NAME + "スクリプト優先" )]
		public static void Open_Script_Execution_Order() {
			EditorApplication.ExecuteMenuItem( "Edit/Project Settings/Script Execution Order" );
		}
#endif
	}
}

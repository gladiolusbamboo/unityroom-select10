
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

using System.IO;
using System.Linq;
using System.Reflection;

using Hananoki;
using UnityReflection;

namespace HananokiEditor {

	public static partial class EditorMenu {
		public const string NAME = "Tools/";

#if LOCAL_TEST
		[MenuItem( NAME + "Export Unitypackage" )]
		public static void Export() {
			var assets = new[] {
				"Assets/Plugins/Hananoki",
			};
			var exportPath = "Hananoki.unitypackage";

			AssetDatabase.ExportPackage(
					assets,
					exportPath,
					ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse |
					ExportPackageOptions.IncludeLibraryAssets );
		}
#endif

		//static void newInspec(string guid) {
		//	var obj = Selection.activeObject;
		//	Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( AssetDatabase.GUIDToAssetPath( guid ) );

		//	var editorType = typeof( Editor );
		//	var inspectorWindowType = editorType.Assembly.GetType( "UnityEditor.InspectorWindow" );
		//	var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		//	var flipLockedMethod = inspectorWindowType.GetMethod( "FlipLocked", bindingAttr );
		//	var inspector = ScriptableObject.CreateInstance( inspectorWindowType ) as EditorWindow;

		//	inspector.titleContent = new GUIContent(Selection.activeObject.name);
		//	inspector.Show( true );
		//	inspector.Repaint();
		//	flipLockedMethod.Invoke( inspector, null );

		//	Selection.activeObject = obj;
		//}


		[MenuItem( NAME + @"Export: シーン&レイヤー&タグ", false, 45 )]
		public static void menuGame_WriteLayerName() {
			if( !menuGame_WriteSceneNameCheck() ) {
				return;
			}

			Export_LayerTagScene.MakeLayerNameFile();
			Export_LayerTagScene.MakeTagNameFile();
			Export_LayerTagScene.MakeSceneNameFile();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		/// <summary>
		/// シーン名を定数で管理するクラスを作成できるかどうかを取得します
		/// </summary>
		[MenuItem( NAME + "Export: シーン&レイヤー&タグ", true )]
		[MenuItem( NAME + "UpdateRigを検索する", true )]
		[MenuItem( NAME + "Export: シーンリストを作成", true )]
		public static bool menuGame_WriteSceneNameCheck() {
			return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
		}


		[MenuItem( NAME + "Export: シーンリストを作成", false, 45 )]
		public static void MakeSceneList() {
			var lst = EditorHelper.getBuildSceneName();
			var dia = AssetDatabase.GUIDToAssetPath( LocalSettings.i.folderSceneList );
			var output =  dia + @"\SceneMenu.cs";
			//output = output.Replace( "/", "\\" );
			EditorHelper.writeFile( output, ( a ) => {
				a.AppendLine( @"/* エディタースクリプト側でコンパイルしてください */" );
				a.AppendLine( @"#if UNITY_EDITOR" );
				a.AppendLine( @"using UnityEditor;" );
				a.AppendLine( @"using UnityEngine;" );
				a.AppendLine( @"using HananokiEditor;" );
				a.AppendLine();
				a.AppendLine( @"public static partial class SceneMenu {" );
				//a.AppendLine( @"	public const string MENU_NAME = ""シーンリスト/"";" );
				for( int i = 0; i < lst.Length; i++ ) {
					var s = lst[ i ];
					var _s00 = string.Format( @"	[MenuItem( ""ゲームシーン/{0:D2}: {1}"", false, 1 )]", i, Path.GetFileNameWithoutExtension( lst[ i ] ) );
					a.AppendLine( _s00 );
					var _s01 = string.Format( @"	public static void SceneMenu_b{0:D2}() {{", i );
					a.AppendLine( _s01 );
					a.AppendFormat( @"		EditorUtils.OpenScene( ""{0}"" );", lst[ i ] ).AppendLine();
					a.AppendLine( @"	}" );
					a.AppendLine();
				}

				var lst2 = AssetDatabase.FindAssets( "l:EditorScene" );
				for( int i = 0; i < lst2.Length; i++ ) {
					var ll = lst2[ i ];
					var assetPath = AssetDatabase.GUIDToAssetPath( ll );
					var fname = Path.GetFileNameWithoutExtension( assetPath );

					a.AppendFormat( @"	[MenuItem( ""編集用シーン/{0}"", false, 12 )]", Path.GetFileNameWithoutExtension( fname ) ).AppendLine();
					a.AppendFormat( @"	public static void SceneMenu_e{0:D2}() {{", i ).AppendLine();
					a.AppendFormat( @"		EditorUtils.OpenScene( ""{0}"" );", assetPath ).AppendLine();
					a.AppendLine( @"	}" );
				}

				a.AppendLine( @"}" );
				a.AppendLine( @"#endif" );
			} );
		}


#if false
		[MenuItem( NAME + @"UpdateRigを検索する", false, 46 )]
		public static void menuGame_UpdateRig() {

			var files = EditorHelper.GetUpdateRigList();

			foreach( var obj in files ) {
				AssetDatabase.SetLabels( obj, new string[] { "UpdateRig" } );
			}
			AssetDatabase.Refresh();
			UnityEditorProjectBrowser.SetSearch( "l:UpdateRig t:GameObject" );
		}
#endif
		
		[MenuItem( NAME + "ScriptingDefineSymbols", false, 101 )]
		public static void menuGame_ScriptingDefineSymbols() {
			UnityEditorProjectBrowser.SelectionChangedLockProjectWindow( LocalSettings.i.scriptingDefineSymbols );
		}

#if false
		[MenuItem( NAME + "Monitor for Animation Curve", false, 102 )]
		public static void menuGame_Monitor4AnimationCurve() {
			Monitor4AnimationCurve.OpenWindow();
		}
#endif



		[MenuItem( "Tools/SaveAssets ＆ Refresh", false, 0 )]
		public static void Open_Refresh() {
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[MenuItem( "Tools/UnlinkPrefab" )]
		public static void UnlinkPrefab() {
			List<GameObject> news = new List<GameObject>();
			while( Selection.gameObjects.Length > 0 ) {
				GameObject old = Selection.gameObjects[ 0 ];

				string name = old.name;
				int index = old.transform.GetSiblingIndex();

				GameObject _new = MonoBehaviour.Instantiate( old ) as GameObject;
				MonoBehaviour.DestroyImmediate( old );

				_new.name = name;
				_new.transform.SetSiblingIndex( index );
				news.Add( _new );
			}
			Selection.objects = news.ToArray();
		}
	}
}




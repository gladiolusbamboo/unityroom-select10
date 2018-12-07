using UnityEngine;
using UnityEditor;


namespace HananokiEditor {
	/// <summary>
	/// 
	/// </summary>
	public class PreferenceSettings {

		public delegate void PreferencesGUICallback();

		static PreferenceSettings instance;

		public static PreferenceSettings i {
			get {
				if( instance == null ) {
					instance = new PreferenceSettings();
				}
				return instance;
			}
		}


		public PreferencesGUICallback s_preferencesGUICallback;


		PreferenceSettings() {
			var r = EditorPrefs.GetFloat( COLOR_TAG_R, 0.00f );
			var g = EditorPrefs.GetFloat( COLOR_TAG_G, 0.00f );
			var b = EditorPrefs.GetFloat( COLOR_TAG_B, 0.00f );
			var a = EditorPrefs.GetFloat( COLOR_TAG_A, 0.00f );
			m_color = new Color( r, g, b, a );


		}

		//public static bool EnableHierarchyItem {
		//	get {
		//		return EditorPrefs.GetBool( prefix + "EnableHierarchyItem", false );
		//	}
		//	set {
		//		EditorPrefs.SetBool( prefix + "EnableHierarchyItem", value );
		//	}
		//}
		const string prefix = "hananoki.";

		public EditorPrefsBool EnableHierarchyItem = new EditorPrefsBool( prefix+"EnableHierarchyItem" );

		public EditorPrefsBool HierarchyActive = new EditorPrefsBool( prefix + "HierarchyActive" );
		public EditorPrefsBool HierarchyPrefab = new EditorPrefsBool( prefix + "HierarchyPrefab" );
		public EditorPrefsBool HierarchyAnim = new EditorPrefsBool( prefix + "HierarchyAnim" );

		public EditorPrefsBool TransformEx = new EditorPrefsBool( prefix + "TransformEx" );
		public EditorPrefsBool AnimatorInspectorPreview = new EditorPrefsBool( prefix + "AnimatorInspectorPreview" );
		public EditorPrefsBool LocalizeJP = new EditorPrefsBool( prefix + "LocalizeJP" );
		public EditorPrefsBool TransformInspector_ShowWorld = new EditorPrefsBool( prefix + "TransformInspector.ShowWorld" );
		public EditorPrefsBool TransformInspector_ScaleMode = new EditorPrefsBool( prefix + "TransformInspector.ScaleMode" );

		public EditorPrefsBool SceneLoaded_UpdateEnvironment = new EditorPrefsBool( prefix + "SceneLoaded.UpdateEnvironment" );

		public EditorPrefsBool EnableFolderIcons = new EditorPrefsBool( prefix + "EnableFolderIcons" );
		public EditorPrefsBool EnableController360 = new EditorPrefsBool( prefix + "EnableController360" );
		public EditorPrefsBool EnableSceneView2GameView = new EditorPrefsBool( prefix + "SceneView2GameView" );
		public EditorPrefsBool EnableSceneSelector = new EditorPrefsBool( prefix + "EnableSceneSelector" );
		public EditorPrefsBool EnableProjectItem = new EditorPrefsBool( prefix + "EnableProjectItem" );
		public EditorPrefsBool EnableTimeScaleSlider = new EditorPrefsBool( prefix + "EnableTimeScaleSlider" );
		public EditorPrefsBool EnableObjectStat = new EditorPrefsBool( prefix + "EnableObjectStat" );

		public Color m_color;
		const string COLOR_TAG_R = prefix + "RowColor.R";
		const string COLOR_TAG_G = prefix + "RowColor.G";
		const string COLOR_TAG_B = prefix + "RowColor.B";
		const string COLOR_TAG_A = prefix + "RowColor.A";


		[PreferenceItem( "Hananoki" )]
		public static void PreferencesGUI() {
			//if( !prefsLoaded ) {
			//	text = EditorPrefs.GetString( TEXT_KEY, "" );
			//	prefsLoaded = true;
			//}
			var i = PreferenceSettings.i;

			GUI.changed = false;
			i.EnableHierarchyItem.Value = EditorGUILayout.ToggleLeft( "ヒエラルキーの拡張", i.EnableHierarchyItem, GUILayout.ExpandWidth( false ) );

			EditorGUI.indentLevel++;
			using( new EditorGUI.DisabledScope( !i.EnableHierarchyItem ) ) {
				i.HierarchyActive.Value = EditorGUILayout.ToggleLeft( "アクティブトグル", i.HierarchyActive, GUILayout.ExpandWidth( false ) );
				i.HierarchyPrefab.Value = EditorGUILayout.ToggleLeft( "プレハブ状態アイコン", i.HierarchyPrefab, GUILayout.ExpandWidth( false ) );
				i.HierarchyAnim.Value = EditorGUILayout.ToggleLeft( "Monitor for Animation Curve", i.HierarchyAnim, GUILayout.ExpandWidth( false ) );
			}
			EditorGUI.indentLevel--;

			if( GUI.changed ) {
				EditorApplication.RepaintHierarchyWindow();
			}

			GUI.changed = false;
			i.EnableProjectItem.Value = EditorGUILayout.ToggleLeft( "プロジェクトブラウザの拡張", i.EnableProjectItem.Value, GUILayout.ExpandWidth( false ) );
			if( GUI.changed ) {
				EditorApplication.RepaintProjectWindow();
			}
			GUI.changed = false;

			EditorGUI.indentLevel++;
			GUI.changed = false;
			var _color = EditorGUILayout.ColorField( "行の色", i.m_color );
			if( GUI.changed ) {
				i.m_color = _color;
				EditorUtils.RepaintEditorWindow();

				EditorPrefs.SetFloat( COLOR_TAG_R, _color.r );
				EditorPrefs.SetFloat( COLOR_TAG_G, _color.g );
				EditorPrefs.SetFloat( COLOR_TAG_B, _color.b );
				EditorPrefs.SetFloat( COLOR_TAG_A, _color.a );
			}
			EditorGUI.indentLevel--;


			PreferenceSettings.i.EnableSceneView2GameView.Value = EditorGUILayout.ToggleLeft( "SceneViewのカメラを任意のカメラに連動する機能", PreferenceSettings.i.EnableSceneView2GameView.Value );

			if( GUI.changed ) {
				SceneView2GameView.menu_ToggleSceneView2GameView();
			}

			GUI.changed = false;
			PreferenceSettings.i.EnableSceneSelector.Value = EditorGUILayout.ToggleLeft( "SceneSelector", PreferenceSettings.i.EnableSceneSelector.Value );
			if( GUI.changed ) {
				EditorApplication.RepaintHierarchyWindow();
			}

#if LOCAL_TEST
			PreferenceSettings.i.EnableTimeScaleSlider.Value = EditorGUILayout.ToggleLeft( "タイムスケールスライダー", PreferenceSettings.i.EnableTimeScaleSlider.Value );


			GUI.changed = false;
			PreferenceSettings.i.EnableObjectStat.Value = EditorGUILayout.ToggleLeft( "ObjectStat", PreferenceSettings.i.EnableObjectStat.Value );
			if( GUI.changed ) {
				if( PreferenceSettings.i.EnableObjectStat.Value ) {
					ObjectStat.Enable();
				}
				else {
					ObjectStat.Disable();
				}
			}
#endif

			GUILayout.Space( 8f );
			if( i.s_preferencesGUICallback != null ) {
				i.s_preferencesGUICallback();
			}

		}
	}
}


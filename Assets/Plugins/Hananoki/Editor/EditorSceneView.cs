using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HananokiEditor {
	[InitializeOnLoad]
	internal partial class EditorSceneView {

		const int WIDTH = 16;

		static Rect rcPop = new Rect( 108, 8, 160, 16 );
		//static float time;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		static EditorSceneView() {
			SceneView.onSceneGUIDelegate += EditorSceneView.onSceneGUI;
		}

		public static void onSceneGUI( SceneView sceneView ) {
			GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Inspector );

			Handles.BeginGUI();

			rcPop.x = Screen.width - 200;
			rcPop.width = 200-8;
			rcPop.y = Screen.height - ( 24 * 2 ) - ( 8 * 2 );

			Time.timeScale = EditorGUI.Slider( rcPop, Time.timeScale, 0.00f, 1.00f );

			Handles.EndGUI();
		}
	}
}

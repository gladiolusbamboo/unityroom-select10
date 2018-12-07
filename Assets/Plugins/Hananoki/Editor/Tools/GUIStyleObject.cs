
//#define TEST

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Harmony.Toolkit {
	[System.Serializable]
	public class GUIStyleObject : ScriptableObject {

		[MenuItem( "Tools/Create - GUIStyleList" )]
		public static void CreateAsset_GUIStyleList() {
			ProjectWindowUtil.CreateAsset( ScriptableObject.CreateInstance<GUIStyleObject>(), "New GUIStyleList.asset" );
		}

		public GUIStyle[] styles;
		public GUIStyleObject() {
			styles = new GUIStyle[ 0 ];
			ArrayUtility.Add( ref styles, new GUIStyle() );
		}

		public GUIStyle this[ int i ] {
			set { this.styles[ i ] = value; }
			get { return this.styles[ i ]; }
		}
	}

	[CustomEditor( typeof( GUIStyleObject ) )]
	public class GUIStyleListInspector : Editor {
		public GUIStyleObject self { get { return target as GUIStyleObject; } }
		public override void OnInspectorGUI() {
			//DrawDefaultInspector();
			if( GUILayout.Button( "GUIStyleEditor" ) ) {
				var w = EditorWindow.GetWindow<EditorStyleEditor>();
				w.titleContent = new GUIContent( "GUIStyleEditor", EditorGUIUtility.FindTexture( "SceneViewFx" ) );
				w.m_GUIStyleList = self;
			}
		}
	}
}

using UnityEditor;
using UnityEngine;

using System;
using System.Reflection;
using Hananoki;

//public static class RectTransformContext {
//	[MenuItem( "CONTEXT/RectTransform/Identity" )]
//	static void SelectCameraContext( MenuCommand command ) {
//		RectTransform s = (RectTransform) command.context;

//		s.transform.localRotation = Quaternion.identity;
//		s.transform.localScale = Vector3.one;
//		s.transform.localPosition = Vector3.zero;
//	}
//}

namespace HananokiEditor {
	[Serializable]
	[CustomEditor( typeof( RectTransform ) )]
	public class RectTransformInspector : Editor {

		RectTransform self { get { return target as RectTransform; } }

		[SerializeField]
		Editor m_Editor;

		[SerializeField]
		Type m_EditorType;

		void OnEnable() {
			m_EditorType = Assembly.Load( "UnityEditor" ).GetType( "UnityEditor.RectTransformEditor" );
			Editor.CreateCachedEditor( target, m_EditorType, ref m_Editor );

			var mi = m_EditorType.GetMethod( "OnEnable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static );
			mi.Invoke( m_Editor, null );
		}

		void OnDisable() {
			var mi = m_EditorType.GetMethod( "OnDisable", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static );
			mi.Invoke( m_Editor, null );

			ScriptableObject.DestroyImmediate( m_Editor );
			m_Editor = null;
		}

		void OnSceneGUI() {
			var mi = m_EditorType.GetMethod( "OnSceneGUI", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static );
			mi.Invoke( m_Editor, null );
		}

		public override void OnInspectorGUI() {

			//DrawDefaultInspector();
			m_Editor.OnInspectorGUI();

			GUILayout.Space( 8 );

			GUI.changed = false;
			var x = EditorGUILayout.FloatField( "scale", self.transform.localScale.x );
			if( GUI.changed ) {
				EditorHelper.Dirty( self, () => {
					self.transform.localScale = new Vector3( x, x, self.transform.localScale.z );
				} );
			}

			using( new GUILayout.HorizontalScope() ) {
				GUILayout.FlexibleSpace();
				//if( GUILayout.Button( "y" ) ) {
				//	Debug.Log( self.transform.position.y );
				//}
				if( GUILayout.Button( "Scale値をWidth & Heightに反映させる" ) ) {
					EditorHelper.Dirty( self, () => {
						self.sizeDelta = new Vector2( self.sizeDelta.x * self.transform.localScale.x, self.sizeDelta.y * self.transform.localScale.y );
						self.transform.localScale = Vector3.one;
						GUI.FocusControl( "" );
					} );
				}
				if( GUILayout.Button( "リセット" ) ) {
					EditorHelper.Dirty( self, () => {
						self.transform.localRotation = Quaternion.identity;
						self.transform.localScale = Vector3.one;
						self.transform.localPosition = Vector3.zero;
					} );
				}
			}
			GUILayout.Label( string.Format( $"{self.transform.localPosition.ToString()} : {self.transform.position.ToString()} : {self.transform.GetSiblingIndex()}: {self.transform.position.y}" ) );
		}
	}
}

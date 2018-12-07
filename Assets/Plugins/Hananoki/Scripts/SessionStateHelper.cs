
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Hananoki {

	[System.Serializable]
	public class SessionStateBool {
		public string name;
		public string label;
		public SessionStateBool( string name, string label ) {
			this.name = name;
			this.label = label;
		}
		public bool Value {
			get {
				return SessionState.GetBool( name, false );
			}
			set {
				SessionState.SetBool( name, value );
			}
		}

		public static implicit operator bool( SessionStateBool c ) { return c.Value; }
	}


	[System.Serializable]
	public class SessionStateInt {
		public string name;
		public string label;
		public SessionStateInt( string name, string label ) {
			this.name = name;
			this.label = label;
		}
		public int Value {
			get {
				return SessionState.GetInt( name, 0 );
			}
			set {
				SessionState.SetInt( name, value );
			}
		}

		public static implicit operator int( SessionStateInt c ) { return c.Value; }
	}


	[CustomPropertyDrawer( typeof( SessionStateBool ) )]
	class SessionStateBoolDrawer : PropertyDrawer {

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {

			var nameProp = property.FindPropertyRelative( "name" );
			var labelProp = property.FindPropertyRelative( "label" );

			EditorGUI.DrawRect( rc, new Color( 0f, 1f, 0f, 0.25f ) );
			GUI.changed = false;

			bool value = EditorGUI.Toggle( rc, labelProp.stringValue, SessionState.GetBool( nameProp.stringValue, false ) );

			if( GUI.changed ) {
				SessionState.SetBool( nameProp.stringValue, value );
			}
		}
	}


	[CustomPropertyDrawer( typeof( SessionStateInt ) )]
	class SessionStateIntDrawer : PropertyDrawer {

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {

			var nameProp = property.FindPropertyRelative( "name" );
			var labelProp = property.FindPropertyRelative( "label" );

			EditorGUI.DrawRect( rc, new Color( 0f, 1f, 0f, 0.25f ) );
			GUI.changed = false;

			int value = EditorGUI.IntField( rc, labelProp.stringValue, SessionState.GetInt( nameProp.stringValue, 0 ) );

			if( GUI.changed ) {
				SessionState.SetInt( nameProp.stringValue, value );
			}
		}
	}

}

#endif

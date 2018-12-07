
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Hananoki;

using UnityObject = UnityEngine.Object;

namespace HananokiEditor {
	[CustomPropertyDrawer( typeof( DefaultObjectReferenceAttribute ) )]
	class DefaultObjectReferenceAttributeDrawer : PropertyDrawer {
		DefaultObjectReferenceAttribute atb { get { return (DefaultObjectReferenceAttribute) attribute; } }

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {

			rc = EditorGUI.PrefixLabel( rc, GUIUtility.GetControlID( FocusType.Passive ), label );
			UnityObject uobj = property.objectReferenceValue;

			if( uobj == null && !string.IsNullOrEmpty( atb.guid ) ) {
				var s = AssetDatabase.GUIDToAssetPath( atb.guid );
				if( !string.IsNullOrEmpty( s ) ) {
					uobj = AssetDatabase.LoadAssetAtPath( s, atb.type );
					property.objectReferenceValue = uobj;
					return;
				}
			}

			//if( atb.type == typeof( GameObject ) ) {
			//	var ui = ((GameObject)uobj).GetComponent<kids02.UIController>();
			//	if( ui != null ){
			//		var rc1 = rc;
			//		rc1.x -= 16;
			//		rc1.width = 16;
			//		GUI.Button( rc1, "aaa" );
			//	}
			//}

			GUI.changed = false;
			uobj = (UnityObject) EditorGUI.ObjectField( rc, uobj, atb.type, true );
			if( GUI.changed ) {
				property.objectReferenceValue = uobj;
				//property.stringValue = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( value ) );
			}
		}
	}
}

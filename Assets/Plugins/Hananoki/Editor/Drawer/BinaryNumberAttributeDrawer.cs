
using UnityEngine;
using UnityEditor;
using Hananoki;

namespace HananokiEditor {
	[CustomPropertyDrawer( typeof( BinaryNumberAttribute ) )]
	class BinaryNumberAttributeDrawer : PropertyDrawer {
		BinaryNumberAttribute atb { get { return (BinaryNumberAttribute) attribute; } }

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {

			rc = EditorGUI.PrefixLabel( rc, GUIUtility.GetControlID( FocusType.Passive ), label );

			EditorGUI.BeginDisabledGroup( true );
			var s = System.Convert.ToString( ( property.intValue ), 2 );
			EditorGUI.TextField( rc, "0b" + s.PadLeft( 32, '0' ) );
			EditorGUI.EndDisabledGroup();
		}
	}
}


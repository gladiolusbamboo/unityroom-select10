
using UnityEngine;
using UnityEditor;
using Hananoki;


namespace HananokiEditor {

	[CustomPropertyDrawer( typeof( EnumStringAttribute ) )]
	class EnumStringAttributeDrawer : PropertyDrawer {
		EnumStringAttribute atb { get { return (EnumStringAttribute) attribute; } }

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {
			
			var values = System.Enum.GetValues( atb.m_type );

			string[] tbl = new string[ values.Length ];
			for( int i = 0; i < values.Length;i++ ) {
				tbl[ i ] = values.GetValue(i).ToString();
			}
			
			int idx = ArrayUtility.IndexOf( tbl, property.stringValue );
			if( idx < 0 ){
				idx = 0;
			}

			GUI.changed = false;
			idx = EditorGUI.Popup( rc, idx, tbl );
			if( GUI.changed ) {
				property.stringValue = tbl[ idx ];
			}
		}
	}
}

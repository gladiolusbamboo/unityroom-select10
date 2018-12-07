
using UnityEngine;
using UnityEditor;
using Hananoki;

namespace HananokiEditor {

	[CustomPropertyDrawer( typeof( DisableAttribute ) )]
	class ReadOnlyAttrbuteDrawer : PropertyDrawer {
		DisableAttribute atb { get { return (DisableAttribute) attribute; } }

		//UnityObject m_scnCache = null;

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {
			EditorGUI.BeginDisabledGroup( true );
			EditorGUI.PropertyField( rc, property, label );
			EditorGUI.EndDisabledGroup();
		}
	}
}

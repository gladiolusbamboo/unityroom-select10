
using UnityEngine;
using UnityEditor;

#if false
[CustomPropertyDrawer( typeof( NameAttribute ) )]
public class NameAttributeDrawer : PropertyDrawer {
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
		var atb = ( (NameAttribute) attribute );
		EditorGUI.BeginDisabledGroup( atb.m_disableFlag );
		
		label.text = atb.m_prop_name;
		EditorGUI.PropertyField( position, property, label );
		
		EditorGUI.EndDisabledGroup();
	}
}
#endif

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;


namespace HananokiEditor {
	public class TransformRotationGUIRefrection {
		object m_instance;
		Type m_type;

		delegate void MethodRotationField();
		MethodRotationField _MethodRotationField;

		public TransformRotationGUIRefrection() {
			string typeName = "UnityEditor.TransformRotationGUI";
			m_type = System.Reflection.Assembly.Load( "UnityEditor.dll" ).GetType( typeName );
			m_instance = Activator.CreateInstance( m_type );

			MethodInfo mi2 = m_type.GetMethod( "RotationField", Type.EmptyTypes );
			_MethodRotationField = (MethodRotationField) Delegate.CreateDelegate( typeof( MethodRotationField ), m_instance, mi2 );
		}

		public void OnEnable( SerializedProperty m_Rotation, GUIContent label ) {
			MethodInfo mi = m_type.GetMethod( "OnEnable", BindingFlags.Public | BindingFlags.Instance );
			mi.Invoke( m_instance, new object[] { m_Rotation, label } );
		}

		public void RotationField() {
			_MethodRotationField();
		}
	}
}


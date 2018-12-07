using System;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace UnityReflection {
	[Serializable]
	public class UnitySplitterState {

		public object m_instance;
		System.Type m_type;

		public UnitySplitterState( params float[] relativeSizes ) {
			string typeName = "UnityEditor.SplitterState";
			m_type = System.Reflection.Assembly.Load( "UnityEditor.dll" ).GetType( typeName );

			m_instance = System.Activator.CreateInstance( m_type, new object[] { relativeSizes } );
		}

		public int[] realSizes {
			get {
				var f = m_type.GetField( "realSizes", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
				var ii = (int[]) f.GetValue( m_instance );
				return ii;
			}
		}
	}
}

using System;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace UnityReflection {
	public class UnitySplitterGUILayout {

		static bool inited;

		object m_instance;
		static System.Type m_type;

		//delegate void Method_BeginSplit(object state, GUIStyle style, bool vertical, params GUILayoutOption[] options);
		//static Method_BeginSplit method_BeginSplit;

		static MethodInfo mi_BeginSplit;
		static MethodInfo mi_BeginHorizontalSplit2;
		static MethodInfo mi_BeginHorizontalSplit3;
		static MethodInfo mi_EndHorizontalSplit;

		//delegate void MethodBeginHorizontalSplit2( object state, params GUILayoutOption[] options );
		//static MethodBeginHorizontalSplit2 methodBeginHorizontalSplit2;
		
		delegate void MethodEndHorizontalSplit();
		static MethodEndHorizontalSplit methodEndHorizontalSplit;

		public UnitySplitterGUILayout() {
			init();
			m_instance = Activator.CreateInstance( m_type );
		}


		public static void init() {
			m_type = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.SplitterGUILayout" );
			
			var methods = m_type.GetMethods( BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
			foreach( var m in methods ) {
				switch(m.Name){
					case "BeginSplit":
						mi_BeginSplit = m;
						break;
					case "BeginHorizontalSplit":
						//mi_BeginSplit = m;
						if( m.GetParameters().Length == 2 ) {
							mi_BeginHorizontalSplit2 = m;
							//typeof( Action<,> ).MakeGenericType( typeof( object ), typeof( params GUILayoutOption[] ) );
							//methodBeginHorizontalSplit2 = Helper.CreateDelegateStatic<MethodBeginHorizontalSplit2>( mi_BeginHorizontalSplit2 );
						}
						if( m.GetParameters().Length == 3 ) {
							mi_BeginHorizontalSplit3 = m;
						}
						break;
					case "EndHorizontalSplit":
						mi_EndHorizontalSplit = m;
						methodEndHorizontalSplit = Helper.CreateDelegateStatic<MethodEndHorizontalSplit>( mi_EndHorizontalSplit );
						break;
				}
				//Debug.Log( m.Name );
			}

			inited = true;
		}

		public static void BeginSplit( UnitySplitterState state, GUIStyle style, bool vertical, params GUILayoutOption[] options ) {
			//method_BeginSplit( state, style, vertical, options );
		}
		public static void BeginHorizontalSplit( UnitySplitterState state, params GUILayoutOption[] options ) {
			if( !inited ) {
				init();
				return;
			}
			mi_BeginHorizontalSplit2.Invoke( null, new object[] { state.m_instance, options } );
			//methodBeginHorizontalSplit2( state.m_instance, options );
		}
		public static void BeginHorizontalSplit( UnitySplitterState state, GUIStyle style, params GUILayoutOption[] options ) {
			if( !inited ) {
				init();
				return;
			}
			mi_BeginHorizontalSplit3.Invoke( null, new object[] { state.m_instance, style, options } );
			//method_BeginSplit( state, style, vertical, options );
		}
		public static void EndHorizontalSplit() {
			if( !inited ) {
				init();
				return;
			}
			//mi_EndHorizontalSplit.Invoke( null, null );
			methodEndHorizontalSplit();
		}
	}
}

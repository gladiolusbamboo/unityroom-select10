using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

using UnityEngine;
using UnityEditor;

namespace UnityReflection {
	public class Helper {
		public static MethodInfo GetMethod( string methodName, string typeName, string dllName = "UnityEditor.dll" ) {
			var m_type = Assembly.Load( dllName ).GetType( typeName );
			return m_type.GetMethod( methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
		}

		public static T CreateDelegate<T>( object firstArgument, MethodInfo methodInfo ) where T : class {
			return Delegate.CreateDelegate( typeof( T ), firstArgument, methodInfo ) as T;
		}
		public static T CreateDelegateStatic<T>( MethodInfo methodInfo ) where T : class {
			return CreateDelegate<T>( null, methodInfo );
		}
	}


	public class UnityEditorGUIUtility {

		delegate Texture2D MethodLoadIcon( string name );
		static MethodLoadIcon methodLoadIcon;

		delegate GUIContent MethodTextContent( string textAndTooltip );
		static MethodTextContent methodTextContent;

		const string CLASS_NAME = "UnityEditor.EditorGUIUtility";

		public static Texture2D LoadIcon( string name ) {
			if( methodLoadIcon == null ) {
				methodLoadIcon = Helper.CreateDelegateStatic<MethodLoadIcon>( Helper.GetMethod( "LoadIcon", CLASS_NAME ) );
			}
			return methodLoadIcon( name );
		}

		public static GUIContent TextContent( string textAndTooltip ) {
			if( methodTextContent == null ) {
				methodTextContent = Helper.CreateDelegateStatic<MethodTextContent>( Helper.GetMethod( "TextContent", CLASS_NAME ) );
			}
			return methodTextContent( textAndTooltip );
		}
	}
}

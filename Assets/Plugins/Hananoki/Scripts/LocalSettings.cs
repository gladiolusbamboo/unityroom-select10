using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Hananoki;

using UnityObject = UnityEngine.Object;


namespace Hananoki {
	public class Hananokia {
		public const string className = "Hananoki";
	}

	[CreateAssetMenu( menuName = Hananokia.className + "/LocalSettings" )]
	[Serializable]
	public class LocalSettings : ScriptableObject {
#if UNITY_EDITOR
		public static LocalSettings i {
			get {
				return AssetDatabase.LoadAssetAtPath<LocalSettings>( AssetDatabase.GUIDToAssetPath( "a92b47051f9ead946a01a1926497aec0" ) );
			}
		}
		[GUIDString( typeof( UnityObject ) )]
		public string folderDefine;
		[GUIDString( typeof( UnityObject ) )]
		public string folderSceneList;

		public ScriptingDefineSymbols scriptingDefineSymbols;

		[ GUIDString( typeof( SceneAsset ) )]
		public string[] sceneList;

		[GUIDString( typeof( SceneAsset ) )]
		public string[] sceneList2;

		[GUIDString( typeof( ScriptableObject ) )]
		public string[] scriptableObjectList;
#endif
	}


#if UNITY_EDITOR
	public class SimpleReorderableList {
		ReorderableList m_lst;

		SerializedObject m_serializedObject;
		SerializedProperty m_serializedProperty;
		//string m_propertyName;

		public void create( SerializedObject serializedObject, string propertyName ) {
			m_serializedObject = serializedObject;
			m_serializedProperty = serializedObject.FindProperty( propertyName );

			//rlist = new ReorderableList( self.m_MayaObject, typeof( CutSceneController.MayaObject ) );
			m_lst = new ReorderableList( serializedObject, serializedObject.FindProperty( propertyName ) );

			m_lst.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				var rc1 = rect;
				rc1.y += 2;
				rc1.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField( rc1, serializedObject.FindProperty( propertyName + ".Array.data[" + index + "]" ) );
			};
			m_lst.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, string.Format( propertyName ) );
			};
		}

		public void draw() {
			m_serializedObject.Update();
			m_lst.DoLayoutList();
			m_serializedObject.ApplyModifiedProperties();
		}
	}


	[CustomEditor( typeof( LocalSettings ) )]
	public class LocalSettingsInspector : Editor {
		LocalSettings self { get { return target as LocalSettings; } }

		SimpleReorderableList m_lst1;
		SimpleReorderableList m_lst2;
		SimpleReorderableList m_lst3;

		/// <summary>
		/// 
		/// </summary>
		void OnEnable() {
			if( self.sceneList == null ) {
				self.sceneList = new string[ 0 ];
			}
			m_lst1 = new SimpleReorderableList();
			m_lst1.create( serializedObject, "sceneList" );

			m_lst2 = new SimpleReorderableList();
			m_lst2.create( serializedObject, "sceneList2" );

			m_lst3 = new SimpleReorderableList();
			m_lst3.create( serializedObject, "scriptableObjectList" );
		}


		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "folderDefine" ), new GUIContent( "folderDefine" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "folderSceneList" ), new GUIContent( "folderSceneList" ) );
			EditorGUILayout.PropertyField( serializedObject.FindProperty( "scriptingDefineSymbols" ), new GUIContent( "scriptingDefineSymbols" ) );
			
			serializedObject.ApplyModifiedProperties();

			m_lst1.draw();

			GUILayout.Space( 8 );

			m_lst2.draw();

			GUILayout.Space( 8 );

			m_lst3.draw();

			//DrawDefaultInspector();
		}
	}
#endif
}


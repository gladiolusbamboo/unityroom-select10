using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Hananoki;

namespace HananokiEditor {
	[CustomEditor( typeof( PrefabList ) )]
	public class PrefabListInspector : Editor {

		PrefabList self { get { return target as PrefabList; } }

		ReorderableList m_lst;


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ReorderableList MakeReorderableList() {
			if( self.m_Data == null ) {
				self.m_Data = new PrefabListData[ 0 ];
			}
			
			var r = new ReorderableList( serializedObject, serializedObject.FindProperty( "m_Data" ) );

			r.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, string.Format( serializedObject.targetObject.name ) );
			};

			r.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				var item = self.m_Data[ index ];

				var rc1 = rect;
				rc1.y += 1;
				EditorGUI.PropertyField( rc1, r.serializedProperty.GetArrayElementAtIndex( index ), GUIContent.none );
			};

			r.elementHeight = PrefabListDataDrawer.propertyHeight;

			return r;
		}


		/// <summary>
		/// 
		/// </summary>
		void OnEnable() {

			m_lst = MakeReorderableList();

		} // void OnEnable()


		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {

			serializedObject.Update();
			m_lst.DoLayoutList();
			serializedObject.ApplyModifiedProperties();


			Rect rc = GUILayoutUtility.GetRect( new GUIContent( "" ), "toolbarbutton" );
			rc.x = 1;
			rc.y -= 4;
			rc.width = EditorGUIUtility.currentViewWidth - 3;
			rc.height = 18;

			var evt = Event.current;
			var dropArea = new Rect( rc.x, rc.y + rc.height, rc.width, 1000.0f );
			//GUI.Box( dropArea, "Drag & Drop", (GUIStyle) "SelectionRect" );
			int id = GUIUtility.GetControlID( FocusType.Passive );
			switch( evt.type ) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if( !dropArea.Contains( evt.mousePosition ) ) break;

					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					DragAndDrop.activeControlID = id;

					if( evt.type == EventType.DragPerform ) {
						DragAndDrop.AcceptDrag();

						foreach( var draggedObject in DragAndDrop.objectReferences ) {
							Debug.Log( "Drag Object:" + AssetDatabase.GetAssetPath( draggedObject ) );

							//if( typeof( AnimationClip ) == draggedObject.GetType() ) {
							//	var clip = draggedObject as AnimationClip;

							//	//Debug.Log( draggedObject.name );

							//	if( isRegistered( clip ) == false ) {
							//		self.AddMotion( clip );
							//	}
							//	continue;
							//}

							//Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( draggedObject ) );
							//for( int i = 0; i < assets.Length; i++ ) {
							//	Object asset = assets[ i ];
							//	//if( AssetDatabase.IsMainAsset( asset ) ) continue;

							//	//asset.hideFlags &= ~HideFlags.HideInHierarchy;
							//	//asset.hideFlags = HideFlags.HideInHierarchy;
							//	if( typeof( AnimationClip ) == asset.GetType() ) {
							//		if( asset.name.Contains( "__preview" ) ) {
							//			continue;
							//		}
							//		Debug.Log( asset.name );
							//		self.AddMotion( (AnimationClip) asset );
							//	}
							//}

							//m_FilePath.stringValue = AssetDatabase.GetAssetPath( draggedObject );
						}

						//m_states = self.layers[ 0 ].stateMachine.states;
						DragAndDrop.activeControlID = 0;
					}
					Event.current.Use();
					break;
			}

			//DrawDefaultInspector();
		} // void OnInspectorGUI()

	} // class PrefabListInspector
}


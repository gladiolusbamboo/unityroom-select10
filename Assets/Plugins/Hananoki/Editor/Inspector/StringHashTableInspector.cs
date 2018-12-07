using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEditor;
using UnityEditorInternal;

using System.IO;
using System.Linq;

using Hananoki;

namespace HananokiEditor {
	
#if UNITY_EDITOR
	[CustomEditor( typeof( StringHashTable ) )]
	public class StringHashTableInspector : Editor {

		ReorderableList m_reorderableList;

		public StringHashTable self { get { return target as StringHashTable; } }


		public void WriteLabelToCS( ) {
			if( LocalSettings.i.folderDefine == null ) {
				Debug.LogWarning( "LocalSettings: null" );
				return;
			}

			string fname = string.Format( "{0}\\{1}.cs", LocalSettings.i.folderDefine.GUID2Path(), self.name );

			using( var writer = new StreamWriter( fname ) ) {
				writer.Write( "public static class " + self.name + " {\n" );
				foreach( var item in self.m_dataList ) {
					writer.Write( string.Format( "  public const int {0} = {1};\n", item.key, item.value ) );
				}
				writer.Write( "  public static int[] toArray() {\n" );
				writer.Write( "    return new int[]{ " );
				foreach( var item in self.m_dataList ) {
					writer.Write( string.Format( "{0}, ", item.key ) );
				}
				writer.Write( "};\n" );
				writer.Write( "  }\n" );

				foreach( var item in self.m_dataList.Select( ( v, i ) => new { v, i } ) ) {
					writer.Write( string.Format( "  public const int _{0} = {1};\n", item.v.key, item.i ) );
				}
				writer.Write( "}\n" );
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		/// <summary>
		/// 
		/// </summary>
		void OnEnable() {
			if( self.m_dataList == null ) {
				self.m_dataList = new List<StringHashTable.Data>();
			}
			m_reorderableList = new ReorderableList( self.m_dataList, typeof( StringHashTable.Data ) );

			m_reorderableList.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, string.Format( "StringToHash: {0}", self.name ) );
			};
			m_reorderableList.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				//EditorGUI.LabelField( rect, self.m_prefabs[ index ].tagName, self.m_prefabs[ index ].prefab.name );
				var item = self.m_dataList[ index ];

				EditorGUI.BeginChangeCheck();

				var w = rect.width * 0.4f;
				var rcR = new Rect( rect.x + w + 1, rect.y, rect.width - w - 1, EditorGUIUtility.singleLineHeight );
				var rcL = new Rect( rect.x + 1, rect.y, w - 1 - 1, EditorGUIUtility.singleLineHeight );

				var _key = EditorGUI.TextField( rcL, item.key );
				EditorGUI.BeginDisabledGroup( true );
				var _value = EditorGUI.IntField( rcR, item.value );
				EditorGUI.EndDisabledGroup();

				if( EditorGUI.EndChangeCheck() ) {
					Undo.RecordObject( this, "Undo " );
					item.key = _key;
					if( self.m_mode == StringHashTable.Mode.ShaderPropertyToID ) {
						item.value = Shader.PropertyToID( _key );
					}
					else {
						item.value = Animator.StringToHash( _key );
					}
					//EditorUtility.SetDirty( self );
				}
			};
			m_reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 3;
		}


		/// @brief  カスタムインスペクターを作成するためにこの関数を実装します
		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();

			using( new EditorGUILayout.HorizontalScope() ) {
				GUI.changed = false;
				var a = (StringHashTable.Mode) EditorGUILayout.EnumPopup( self.m_mode )  ;
				if( GUI.changed ) {
					self.m_mode = a;
				}
				//GUILayout.FlexibleSpace();
				if( LocalSettings.i.folderDefine == null ) {
					GUILayout.Label( "null" );
				}
				else {
					GUILayout.Label( LocalSettings.i.folderDefine.GUID2Path() );
				}
				if( GUILayout.Button( "CS書き出し" ) ) {
					WriteLabelToCS();
				}
			}

			SerializedObject serializedObject = new SerializedObject( self );
			serializedObject.Update();

			// リスト・配列の変更可能なリストの表示
			m_reorderableList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}
	}

#endif
}


using UnityEngine;

using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Hananoki;

namespace Hananoki {

	/// <summary>
	/// 
	/// </summary>
	[CreateAssetMenu( menuName = Hananokia.className + "/ScriptingDefineSymbols" ) ]
	[Serializable]
	public class ScriptingDefineSymbols : ScriptableObject {

		[Serializable]
		public class Data {
			[NonSerialized]
			public bool   toggle;
			public string name;
			public Data( bool toggle, string name ) {
				this.toggle = toggle;
				this.name = name;
			}
			public Data()
				: this( false, "" ) {
			}
		}

		public void enable( params string[] strs ) {
			foreach( var s in strs ) {
				int i = m_lst.FindIndex( x => x.name == s );
				if( i < 0 ) continue; ;
				m_lst[ i ].toggle = true;
			}
		}

		public void disable( params string[] strs ) {
			foreach( var s in strs ) {
				int i = m_lst.FindIndex( x => x.name == s );
				if( i < 0 ) continue; ;
				m_lst[ i ].toggle = false;
			}
		}

		public List<Data>  m_lst;
	}


#if UNITY_EDITOR
	[CustomEditor( typeof( ScriptingDefineSymbols ) )]
	class ScriptingDefineSymbols_Inspector : Editor {

		public ScriptingDefineSymbols self { get { return target as ScriptingDefineSymbols; } }
		public bool m_editMode =false;
		public bool m_hasDirty = false;

		ReorderableList m_rlst;
		List<string> m_symbols;

		BuildTargetGroup activeBuildTargetGroup {
			get {
				if( EditorUserBuildSettings.activeBuildTarget == BuildTarget.PS4 ) {
					return BuildTargetGroup.PS4;
				}
				else {
					return EditorUserBuildSettings.selectedBuildTargetGroup;
				}
			}
		}


		void OnEnable() {
			if( self.m_lst == null ) {
				self.m_lst = new List<ScriptingDefineSymbols.Data>();
			}
			
			foreach( var p in self.m_lst ) {
				p.toggle = false;
			}

			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup( activeBuildTargetGroup ).Split( ';' );
			foreach( var s in symbols ) {
				int index = self.m_lst.FindIndex( x => x.name == s );
				if( 0 <= index ){
					self.m_lst[ index ].toggle = true;
				}
			}
			EditorUtility.SetDirty( self );

			m_rlst = makeReorderableList( self );
		}


		public static ReorderableList makeReorderableList( ScriptingDefineSymbols self ) {
			ReorderableList rlist;
			rlist = new ReorderableList( self.m_lst, typeof( ScriptingDefineSymbols.Data ) );

			var paddingY = 4.0f;
			var ctrlH = EditorGUIUtility.singleLineHeight;

			rlist.drawHeaderCallback = ( rect ) => {
				EditorGUI.LabelField( rect, "ScriptingDefineSymbols" );
			};
			rlist.drawElementCallback = ( rect, index, isActive, isFocused ) => {
				var item = self.m_lst[index];
				var padH = paddingY * 0.5f;
				var h = EditorGUIUtility.singleLineHeight + paddingY;
				var r1 = new Rect( rect.x, padH + rect.y + ( h * 0 ), rect.width, ctrlH );
				
			//	var r4 = new Rect( rect.x, padH + rect.y + ( h * 0 ), 16, ctrlH );
			//	var _toggle = EditorGUI.Toggle( r4, item.toggle );
				EditorGUI.BeginChangeCheck();

				var _no = EditorGUI.TextField( r1, item.name );

				if( EditorGUI.EndChangeCheck() ) {
					Undo.RecordObject( self, "Undo " );

					item.name = _no;

					EditorUtility.SetDirty( self );
				}
			};

			rlist.elementHeight = ( ( ctrlH + paddingY ) );

			return rlist;
		}


		List<string> makeSymbolList() {
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup( activeBuildTargetGroup );
			return new List<string>( symbols.Split( ';' ) );
		}


		public static void drawGUI( ScriptingDefineSymbols self, ReorderableList m_lst ) {
			try {
				EditorGUILayout.Space();

				SerializedObject serializedObject = new SerializedObject( self );
				serializedObject.Update();

				// リスト・配列の変更可能なリストの表示
				m_lst.DoLayoutList();

				serializedObject.ApplyModifiedProperties();
			}
			catch( System.Exception e ) {
				UnityEngine.Debug.LogError( e );
			}
		}


		/// <summary>
		/// カスタムインスペクターを作成するためにこの関数を実装します
		/// </summary>
		public override void OnInspectorGUI() {
			Action<bool, string> toggleSysmbol = ( toggle, symbolName ) => {
				var lst = makeSymbolList();
				if( toggle ) {
					if( !lst.Contains( symbolName ) ) {
						lst.Add( symbolName );
					}
				}
				else {
					lst.RemoveAll( x => x == symbolName );
				}

				string s = "";
				foreach( var a in lst ) {
					if( s != "" ) s += ";";
					s += a;
				}
				PlayerSettings.SetScriptingDefineSymbolsForGroup( activeBuildTargetGroup, s );
			};

			using( new EditorGUI.DisabledGroupScope( EditorApplication.isCompiling ) ) {
				using( new EditorGUILayout.HorizontalScope() ) {
					EditorGUILayout.LabelField( "ActiveBuildTarget: " + EditorUserBuildSettings.activeBuildTarget, (GUIStyle) "BoldLabel" );
					GUILayout.FlexibleSpace();
					m_editMode = GUILayout.Toggle( m_editMode, "編集モード", (GUIStyle) "LargeButton" );
					if( m_editMode == false ) {
						m_symbols = makeSymbolList();
					}
				}

				if( m_editMode == false ) {
					GUILayout.Space( 8 );
					using( new EditorGUILayout.VerticalScope( /*(GUIStyle) "HelpBox"*/ ) ) {
						EditorGUILayout.LabelField( "ScriptingDefineSymbols", (GUIStyle) "ShurikenModuleTitle" );
						GUILayout.Space( 4 );
						foreach( var s in self.m_lst ) {
							bool _toggle = EditorGUILayout.ToggleLeft( " " + s.name, s.toggle );
							if( s.toggle != _toggle ) {
								//Undo.RecordObject( self, "Undo " );
								s.toggle = _toggle;
								//toggleSysmbol( _toggle, s.name );
								//EditorUtility.SetDirty( self );
								m_hasDirty = true;
							}
						}
					}

					GUILayout.Space( 8 );

					EditorGUI.BeginDisabledGroup( true );
					EditorGUILayout.TextField( PlayerSettings.GetScriptingDefineSymbolsForGroup( activeBuildTargetGroup ) );
					EditorGUI.EndDisabledGroup();
					using( new EditorGUILayout.HorizontalScope() ) {
						GUILayout.FlexibleSpace();
						EditorGUI.BeginDisabledGroup( !m_hasDirty );
						if( GUILayout.Button( "Apply" ) ) {
							m_hasDirty = false;
							Undo.RecordObject( self, "Undo " );
							var lst = makeSymbolList();
							foreach( var item in self.m_lst ) {
								if( item.toggle ) {
									if( !lst.Contains( item.name ) ) {
										lst.Add( item.name );
									}
								}
								else {
									lst.RemoveAll( x => x == item.name );
								}
							}
							string s = "";
							foreach( var a in lst ) {
								if( s != "" ) s += ";";
								s += a;
							}
							PlayerSettings.SetScriptingDefineSymbolsForGroup( activeBuildTargetGroup, s );
							EditorUtility.SetDirty( self );
						}
						EditorGUI.EndDisabledGroup();
						if( GUILayout.Button( "Clear" ) ) {
							PlayerSettings.SetScriptingDefineSymbolsForGroup( activeBuildTargetGroup, "" );
						}
					}
				}
				else {
					drawGUI( self, m_rlst );
				}
			}
		}
	}
#endif
}

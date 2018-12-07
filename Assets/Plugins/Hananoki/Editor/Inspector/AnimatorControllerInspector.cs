
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Animations;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Hananoki;

using UnityObject = UnityEngine.Object;


namespace HananokiEditor {

	[CustomEditor( typeof( UnityEditor.Animations.AnimatorController ) )]
	public class AnimatorControllerInspector : Editor {

		UnityEditor.Animations.AnimatorController self { get { return target as UnityEditor.Animations.AnimatorController; } }

		UnityEditor.Animations.ChildAnimatorState[] m_states;

		/// <summary>
		/// 
		/// </summary>
		void redraw() {
			// Animatorウィンドウ取得
			var asm = Assembly.Load( "UnityEditor.Graphs" );
			var editorGraphModule = asm.GetModule( "UnityEditor.Graphs.dll" );
			var typeAnimatorWindow = editorGraphModule.GetType( "UnityEditor.Graphs.AnimatorControllerTool" );
			var animatorWindow = EditorWindow.GetWindow( typeAnimatorWindow );

			animatorWindow.GetType()
					.GetMethod( "RebuildGraph", BindingFlags.Public | BindingFlags.Instance )
					.Invoke( animatorWindow, null );
		}


		/// <summary>
		/// </summary>
		public void OnEnable() {
			try {
				m_states = self.layers[ 0 ].stateMachine.states;
			}
			catch( System.Exception e ) {
				Debug.Log( e.ToString() );
				return;
			}
			AnimationControllerHelper.DefaultStatePosition( self );
		}


		void Toolbar_CreateSubAssetAnimationClip() {
			var newClip = UnityEditor.Animations.AnimatorController.AllocateAnimatorClip( "new clip" );
			AssetDatabase.AddObjectToAsset( newClip, self );
			AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( self ) );
			AssetDatabase.Refresh();
		}

		void Toolbar_AddMotionFBXs() {
			string motPath = Path.GetDirectoryName( AssetDatabase.GetAssetPath( self ) ).Replace( "/", "\\" );

			string[] folders = { "Animations", "animations", "Motion", "motion", "mot", "anm", "Animation", "animation", };
			foreach( var f in folders ) {
				if( Directory.Exists( motPath + "/" + f ) ) {
					_onClipListAddMotion( motPath + "/" + f );
					break;
				}
			}

			m_states = self.layers[ 0 ].stateMachine.states;
		}

		void _onClipListAddMotion( string motPath ) {
			var st = self.layers[ 0 ].stateMachine.states;
			int addcnt = 0;
			var files = Directory.GetFiles( motPath, "*.fbx" );
			float fval = 0.00f;
			float fadd = 1.00f / files.Length;

			foreach( string path in files ) {
				fval += fadd;
				EditorUtility.DisplayProgressBar( "クリップ複製中", path, fval );

				var fbxClipList = AssetDatabase.LoadAllAssetsAtPath( path.Replace( "\\", "/" ) ).
					Where( x => x.GetType() == typeof( AnimationClip ) ).
					Where( x => !x.name.Contains( "__preview__" ) ).ToArray();

				foreach( AnimationClip fbxClip in fbxClipList ) {
					//if( 0 <= System.Array.FindIndex( self.animationClips, ( c ) => { return c.name == fbxClip.name; } ) ) {
					int i = System.Array.FindIndex( st, a => a.state.motion == fbxClip );
					if( 0 <= i ) {
						if( st[ i ].state.name != fbxClip.name ) {
							//			//console.log( st[ i ].state.name );
							st[ i ].state.name = fbxClip.name;
						}
					}
					//	continue;
					//}

					if( isRegistered( fbxClip ) ) {
						continue;
					}

					var lsls = AssetDatabase.GetLabels( fbxClip );
					var lidx = System.Array.FindIndex( lsls, x => x == "Unused" );
					if( -1 == lidx ) {
						addcnt++;
						var state = self.AddMotion( fbxClip );
					}
				}
			}
			EditorUtility.ClearProgressBar();
			//Debug.LogFormat( "コントローラに{0}個追加しました: ", addcnt );
		}

		bool isRegistered( AnimationClip clip ) {
			var st = self.layers[ 0 ].stateMachine.states;

			if( 0 <= System.Array.FindIndex( self.animationClips, ( c ) => { return c.name == clip.name; } ) ) {
				int i = System.Array.FindIndex( st, c => c.state.motion == clip );
				if( 0 <= i ) {
					if( st[ i ].state.name != clip.name ) {
						//console.log( st[ i ].state.name );
						st[ i ].state.name = clip.name;
					}
				}
				return true;
			}
			return false;
		}


		void Toolbar_AllStateClear() {
			var animations = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( self ) );

			for( int j = 0; j < animations.Length; j++ ) {
				if( typeof( AnimatorState ) == animations[ j ].GetType() ) {
					Object.DestroyImmediate( animations[ j ], true );
				}
			}

			self.layers[ 0 ].stateMachine.states = new ChildAnimatorState[ 0 ];
			m_states = self.layers[ 0 ].stateMachine.states;

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		/// <summary>
		/// 
		/// </summary>
		void Toolbar_ExportDefine() {
			var a = PrefabUtility.GetCorrespondingObjectFromSource( self );
			if( a == null ) {
				a = self;
			}
			var path = AssetDatabase.GetAssetPath( a );
			if( string.IsNullOrEmpty( path ) ) return;

			path = string.Format( "{0}/{1}.cs", path.directoryName(), path.fileNameWithoutExtension() );
			using( var st = new StreamWriter( path ) ) {
				st.WriteLine( "public static class " + path.fileNameWithoutExtension() + " {" );
				st.WriteLine( "  public enum Motion {" );
				foreach( var state in m_states ) {
					var s = state.state.name.Replace( " ", "_" );
					st.WriteLine( string.Format( "    {0} = {1},", s, Animator.StringToHash( state.state.name ) ) );
				}
				st.WriteLine( "  }" );

				st.WriteLine( "  public static int[] toArray() {" );
				st.Write( "    return new int[]{ " );
				foreach( var state in m_states ) {
					var s = state.state.name.Replace( " ", "_" );
					st.Write( "(int) Motion." + s + ", " );
				}

				st.WriteLine( "  };" );

				st.WriteLine( "  }" );

				st.WriteLine( "}" );
			}

			AssetDatabase.Refresh();
			EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath<UnityObject>( path ) );
		}


		/// <summary>
		/// 
		/// </summary>
		void Toolbar_StateSort() {
			System.Array.Sort( m_states, ( x, y ) => string.Compare( x.state.name, y.state.name ) );
			self.layers[ 0 ].stateMachine.states = m_states;
		}


		/// <summary>
		/// ツールバー部の描画を行います
		/// </summary>
		void DrawToolBar() {
			Rect r = new Rect( 1, 45, EditorGUIUtility.currentViewWidth - 3, 18 );// GUILayoutUtility.GetRect( new GUIContent( "" ), "toolbarbutton" );

			using( new GUILayout.AreaScope( r ) ) {
				using( new GUILayout.HorizontalScope( EditorStyles.toolbar ) ) {

					GUILayout.Label( EditorGUIUtility.IconContent( "_Popup" ), EditorStyles.toolbarButton );
					if( EditorHelper.HasMouseClick( GUILayoutUtility.GetLastRect() ) ) {
						var optionsMenu = new GenericMenu();
						//optionsMenu.AddItem( new GUIContent( "Create SubAsset AnimationClip" ), false, Toolbar_CreateSubAssetAnimationClip );
						optionsMenu.AddItem( new GUIContent( "Add Motion FBXs" ), false, Toolbar_AddMotionFBXs );
						optionsMenu.AddItem( new GUIContent( "Clear All State" ), false, Toolbar_AllStateClear );
						optionsMenu.AddItem( new GUIContent( "Export Enum" ), false, Toolbar_ExportDefine );
						optionsMenu.AddItem( new GUIContent( "Sort State" ), false, Toolbar_StateSort );

						optionsMenu.DropDown( GUILayoutUtility.GetLastRect() );
					}

					GUILayout.FlexibleSpace();

					//GUILayout.Label( "num: " + self.layers[ 0 ].stateMachine.states.Length, EditorStyles.toolbar );
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		void DrawClipInfo() {
			for( int i = 0; i < m_states.Length; ++i ) {
				var asset = m_states[ i ];
				EditorGUI.BeginDisabledGroup( true );

				GUI.changed = false;
				var _mot = EditorGUILayout.ObjectField( asset.state.name, asset.state.motion, typeof( AnimationClip ), false );

				if( GUI.changed && _mot == null ) {
					Debug.Log( "mot is snull" );
				}

				EditorGUI.EndDisabledGroup();
			}
		}



		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			Rect rc = GUILayoutUtility.GetRect( new GUIContent( "" ), "toolbarbutton" );
			rc.x = 1;
			rc.y -= 4;
			rc.width = EditorGUIUtility.currentViewWidth - 3;
			rc.height = 18;

			DrawToolBar();

			DrawClipInfo();

			var evt = Event.current;
			var dropArea = new Rect( rc.x, rc.y + rc.height, rc.width, 1000.0f );
			//GUI.Box( dropArea, "Drag & Drop" );
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

							if( typeof( AnimationClip ) == draggedObject.GetType() ) {
								var clip = draggedObject as AnimationClip;

								//Debug.Log( draggedObject.name );

								if( isRegistered( clip ) == false ) {
									self.AddMotion( clip );
								}
								continue;
							}

							Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( draggedObject ) );
							for( int i = 0; i < assets.Length; i++ ) {
								Object asset = assets[ i ];
								//if( AssetDatabase.IsMainAsset( asset ) ) continue;

								//asset.hideFlags &= ~HideFlags.HideInHierarchy;
								//asset.hideFlags = HideFlags.HideInHierarchy;
								if( typeof( AnimationClip ) == asset.GetType() ) {
									if( asset.name.Contains( "__preview" ) ) {
										continue;
									}
									Debug.Log( asset.name );
									self.AddMotion( (AnimationClip) asset );
								}
							}

							//m_FilePath.stringValue = AssetDatabase.GetAssetPath( draggedObject );
						}

						m_states = self.layers[ 0 ].stateMachine.states;
						DragAndDrop.activeControlID = 0;


					}
					Event.current.Use();
					break;
			}
		}

	} // class Inspector_AnimatorController
}


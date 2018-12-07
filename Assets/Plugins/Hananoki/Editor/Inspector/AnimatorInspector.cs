
#if true

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System.Linq;
using System;
using System.Reflection;

using System.IO;
using Hananoki;

using UnityObject = UnityEngine.Object;

namespace HananokiEditor {
	[CanEditMultipleObjects, CustomEditor( typeof( Animator ) )]
	public class AnimatorInspector : Editor {

		class Styles {
			public GUIStyle HelpBox;
			public Styles() {
				HelpBox = new GUIStyle( (GUIStyle) "HelpBox" );
				HelpBox.padding = new RectOffset( 1, 1, 1, 1 );
			}
		}
		Styles m_Styles;

		public Animator self { get { return target as Animator; } }
		string prefabPath { get { return AssetDatabase.GetAssetPath( PrefabUtility.GetCorrespondingObjectFromSource( self.gameObject ) ); } }
		//	Controller	コントローラ
		//Avatar	アバター"
		//Apply RootMotion	ルートモーション適用
		//Update Mode	アップデートモード
		//Culling Mode	カリングモード
		//AnimatorControllerInspector m_editor;

		AnimatorController anmCtr { get { return self.runtimeAnimatorController as UnityEditor.Animations.AnimatorController; } }

		SerializedProperty m_Controller;
		SerializedProperty m_Avatar;
		SerializedProperty m_ApplyRootMotion;
		SerializedProperty m_CullingMode;
		SerializedProperty m_UpdateMode;
		SerializedProperty m_WarningMessage;

		//UnityEditor.AnimatedValues.AnimBool m_ShowWarningMessage = new UnityEditor.AnimatedValues.AnimBool();
		private Vector2 m_KeyScrollPos;
		//int m_selectSound;
		bool IsWarningMessageEmpty {
			get {
				return this.m_WarningMessage != null && this.m_WarningMessage.stringValue.Length > 0;
			}
		}

		AnimatorState[] m_clipState;

		int m_motionSelect;
		//float m_motionSlider;

		bool m_clipPlay;

		bool hasController() {
			return self.runtimeAnimatorController == null ? false : true;
		}

		//void InitShowOptions() {
		//	m_ShowWarningMessage.value = this.IsWarningMessageEmpty;
		//	m_ShowWarningMessage.valueChanged.AddListener( new UnityAction( base.Repaint ) );
		//}

		void makeClipList() {
			if( !hasController() ) return;
			try {
				m_clipState = anmCtr.layers[ 0 ].stateMachine.states.Select( x => x.state ).ToArray();
			}
			catch(Exception e){
				Debug.LogError(e);
			}
		}

		void MenuAddMotion( object userData ) {
			string path = (string) userData;

			AnimationControllerHelper.AddFolder( anmCtr, Path.GetDirectoryName( path )  + "/Animations" );
			makeClipList();
			Repaint();
		}

		void MenuSort() {
			AnimationControllerHelper.DefaultStatePosition( anmCtr );

			var states = anmCtr.layers[ 0 ].stateMachine.states;

			System.Array.Sort( states, ( x, y ) => string.Compare( x.state.name, y.state.name ) );

			Vector2 def = new Vector2( -204.0f, 72.0f );
			//float posY = 0;
			for( int i = 0; i < states.Length; ++i ) {
				string name = states[ i ].state.name;
				//int j;
				//for( j = 0; j < m_matchPattern.Count; ++j ) {
				//	var s = m_matchPattern[ j ];

				//	if( s == "##-" ) {
				//		m_states[ i ].position = def;
				//		def.y += 24;
				//		break;
				//	}

				//	if( s[ 0 ] == '#' ) continue;

				//	var mm = Regex.Matches( name, s );
				//	if( 0 < mm.Count ) {
				//		m_states[ i ].position = new Vector3( m_position[ j ].x, m_position[ j ].y, 0.00f );
				//		break;
				//	}
				//}
				//if( m_matchPattern.Count <= j ) {
				states[ i ].position = def;
				def.y += 36;
				//}
			}

			anmCtr.layers[ 0 ].stateMachine.states = states;

			makeClipList();
			Repaint();
		}

		void MenuClear() {
			AnimationControllerHelper.RemoveAll( anmCtr );

			makeClipList();
			Repaint();
		}

		//void MenuEvent() {
		//	//var c = anmCtr.animationClips[0];
		//	//var e = new AnimationEvent();
		//	//e.time = 0.50f;
		//	//e.functionName = "calltest";
		//	//c.AddEvent(e );

		//	var a = ScriptableObject.CreateInstance( typeof( AnimatorEvents ) );
		//	a.name = "AnimatorEvents";
		//	AssetDatabase.AddObjectToAsset( a, anmCtr );
		//}
		void MenuEventClear() {
			var c = anmCtr.animationClips[ 0 ];

			c.events = null;
		}

		void On_Popup() {
			var menu = new GenericMenu();
			menu.AddItem( new GUIContent( "Add Motion" ), false, MenuAddMotion, AssetDatabase.GetAssetPath( PrefabUtility.GetCorrespondingObjectFromSource( self.gameObject ) ) );
			menu.AddItem( new GUIContent( "Sort" ), false, MenuSort );
			menu.AddItem( new GUIContent( "Clear" ), false, MenuClear );
			//menu.AddItem( new GUIContent( "EventAdd" ), false, MenuEvent );
			menu.AddItem( new GUIContent( "EventClear" ), false, MenuEventClear );
			menu.ShowAsContext();
		}

		void On_Animator_Icon() {
			var asm2 = Assembly.Load( "UnityEditor.Graphs" );
			Module editorGraphModule = asm2.GetModule( "UnityEditor.Graphs.dll" );
			var typeAnimatorWindow = editorGraphModule.GetType( "UnityEditor.Graphs.AnimatorControllerTool" );
			var animatorWindow = EditorWindow.GetWindow( typeAnimatorWindow, false, "Animator", false );
		}

		void On_Animation_Icon() {
			var asm = Assembly.Load( "UnityEditor" );
			var typeAnimWindow = asm.GetType( "UnityEditor.AnimationWindow" );
			var animWindow = EditorWindow.GetWindow( typeAnimWindow, false, "Animation", false );
		}

		void On_AnimatorController_Icon() {
			var path = AssetDatabase.GetAssetPath( m_Controller.objectReferenceValue );

			//ProjectBrowser.selectionChangedLockProjectWindow( path );
		}

		void On_cs_Script_Icon() {
			var a = PrefabUtility.GetCorrespondingObjectFromSource( self.gameObject );
			var path = AssetDatabase.GetAssetPath( a );
			if( string.IsNullOrEmpty( path ) ) return;

			path = string.Format( "{0}/{1}.txt", Path.GetDirectoryName(path), Path.GetFileName( path) );
			using( var st = new StreamWriter( path ) ) {
				foreach( var state in m_clipState ) {
					st.WriteLine( string.Format( "{0} = {1}", state.name, Animator.StringToHash( state.name ) ) );
				}
			}
			AssetDatabase.Refresh();
			EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath<UnityObject>( path ) );
		}



		public void OnEnable() {

			m_Controller = serializedObject.FindProperty( "m_Controller" );
			m_Avatar = serializedObject.FindProperty( "m_Avatar" );
			m_ApplyRootMotion = serializedObject.FindProperty( "m_ApplyRootMotion" );
			m_CullingMode = serializedObject.FindProperty( "m_CullingMode" );
			m_UpdateMode = serializedObject.FindProperty( "m_UpdateMode" );
			m_WarningMessage = serializedObject.FindProperty( "m_WarningMessage" );
			//Init();



			makeClipList();

			//if( m_Controller.objectReferenceValue != null ) {
			//	m_editor = Editor.CreateEditor( m_Controller.objectReferenceValue, typeof( AnimatorControllerInspector ) ) as AnimatorControllerInspector;
			//}
		}

		public void OnDisable() {
			//	Debug.Log( "OnDisable" );
			if( m_clipPlay ) {
				EditorApplication.update -= updateMotion;
			}
		}

		#region update module

		float LastTime;
		float CurTime;
		float mTimeElipsed = 0.0f;
		float m_framePos;

		bool hasClipName() {
			if( m_clipState == null ) return false;
			if( m_clipState.Length == 0 ) return false;

			return true;
		}

		void updateMotion() {
			CurTime = Time.realtimeSinceStartup;
			float deltaTime = (float) ( CurTime - LastTime );
			mTimeElipsed += deltaTime * 1.00f;

			var actClip = m_clipState[ m_motionSelect ].motion as AnimationClip;
			if( actClip == null ) return;

			var st = self.GetCurrentAnimatorStateInfo( 0 );
			m_framePos += ( deltaTime * actClip.frameRate ); // ノーマライズ単位のdeltaTimeを足す
			if( actClip.toRealTime() < m_framePos ) {
				m_framePos = 0.00f;
			}
			LastTime = CurTime;

			self.Play( m_clipState[ m_motionSelect ].name, 0, m_framePos / actClip.toRealTime() );
			self.Update( 0 );

			//Debug.Log( "__Update: " );
			SceneView.RepaintAll();
			EditorHelper.ForceReloadInspectors();
		}

		#endregion


		bool ClickMenu( string label ) {
			if( Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown ) {
				var r = GUILayoutUtility.GetLastRect();
				var v2 = EditorStyles.label.CalcSize( EditorHelper.TempContent( label ) );
				r.width = v2.x;
				r.height = v2.y;
				//GUI.Box( r, "", (GUIStyle) "SelectionRect" );
				if( EditorHelper.HasMouseClick( r, EventMouseButton.M ) ) {
					return true;
				}
			}
			return false;
		}

		void MenuRevert( string label, SerializedProperty prop ) {
			if( ClickMenu( label ) ) {
				var menu = new GenericMenu();
				menu.AddItem( EditorHelper.TempContent( "Revert Value to Prefab" ), false, EditorHelper.SetPrefabOverride, prop );
				menu.ShowAsContext();
			}
		}

		void CreateNewController() {
			// UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/anm.controller");
			//	AssetDatabase.CreateAsset( anm,  );
			//m_Avatar.objectReferenceValue
			var assetPath = AssetDatabase.GetAssetPath( (UnityEngine.Object) m_Avatar.objectReferenceValue );
			if( string.IsNullOrEmpty( assetPath ) ) {
				Debug.LogError( "assetPath not found. avatar is null." );
				return;
			}

			var anmctr = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath( Path.GetDirectoryName( assetPath ) + "/" + Path.GetFileNameWithoutExtension( assetPath ) + ".controller" );

			serializedObject.Update();

			m_Controller.objectReferenceValue = anmctr;

			serializedObject.ApplyModifiedProperties();

			//Debug.Log(  );
			AssetDatabase.Refresh();

			EditorGUIUtility.PingObject( anmctr );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="dropArea"></param>
		void DragDrop( Rect dropArea ) {
			int id = GUIUtility.GetControlID( FocusType.Passive );
			var evt = Event.current;

			switch( evt.type ) {
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if( !dropArea.Contains( evt.mousePosition ) ) break;

					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					DragAndDrop.activeControlID = id;

					if( evt.type == EventType.DragPerform ) {
						DragAndDrop.AcceptDrag();

						foreach( var draggedObject in DragAndDrop.objectReferences ) {
							//Debug.Log( "Drag Object:" + AssetDatabase.GetAssetPath( draggedObject ) );

							if( typeof( AnimationClip ) == draggedObject.GetType() ) {
								var clip = draggedObject as AnimationClip;

								anmCtr.AddMotion( clip );
								makeClipList();
								continue;
							}

							UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( draggedObject ) );
							for( int i = 0; i < assets.Length; i++ ) {
								UnityEngine.Object asset = assets[ i ];
								//if( AssetDatabase.IsMainAsset( asset ) ) continue;

								//asset.hideFlags &= ~HideFlags.HideInHierarchy;
								//asset.hideFlags = HideFlags.HideInHierarchy;
								if( typeof( AnimationClip ) == asset.GetType() ) {
									if( asset.name.Contains( "__preview" ) ) {
										continue;
									}

									anmCtr.AddMotion( (AnimationClip) asset );
									makeClipList();
								}
							}

							//m_FilePath.stringValue = AssetDatabase.GetAssetPath( draggedObject );
						}
						DragAndDrop.activeControlID = 0;
					}
					Event.current.Use();
					break;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		void DrawPreviewButton() {
			using( new EditorGUI.DisabledGroupScope( !hasClipName() ) ) {

				using( new EditorGUILayout.HorizontalScope() ) {
					//if( GUILayout.Button( new GUIContent( "Cr" ), (GUIStyle) "Button", GUILayout.Width( 24f ), GUILayout.Height( 18f ) ) ) {
					//	self.CrossFadeInFixedTime( m_clipName[ m_motionSelect ], 0.1f, 0, 0.0f );
					//}
					if( m_clipPlay ) {
						if( GUILayout.Button( EditorGUIUtility.IconContent( "PauseButton" ), (GUIStyle) "Button", GUILayout.Width( 24f ), GUILayout.Height( 18f ) ) ) {
							m_clipPlay = !m_clipPlay;
							EditorApplication.update -= updateMotion;
						}
					}
					else {
						if( GUILayout.Button( EditorGUIUtility.IconContent( "PlayButton" ), (GUIStyle) "Button", GUILayout.Width( 24f ), GUILayout.Height( 18f ) ) ) {
							m_clipPlay = !m_clipPlay;
							EditorApplication.update += updateMotion;
						}
					}
					if( hasClipName() ) {
						var actClip = m_clipState[ m_motionSelect ].motion as AnimationClip;
						if( actClip != null ) {
							if( m_clipPlay ) {
								using( new EditorGUI.DisabledGroupScope( m_clipPlay ) ) {
									EditorGUILayout.Slider( m_framePos, 0.00f, actClip.toRealTime() );
								}
							}
							else {
								EditorGUI.BeginChangeCheck();
								m_framePos = EditorGUILayout.Slider( m_framePos, 0.00f, actClip.toRealTime() );
								if( EditorGUI.EndChangeCheck() ) {
									self.Play( actClip.name, 0, m_framePos / ( actClip.toRealTime() ) );
									self.Update( 0 );
									//AnimatorClipInfo[] ai = self.GetCurrentAnimatorClipInfo( 0 );
									//foreach( var a in ai ) {
									//	//Debug.Log( a.clip.length );
									//	Debug.Log( ( a.clip.frameRate * a.clip.length ) * m_framePos );
									//}
									SceneView.RepaintAll();
								}
							}
						}
					}
				}
			}

			if( !hasClipName() ) {
				EditorGUILayout.LabelField( "アニメーションがありません", (GUIStyle) "HelpBox" );
				DragDrop( GUILayoutUtility.GetLastRect() );
				return;
			}

			//GUIStyle styleListTitle = "OL Title";
			//GUIStyle styleListBox = "HelpBox";
			if( ( Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete ) ) {
				Debug.Log( "Delete: " + m_clipState[ m_motionSelect ] );
				AnimationControllerHelper.Remove( anmCtr, m_clipState[ m_motionSelect ] );
				makeClipList();
				if( m_clipState.Length <= m_motionSelect ) {
					m_motionSelect = m_clipState.Length - 1;
				}
				this.Repaint();
				return;
			}
			GUIStyle styleListElement = "PreferencesKeysElement";

			using( new GUILayout.VerticalScope( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) ) ) {
				using( var scr = new GUILayout.ScrollViewScope( this.m_KeyScrollPos, m_Styles.HelpBox ) ) {
					m_KeyScrollPos = scr.scrollPosition;

					for( int i = 0; i < m_clipState.Length; ++i ) {
						var a = m_clipState[ i ].name;
						GUI.changed = false;
						GUILayout.Toggle( i == m_motionSelect, a, styleListElement );
						if( GUI.changed ) {
							m_motionSelect = i;
							GUI.FocusControl( "" );
						}
					}
				}
			}

			DragDrop( GUILayoutUtility.GetLastRect() );
		}


		/// <summary>
		/// 
		/// </summary>
		bool DrawTitleToolIcon( Rect rc ) {
			Rect rc2 = rc;
			float yy = rc2.y;
			rc2.x = EditorGUIUtility.currentViewWidth - 18;
			rc2.y = yy - 1;
			rc2.width = 16;
			rc2.height = 18;
			GUI.Label( rc2, EditorGUIUtility.IconContent( "_Popup" ), GUIStyle.none );
			if( EditorHelper.HasMouseClick( rc2 ) ) {
				On_Popup();
				return true;
			}

			rc2.x -= 20;
			rc2.y = yy - 4;
			GUI.Label( rc2, EditorGUIUtility.IconContent( "Animator Icon" ), GUIStyle.none );
			if( EditorHelper.HasMouseClick( rc2 ) ) {
				On_Animator_Icon();
				return true;
			}

			rc2.x -= 20;
			rc2.width = 16;
			rc2.y = yy - 2;
			GUI.Label( rc2, EditorGUIUtility.IconContent( "Animation Icon" ), GUIStyle.none );
			if( EditorHelper.HasMouseClick( rc2 ) ) {
				On_Animation_Icon();
				return true;
			}

			rc2.x -= 20;
			rc2.y = yy - 3;
			GUI.Label( rc2, EditorGUIUtility.IconContent( "AnimatorController Icon" ), GUIStyle.none );
			if( EditorHelper.HasMouseClick( rc2 ) ) {
				On_AnimatorController_Icon();
				return true;
			}

			rc2.x -= 20;
			GUI.Label( rc2, EditorGUIUtility.IconContent( "cs Script Icon" ), GUIStyle.none );
			if( EditorHelper.HasMouseClick( rc2 ) ) {
				On_cs_Script_Icon();
				return true;
			}

			return false;
		}



		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			if( m_Styles == null ) {
				m_Styles = new Styles();
			}
			bool multiMode = base.targets.Length > 1;

			serializedObject.Update();

			EditorGUILayout.PropertyField( m_Controller, EditorHelper.TempContent( "コントローラ" ) );
			if( ClickMenu( "コントローラ" ) ) {
				var menu = new GenericMenu();
				menu.AddItem( new GUIContent( "Revert Value to Prefab" ), false, EditorHelper.SetPrefabOverride, m_Controller );
				menu.AddItem( new GUIContent( "Create New Controller" ), false, CreateNewController );
				menu.ShowAsContext();
			}


			EditorGUILayout.PropertyField( m_Avatar, EditorHelper.TempContent( "アバター" ) );
			MenuRevert( "アバター", m_Avatar );


			EditorGUILayout.PropertyField( m_ApplyRootMotion, EditorHelper.TempContent( "ルートモーション適用" ) );
			MenuRevert( "ルートモーション適用", m_ApplyRootMotion );

			EditorGUILayout.PropertyField( m_UpdateMode, EditorHelper.TempContent( "アップデートモード" ) );
			MenuRevert( "アップデートモード", m_UpdateMode );

			EditorGUILayout.PropertyField( m_CullingMode, EditorHelper.TempContent( "カリングモード" ) );
			MenuRevert( "カリングモード", m_CullingMode );

			serializedObject.ApplyModifiedProperties();


			if( !hasController() ) return;
			//if( string.IsNullOrEmpty( prefabPath ) ) return;

			GUILayout.Space( 4 );

			Rect rc = GUILayoutUtility.GetRect( 10, 14/*new GUIContent( "" ), "toolbarbutton"*/ );
			DrawTitleToolIcon( rc );

			rc.x = 1;
			rc.y -= 4;
			rc.width = EditorGUIUtility.currentViewWidth - 3;
			rc.height = 18;

			PreferenceSettings.i.AnimatorInspectorPreview.Value = EditorHelper.FoldoutTitlebar( rc, PreferenceSettings.i.AnimatorInspectorPreview, "Animator - Preview", false );
			if( PreferenceSettings.i.AnimatorInspectorPreview ) {
				DrawPreviewButton();
			}
			//EditorGUIUtility.FindTexture( "Animator Icon" )

			//GUILayout.Space( 8 );
		}

#endif
	}
}

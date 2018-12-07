


using UnityEngine;
using UnityEditor;

using System.Text.RegularExpressions;
using Hananoki;

namespace HananokiEditor {

	public static class Localization {
		public static string GetLocalizedString( string s ) {
			if( s == "Position" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "ローカル位置";
				}
			}
			if( s == "Position (World)" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "ワールド位置";
				}
			}
			
			if( s == "Rotation" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "ローカル角度";
				}
			}
			if( s == "Scale" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "スケール";
				}
			}
			if( s == "Scale (XYZ)" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "スケール (XYZ)";
				}
			}
			if( s == "Show World Position" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "ワールド位置を表示する";
				}
			}
			if( s == "Scale (XYZ) Mode" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "Scale (XYZ) モード";
				}
			}
			if( s == "Revert Value to Prefab" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "プレハブの値に戻す";
				}
			}
			if( s == "Fit Ground" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "地面に設置";
				}
			}
			if( s == "Copy" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "コピー";
				}
			}
			if( s == "Paste" ) {
				if( PreferenceSettings.i.LocalizeJP ) {
					return "ペースト";
				}
			}

			return s;
		}
	}


	[CustomEditor( typeof( Transform ) )]
	public class TransformInspector : Editor {

		public Transform self { get { return target as Transform; } }

		class Contents {
			public GUIContent positionContent = new GUIContent( Localization.GetLocalizedString( "Position" ) );
			public GUIContent scaleContent    = new GUIContent( Localization.GetLocalizedString( "Scale" ) );
			public GUIContent scaleXYZContent = new GUIContent( Localization.GetLocalizedString( "Scale (XYZ)" ) );
			public GUIContent worldPosContent = new GUIContent( Localization.GetLocalizedString( "Position (World)" ) );

			public GUIContent CopyContent = new GUIContent( Localization.GetLocalizedString( "Copy" ) );
			public GUIContent PasteContent = new GUIContent( Localization.GetLocalizedString( "Paste" ) );
			public GUIContent FitGroundContent = new GUIContent( Localization.GetLocalizedString( "Fit Ground" ) );
			public GUIContent ToggleWorldPosContnte = new GUIContent( Localization.GetLocalizedString( "Show World Position" ) );
			public GUIContent ScaleXYZModeContent = new GUIContent( Localization.GetLocalizedString( "Scale (XYZ) Mode" ) );
			public GUIContent RevertValuetoPrefabContent = new GUIContent( Localization.GetLocalizedString( "Revert Value to Prefab" ) );

			//public string floatingPointWarning = LocalizationDatabase.GetLocalizedString( "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range." );

			public string floatingPointWarning = "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";
		}

		static TransformInspector.Contents s_Contents;

		//TransformRotationGUI m_RotationGUI;
		TransformRotationGUIRefrection m_RotationGUI;

		static bool s_scaleMode = false;

		SerializedProperty m_positionProperty;
		SerializedProperty m_rotationProperty;
		SerializedProperty m_scaleProperty;

		GUIContent m_description00;
		GUIContent m_description01;



		[InitializeOnLoadMethod]
		static void InitializeOnLoadMethod() {
			PreferenceSettings.i.s_preferencesGUICallback += PreferencesGUI;
		}


		static void PreferencesGUI() {
			EditorGUILayout.LabelField( "TransformInspector", (GUIStyle) "ShurikenModuleTitle" );

			PreferenceSettings.i.LocalizeJP.Value = EditorGUILayout.ToggleLeft( "日本語化", PreferenceSettings.i.LocalizeJP, GUILayout.ExpandWidth( false ) );
			TransformInspector.s_Contents = null;

			var typeInspctWindow = System.Reflection.Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.InspectorWindow" );
			var InspctWindow = EditorWindow.GetWindow( typeInspctWindow, false, "Inspector", false );
			InspctWindow.Repaint();
		}


		bool GUIResetButton() {
			return GUILayout.Button( "", (GUIStyle) "ToggleMixed", GUILayout.Width( 12f ), GUILayout.Height( 12f ) );
		}


		bool isClickHit( Vector2 pos, Rect rc ) {
			if( rc.x < pos.x && pos.x < rc.max.x && rc.y < pos.y && pos.y < rc.max.y ) {
				return true;
			}
			return false;
		}

		void MenuCopyPos( object userData ) {
			SerializedProperty serializedProperty = (SerializedProperty) userData;
			var pos = serializedProperty.vector3Value;

			GUIUtility.systemCopyBuffer = string.Format( "{0}, {1}, {2}", pos.x, pos.y, pos.z );
		}

		void MenuPastePos( object userData ) {
			SerializedProperty serializedProperty = (SerializedProperty) userData;

			var mm = Regex.Matches( GUIUtility.systemCopyBuffer, @"(-?[0-9]+\.*[0-9]*)[\s,]+(-?[0-9]+\.*[0-9]*)[\s,]+(-?[0-9]+\.*[0-9]*)" );
			if( 0 < mm.Count ) {
				if( mm[ 0 ].Groups.Count == 4 ) {
					var a = float.Parse( mm[ 0 ].Groups[ 1 ].Value );
					var b = float.Parse( mm[ 0 ].Groups[ 2 ].Value );
					var c = float.Parse( mm[ 0 ].Groups[ 3 ].Value );

					serializedObject.Update();
					m_positionProperty.vector3Value = new Vector3( a, b, c );
					serializedObject.ApplyModifiedProperties();
				}
			}
			else {
				Debug.LogWarning( "transform is parse failed" );
			}
		}


		void Menu_FitGround( object userData ) {
			SerializedProperty serializedProperty = (SerializedProperty) userData;

			var result = new RaycastHit();
			if( Physics.Raycast( serializedProperty.vector3Value, Vector3.down, out result, 1000.0f, 1 ) ) {
				//serializedObject.Update();
				//serializedProperty.vector3Value = result.point;
				//serializedObject.ApplyModifiedProperties();
				Undo.RecordObject( self, "self" );
				self.position = result.point;
				EditorUtility.SetDirty( self );
			}
			//Debug.Log( "hitGround" );
		}

		void menuRotation( object userData, string[] options, int selected ) {
			switch( selected ) {
				case 0:
					m_copyRot = self.localEulerAngles;
					break;
				case 1:
					self.localEulerAngles = m_copyRot;
					break;
			}
		}


		void PosMenu_ShowWorldPos() {
			PreferenceSettings.i.TransformInspector_ShowWorld.Value = !PreferenceSettings.i.TransformInspector_ShowWorld.Value;
		}

		void SclMenu_ScaleXYZMode() {
			PreferenceSettings.i.TransformInspector_ScaleMode.Value = !PreferenceSettings.i.TransformInspector_ScaleMode.Value;
			s_scaleMode = PreferenceSettings.i.TransformInspector_ScaleMode.Value;
		}

		static Vector3 m_copyRot;


		/// <summary>
		/// ラベルクリックの判定を行います
		/// </summary>
		/// <param name="label"></param>
		/// <param name="prop"></param>
		/// <returns></returns>
		bool IsLabelClick( GUIContent label, SerializedProperty prop ) {
			if( Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown ) {
				var r = GUILayoutUtility.GetLastRect();
				var v2 = EditorStyles.label.CalcSize( label );
				r.width = v2.x;
				r.height = v2.y;
				//GUI.Box( r, "", (GUIStyle) "SelectionRect" );
				if( EditorHelper.HasMouseClick( r, EventMouseButton.M ) ) {
					return true;
				}
			}
			return false;
		}



		/// <summary>
		/// 
		/// </summary>
		public void OnEnable() {
			m_positionProperty = serializedObject.FindProperty( "m_LocalPosition" );
			m_rotationProperty = serializedObject.FindProperty( "m_LocalRotation" );
			m_scaleProperty = serializedObject.FindProperty( "m_LocalScale" );

			//m_CameraLookAt = self.GetComponent<CameraLookAt>();
			//if( m_CameraLookAt != null ) {
			//	m_editor = (Inspector_CameraLookAt) Editor.CreateEditor( m_CameraLookAt, typeof( Inspector_CameraLookAt ) );
			//}
			m_description00 = new GUIContent( "", "(0, 0, 0) にします" );
			m_description01 = new GUIContent( "", "(1, 1, 1) にします" );

			if( m_RotationGUI == null ) {
				m_RotationGUI = new TransformRotationGUIRefrection();
			}
			m_RotationGUI.OnEnable( serializedObject.FindProperty( "m_LocalRotation" ), new GUIContent( Localization.GetLocalizedString( "Rotation" ) ) );

			s_scaleMode = PreferenceSettings.i.TransformInspector_ScaleMode.Value;
		}


		

		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {

			if( TransformInspector.s_Contents == null ) {
				TransformInspector.s_Contents = new TransformInspector.Contents();
				//if( m_RotationGUI == null ) {
					m_RotationGUI = new TransformRotationGUIRefrection();
					m_RotationGUI.OnEnable( serializedObject.FindProperty( "m_LocalRotation" ), new GUIContent( Localization.GetLocalizedString( "Rotation" ) ) );
				//}
			}

			if( !EditorGUIUtility.wideMode ) {
				EditorGUIUtility.wideMode = true;
				EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212f;
			}

			this.serializedObject.Update();

			using( var h = new EditorGUILayout.HorizontalScope() ) {
				EditorGUILayout.PropertyField( m_positionProperty, TransformInspector.s_Contents.positionContent );
				if( IsLabelClick( s_Contents.positionContent, m_positionProperty ) ) {
					var menu = new GenericMenu();
					menu.AddItem( s_Contents.RevertValuetoPrefabContent, false, Helper.SetPrefabOverride, m_positionProperty );
					menu.AddItem( s_Contents.CopyContent, false, MenuCopyPos, m_positionProperty );
					menu.AddItem( s_Contents.PasteContent, false, MenuPastePos, m_positionProperty );
					menu.AddItem( s_Contents.ToggleWorldPosContnte, PreferenceSettings.i.TransformInspector_ShowWorld.Value, PosMenu_ShowWorldPos );
					menu.AddItem( s_Contents.FitGroundContent, false, Menu_FitGround, m_positionProperty );
					menu.ShowAsContext();
				}

				if( GUIResetButton() ) {
					m_positionProperty.vector3Value = Vector3.zero;
					GUI.FocusControl( "" );
				}
			}
			if( PreferenceSettings.i.TransformInspector_ShowWorld.Value ) {
				using( var h = new EditorGUILayout.HorizontalScope() ) {
					using( new EditorGUI.DisabledGroupScope( true ) ){
						EditorGUILayout.Vector3Field( TransformInspector.s_Contents.worldPosContent, self.position );
					}
				}
			}

			using( var h = new EditorGUILayout.HorizontalScope() ) {
				m_RotationGUI.RotationField();

				if( IsLabelClick( TransformInspector.s_Contents.positionContent, m_rotationProperty ) ) {
					var menu = new GenericMenu();
					menu.AddItem( s_Contents.RevertValuetoPrefabContent, false, Helper.SetPrefabOverride, m_rotationProperty );
					menu.ShowAsContext();
				}

				if( GUIResetButton() ) {
					if( m_rotationProperty == null ) {
						Debug.Log( "m_rotationProperty is null." );
					}
					else {
						Debug.Log( "m_rotationProperty is reset value." );
						m_rotationProperty.quaternionValue = Quaternion.Euler( Vector3.zero );
						GUI.FocusControl( "" );
					}
				}
			}


			using( new EditorGUILayout.HorizontalScope() ) {
				if( s_scaleMode ) {
					float f = m_scaleProperty.vector3Value.x;
					f = EditorGUILayout.FloatField( TransformInspector.s_Contents.scaleXYZContent, f, GUILayout.ExpandWidth( true ) );
					if( IsLabelClick( TransformInspector.s_Contents.scaleXYZContent, m_scaleProperty ) ) {
						var menu = new GenericMenu();
						menu.AddItem( s_Contents.RevertValuetoPrefabContent, false, Helper.SetPrefabOverride, m_scaleProperty );
						menu.AddItem( s_Contents.ScaleXYZModeContent, s_scaleMode, SclMenu_ScaleXYZMode );
						menu.ShowAsContext();
					}
					
					m_scaleProperty.vector3Value = new Vector3( f, f, f );
				}
				else {
					EditorGUILayout.PropertyField( m_scaleProperty, s_Contents.scaleContent );
					if( IsLabelClick( s_Contents.scaleContent, m_scaleProperty ) ) {
						var menu = new GenericMenu();
						menu.AddItem( s_Contents.RevertValuetoPrefabContent, false, Helper.SetPrefabOverride, m_scaleProperty );
						menu.AddItem( s_Contents.ScaleXYZModeContent, s_scaleMode, SclMenu_ScaleXYZMode );
						menu.ShowAsContext();
					}
				}

				if( GUIResetButton() ) {
					m_scaleProperty.vector3Value = Vector3.one; // Scaleの初期値はVector3(1f,1f,1f)
					GUI.FocusControl( "" );
				}
			}


			Transform transform = base.target as Transform;
			Vector3 position = transform.position;
			if( Mathf.Abs( position.x ) > 100000f || Mathf.Abs( position.y ) > 100000f || Mathf.Abs( position.z ) > 100000f ) {
				EditorGUILayout.HelpBox( TransformInspector.s_Contents.floatingPointWarning, MessageType.Warning );
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

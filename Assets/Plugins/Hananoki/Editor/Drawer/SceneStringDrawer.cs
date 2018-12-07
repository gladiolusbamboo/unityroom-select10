
using UnityEngine;
using UnityEditor;
using System.Linq;

using System.IO;
using Hananoki;

namespace HananokiEditor {

	[CustomPropertyDrawer( typeof( SceneString ) )]
	class SceneStringDrawer : PropertyDrawer {

		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {
			
			SceneAsset scn = null;
			var _guid = property.FindPropertyRelative( "guid" );
			var _name = property.FindPropertyRelative( "name" );

			if( !string.IsNullOrEmpty( _guid.stringValue ) ) {
				scn = AssetDatabase.LoadAssetAtPath<SceneAsset>( AssetDatabase.GUIDToAssetPath( _guid.stringValue ) );
			}
			var rcField = EditorGUI.PrefixLabel( rc, GUIUtility.GetControlID( FocusType.Passive ), label );
			var rcLabel = rc;
			var size = EditorStyles.label.CalcSize( label );
			rcLabel.width = size.x;
			//GUI.Box( rcLabel, "", (GUIStyle) "SelectionRect" );
			
			if( EditorHelper.HasMouseClick( rcLabel ) ) {
				var menu = new GenericMenu();
				foreach( var s in getBuildSceneName() ) {
					menu.AddItem( new GUIContent( Path.GetFileNameWithoutExtension( s ) ), false, ( userData ) => {
						property.serializedObject.Update();

						string ss = (string) userData;
						_name.stringValue = Path.GetFileNameWithoutExtension(ss);
						var gu = GetGUID( s );
						_guid.stringValue = gu;

						property.serializedObject.ApplyModifiedProperties();
					}, s );
				}
				menu.ShowAsContext();
			}
			
			GUI.changed = false;
			SceneAsset value = (SceneAsset) EditorGUI.ObjectField( rcField, scn, typeof( SceneAsset ), false );
			if( GUI.changed ) {
				_name.stringValue = value.name;
				var ss = AssetDatabase.GetAssetPath( value );
				_guid.stringValue = Path.GetFileNameWithoutExtension( AssetDatabase.AssetPathToGUID( ss ) );
			}
		}

		static string GetGUID( string path ) {
			foreach(  var f in EditorBuildSettings.scenes){
				if( f.path == path ) {
					return f.guid.ToString();
				}
			}
			return "";
		}

		/// <summary>
		/// EditorBuildSettingsからシーン名の配列を取得する
		/// </summary>
		/// <returns>シーン名の配列</returns>
		static string[] getBuildSceneName() {
			return EditorBuildSettings.scenes
						.Where( it => it.enabled )
						.Select( it => it.path )
						.ToArray();
		}


	} // class SceneNameAttributeDrawer
}

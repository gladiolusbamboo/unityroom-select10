
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;

namespace Harmony.Toolkit {

	public class SubAssetEditorWindow : EditorWindow {
		[MenuItem( "Tools/SubAssetEditor" )]
		static void open() {
			EditorWindow.GetWindow<SubAssetEditorWindow>();//.titleContent = new GUIContent( "SubAsset", EditorGUIUtility.FindTexture( "SceneViewFx" ) );
		}

		UnityObject m_target;
		bool m_hide;
		Object m_addObj;

		void drawGUI() {

			var r1 = new Rect( 10, 10, position.width - 10-10, 16 );
			bool modifid = false;

			GUILayout.BeginArea( new Rect( 10, 10, position.width - 10 - 10, position.height ) );

			m_target = EditorGUILayout.ObjectField( "Target Obj", m_target, typeof( UnityObject ), false ) as UnityObject;
			m_addObj = EditorGUILayout.ObjectField( "Add Obj", m_addObj, typeof( Object ), false ) as Object;

			GUILayout.BeginHorizontal();
			if( GUILayout.Button( "Hide/On" ) ) {
				Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( m_target ) );
				for( int i = 0; i < assets.Length; i++ ) {
					Object asset = assets[ i ];
					if( AssetDatabase.IsMainAsset( asset ) ) continue;

					asset.hideFlags = HideFlags.HideInHierarchy;
				}
				modifid = true;
			}
			if( GUILayout.Button( "Hide/Off" ) ) {
				Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( m_target ) );
				for( int i = 0; i < assets.Length; i++ ) {
					Object asset = assets[ i ];
					if( AssetDatabase.IsMainAsset( asset ) ) continue;

					asset.hideFlags &= ~HideFlags.HideInHierarchy; 
					//asset.hideFlags = HideFlags.HideInHierarchy;
				}
				modifid = true;
			}
			if( GUILayout.Button(  "Check" ) ) {
				Object[] assets = AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( m_target ) );
				for( int i = 0; i < assets.Length; i++ ) {
					Object asset = assets[ i ];
					//if( UnityEditor.AssetDatabase.IsMainAsset( asset ) || asset is GameObject || asset is Component ) continue;
					//else Object.DestroyImmediate( asset, true );
					Debug.Log( "typeof= " + asset.GetType().Name + " : " + asset.name + " : " + AssetDatabase.IsMainAsset( asset ) );
				}
			}
			GUILayout.EndHorizontal();

			var rr = GUILayoutUtility.GetRect( new GUIContent( "" ), GUIStyle.none );

			var r2 = new Rect( rr );
			r2.x = rr.x + rr.width - 32;
			r2.width = 16;
			if( GUI.Button( r2, "", "OL Plus" ) ) {
				bool add = false;
				System.Type t = m_addObj.GetType();
				if( t == typeof( Texture2D ) ) {
					//Texture2D addTex = (Texture2D) m_addObj;
					//Texture2D a = exportTexturePng( (Texture2D) m_addObj );
					//a.name = m_addObj.name;

					//Texture2D texCopy = new Texture2D( addTex.width, addTex.height, addTex.format, addTex.mipmapCount > 1 );
					//texCopy.LoadImage( addTex.EncodeToPNG() );
					//texCopy.Apply();

					//AssetDatabase.AddObjectToAsset( a, m_target );
					//add = true;

					var a = Object.Instantiate( m_addObj );
					a.name = m_addObj.name;
					AssetDatabase.AddObjectToAsset( a, m_target );
					add = true;
				}
				else {
					try {
						var a = Object.Instantiate( m_addObj );
						a.name = m_addObj.name;
						AssetDatabase.AddObjectToAsset( a, m_target );
						add = true;
					}
					catch( System.Exception e ) {
						Debug.LogError( e );
					}
				}

				if( add ) {
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}
			r2.x += 16;
			if( GUI.Button( r2, "", "OL Minus" ) ) {
				var obj = Selection.activeObject;
				Object.DestroyImmediate( obj, true );

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			GUILayout.EndArea();

			if( modifid ) {
				EditorUtility.SetDirty( m_target );

				AssetDatabase.SaveAssets();
				//AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_target ) );
				AssetDatabase.Refresh();
			}
		}


		Texture2D exportTexturePng( Texture2D m_tex ) {
			Texture2D texCopy = new Texture2D( m_tex.width, m_tex.height, m_tex.format, m_tex.mipmapCount > 1 );
			texCopy.LoadRawTextureData( m_tex.GetRawTextureData() );
			texCopy.Apply();

//			byte[] pngData = texCopy.EncodeToPNG();   // pngのバイト情報を取得.

			return texCopy;
		}

		// Windowのクライアント領域のGUI処理を記述
		void OnGUI() {
			try {
				drawGUI();
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}
	}
}


// Simply editor window that lets you quick check a path of
	// a texture in your project instead of waiting your code to 
	// compile.
	//
	// if the path exists then it shows a message
	// else displays an error message
	
using UnityEditor;
using UnityEngine;

using System.IO;

namespace Harmony.Toolkit {
	class WindowIconExport : EditorWindow {

		string m_path = "";
		public Texture2D m_tex;

		[MenuItem( "Tools/Icon Export" )]
		static void Init() {
			var window = EditorWindow.GetWindow<WindowIconExport>();
			window.position = new Rect( 0, 0, 180, 55 );
			window.Show();
		}

		public void OnEnable() {
			var rc = this.position;
			rc.width = 180;
			rc.height = 55;
		}


		void OnGUI() {
			using( new EditorGUILayout.HorizontalScope() ) {
				m_path = EditorGUILayout.TextField( m_path );

				if( GUILayout.Button( "Check" ) ) {
					var tex = EditorGUIUtility.LoadRequired( m_path ) as Texture2D;
					if( tex != null ) {
						m_tex = tex;
					}
				}
			}

			m_tex = EditorGUILayout.ObjectField( m_tex, typeof( Texture2D ), false ) as Texture2D;



			if( GUILayout.Button( "Save" ) ) {
#if UNITY_5_0
#else
				Texture2D texCopy = new Texture2D( m_tex.width, m_tex.height, m_tex.format, m_tex.mipmapCount > 1 );
				texCopy.LoadRawTextureData( m_tex.GetRawTextureData() );
				texCopy.Apply();

				byte[] pngData = texCopy.EncodeToPNG();   // pngのバイト情報を取得.

				// ファイルダイアログの表示.
				string filePath = EditorUtility.SaveFilePanel( "Save Texture", "", m_tex.name + ".png", "png" );

				if( filePath.Length > 0 ) {
					// pngファイル保存.
					File.WriteAllBytes( filePath, pngData );
				}
#endif
			}
		}
	}
}

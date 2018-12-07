using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Hananoki;

namespace HananokiEditor {

	/// <summary>
	/// レイヤー名を定数で管理するクラスを作成するスクリプト
	/// </summary>
	public static class Export_LayerTagScene {
		// 無効な文字を管理する配列
		private static readonly string[] INVALUD_CHARS =
		{
				" ", "!", "\"", "#", "$",
				"%", "&", "\'", "(", ")",
				"-", "=", "^",  "~", "\\",
				"|", "[", "{",  "@", "`",
				"]", "}", ":",  "*", ";",
				"+", "/", "?",  ".", ">",
				",", "<", "・"
		};

		static string PATH_OUTPUT { get { return AssetDatabase.GUIDToAssetPath( LocalSettings.i.folderDefine ); } }
		static string PATH_LAYER { get { return PATH_OUTPUT + "/LayerName.cs"; } }
		static string PATH_TAG { get { return PATH_OUTPUT + "/TagName.cs"; } }
		static string PATH_SCENE { get { return PATH_OUTPUT + "/SceneName.cs"; } }

		static string RemoveInvalidChars( string str ) {
			Array.ForEach( INVALUD_CHARS, c => str = str.Replace( c, string.Empty ) );
			return str;
		}


		static void WriteFile( string fname, Action<StringBuilder> func ) {
			var builder = new StringBuilder();

			func( builder );

			var directoryName = Path.GetDirectoryName( fname );
			if( !Directory.Exists( directoryName ) ) {
				Directory.CreateDirectory( directoryName );
			}


			File.WriteAllText( fname, builder.ToString().Replace( "\r\n", "\n" ), Encoding.UTF8 );
			AssetDatabase.Refresh( ImportAssetOptions.ImportRecursive );
		}


		/// <summary>
		/// レイヤー名を定数で書き出します
		/// </summary>
		public static void MakeLayerNameFile() {
			Debug.Log( PATH_LAYER );
			WriteFile( PATH_LAYER, ( builder ) => {
				builder.AppendLine( "/// このファイルは自動作成しました" );
				builder.AppendLine( "" );
				builder.AppendLine( "/// <summary>" );
				builder.AppendLine( "/// レイヤー名を定数で管理するクラス" );
				builder.AppendLine( "/// </summary>" );
				builder.AppendLine( "namespace Game {" );
				builder.AppendFormat( "\tpublic static partial class Layer" );
				builder.AppendLine( " {" );


				foreach( var n in InternalEditorUtility.layers.
						Select( c => new { var = RemoveInvalidChars( c ), val = LayerMask.NameToLayer( c ) } ) ) {
					builder.Append( "\t\t" ).AppendFormat( @"public const int {0} = {1};", n.var, n.val ).AppendLine();
				}
				builder.AppendLine();
				foreach( var n in InternalEditorUtility.layers.
						Select( c => new { var = RemoveInvalidChars( c ), val = 1 << LayerMask.NameToLayer( c ) } ) ) {
					builder.Append( "\t\t" ).AppendFormat( @"public const int {0}Mask = {1};", n.var, n.val ).AppendLine();
				}

				builder.AppendLine( "\t}\n}" );
			} );
		}


		/// <summary>
		/// タグ名を定数で書き出します
		/// </summary>
		public static void MakeTagNameFile() {
			WriteFile( PATH_TAG, ( builder ) => {

				builder.AppendLine( "/// このファイルは自動作成しました" );
				builder.AppendLine( "" );
				builder.AppendLine( "/// <summary>" );
				builder.AppendLine( "/// タグ名を定数で管理するクラス" );
				builder.AppendLine( "/// </summary>" );
				builder.AppendLine( "namespace Game {" );
				builder.AppendFormat( "\tpublic static class Tag" );
				builder.AppendLine( "{" );

				foreach( var n in InternalEditorUtility.tags.
						Select( c => new { var = RemoveInvalidChars( c ), val = c } ) ) {
					builder.Append( "\t\t" ).AppendFormat( @"public const string {0} = ""{1}"";", n.var, n.val ).AppendLine();
				}

				builder.AppendLine( "\t}\n}" );
			} );
		}


		/// <summary>
		/// シーン名を定数で書き出します
		/// </summary>
		public static void MakeSceneNameFile() {
			WriteFile( PATH_SCENE, ( builder ) => {
				builder.AppendLine( "/// このファイルは自動作成しました" );
				builder.AppendLine( "" );
				builder.AppendLine( "/// <summary>" );
				builder.AppendLine( "/// シーン名を定数で管理するクラス" );
				builder.AppendLine( "/// </summary>" );
				builder.AppendLine( "namespace Game {" );
				builder.AppendFormat( "	public class Scene" );
				builder.AppendLine( "{" );
				
				//List<string> sceneNames = new List<string>();
				foreach( var n in EditorBuildSettings.scenes
						.Select( c => Path.GetFileNameWithoutExtension( c.path ) )
						.Distinct()
						.Select( c => new { var = RemoveInvalidChars( c ), val = c } ) ) {
					builder.Append( "\t\t" ).AppendFormat( @"public const string {0} = ""{1}"";", n.var, n.val ).AppendLine();
				}

				builder.AppendLine( "	}\n}" );
			} );
		}
	}
}

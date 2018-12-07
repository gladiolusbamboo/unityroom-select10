
using System.IO;
using System.Linq;
using UnityEngine;

namespace Hananoki {

	public static class DirectoryUtils {
		/// <summary>
		/// <para>指定したディレクトリ内のファイルの名前 (パスを含む) を返します</para>
		/// <para>パスの区切り文字は「\\」ではなく「/」です</para>
		/// </summary>
		public static string[] GetFiles( string path ) {
			return Directory
					.GetFiles( path )
					.Select( c => c.Replace( "\\", "/" ) )
					.ToArray();
		}

		/// <summary>
		/// <para>指定したディレクトリ内の指定した検索パターンに一致するファイル名 (パスを含む) を返します</para>
		/// <para>パスの区切り文字は「\\」ではなく「/」です</para>
		/// </summary>
		public static string[] GetFiles( string path, string searchPattern ) {
			return Directory
					.GetFiles( path, searchPattern )
					.Select( c => c.Replace( "\\", "/" ) )
					.ToArray();
		}

		/// <summary>
		/// <para>指定したディレクトリの中から、指定した検索パターンに一致し、</para>
		/// <para>サブディレクトリを検索するかどうかを決定する値を持つファイル名 (パスを含む) を返します</para>
		/// <para>パスの区切り文字は「\\」ではなく「/」です</para>
		/// </summary>
		public static string[] GetFiles( string path, string searchPattern, SearchOption searchOption ) {
			return Directory
					.GetFiles( path, searchPattern, searchOption )
					.Select( c => c.Replace( "\\", "/" ) )
					.ToArray();
		}
	}



	internal static class fs {
		public static void mkdir( string path ) {
			if( !Directory.Exists( path ) ) {
				Directory.CreateDirectory( path );
			}
		}
		public static void mv( string src, string dst ) {
			if( File.Exists( dst ) ) {
				File.Delete( dst );
			}
			if( File.Exists( src ) ) {
				File.Move( src, dst );
			}
		}
		public static void cp( string src, string dst ) {
			if( File.Exists( dst ) ) return;

			File.Copy( src, dst );
		}
		public static void rm( string path ) {
			try {
				if( Directory.Exists( path ) ) {
					Directory.Delete( path, true );
				}
				else if( File.Exists( path ) ) {
					File.Delete( path );
				}
			}
			catch( DirectoryNotFoundException e ) {
				Debug.LogError( e );
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}
	}
}

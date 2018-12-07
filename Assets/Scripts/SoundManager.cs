using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Hananoki;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using Hananoki;
#endif

namespace GJW {
	public class SoundManager : MonoBehaviour {
		[FormerlySerializedAs( "clips" )]
		public AudioClip[] se;
		public AudioClip[] bgm;
		public AudioClip[] voice;
		public AudioSource[] source;
		static SoundManager instance;

		public static void PlayVoice( int no ) {
			instance._PlayVoice( no, 0 );
		}
		void _PlayVoice( int no, int cannel ) {
			source[ cannel ].clip = voice[ no ];
			source[ cannel ].Play();
		}

		public static void PlayBgm( int no ) {
			instance._PlayBgm( no, 1 );
		}
		void _PlayBgm( int no, int cannel ) {
			var s = source[ cannel ];
			if( s.isPlaying ) {
				if( s.clip == bgm[ no ] ) return;
			}
			s.clip = bgm[ no ];
			s.Play();
		}

		public static void StopBgm( ) {
			instance._StopBgm( 1 );
		}
		void _StopBgm( int cannel ) {
			source[ cannel ].Stop();
		}

		public static void PlaySound( int no, int cannel = 0 ) {
			instance._PlaySound( no, cannel + 2 );
		}

		void _PlaySound( int no , int cannel ) {
			source[ cannel ].clip = se[ no ];
			source[ cannel ].Play();
		}

		public static void StopAll() {
			instance._StopAll();
		}

		void _StopAll() {
			foreach(var s in source ) {
				s.Stop();
			}
		}

		/// <summary>
		/// この関数はオブジェクトが有効/アクティブになった時に呼び出されます
		/// </summary>
		void OnEnable() {
			if( instance == null ) {
				instance = this;
			}
		}

		void Awake() {
			if( instance != null ) {
				Destroy( gameObject );
				return;
			}
			DontDestroyOnLoad( gameObject );
			instance = this;
		}
#if UNITY_EDITOR
		[InspectorGUI]
		void InspectorGUI() {
			if( GUILayout.Button("出力") ) {
				var lst = se.Select( x => x.name ).ToArray();
				var lst2 = bgm.Select( x => x.name ).ToArray();
				var lst3 = voice.Select( x => x.name ).ToArray();
				var dia = AssetDatabase.GUIDToAssetPath( LocalSettings.i.folderDefine );
				var output = dia + @"\SoundManagerHelper.cs";
				//output = output.Replace( "/", "\\" );
				EditorHelper.writeFile( output, ( a ) => {
					a.AppendLine( @"/* エディタースクリプト側でコンパイルしてください */" );
					a.AppendLine( @"using UnityEngine;" );
					a.AppendLine( @"using GJW;" );
					
					a.AppendLine();
					a.AppendLine( @"public static partial class sound {" );
					//a.AppendLine( @"	public const string MENU_NAME = ""シーンリスト/"";" );
					for( int i = 0; i < lst.Length; i++ ) {
						a.Append( $"	public static void {lst[ i ]}( int c = 0 ) {{ SoundManager.PlaySound( {i}, c ); }} " ).AppendLine();
						a.AppendLine();
					}
					for( int i = 0; i < lst2.Length; i++ ) {
						a.Append( $"	public static void bgm_{lst2[ i ]}( int c = 0 ) {{ SoundManager.PlayBgm( {i} ); }} " ).AppendLine();
						a.AppendLine();
					}
					for( int i = 0; i < lst3.Length; i++ ) {
						a.Append( $"	public static void voice_{lst3[ i ]}( int c = 0 ) {{ SoundManager.PlayVoice( {i} ); }} " ).AppendLine();
						a.AppendLine();
					}

					a.AppendLine( @"}" );
				} );
			}
			if( GUILayout.Button( "get" ) ) {
				EditorHelper.Dirty(this,()=> { source = GetComponents<AudioSource>(); } );
				
			}
		}
#endif
	}
}

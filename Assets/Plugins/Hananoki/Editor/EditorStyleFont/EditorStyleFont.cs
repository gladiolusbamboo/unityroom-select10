
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Reflection;
using Hananoki;


namespace HananokiEditor {

	public class EditorStyleFontPreference : EditorWindow {

		[MenuItem( "Edit/EditorStyleFont Settings" )]
		static void ShowEditorStyleFontSettings() {
			var window = EditorWindow.GetWindow<EditorStyleFontPreference>();
			window.SetTitle( new GUIContent( "EditorStyleFont", Icon.Get( "SettingsIcon" ) ) );
		}

		const string BASE = "EditorStyleFontSettings.";
		const string STARTUPAPPLY = BASE + "StartupApply";

		GUIStyle sectionHeader;


		//Font m_StandardFont;
		//Font m_BoldFont;
		//Font m_MiniFont;
		//Font m_MiniBoldFont;
		//Font m_BigFont;
		//Font m_WarningFont;
		public enum Kind {
			StandardFont,
			BoldFont,
			MiniFont,
			MiniBoldFont,
			BigFont,
			WarningFont,
		}

		static Font[] s_fontList;


		public static bool startupApply {
			get {
				return EditorPrefs.GetBool( STARTUPAPPLY, false );
			}
			set {
				EditorPrefs.SetBool( STARTUPAPPLY, value );
			}
		}

		static int fontTypeLength { get { return System.Enum.GetNames( typeof( Kind ) ).Length; } }

		static Font[] fontList {
			get {
				if( s_fontList == null ) {
					s_fontList= GetFontList();
				}
				return s_fontList;
			}
		}



		static string GetPrefString( string prefName ) {
			return EditorPrefs.GetString( prefName, "" );
		}

		static void SetPrefString( string prefName, string value ) {
			EditorPrefs.SetString( prefName, value );
		}

		static void UpdatePref() {
			int len = fontTypeLength;
			for( int i =0; i < len; i++ ) {
				var name = BASE+( (Kind) i ).ToString();
				SetPrefString( name, EditorHelper.ToGUID( fontList[i] ) );
			}
		}

		internal static Font[] GetFontList() {
			var lst = new Font[ 0 ];
			int len = fontTypeLength;
			for( int i =0; i < len; i++ ) {
				var name = BASE+( (Kind) i ).ToString();
				ArrayUtility.Add( ref lst, GetPrefString( name ).GUID2Asset<Font>() );
			}
			return lst;
		}



		/// <summary>
		/// 
		/// </summary>
		static void DrawGUI() {
			startupApply = EditorGUILayout.Toggle( "Startup Apply", startupApply );

			GUILayout.Space( 8f );

			int n = fontTypeLength;
			GUI.changed = false;
			for( int i =0; i < n; i++ ) {
				var label = ( (Kind) i ).ToString();
				fontList[ i ] = EditorGUILayout.ObjectField( label, fontList[ i ], typeof( Font ), false ) as Font;
			}
			if( GUI.changed ) {
				UpdatePref();
			}

			GUILayout.Space( 8f );

			using( new GUILayout.HorizontalScope() ) {
				GUILayout.FlexibleSpace();
				if( EditorStyleFont.s_Apply ) {
					if( GUILayout.Button( "Revert", GUILayout.Width( 80 ) ) ) {
						EditorStyleFont.Revert();
					}
				}
				else {
					if( GUILayout.Button( "Apply", GUILayout.Width( 80 ) ) ) {
						EditorStyleFont.Apply();
					}
				}
			}
		}



		/// <summary>
		/// Implement your own editor GUI here.
		/// </summary>
		void OnGUI() {
			try {
				var w = position.width - 10;
				var h = position.height;
				using( new EditorGUI.DisabledGroupScope( EditorApplication.isCompiling ) ) {
					using( new GUILayout.AreaScope( new Rect( 10, 10, w - 10, h ) ) ) {
						if( sectionHeader == null ) {
							sectionHeader = new GUIStyle( EditorStyles.largeLabel );
							sectionHeader.fontStyle = FontStyle.Bold;
							sectionHeader.fontSize = 18;
							sectionHeader.margin.top = 10;
							sectionHeader.margin.left++;
							if( !EditorGUIUtility.isProSkin ) {
								sectionHeader.normal.textColor = new Color( 0.4f, 0.4f, 0.4f, 1f );
							}
							else {
								sectionHeader.normal.textColor = new Color( 0.7f, 0.7f, 0.7f, 1f );
							}
						}
						GUILayout.Label( "EditorStyleFont", sectionHeader );
						GUILayout.Space( 10f );
						DrawGUI();
					}
				}
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
		}


		[PreferenceItem( "EditorStyleFont" )]
		public static void PreferencesGUI() {
			DrawGUI();
		}
	}




	[InitializeOnLoad]
	public static class EditorStyleFont {

		public static bool s_Apply;

		static Dictionary<string, string> s_fontBak;


		static string[] LucidaGrande = {
			"Lucida Grande",
			"Lucida Grande Bold",
			"Lucida Grande small",
			"Lucida Grande small bold",
			"Lucida Grande Big",
			"Lucida Grande Warning",
		};

		

		public static Font StandardFont( this Font[] f ) {
			return f[ (int) EditorStyleFontPreference.Kind.StandardFont ];
		}
		public static Font BoldFont( this Font[] f ) {
			return f[ (int) EditorStyleFontPreference.Kind.BoldFont ];
		}
		public static Font MiniFont( this Font[] f ) {
			return f[ (int) EditorStyleFontPreference.Kind.MiniFont ];
		}
		public static Font MiniBoldFont( this Font[] f ) {
			return f[ (int) EditorStyleFontPreference.Kind.MiniBoldFont ];
		}




		static EditorStyleFont() {
			s_Apply = false;
			if( !EditorStyleFontPreference.startupApply ) return;
			Apply();
		}

		
		public static void Apply() {
			EditorApplication.update += _apply;
		}

		public static void Revert() {
			EditorApplication.update += _revert;
		}



		public static void _revert() {
			var fonts = Resources.FindObjectsOfTypeAll( typeof(Font) );
			Font[] lst = new Font[0];
			foreach( var p in LucidaGrande ) {
				var fontAsset = (Font) ArrayUtility.Find( fonts, a => a.name == p );
				ArrayUtility.Add( ref lst, fontAsset );
			}
			
			//foreach( var aa in lst ) {
			//	Debug.Log(aa.name);
			//}

			try {
				if( !_set( lst ) ) {
					return;
				}
				s_Apply = false;
			}
			catch( System.Exception e ){
				Debug.LogError( e );
			}
			EditorApplication.update -= _revert;
		}



		public static void _apply() {
			try {
				s_fontBak = new Dictionary<string, string>();
				if( !_set( EditorStyleFontPreference.GetFontList() ) ) {
					return;
				}
				s_Apply = true;
			}
			catch( System.Exception e ) {
				Debug.LogError( e );
			}
			EditorApplication.update -= _apply;
		}


		public static bool _set( Font[] fontList ) {

			var EditorStylesType = Assembly.Load( "UnityEditor.dll" ).GetType( "UnityEditor.EditorStyles" );
			var s_Current = EditorStylesType.GetField( "s_Current", BindingFlags.NonPublic | BindingFlags.Static );
			var s_CurrentV = s_Current.GetValue( null );
			if( s_CurrentV == null ) return false;

			var EditorStylesFields = EditorStylesType.GetFields( BindingFlags.NonPublic | BindingFlags.Instance );

			foreach( var p in EditorStylesFields ) {
				if( typeof( GUIStyle ) == p.FieldType ) {
					var style = p.GetValue( s_CurrentV ) as GUIStyle;
					if( style.font == null ) {
					}
					else {
						//Lucida Grande Big
						//Lucida Grande Warning
						if( s_Apply ) {
							if( s_fontBak.ContainsKey( p.Name ) ) {
								var idx = ArrayUtility.FindIndex( LucidaGrande, a => a == s_fontBak[ p.Name ] );
								style.font = fontList[ idx ];
								//Debug.Log( "revert: " + p.Name +" : "+ s_fontBak[ p.Name ] );
							}
						}
						else {
							for( int i = 0; i < fontList.Length; i++ ) {
								if( style.font.name == LucidaGrande[ i ] && fontList[ i ] != null ) {
									style.font = fontList[ i ];
									s_fontBak.Add( p.Name, LucidaGrande[ i ] );
								}
							}
						}
					}
				}
				if( typeof( Font ) == p.FieldType ) {
					if( p.Name == "m_StandardFont" ) {
						if( fontList.StandardFont() != null ) {
							p.SetValue( s_CurrentV, fontList.StandardFont() );
						}
					}
					if( p.Name == "m_BoldFont" ) {
						if( fontList.BoldFont() != null ) {
							p.SetValue( s_CurrentV, fontList.BoldFont() );
						}
					}
					if( p.Name == "m_MiniFont" ) {
						if( fontList.MiniFont() != null ) {
							p.SetValue( s_CurrentV, fontList.MiniFont() );
						}
					}
					if( p.Name == "m_MiniBoldFont" ) {
						if( fontList.MiniBoldFont() != null ) {
							p.SetValue( s_CurrentV, fontList.MiniBoldFont() );
						}
					}
				}
			}

			//foreach( var e in Resources.FindObjectsOfTypeAll( typeof( EditorWindow ) ) ) {
			//	if( e == null ) continue;
			//	( (EditorWindow) e ).Repaint();
			//}
			EditorUtils.RepaintEditorWindow();

			return true;
		}
	}
}

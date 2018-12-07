
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。

using System;
using System.IO;
using UnityEngine.Profiling;

namespace kids02 {

	[System.Serializable]
	public class GameSysWindow : EditorWindow {
		[MenuItem( "Window/GameSysWindow" )]
		static void open() {
			EditorWindow.GetWindow<GameSysWindow>();
		}

		class Styles {
			public GUIStyle BoldLabel;
			public Styles() {
				BoldLabel = new GUIStyle( (GUIStyle) "BoldLabel" );
			}
		}
		Styles m_style;

		string m_os_info;
		string m_cpu_info;
		string m_gpu_info;
		string m_resolution_info;
		//string m_audio_info;
		//int m_frame_counts;
		//float m_start_time;
		//float m_fps;

		string m_memory;

		/// <summary>
		/// 
		/// </summary>
		public void OnEnable() {
			titleContent = new GUIContent( "GameSys" );

			m_os_info = string.Format( "OS: {0}", SystemInfo.operatingSystem );
			m_cpu_info = string.Format( "CPU: {0} / {1}cores", SystemInfo.processorType, SystemInfo.processorCount );
			m_gpu_info = string.Format( "GPU: {0} / {1}MB API: {2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.graphicsDeviceType );

			//OnRectTransformDimensionsChange();
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnDisable() {
		}

		void OnInspectorUpdate() {
			Resolution reso = Screen.currentResolution;
			m_resolution_info = string.Format( "Resolution: {0} x {1} RefreshRate: {2}Hz", reso.width, reso.height, reso.refreshRate );

			const uint mega = 1024 * 1024;
			m_memory = string.Format( "Memory: {0:####.0} / {1}.0MB GCCount: {2}", Profiler.usedHeapSizeLong / (float) mega, SystemInfo.systemMemorySize, System.GC.CollectionCount( 0 ) );
			
		}



		int SSSize = 0;
		public static int Multi = 1;

		static string MakeScreenCaptureFileName( int width, int height ) {
			string fpath = string.Format( "{0}/ScreenShot/ScreenShot_{1}x{2}_{3}",
													Directory.GetCurrentDirectory(),
													width, height,
													DateTime.Now.ToString( "yyyy-MM-dd_HH-mm-ss" ) );

			string f = fpath;
			int index = 1;
			//もし名前が被ったら、+1を最後に付ける（多分被ることはないと思うけど念のため）
			while( File.Exists( f + ".png" ) ) {
				f = string.Format( "{0}_{1}", fpath, index );
				index++;
			}

			return f + ".png";
		}


		public static void SaveScreenCapture() {
			var dname = Directory.GetCurrentDirectory() + "/ScreenShot";
			if( !Directory.Exists( dname ) ) {
				Directory.CreateDirectory( dname );
			}

			string filename = MakeScreenCaptureFileName( (int) Handles.GetMainGameViewSize().x * Multi, (int) Handles.GetMainGameViewSize().y * Multi );
#if true//UNITY_2017_3_OR_NEWER
			ScreenCapture.CaptureScreenshot( filename, Multi );
#else
			Application.CaptureScreenshot( filename, Multi );
#endif
			//Debug.Log( string.Format( "スクリーンショット撮影完了: {0}", filename ) );

			var gameview = EditorWindow.GetWindow( typeof( EditorWindow ).Assembly.GetType( "UnityEditor.GameView" ) );
			// GameViewを再描画 
			gameview.Repaint();
		}


		/// <summary>
		/// 
		/// </summary>
		void DrawGUI() {
			//if( GUILayout.Button( "resolution" ) ) {
			//	Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSize gw = new Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSize(); ;
			//	//{
			//	//		public GameViewSizeType type;
			//	//		public int width;
			//	//		public int height;
			//	//		public string baseText;
			//	//}
			//	gw.type = Kyusyukeigo.Helper.GameViewSizeHelper.GameViewSizeType.FixedResolution;
			//	gw.width = 1920;
			//	gw.height = 1080;
			//	gw.baseText = "FHD";
			//	Kyusyukeigo.Helper.GameViewSizeHelper.ChangeGameViewSize( GameViewSizeGroupType.Standalone, gw );
			//}
			using( new EditorGUILayout.HorizontalScope( (GUIStyle) "HelpBox" ) ) {
				int[] RealSize = { 1, 2, 3, 4, 5, 6, 7, 8 };
				EditorGUILayout.LabelField( "スクリーンショット", GUILayout.Width( 100 ) );
				string[] SSsizeDisplay = { "x1", "x2", "x3", "x4", "x5", "x6", "x7", "x8" };
				SSSize = EditorGUILayout.Popup( SSSize, SSsizeDisplay, (GUIStyle) "Popup" );
				if( GUILayout.Button( "スクリーンショット撮影" ) ) {
					Multi = RealSize[ SSSize ];
					SaveScreenCapture();
				}
				if( GUILayout.Button( "...", GUILayout.Width( 16 ) ) ) {
					var dname = Directory.GetCurrentDirectory() + "/ScreenShot";
					dname = dname.Replace( "/", "\\" );
					System.Diagnostics.Process.Start( "EXPLORER.EXE", dname );
				}
			}

			GUILayout.Label( "SystemInfo", m_style.BoldLabel );
			GUILayout.Label( m_os_info );
			GUILayout.Label( m_cpu_info );
			GUILayout.Label( m_gpu_info );
			GUILayout.Label( m_resolution_info );
			GUILayout.Label( m_memory );
		}


		/// <summary>
		/// Windowのクライアント領域のGUI処理を記述
		/// </summary>
		void OnGUI() {
			try {
				if( m_style == null ) {
					m_style = new Styles();
				}
				var w = position.width - 10;
				GUILayout.BeginArea( new Rect( 10, 10, w - 10, position.height ) );
				DrawGUI();
				GUILayout.EndArea();
			}
			catch( System.Exception e ) {
				UnityEngine.Debug.LogError( e.ToString() );
			}
		}


	} // class GameSysWindow
} // namespace SwKids

#endif

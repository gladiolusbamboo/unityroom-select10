using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using TMPro;

namespace GJW {

	public class TitleController : MonoBehaviour {

		public TextMeshProUGUI m_loadingText;

		AsyncOperation m_asynOpScene;
		public float m_loadProgress;
		public static Action<float, bool> cbLoadingPercent;
		public bool m_changeScene;

		// Start is called before the first frame update
		void Start() {
			StartCoroutine( Load( Game.Scene.GameMain ) );
		}

		// Update is called once per frame
		void Update() {

		}


		public void OnStart() {
			m_changeScene = true;
		}

		IEnumerator Load( string sceneName ) {
			m_asynOpScene = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );

			if( m_asynOpScene == null ) {
				Debug.LogErrorFormat( "{0}: シーンが見つからない", sceneName );
				goto exit;
			}
			m_asynOpScene.allowSceneActivation = false;
			m_loadProgress = 0.00f;
			Debug.LogFormat( "{0}: LoadSceneAsync", sceneName );
			
			while( m_asynOpScene.progress < 0.9f ) {

				m_loadProgress = m_asynOpScene.progress;

				cbLoadingPercent?.Invoke( m_asynOpScene.progress, false );
				m_loadingText.SetText( "loading: " + m_asynOpScene.progress );
				yield return null;
			}
			Debug.LogFormat( "{0}: complete", sceneName );
			m_loadProgress = 0.95f;


			m_loadingText.SetText( "" );
			while( m_changeScene == false ) {
				yield return null;
			}

		exit:
			if( cbLoadingPercent != null ) {
				cbLoadingPercent( 1.00f, true );
			}

			//ブロック対策でシーン遷移前に１度抜けておく
			yield return null;

			if( m_asynOpScene != null ) {
				m_asynOpScene.allowSceneActivation = true;
			}
			//ResetNextScene();
			m_asynOpScene = null;

			yield break;
		}
	}

}

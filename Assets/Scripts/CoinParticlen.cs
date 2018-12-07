using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hananoki.Extensions;

namespace GJW {

	public class CoinParticlen : MonoBehaviour {

		[NonSerialized]
		public ParticleSystem m_ps;
		ParticleSystem.Particle[] m_pList;

		[System.NonSerialized]
		public GameObject m_target;
		public float m_spdMul;
		public float m_TimeCheck;
		public float m_range;

		[System.NonSerialized]
		public float addPt;
		static int patcleCnt;
		public System.Action<int> hitAction;

		public bool fixedMode;

		public static void Clear() {
			patcleCnt = 0;
		}

		public void RePos( float xrate ) {
			var f = 0.90f * xrate;
			f -= 0.45f;
			transform.SetLocalPosX( f );
		}

		void OnEnable() {
			if( m_ps ==null) {
				m_ps = GetComponent<ParticleSystem>();
			}

			//if( !fixedMode ) {
			//	var info = GameWork.GetCurrentMiniGameInfo();
			//	addPt = (float) GameWork.i.m_ResultAddPawawaPt / (float) info.rankStar;
			//	addPt = addPt / m_ps.main.maxParticles;
			//}
			patcleCnt += m_ps.main.maxParticles;
		}


		// Use this for initialization
		void Start() {
			m_ps = GetComponent<ParticleSystem>();
		}


		// Update is called once per frame
		void LateUpdate() {
			
			if( m_pList == null || m_pList.Length != m_ps.particleCount ) {
				m_pList = new ParticleSystem.Particle[ m_ps.particleCount ];
			}

			m_ps.GetParticles( m_pList );

			var f = GameController.instance.m_params.画面下;

			for( int z = 0; z < m_ps.particleCount; z++ ) {
				Vector3 pos= m_pList[ z ].position;
				if( pos.y < f ) {
					m_pList[ z ].remainingLifetime = 0;
					sound.コインゲット();
					//var obj = Instantiate( GameController.instance.コインゲットパーティクル.gameObject, pos, Quaternion.Euler( -90, 0, 0 ) ).GetComponent<ParticleSystem>();
					var obj = GameController.instance.m_ptclPool.GetPool( pos, Quaternion.Euler( -90, 0, 0 ) );
					obj.SetActive( false );
					obj.SetActive(true);

					GameController.instance.AddScore();
				}
			}

			m_ps.SetParticles( m_pList, m_ps.particleCount );
		}
	}
}

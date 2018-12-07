using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Coffee.UIExtensions;
using DG.Tweening;

namespace GJW {
	public class BGAnim : MonoBehaviour {
		public UIGradient m_uiGradient;
		public TweenParams m_params;
		Color[] col;
		public Color colF1;
		public Color colF2;
		void Awake() {
			col = new Color[] { m_uiGradient.color1, m_uiGradient.color2, m_uiGradient.color3, m_uiGradient.color4 };
		}
		void Start() {

			m_uiGradient.rotation = -180;
			DOTween.To( () => m_uiGradient.rotation, ( f ) => m_uiGradient.rotation = f, 180, m_params.bg.回転時間 ).SetLoops( -1, LoopType.Restart );
		}

		public void Reset() {
			if( m_uiGradient == null ) return;
			DOTween.To( () => m_uiGradient.color1, ( f ) => m_uiGradient.color1 = f, col[0], 1.0f );
			DOTween.To( () => m_uiGradient.color2, ( f ) => m_uiGradient.color2 = f, col[ 1 ], 1.0f );

			//m_uiGradient.color1 = col[ 0 ];
			//m_uiGradient.color2 = col[ 1 ];
			//m_uiGradient.color3 = col[ 2 ];
			//m_uiGradient.color4 = col[ 3 ];
		}
		public void フィーバー() {
			DOTween.To( () => m_uiGradient.color1, ( f ) => m_uiGradient.color1 = f, colF1, 1.0f );
			DOTween.To( () => m_uiGradient.color2, ( f ) => m_uiGradient.color2 = f, colF2, 1.0f );
		}
	}
}

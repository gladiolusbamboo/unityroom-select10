using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

using Hananoki.Extensions;

namespace GJW {
	public class NumButton : MonoBehaviour {

		public Button button;
		public Image image;
		public TextMeshProUGUI text;
		public CanvasGroup canvasGroup;
		public bool atari;

		public Button buttonBlock;
		public Image imageBlock;

		public void ResetHide() {
			SetAlpha( 0.00f );
			atari = false;
			button.enabled = true;
		}

		public void SetText( string t ) {
			text.SetText( t );
		}
		public void SetText( int i ) {
			text.SetText( i.ToString() );
		}

		public void 問題初期化( bool block ) {
			var aa = button.colors;
			aa.normalColor = Color.white;
			button.colors = aa;
			button.enabled = false;
			SetAlpha( 0.00f );
			SetTextAlpha( 0.00f );

			buttonBlock.SetActive( block );
		}

		public void SetTextAlpha( float f ) {
			text.SetAlpha( f );
		}
		public void SetAlpha( float f ) {
			canvasGroup.SetAlpha( f );
		}

		public void ShowButtonGroup() {
			canvasGroup.DOFade( 1.00f, 0.50f );
			canvasGroup.TwScale( GameController.instance.m_params.game.かくだい );
		}

		public void HideButtonGroup() {
			canvasGroup.DOFade( 0.00f, 0.50f );
			canvasGroup.TwScale( 0.00f, GameController.instance.m_params.game.かくだい );
		}

		public void ShowText() {
			text.DOFade( 1.00f, 0.50f );
		}

		public void OnStartButton() {
			GameController.instance.m_startOn = true;
			sound.スタート音();
			canvasGroup.TwScale( GameController.instance.m_params.title.スタートクリック );
		}

		public void OnうにClick() {
			GameController.instance.OnうにClick();
		}

		public void OnどっどゆにてぃClick() {
			GameController.instance.OnどっどゆにてぃClick();
		}

		public void OnBLockClick() {
			sound.ブロック破壊();
			buttonBlock.SetActive(false);
		}

		public void OnClick(  ) {
			if( atari ) {
				sound.ピンポーン();
				sound.voice_やったー();
				GameController.instance.正解選んだ = true;
				GameController.instance.正解のボタン = this;
			}
			else {
				//sound.missButton();
				sound.ブブー();
			}
		}
	}
}

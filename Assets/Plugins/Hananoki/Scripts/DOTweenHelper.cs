//
// DOTweenをPluginフォルダに移動しないと使えない
//
#if ENABLE_DOTWEEN

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

#if ENABLE_TMPRO
using TMPro;
#endif

using Hananoki.ShortCut;

namespace Hananoki {
	//[System.Serializable]
	//public class DOTweenSets {
	//	public Vector4 start;
	//	public Vector4 end;
	//	public Ease ease;
	//	public float time;
	//}

	[System.Serializable]
	public class DOTweenEaseTime {
		public Ease ease;
		public float time;
	}
	[System.Serializable]
	public class DOTweenFloat : DOTweenEaseTime {
		public float start;
		public float end;
		//public Ease ease;
		//public float time;
	}
	[System.Serializable]
	public class DOTweenJump {
		public float jumpPower;
		public int numJumps;
		public float duration;
		public bool snapping;
	}
	[System.Serializable]
	public class DOTweenShake {
		public float duration;
		public float strength = 1f;
		public int vibrato = 10;
		public float randomness = 90f;
		public bool snapping = false;
		public bool fadeOut = true;
	}

}

namespace Hananoki.Extensions {
	public static partial class DOTweenHelper {

		#region DOFade

		/* SpriteRenderer */
		public static Tween TwFade( this SpriteRenderer p, DOTweenFloat tf, System.Action complete = null ) {
			p.SetAlpha( tf.start );
			return p.DOFade( tf.end, tf.time ).SetEase( tf.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwFade( this SpriteRenderer p, float f, DOTweenEaseTime t, System.Action complete = null ) {
			return p.DOFade( f, t.time ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}


		/* Image */
		public static Tween TwFade( this Image p, DOTweenFloat tf, System.Action complete = null ) {
			p.SetAlpha( tf.start );
			return p.DOFade( tf.end, tf.time ).SetEase( tf.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwFade( this Image p, float f, DOTweenEaseTime t, System.Action complete = null ) {
			return p.DOFade( f, t.time ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}


		/* CanvasGroup */
		public static Tween TwFade( this CanvasGroup p, DOTweenFloat tf, Action complete = null ) {
			p.SetAlpha( tf.start );
			return p.DOFade( tf.end, tf.time ).SetEase( tf.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwFade( this CanvasGroup p, float f, DOTweenEaseTime t, Action complete = null ) {
			return p.DOFade( f, t.time ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}


		/* TextMeshProUGUI */
#if ENABLE_TMPRO
		public static Tween TwFade( this TextMeshProUGUI p, DOTweenFloat tf, Action complete = null ) {
			p.SetAlpha( tf.start );
			return p.DOFade( tf.end, tf.time ).SetEase( tf.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwFade( this TextMeshProUGUI p, float f, DOTweenEaseTime t, Action complete = null ) {
			return p.DOFade( f, t.time ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}
#endif

		#endregion


		#region DOMove

		public static Tween TwMove<T>( this T p, Vector3 v, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOMove( v, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwMoveX<T>( this T p, float x, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOMoveX( x, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}

		#endregion


		#region DOLocalMove

		public static Tween TwLocalMove<T>( this T p, Vector3 v, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOLocalMove( v, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}


		public static Tween TwLocalMoveX<T>( this T p, float x, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOLocalMoveX( x, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwLocalMoveX<T>( this T p, DOTweenFloat tw, Action complete = null ) where T : Component {
			p.transform.SetLocalPosX( tw.start );
			return p.transform.DOLocalMoveX( tw.end, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}


		public static Tween TwLocalMoveY<T>( this T p, float y, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOLocalMoveY( y, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwLocalMoveY<T>( this T p, DOTweenFloat tw, Action complete = null ) where T : Component {
			p.transform.SetLocalPosY( tw.start );
			return p.transform.DOLocalMoveY( tw.end, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}

		#endregion


		#region Rotate

		public static Tween TwLocalRotate<T>( this T p, Vector3 v, DOTweenEaseTime t, RotateMode rt = RotateMode.Fast, Action complete = null ) where T : Component {
			return p.transform.DOLocalRotate( v, t.time, rt ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}

		public static Tween TwRotateZ<T>( this T p, DOTweenFloat t, RotateMode rt = RotateMode.Fast, Action complete = null ) where T : Component {
			p.transform.localEulerAngles = v3.v( 0, 0, t.start );
			return p.transform.DOLocalRotate( v3.v( 0, 0, t.end ), t.time, rt ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwRotateZ<T>( this T p, float z, DOTweenEaseTime t, RotateMode rt = RotateMode.Fast, Action complete = null ) where T : Component {
			return p.transform.DOLocalRotate( v3.v( 0, 0, z ), t.time ).SetEase( t.ease ).OnComplete( () => { complete.Call(); } );
		}

		#endregion
		
		
		#region Scale

		public static Tween TwScale<T>( this T p, DOTweenFloat tf, Action complete = null ) where T : Component {
			p.transform.SetLocalScl( tf.start );
			return p.transform.DOScale( tf.end, tf.time ).SetEase( tf.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwScale<T>( this T p, float scale, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOScale( scale, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}
		public static Tween TwScale<T>( this T p, Vector3 scale, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOScale( scale, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}

		public static Tween TwScaleX<T>( this T p, float scale, DOTweenEaseTime tw, Action complete = null ) where T : Component {
			return p.transform.DOScaleX( scale, tw.time ).SetEase( tw.ease ).OnComplete( () => { complete.Call(); } );
		}

		#endregion



		public static Tween TwLocalJump<T>( this T p, DOTweenJump tf, Action complete = null ) where T : Component {
			return p.transform.DOLocalJump( p.transform.localPosition, tf.jumpPower, tf.numJumps, tf.duration, tf.snapping ).OnComplete( () => complete.Call() );
		}
		public static Tween TwLocalJump<T>( this T p, Vector3 pos, DOTweenJump tf, Action complete = null ) where T : Component {
			return p.transform.DOLocalJump( pos, tf.jumpPower, tf.numJumps, tf.duration, tf.snapping ).OnComplete( () => complete.Call() );
		}


		public static Tween TwShakePosition<T>( this T p, DOTweenShake t, Action complete = null ) where T : Component {
			return p.transform.DOShakePosition( t.duration, t.strength, t.vibrato, t.randomness, t.snapping, t.fadeOut ).OnComplete( () => complete.Call() );
		}

		
	}
}


#endif


#pragma warning disable 0618

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

#if ENABLE_TMPRO
using TMPro;
#endif

namespace Hananoki.Extensions {
	public static partial class Extensions {
		
		#region Generic

		public static string[] ToStringArray<T>( this T[] lst ) where T : struct {
			return lst.Select( a => a.ToString() ).ToArray();
		}

		#endregion
		
		
		/// <summary>
		/// パラメーターを受け取らない Action デリゲートを実行します
		/// </summary>
		public static void Call( this Action action ) {
			if( action == null ) return;
			action();
		}

		public static void Call<T>( this Action<T> action, T param ) {
			if( action == null ) return;
			action( param );
		}

		public static void Call<T1, T2>( this Action<T1, T2> action, T1 param01, T2 param02 ) {
			if( action == null ) return;
			action( param01, param02 );
		}
		
		
		
		
		public static void SetActive<T>( this T p, bool value ) where T : Component {
			if( p == null ) return;
			p.gameObject.SetActive( value );
		}
		public static bool GetActive<T>( this T p ) where T : Component {
			if( p == null ) return false;
			return p.gameObject.activeSelf;
		}

		public static void SetActiveArray<T>( this T[] pp, bool value ) where T : Component {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.gameObject.SetActive( value );
			}
		}


		public static void EnableCompomentArray<T>( this T[] pp ) where T : Behaviour {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.enabled = true;
			}
		}
		public static void DisableCompomentArray<T>( this T[] pp ) where T : Behaviour {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.enabled = false;
			}
		}

		#region Transform

		public static Vector3 GetLocalPos<T>( this T i ) where T : Component {
			return i.transform.localPosition;
		}
		public static void SetLocalPos<T>( this T i, Vector3 pos ) where T : Component {
			i.transform.localPosition = pos;
		}
		public static void SetLocalPosX<T>( this T i, float x ) where T : Component {
			i.transform.localPosition = new Vector3( x, i.transform.localPosition.y, i.transform.localPosition.z );
		}
		public static void SetLocalPosY<T>( this T i, float y ) where T : Component {
			i.transform.localPosition = new Vector3( i.transform.localPosition.x, y, i.transform.localPosition.z );
		}
		public static void SetLocalPosZ<T>( this T i, float z ) where T : Component {
			i.transform.localPosition = new Vector3( i.transform.localPosition.x, i.transform.localPosition.y, z );
		}


		public static void SetLocalSclX<T>( this T p, float x ) where T : Component {
			p.transform.localScale = new Vector3( x, p.transform.localScale.y, p.transform.localScale.z );
		}
		public static void SetLocalSclY<T>( this T p, float y ) where T : Component {
			p.transform.localScale = new Vector3( p.transform.localScale.x, y, p.transform.localScale.z );
		}
		public static void SetLocalSclZ<T>( this T p, float z ) where T : Component {
			p.transform.localScale = new Vector3( p.transform.localScale.x, p.transform.localScale.y, z );
		}
		public static void SetLocalScl<T>( this T p, float xyz ) where T : Component {
			p.transform.localScale = new Vector3( xyz, xyz, xyz );
		}

		#endregion


		#region SpriteRenderer

		public static float GetAlpha( this SpriteRenderer p ) {
			return p.color.a;
		}

		public static void SetAlpha( this SpriteRenderer p, float alpha ) {
			p.color = new Color( p.color.r, p.color.g, p.color.b, alpha );
		}
		public static void SetAlphaArray( this SpriteRenderer[] pp, float value ) {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.SetAlpha( value );
			}
		}

		public static void SetSpriteArray( this SpriteRenderer[] pp, Sprite spr ) {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.sprite = spr;
			}
		}

		#endregion


		#region Image

		public static float GetAlpha( this Image p ) {
			return p.color.a;
		}

		public static void SetAlpha( this Image p, float alpha ) {
			p.color = new Color( p.color.r, p.color.g, p.color.b, alpha );
		}
		public static void SetColor( this Image p, Color color ) {
			p.color = new Color( color.r, color.g, color.b, p.color.a );
		}

		public static void SetAlphaArray( this Image[] pp, float value ) {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.SetAlpha( value );
			}
		}
		#endregion


		#region TextMeshProUGUI
#if ENABLE_TMPRO
		public static void SetAlpha( this TextMeshProUGUI img, float alpha ) {
			img.color = new Color( img.color.r, img.color.g, img.color.b, alpha );
		}
#endif
		#endregion


		#region CanvasGroup

		public static void Default( this CanvasGroup p ) {
			p.SetActive( true );
			p.SetAlpha( 0.0f );
		}

		public static void SetAlpha( this CanvasGroup p, float alpha ) {
			if( p == null ) return;
			p.alpha = alpha;
		}

		public static void SetAlphaArray( this CanvasGroup[] pp, float value ) {
			if( pp == null ) return;
			foreach( var p in pp ) {
				if( p == null ) continue;
				p.SetAlpha( value );
			}
		}

		#endregion
	}
}

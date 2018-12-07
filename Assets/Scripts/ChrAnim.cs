using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GJW {
	public class ChrAnim : MonoBehaviour {
		public enum State {
			Normal,
			Dmg,
			Jump,
		}
		public Image m_img;
		public Sprite[] m_spr;

		public void Set( State st) {
		}
	}
}

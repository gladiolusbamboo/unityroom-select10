
using UnityEngine;

namespace Hananoki {

	/// <summary>
	/// SceneManager.LoadScene などに渡すシーン名を保持するstringです
	/// エディタ時のみGUIDを保持して参照維持を行います
	/// SceneStringDrawerによる連携があります
	/// </summary>
	[System.Serializable]
	public class SceneString {
#if UNITY_EDITOR
		public string guid;
#endif
		public string name;

		// 普通のstringのように振る舞いたい
		public static implicit operator string( SceneString s ) {
			return s.name;
		}
	}

}

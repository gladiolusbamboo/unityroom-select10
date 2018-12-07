using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Hananoki {
	/// <summary>
	/// 任意の文字列からアニメーションハッシュ値を求め、配列に保存するスクリプタブルオブジェト
	/// スクリプト上から扱うための定義ファイルを出力して利用することを想定します
	/// </summary>
	[CreateAssetMenu( menuName = "Hananoki/AnimatorStringHashTable" )]
	public class StringHashTable : ScriptableObject {

		public enum Mode {
			AnimatorStringToHash,
			ShaderPropertyToID,
		}

		[System.Serializable]
		public class Data {
			public string key;
			public int value;
			public Data() : this( "", 0 ) {
			}
			public Data( string key, int value ) {
				this.key = key;
				this.value = value;
			}
		}

		public Mode m_mode;
		public List<Data> m_dataList;
	}
}


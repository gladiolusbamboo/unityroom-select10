using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hananoki;

namespace GJW {
	[CreateAssetMenu( menuName = "LevelSetting" )]
	[Serializable]
	public class LevelSetting : ScriptableObject {
		[Serializable]
		public class Data {
			public int num;
		}
		public int 割る数;
		public float 制限時間;
		public int ブロックレベル;
		public Data[] m_data;
	}
}
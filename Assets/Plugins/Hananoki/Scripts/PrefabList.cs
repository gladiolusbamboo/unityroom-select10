
using UnityEngine;
using System;

namespace Hananoki {

	[System.Serializable]
	public struct PrefabListData {
		public string label;
		public GameObject prefab;
		public PrefabListData( string label, GameObject prefab ) {
			this.label = label;
			this.prefab = prefab;
			//}
		}
	} // struct PrefabListData


	/// <summary>
	/// プレハブの参照を配列にして保存してあるスクリプタブルオブジェクト
	/// </summary>
	[CreateAssetMenu( menuName = "Hananoki/PrefabList" )]
	[Serializable]
	public class PrefabList : ScriptableObject {

		public PrefabListData[] m_Data;

	} // class PrefabList

}

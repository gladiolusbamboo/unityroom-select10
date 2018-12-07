
using UnityEngine;
using System;

namespace Hananoki {
	/// <summary>
	/// stringに設定するアトリビュート
	/// enumの選択リストから選択したものを文字列として扱う
	/// stringなのでenum名に変更があった場合、変更前の文字列がそのまま残る
	/// </summary>
	public class EnumStringAttribute : PropertyAttribute {
		public Type m_type;
		public EnumStringAttribute( Type t ) {
			m_type = t;
		}
	}
}

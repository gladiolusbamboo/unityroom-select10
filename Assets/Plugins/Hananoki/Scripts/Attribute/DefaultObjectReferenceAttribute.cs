
using System;
using UnityEngine;

namespace Hananoki {
	public class DefaultObjectReferenceAttribute : PropertyAttribute {
		public Type type;
		public string guid;
		public DefaultObjectReferenceAttribute( Type type, string guid ) {
			this.type = type;
			this.guid = guid;
		}
	}
}


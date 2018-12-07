using UnityEngine;
using System.Collections;

namespace Hananoki.ShortCut {

	public static class v3 {
		public static Vector3 v000 = Vector3.zero;
		public static Vector3 v111 = Vector3.one;
		public static Vector3 v010 = v( 0, 1, 0 );
		public static Vector3 v( float x, float y, float z ) { return new Vector3( x, y, z ); }
		public static Vector3 fill( float f ) { return new Vector3( f, f, f ); }
	}

	public static class qt {
		public static Quaternion Identity = Quaternion.identity;
		public static Quaternion X( float f ) { return Quaternion.Euler( f, 0, 0 ); }
		public static Quaternion Y( float f ) { return Quaternion.Euler( 0, f, 0 ); }
		//public static Quaternion Euler( Vector3 euler ) { return Quaternion.Euler( euler ); }
	}

	public static class rand {
		public static int Range( int min, int max ) { return UnityEngine.Random.Range( min, max ); }
		public static float Range( float min, float max ) { return UnityEngine.Random.Range( min, max ); }
	}
}

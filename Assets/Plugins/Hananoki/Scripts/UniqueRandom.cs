using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityRandom = UnityEngine.Random;

namespace Hananoki {
	[System.Serializable]
	public class UniqueRandom {

		public List<int> m_numbers;

		int max;
		int[] m_tbl;

		public UniqueRandom( int max ) {
			Create( max );
		}

		public UniqueRandom( int[] tbl ) {
			Create( tbl );
		}

		public void Create( int max ) {
			m_numbers = new List<int>();
			this.max = max;

			for( int i = 0; i < max; i++ ) {
				m_numbers.Add( i );
			}
		}
		public void Create( int[] tbl ) {
			m_tbl = tbl;
			m_numbers = new List<int>();

			for( int i = 0; i < tbl.Length; i++ ) {
				m_numbers.Add( m_tbl[ i ] );
			}
		}

		public void ReInit() {
			try {
				if( m_tbl == null ) {
					for( int i = 0; i < max; i++ ) {
						m_numbers.Add( i );
					}
				}
				else {
					for( int i = 0; i < m_tbl.Length; i++ ) {
						m_numbers.Add( m_tbl[ i ] );
					}
				}
			}
			catch( System.Exception e ) {
				Debug.Log( e );
			}
		}

		public int GetFront() {
			if( m_numbers.Count == 0 ) {
#if DEBELOP_MODE
			Debug.LogWarning( "UniqueRandom: 使いすぎ" );
#endif
				//return 0;
				ReInit();
			}
			int ransu = m_numbers[ 0 ];
			m_numbers.RemoveAt( 0 );

			return ransu;
		}

		public int Get() {
			if( m_numbers.Count == 0 ) {
#if DEBELOP_MODE
			Debug.LogWarning( "UniqueRandom: 使いすぎ" );
#endif
				//return 0;
				ReInit();
			}

			int index = UnityRandom.Range( 0, m_numbers.Count );

			int ransu = m_numbers[ index ];
			//Debug.Log( ransu );

			//m_cache[ count ].SetFlower( (MiniGame.FlowerID) ransu );

			m_numbers.RemoveAt( index );

			return ransu;
		}
	}
}

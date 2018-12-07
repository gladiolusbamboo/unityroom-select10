using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PtclPool : MonoBehaviour {

	public ParticleSystem prefab;

	public List<ParticleSystem> m_lst;

	// Use this for initialization
	void Start () {
		m_lst = new List<ParticleSystem>();
	}

	// Update is called once per frame
	public ParticleSystem GetPool ( Vector3 pos, Quaternion rot )  {
		for( int i=0;i< m_lst.Count;i++ ) {
			if( !m_lst[ i ].IsAlive() ) {
				m_lst[ i ].transform.SetPositionAndRotation( pos, rot );
				return m_lst[ i ];
			}
		}
		var obj = Instantiate( prefab.gameObject, pos, rot ).GetComponent<ParticleSystem>();
		m_lst.Add( obj );
		return null;
	}
}

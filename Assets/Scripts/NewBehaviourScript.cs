using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NCMB;
using Hananoki;

public class NewBehaviourScript : MonoBehaviour {
	
	// Start is called before the first frame update
	void Start() {
		// クラスのNCMBObjectを作成
		//NCMBObject testClass = new NCMBObject( "TestClass" );

		//// オブジェクトに値を設定
		//testClass[ "message" ] = "Hello, NCMB!";
		
		//// データストアへの登録
		//testClass.SaveAsync();
	}

	// Update is called once per frame
	void Update() {

	}


	[InspectorGUI]
	void InspectorGUI() {
		if( GUILayout.Button("ranking") ) {
			// Type == Time の場合
			//var millsec = 5000;
			//var timeScore = new System.TimeSpan( 0, 0, 0, 0, millsec );
			//naichilab.RankingLoader.Instance.SendScoreAndShowRanking( timeScore );

			// Type == Number の場合
			naichilab.RankingLoader.Instance.SendScoreAndShowRanking( 100 );
		}
	}
}

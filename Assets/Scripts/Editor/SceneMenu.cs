/* エディタースクリプト側でコンパイルしてください */
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using HananokiEditor;

public static partial class SceneMenu {
	[MenuItem( "ゲームシーン/00: GameMain", false, 1 )]
	public static void SceneMenu_b00() {
		EditorUtils.OpenScene( "Assets/Scenes/GameMain.unity" );
	}

	[MenuItem( "ゲームシーン/01: Ranking", false, 1 )]
	public static void SceneMenu_b01() {
		EditorUtils.OpenScene( "Assets/naichilab/unity-simple-ranking/Scenes/Ranking.unity" );
	}

}
#endif

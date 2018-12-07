using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Hananoki;

using UnityObject = UnityEngine.Object;

namespace HananokiEditor {

	[CustomPropertyDrawer( typeof( PrefabListData ) )]
	class PrefabListDataDrawer : PropertyDrawer {

		public static float propertyHeight { get { return (EditorGUIUtility.singleLineHeight+3) * 1; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="property"></param>
		/// <param name="label"></param>
		public override void OnGUI( Rect rc, SerializedProperty property, GUIContent label ) {

			rc.height = EditorGUIUtility.singleLineHeight;
			var rc1 = rc;
			
			rc.width = 16;
			if( GUI.Button( rc, new GUIContent( "-" ) ) ) {
				GameObject.Instantiate( property.FindPropertyRelative( "prefab" ).objectReferenceValue );
			}

			rc1.width -= 20;
			rc1.x += 20;

			var rc2 = rc1;
			rc1.width *= 0.3f;
			rc2.x += rc1.width + 2;
			rc2.width -= rc1.width + 2;

			var labelProp  = property.FindPropertyRelative( "label" );
			var prefabProp = property.FindPropertyRelative( "prefab" );
			EditorGUI.PropertyField( rc1, labelProp, GUIContent.none );
			if( string.IsNullOrEmpty( labelProp.stringValue ) ) {
				if( prefabProp.objectReferenceValue != null ) {
					labelProp.stringValue = prefabProp.objectReferenceValue.name;
				}
			}
			EditorGUI.PropertyField( rc2, prefabProp, GUIContent.none );

		} // void OnGUI


		/// <summary>
		/// ピクセル単位で GUI の高さを設定します そのためにはこのメソッドをオーバーライドしてください
		/// デフォルトは 1 行の高さです
		/// </summary>
		/// <param name="property">対象となるシリアライズ化されたプロパティー</param>
		/// <param name="label">このプロパティーのラベル</param>
		/// <returns></returns>
		public override float GetPropertyHeight( SerializedProperty property, GUIContent label ) {
			return propertyHeight;
		} // GetPropertyHeight

	} // class PrefabListDrawer
}


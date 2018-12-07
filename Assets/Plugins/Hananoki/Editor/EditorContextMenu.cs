
//#defien ENABLE_UGUI

using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

//using HC.UI;

using UnityObject = UnityEngine.Object;

namespace HananokiEditor {
	[InitializeOnLoad]
	public static class EditorContextMenu {

		public class Command {
			public GUIContent content;
			public Action<UnityObject> func;
			public Func<UnityObject, bool> condition;

			public Command( GUIContent content, Action<UnityEngine.Object> func, Func<UnityEngine.Object, bool> condition ) {
				this.content = content;
				this.func = func;
				this.condition = condition;
			}
			public Command( string name, Action<UnityEngine.Object> func, Func<UnityEngine.Object, bool> condition ) :
				this( new GUIContent( name ), func, condition ) {
			}
		}
		public enum CommandType {
			SHOW_GUID,
			ASSET2BAK,
			ASSET_FILE_DUP,
			EDITOR_ONLY_CHILDRENS,
			SELECT_MAKE_IMAGE,
			SELECT_MAKE_SPRITE,
			SELECT_ATTACH_SPRITE,
			MOVE_PARENT,

			ADD_COMPOMENT,
			ADD_NETWORK_COMPOMENT,
			PARAM_COMPOMENT,
			ADD_FACE,
			CHR_PREFAB_GEN,
			ADD_CLIP,
			SPRING_BONE,

			ASSET_PATH_COPY,
			ENABLE_KEYWORD,
		}

		static Dictionary<CommandType, Command> m_commandList;



		#region CommandType.ASSET_DUP_COPY

		static bool ASSET_FILE_DUP_condition( UnityEngine.Object obj ) {
			return true;
		}
		static void ASSET_FILE_DUP_func( UnityEngine.Object obj ) {
			//var path = AssetDatabase.GetAssetPath( obj );

			//foreach( var n in Selection.assetGUIDs ) {
			var path = AssetDatabase.GUIDToAssetPath( Selection.assetGUIDs[ 0 ] );
			var dir = System.IO.Path.GetDirectoryName( path );
			var fname = System.IO.Path.GetFileName( path );
			var newPath = dir + "/" + "_" + fname;
			var uniquePath = AssetDatabase.GenerateUniqueAssetPath( newPath );
			AssetDatabase.CopyAsset( path, uniquePath );
			AssetDatabase.Refresh();
			var asset = AssetDatabase.LoadAssetAtPath( uniquePath, typeof( UnityEngine.Object ) );
			//list.Add( asset );
			//}
			Selection.activeObject = asset;
		}
		#endregion




		/// <summary>
		/// コンストラクタ
		/// </summary>
		static EditorContextMenu() {

			m_commandList = new Dictionary<CommandType, Command>();

			m_commandList.Add(
				CommandType.ASSET_FILE_DUP,
				new Command(
					new GUIContent( "アセット複製" ), ASSET_FILE_DUP_func, ASSET_FILE_DUP_condition ) );

			#region CommandType.SHOW_GUID
			m_commandList.Add(
				CommandType.SHOW_GUID,
				new Command(
					new GUIContent( "Info" ),
					( uobj ) => {
						Debug.Log( uobj.name );
						var po = PrefabUtility.GetPrefabObject( uobj );
						if( po == null ) {
							Debug.Log( "PrefabUtility.GetPrefabObject: null" );
						}
						else {
							Debug.Log( "PrefabUtility.GetPrefabObject: " + po.name );
						}

						var pp = PrefabUtility.GetCorrespondingObjectFromSource( uobj );
						if( pp == null ) {
							Debug.Log( "PrefabUtility.GetPrefabParent: null" );
						}
						else {
							Debug.Log( "PrefabUtility.GetPrefabParent: " + pp.name );
						}
					},
					( uobj ) => {
						return true;
					} ) );
			#endregion

			#region CommandType.ASSET2BAK
			m_commandList.Add( CommandType.ASSET2BAK,
				new Command(
					new GUIContent( "bakにする" ),
					( uobj ) => {
						var path = AssetDatabase.GetAssetPath( uobj );
						File.Move( path, path + ".bak" );
						File.Move( path + ".meta", path + ".bak.meta" );
						AssetDatabase.Refresh();
					},
					( uobj ) => {
						return true;
					} ) );
			#endregion

			#region CommandType.EDITOR_ONLY_CHILDRENS
			m_commandList.Add( CommandType.EDITOR_ONLY_CHILDRENS,
				new Command(
					new GUIContent( "EditorOnly Childrens" ),
					( uobj ) => {
						var gobj = uobj as GameObject;
						var array = gobj.GetComponentsInChildren<Transform>( true )
						.Where( c => c != gobj.transform )
						.Select( c => c.gameObject )
						.ToArray();

						foreach( var a in array ) {
							a.tag = "EditorOnly";
						}
					},
					( uobj ) => {
						return true;
					} ) );
			#endregion

			#region CommandType.SELECT_MAKE_IMAGE
#if ENABLE_UGUI
			m_commandList.Add( CommandType.SELECT_MAKE_IMAGE,
				new Command(
					new GUIContent( "Make Image" ),
					( uobj ) => {
						Debug.Log(uobj.name);
						Sprite spr = uobj as Sprite;
						Texture2D tex = uobj as Texture2D;

						foreach( var item in AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( uobj ) ) ) {
							//フラグをすべて None にして非表示設定を解除
							//item.hideFlags = HideFlags.None;
							if( item.GetType() == typeof( Sprite ) ) {
								var img = UICreator.CreateImage();
								img.sprite = item as Sprite;
								img.name = item.name;
								img.SetNativeSize();
							}
						}
					},
					( uobj ) => {
						if( uobj.GetType() == typeof( Texture2D ) ) return true;
						return false;
					} ) );
#endif
			#endregion

			#region CommandType.SELECT_MAKE_IMAGE
#if ENABLE_UGUI
			m_commandList.Add( CommandType.SELECT_MAKE_SPRITE,
				new Command(
					new GUIContent( "Make Sprite" ),
					( uobj ) => {
						Debug.Log( uobj.name );
						Sprite spr = uobj as Sprite;
						Texture2D tex = uobj as Texture2D;

						foreach( var item in AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( uobj ) ) ) {
							//フラグをすべて None にして非表示設定を解除
							//item.hideFlags = HideFlags.None;
							if( item.GetType() == typeof( Sprite ) ) {
								//var img = UICreator.CreateImage();
								//img.sprite = item as Sprite;
								//img.name = item.name;

								EditorApplication.ExecuteMenuItem( "GameObject/2D Object/Sprite" );
								var b = Selection.activeGameObject.GetComponent<SpriteRenderer>();
								b.sprite = item as Sprite;
								b.name = item.name;
							}
						}
					},
					( uobj ) => {
						if( uobj.GetType() == typeof( Texture2D ) ) return true;
						return false;
					} ) );
#endif
			#endregion

			#region CommandType.SELECT_ATTACH_SPRITE
#if ENABLE_UGUI
			m_commandList.Add( CommandType.SELECT_ATTACH_SPRITE,
				new Command(
					new GUIContent( "選択中のコンポーネントに設定" ),
					( uobj ) => {
						//Debug.Log( uobj.name );
						Sprite spr = uobj as Sprite;
						//Texture2D tex = uobj as Texture2D;
						var img = Selection.activeGameObject.GetComponent<Image>();
						if( img != null ) {
							img.sprite = spr;
						}
						//foreach( var item in AssetDatabase.LoadAllAssetsAtPath( AssetDatabase.GetAssetPath( uobj ) ) ) {
						//	//フラグをすべて None にして非表示設定を解除
						//	//item.hideFlags = HideFlags.None;
						//	if( item.GetType() == typeof( Sprite ) ) {
						//		//var img = UICreator.CreateImage();
						//		//img.sprite = item as Sprite;
						//		//img.name = item.name;

						//		EditorApplication.ExecuteMenuItem( "GameObject/2D Object/Sprite" );
						//		var b = Selection.activeGameObject.GetComponent<SpriteRenderer>();
						//		b.sprite = item as Sprite;
						//		b.name = item.name;
						//	}
						//}
					},
					( uobj ) => {
						if( uobj.GetType() == typeof( Sprite ) ) return true;
						return false;
					} ) );
#endif
			#endregion

			#region CommandType.MOVE_PARENT
			m_commandList.Add( CommandType.MOVE_PARENT,
				new Command(
					new GUIContent( "上の階層に移動する" ),
					( uobj ) => {
						var gobj = uobj as GameObject;
						gobj.transform.parent = gobj.transform.parent.parent;
					},
					( uobj ) => {
						return true;
					} ) );
			#endregion



			// Hirarchy ウィンドウの OnGUI イベントで呼び出されるコールバックを登録します
			EditorApplication.hierarchyWindowItemOnGUI += ( instanceID, selectionRect ) => {
				ShowContextMenu( 0 );
			};

			// 	OnGUI イベントごとに ProjectWindow に表示されているリスト項目ごとに呼び出されるデリゲートです
			EditorApplication.projectWindowItemOnGUI += ( guid, selectionRect ) => {
				ShowContextMenu( 1 );
			};


			/// 汎用で書けるInitializeOnLoad時の初期化がないので
			/// コンテキストとは関係ないがここに処理を挟む
			Selection.selectionChanged += () => {
				if( Selection.activeGameObject == null ) return;
				try {
					Selection.activeGameObject.SendMessage( "OnSelectionChanged", SendMessageOptions.DontRequireReceiver );
				}
				catch( Exception ) {
				}
			};
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="type">(0: hierarchyWindowItemOnGUI) (1:projectWindowItemOnGUI)</param>
		public static void ShowContextMenu( int type ) {
			var ev = Event.current;

			if( ev.type == EventType.MouseDown && ev.button == 2 ) {
				if( type == 1 ) {
					// プロジェクトブラウザ
					showMenu2( ev, new Command[] {
						#if ENABLE_UGUI
							m_commandList[ CommandType.SELECT_MAKE_IMAGE ],
							m_commandList[ CommandType.SELECT_MAKE_SPRITE ],
							m_commandList[ CommandType.SELECT_ATTACH_SPRITE ],
#endif
							//m_commandList[ CommandType.ADD_NETWORK_COMPOMENT ],
							m_commandList[ CommandType.SHOW_GUID ],
							m_commandList[ CommandType.ASSET2BAK ],
							//m_commandList[ CommandType.ASSET_PATH_COPY ],
							m_commandList[ CommandType.ASSET_FILE_DUP ],
							//m_commandList[ CommandType.ENABLE_KEYWORD ],
							
						} );
				}
				else {
					// ヒエラルキー
					showMenu2( ev, new Command[] {
							m_commandList[ CommandType.SHOW_GUID ],
							m_commandList[ CommandType.MOVE_PARENT ],

							m_commandList[ CommandType.EDITOR_ONLY_CHILDRENS ],
				//			m_commandList[ CommandType.PARAM_COMPOMENT ],
				//			m_commandList[ CommandType.CHR_PREFAB_GEN ],
				//			m_commandList[ CommandType.ADD_CLIP ],
				//			m_commandList[ CommandType.ADD_FACE ],
				//			m_commandList[ CommandType.SPRING_BONE ],
				//			m_commandList[ CommandType.ENABLE_KEYWORD ],
						} );
				}
			}
		}


		static void showMenu2( Event ev, Command[] cmd ) {
			Action<GUIContent[], Action<UnityEngine.Object>[]> showMenu = ( content, action ) => {
				var mousePosition = ev.mousePosition;
				var width = 100;
				var height = 100;
				var position = new Rect( mousePosition.x, mousePosition.y - height, width, height );
				EditorUtility.DisplayCustomMenu(
					position,
					content,
					-1,
					( userData, options, selected ) => {
						var objects = userData as UnityEngine.Object[];

						foreach( var obj in objects ) {
							action[ selected ]( obj );
						}

						Selection.objects = objects;
					},
					Selection.objects );
			};

			var a = new List<GUIContent>();
			var b = new List<Action<UnityEngine.Object>>();

			foreach( var c in cmd ) {
				if( !c.condition( Selection.activeObject ) ) continue;
				a.Add( c.content );
				b.Add( c.func );
			}
			showMenu( a.ToArray(), b.ToArray() );
		}
	}
}

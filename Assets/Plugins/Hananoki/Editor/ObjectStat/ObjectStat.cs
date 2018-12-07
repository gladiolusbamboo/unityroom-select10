
#define UNITY_5_3_AND_UP

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#undef UNITY_5_3_AND_UP
#endif

using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

//#if UNITY_5_5_OR_NEWER
	using UnityEngine.Profiling;
//#endif

namespace HananokiEditor {

	public static class ObjectStat {

		[System.Serializable]
		class ObjectStatInfo : System.IEquatable<ObjectStatInfo> {
			static readonly string[] _memory_suffixes = { " B", " KB", " MB", " GB", " TB" };
			public string name;
			public ulong mesh_tris;
			public HashSet<int> mesh_total;
			public ulong mesh_instances;
			public double mesh_memory;
			public StringBuilder mesh_instances_log;
			public HashSet<int> mat_total;
			public ulong mat_instances;
			public StringBuilder mat_instances_log;
			public double mat_memory;
			public HashSet<int> texture_total;
			public double texture_memory;
			public ulong shader_passes;
			public ulong renderers_total;
			public ulong renderers_visible;
			public ulong renderers_batched;
			public HashSet<int> anim_total;
			public double anim_memory;
			public HashSet<int> audio_total;
			public double audio_memory;
			public ulong sprite_packed;

			public ObjectStatInfo( string name ) {
				this.name = name;
				mesh_tris = 0u;
				mesh_total = new HashSet<int>();
				mesh_instances_log = new StringBuilder();
				mesh_instances = 0u;
				mesh_memory = 0f;
				mat_total = new HashSet<int>();
				mat_instances = 0u;
				mat_instances_log = new StringBuilder();
				mat_memory = 0f;
				texture_total = new HashSet<int>();
				texture_memory = 0f;
				shader_passes = 0u;
				renderers_total = 0u;
				renderers_visible = 0u;
				renderers_batched = 0u;
				anim_total = new HashSet<int>();
				anim_memory = 0f;
				audio_total = new HashSet<int>();
				audio_memory = 0f;
				sprite_packed = 0u;
			}

			public override bool Equals( object obj ) {
				return obj is ObjectStatInfo && this.Equals( (ObjectStatInfo) obj );
			}

			public bool Equals( ObjectStatInfo obj ) {
				return name == obj.name
				&& mesh_tris == obj.mesh_tris
				&& mesh_total.Equals( obj.mesh_total )
				&& mesh_instances == obj.mesh_instances
				&& mesh_memory == obj.mesh_memory
				&& mat_total.Equals( obj.mat_total )
				&& mat_instances == obj.mat_instances
				&& mat_memory == obj.mat_memory
				&& texture_total.Equals( obj.texture_total )
				&& texture_memory == obj.texture_memory
				&& shader_passes == obj.shader_passes
				&& renderers_total == obj.renderers_total
				&& renderers_visible == obj.renderers_visible
				&& renderers_batched == obj.renderers_batched
				&& anim_total.Equals( obj.anim_total )
				&& anim_memory == obj.anim_memory
				&& audio_total.Equals( obj.audio_total )
				&& audio_memory == obj.audio_memory
				&& sprite_packed == obj.sprite_packed;
			}

			public override int GetHashCode() {
				return name.GetHashCode()
				^ mesh_tris.GetHashCode()
				^ mesh_total.GetHashCode()
				^ mesh_instances.GetHashCode()
				^ mesh_memory.GetHashCode()
				^ mat_total.GetHashCode()
				^ mat_instances.GetHashCode()
				^ mat_memory.GetHashCode()
				^ texture_total.GetHashCode()
				^ texture_memory.GetHashCode()
				^ shader_passes.GetHashCode()
				^ renderers_total.GetHashCode()
				^ renderers_visible.GetHashCode()
				^ renderers_batched.GetHashCode()
				^ anim_total.GetHashCode()
				^ anim_memory.GetHashCode()
				^ audio_total.GetHashCode()
				^ audio_memory.GetHashCode()
				^ sprite_packed.GetHashCode();
			}

			static string GetAppropriateSize( double mem ) {
				int s = 0;
				while( mem > 1024f && ++s < _memory_suffixes.Length ) {
					mem /= 1024f;
				}
				return mem.ToString( "F" ) + _memory_suffixes[ s ];
			}

			public string TotalMem {
				get { return GetAppropriateSize( mesh_memory + mat_memory + texture_memory + anim_memory + audio_memory ); }
			}

			public string MeshMem {
				get { return GetAppropriateSize( mesh_memory ); }
			}

			public string MatMem {
				get { return GetAppropriateSize( mat_memory ); }
			}

			public string TextureMem {
				get { return GetAppropriateSize( texture_memory ); }
			}

			public string AnimMem {
				get { return GetAppropriateSize( anim_memory ); }
			}

			public string AudioMem {
				get { return GetAppropriateSize( audio_memory ); }
			}

			public override string ToString() {
				StringBuilder ret = new StringBuilder( name + "\n" );
				ret.EnsureCapacity( 512 );

				ret.AppendLine( new string( '-', name.Length ) );
				ret.AppendLine( "" );
				//			ret.AppendFormat("{0, 0}{1, 40}\n", "test", TotalMem);
				ret.AppendLine( "Total Memory : " + TotalMem );
				ret.AppendLine( "Mesh Memory : " + MeshMem );
				ret.AppendLine( "Material Memory : " + MatMem );
				ret.AppendLine( "Texture Memory : " + TextureMem );
				ret.AppendLine( "AnimationClips Memory : " + AnimMem );
				ret.AppendLine( "AudioClip Memory : " + AudioMem );
				ret.AppendLine( "Renderers : " + renderers_total );
				ret.AppendLine( "Visible Renderers : " + renderers_visible );
				ret.AppendLine( "Static Batches : " + renderers_batched );
				ret.AppendLine( "Packed Sprites : " + sprite_packed );
				ret.AppendLine( "Triangles : " + mesh_tris );
				ret.AppendLine( "Meshes : " + mesh_total.Count );
				ret.AppendLine( "Mesh Instances : " + mesh_instances );
				ret.AppendLine( "Materials : " + mat_total.Count );
				ret.AppendLine( "Material Instances : " + mat_instances );
				ret.AppendLine( "Maximum Shader Passes : " + shader_passes );
				ret.AppendLine( "Textures : " + texture_total.Count );
				ret.AppendLine( "AnimationClips : " + anim_total.Count );
				ret.AppendLine( "AudioClips : " + audio_total.Count );
				if( mesh_instances > 0 ) {
					ret.AppendLine( "" );
					ret.AppendLine( "Mesh Instances : " );
					ret.Append( mesh_instances_log.ToString() );
				}
				if( mat_instances > 0 ) {
					ret.AppendLine( "" );
					ret.AppendLine( "Material Instances : " );
					ret.AppendLine( mat_instances_log.ToString() );
				}
				ret.AppendLine( "" );
				return ret.ToString();
			}
		}

		static ObjectStatInfo _selected_objects;

		const uint _billboard_tris = 2u;

		class Style {
			public GUIStyle window;
			public GUIStyle boldLabel;
			public GUIStyle miniLabel;
			public GUIStyle label;
			public Style() {
				var txtcol = new Color( 0.75f, 0.75f, 0.75f );
				window = new GUIStyle( GUI.skin.window );
				window.onNormal = window.normal;
				window.normal.textColor = txtcol;

				boldLabel = new GUIStyle( EditorStyles.boldLabel );
				boldLabel.normal.textColor = txtcol;

				miniLabel = new GUIStyle( EditorStyles.label );
				miniLabel.normal.textColor = txtcol;

				label = new GUIStyle( EditorStyles.label );
				label.normal.textColor = txtcol;
			}
		}//(GUIStyle)"ShurikenModuleTitle"

		static Style s_style;

		internal static void Enable() {
			//Unity 5.2 
			Selection.selectionChanged += ObjectStat.OnSelectionChange;
			SceneView.onSceneGUIDelegate += ObjectStat.OnSceneGUI;
		}
		internal static void Disable() {
			Selection.selectionChanged -= ObjectStat.OnSelectionChange;
			SceneView.onSceneGUIDelegate -= ObjectStat.OnSceneGUI;
		}

		internal static void OnSceneGUI( SceneView sceneView ) {
			GUI.skin = EditorGUIUtility.GetBuiltinSkin( EditorSkin.Scene );
			if( s_style == null ) {
				s_style = new Style();
			}
			Handles.BeginGUI();
			
			Color bak = GUI.backgroundColor;
			Color c = Color.black;
			c.a = 0.95f;
			GUI.backgroundColor=c;
			if( Selection.objects.Length == 0 || _selected_objects == null ) {
				GUILayout.BeginArea( new Rect( 10, 10, 320, 40 ), /*"ObjectStat", */s_style.window );
				EditorGUILayout.LabelField( "None", s_style.boldLabel );
				GUILayout.EndArea();
				return;
			}

			GUILayout.BeginArea( new Rect( 10, 10, 320, 220 ), /*"ObjectStat",*/ s_style.window );
			GUI.backgroundColor = bak;

			if( _selected_objects != null ) {
				var x = _selected_objects;
				EditorGUILayout.LabelField( x.name + " - " + x.TotalMem, s_style.boldLabel );

				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				EditorGUIUtility.labelWidth = 90;
				const int total_width = 130;
				EditorGUILayout.LabelField( "Meshes : " + x.MeshMem, s_style.miniLabel, GUILayout.Width( total_width ) );
				EditorGUILayout.LabelField( "Materials : " + x.MatMem, s_style.miniLabel );
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Textures : " + x.TextureMem, s_style.miniLabel, GUILayout.Width( total_width ) );
				EditorGUILayout.LabelField( "AnimationClips : " + x.AnimMem, s_style.miniLabel );
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "AudioClips : " + x.AudioMem, s_style.miniLabel, GUILayout.Width( total_width ) );
				EditorGUILayout.EndHorizontal();

				//				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Renderers : " + x.renderers_total.ToString( "N0" ), s_style.miniLabel, GUILayout.Width( total_width ) );
				EditorGUILayout.LabelField( "Visible : " + x.renderers_visible.ToString( "N0" ), s_style.miniLabel );
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Static Batches : " + x.renderers_batched.ToString( "N0" ), s_style.miniLabel, GUILayout.Width( total_width ) );
				EditorGUILayout.LabelField( "Packed Sprites : " + x.sprite_packed.ToString( "N0" ), s_style.miniLabel );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				EditorGUIUtility.labelWidth = 125;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Tris :", s_style.label, GUILayout.Width( 125 ) );
				EditorGUILayout.LabelField( x.mesh_tris.ToString( "N0" ), s_style.label );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Meshes :", s_style.label, GUILayout.Width( 125 ) );
				EditorGUILayout.LabelField( x.mesh_total.Count.ToString( "N0" ) + " (" + x.mesh_instances.ToString( "N0" ) + " instances)", s_style.label );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Materials :", s_style.label, GUILayout.Width( 125 ) );
				EditorGUILayout.LabelField( x.mat_total.Count.ToString( "N0" ) + " (" + x.mat_instances.ToString( "N0" ) + " instances)", s_style.label );
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Shader Passes :", s_style.label, GUILayout.Width( 125 ) );
				EditorGUILayout.LabelField( x.shader_passes.ToString( "N0" ) + " (max)", s_style.label );
				EditorGUILayout.EndHorizontal();
				//				EditorGUILayout.LabelField("AnimationClips :", x.anim_total.Count.ToString("N0"));
				//				EditorGUILayout.LabelField("AudioClips :", x.audio_total.Count.ToString("N0"));
				EditorGUI.indentLevel--;
			}
			GUILayout.EndArea();

			Handles.EndGUI();
		}


		internal static void OnSelectionChange() {
			if( Selection.objects.Length == 0 ) return;
			//Debug.Log( "SelectionChange: " + Selection.activeObject.name );

			//_selected_objects = GetStats( Selection.gameObjects );
			_selected_objects = GetStats( Selection.activeGameObject );

			//Debug.Log( _selected_objects.ToString() );
		}


		static void GetSharedMeshStats( Mesh sm, ref ObjectStatInfo ret, bool is_static_batched, GameObject parent ) {
			if( sm == null )
				return;

			/* First time we encounter or is an instance. */
			if( !ret.mesh_total.Contains( sm.GetInstanceID() ) ) {
				ret.mesh_memory += (double) Profiler.GetRuntimeMemorySizeLong( sm );
				ret.mesh_tris += (ulong) ( sm.triangles.LongLength / 3 );
				ret.mesh_total.Add( sm.GetInstanceID() );
			}
			else if( !is_static_batched ) {
				ret.mesh_tris += (ulong) ( sm.triangles.LongLength / 3 );
			}

			/* Is an instance. */
			if( sm.name.Contains( "Instance" ) ) {
				++ret.mesh_instances;
				ret.mesh_instances_log.AppendLine( "    " + parent.name + " : " + sm.name );
			}
		}


		static void GetMaterialStats( Material[] mats, ref ObjectStatInfo ret, GameObject parent ) {
			if( mats == null )
				return;

			for( int i = 0; i < mats.Length; ++i ) {
				if( mats[ i ] == null )
					continue;

				/* First time we encounter or is an instance. */
				if( !ret.mat_total.Contains( mats[ i ].GetInstanceID() ) ) {
					ret.shader_passes += (ulong) mats[ i ].passCount;
					ret.mat_memory += (double) Profiler.GetRuntimeMemorySizeLong( mats[ i ] );
					ret.mat_total.Add( mats[ i ].GetInstanceID() );

					/* Get all textures from shader. */
					GetTextureStats( mats[ i ], ref ret );
				}

				/* Is an instance. */
				if( mats[ i ].name.Contains( "Instance" ) ) {
					++ret.mat_instances;
					ret.mat_instances_log.AppendLine( "    " + parent.name + " : " + mats[ i ].name );
				}
			}
		}


		static void GetTextureStats( Material m, ref ObjectStatInfo ret ) {
			if( m == null )
				return;

			Shader s = m.shader;

			if( s == null )
				return;

			for( int i = 0; i < ShaderUtil.GetPropertyCount( s ); ++i ) {
				if( ShaderUtil.GetPropertyType( s, i ) != ShaderUtil.ShaderPropertyType.TexEnv )
					continue;

				Texture t = m.GetTexture( ShaderUtil.GetPropertyName( s, i ) );
				if( t == null )
					continue;

				/* First encounter. */
				if( !ret.texture_total.Contains( t.GetInstanceID() ) ) {
					ret.texture_memory += (double) Profiler.GetRuntimeMemorySizeLong( t );
					ret.texture_total.Add( t.GetInstanceID() );
				}
			}
		}


		static void GetParticleStats( ParticleSystem ps, ref ObjectStatInfo ret ) {
			if( ps == null )
				return;

			ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
			if( !psr )
				return;

			if( psr.renderMode != ParticleSystemRenderMode.Mesh ) {
				ret.mesh_tris += _billboard_tris * (ulong) ps.particleCount;
			}
			else {
				ret.mesh_tris += (ulong) ( psr.mesh.triangles.LongLength / 3 ) * (ulong) ps.particleCount;
				ret.mesh_total.Add( psr.mesh.GetInstanceID() );
			}
		}


		static void GetAnimationClipStats( Animator a, ref ObjectStatInfo ret ) {
			RuntimeAnimatorController rac = a.runtimeAnimatorController;
			if( rac == null )
				return;

			AnimationClip[] acs = rac.animationClips;
			if( acs == null )
				return;

			for( int i = 0; i < acs.Length; ++i ) {
				if( acs[ i ] == null )
					continue;

				/* First encounter. */
				if( !ret.anim_total.Contains( acs[ i ].GetInstanceID() ) ) {
					ret.anim_memory += (double) Profiler.GetRuntimeMemorySizeLong( acs[ i ] );
					ret.anim_total.Add( acs[ i ].GetInstanceID() );
				}
			}
		}


		static void GetAudioStats( AudioClip ac, ref ObjectStatInfo ret ) {
			if( ac == null )
				return;

			if( !ret.audio_total.Contains( ac.GetInstanceID() ) ) {
				ret.audio_memory += (double) Profiler.GetRuntimeMemorySizeLong( ac );
				ret.audio_total.Add( ac.GetInstanceID() );
			}
		}


		static void GetSpriteStats( Sprite s, ref ObjectStatInfo ret ) {
			if( s == null )
				return;

			ret.sprite_packed += s.packed ? 1u : 0u;
			ret.mesh_tris += (ulong) ( s.triangles.LongLength / 3 );

			if( s.texture != null ) {
				if( !ret.texture_total.Contains( s.texture.GetInstanceID() ) ) {
					ret.texture_memory += (double) Profiler.GetRuntimeMemorySizeLong( s.texture );
					ret.texture_total.Add( s.texture.GetInstanceID() );
				}

			}

			if( GetAssociatedAlphaSplitTexture( s ) != null ) {
				if( !ret.texture_total.Contains( GetAssociatedAlphaSplitTexture( s ).GetInstanceID() ) ) {
					ret.texture_memory += (double) Profiler.GetRuntimeMemorySizeLong( GetAssociatedAlphaSplitTexture( s ) );
					ret.texture_total.Add( GetAssociatedAlphaSplitTexture( s ).GetInstanceID() );
				}
			}
		}


		public static Texture2D GetAssociatedAlphaSplitTexture( Sprite s ) {
#if UNITY_5_3_AND_UP
			return s.associatedAlphaSplitTexture;
#else
			return null;
#endif
		}


		/* Drill down hierarchy and collect stats. */
		static void RecurseStats( GameObject obj, ref ObjectStatInfo ret ) {
			bool is_static_batched = false;
			Renderer r = obj.GetComponent<Renderer>();
			if( r ) {
				ret.renderers_total += 1u;
				ret.renderers_visible += r.isVisible ? 1u : 0u;
				is_static_batched = r.isPartOfStaticBatch;
				ret.renderers_batched += is_static_batched ? 1u : 0u;
				GetMaterialStats( r.sharedMaterials, ref ret, obj );
			}

			MeshFilter mf = obj.GetComponent<MeshFilter>();
			if( mf ) {
				GetSharedMeshStats( mf.sharedMesh, ref ret, is_static_batched, obj );
			}

			SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();
			if( smr ) {
				GetSharedMeshStats( smr.sharedMesh, ref ret, is_static_batched, obj );
			}

			SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
			if( sr ) {
				GetSpriteStats( sr.sprite, ref ret );
			}

			ParticleSystem ps = obj.GetComponent<ParticleSystem>();
			if( ps ) {
				GetParticleStats( ps, ref ret );
			}

			Animator a = obj.GetComponent<Animator>();
			if( a ) {
				GetAnimationClipStats( a, ref ret );
			}

			AudioSource au = obj.GetComponent<AudioSource>();
			if( au ) {
				GetAudioStats( au.clip, ref ret );
			}

			for( int i = 0; i < obj.transform.childCount; ++i ) {
				RecurseStats( obj.transform.GetChild( i ).gameObject, ref ret );
			}

			/* TODO : Recurse on importers and assets. */
		}


		/* Return a cached stat object or generate new one. */
		static ObjectStatInfo GetStats( GameObject obj ) {
			//if( _cache.ContainsKey( obj ) ) {
			//	return _cache[ obj ];
			//}
			ObjectStatInfo ret = new ObjectStatInfo( obj.name );
			RecurseStats( obj, ref ret );

			//if( _enable_cache ) {
			//	_cache[ obj ] = ret;
			//}
			return ret;
		}

	} // class SelectionChange
}

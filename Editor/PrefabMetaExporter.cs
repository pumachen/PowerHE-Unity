using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IGG.IT4M;
using UnityEditor;
using UnityEngine;
using PrimitiveMeta = PrefabMetaExporter.PrefabMeta.PrimitiveMeta;

namespace PowerHE
{
	public static class PrefabMetaExporter
	{
		[System.Serializable]
		public class PrefabMeta
		{
			public string prefabPath;
			public PrimitiveMeta[] primitives;

			[System.Serializable]
			public class PrimitiveMeta
			{
				public Vector3 position;
				public Vector3 rotation;
				public Vector3 scale;
				public string model;
				public string[] textures;
			}
		}

		public class Primitive
		{
			public Vector3 position;
			public Vector3 rotation;
			public Vector3 scale;
			public Mesh mesh;
			public Texture2D[] textures;
		}

		public static string rootDir = Application.dataPath;

		[MenuItem("PowerHE/Export Selected Prefabs")]
		public static void GenPrefabMeta()
		{
			rootDir = EditorUtility.OpenFolderPanel("Prefab Gallery", rootDir, "PrefabGallery");
			//string rootDir = @"D://workspace/houdini/WorldMap/PrefabGallery";
			foreach (var prefab in Selection.GetFiltered<GameObject>(SelectionMode.Assets))
			{
				GenPrefabMeta(rootDir, prefab);
			}
			AssetDatabase.Refresh();
		}
		
		public static void GenPrefabMeta(string rootDir, GameObject prefab)
		{
			string prefabPath = AssetDatabase.GetAssetPath(prefab);
			string outputDir = Path.Combine(rootDir, Path.GetDirectoryName(prefabPath));
			Directory.CreateDirectory(outputDir);
			Primitive[] primitives = GetPrefabMeta(prefab);
			if (primitives == null)
			{
				Debug.Log(prefabPath);
				return;
			}

			Dictionary<Texture2D, string> texturePaths = new Dictionary<Texture2D, string>();
			Dictionary<Mesh, string> modelPaths = new Dictionary<Mesh, string>();
			List<PrimitiveMeta> primitiveMetas = new List<PrimitiveMeta>();
			foreach (var primitive in primitives)
			{
				Mesh mesh = primitive.mesh;
				string model;
				if (!modelPaths.TryGetValue(mesh, out model))
				{
					model = $"{mesh.name}_{mesh.GetHashCode()}.obj";
					File.WriteAllText(Path.Combine(outputDir, model), mesh.EncodeToOBJ());
					modelPaths.Add(mesh, model);
				}

				List<string> textures = new List<string>();
				foreach (var texture in primitive.textures)
				{
					string texturePath = "";
					if (texture != null && !texturePaths.TryGetValue(texture, out texturePath))
					{
						texturePath = $"{texture.name}_{texture.GetHashCode()}.png";
						int width = texture.width;
						int height = texture.height;
						TextureFormat format = TextureFormat.RGB24;
						TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
						if (textureImporter != null && textureImporter.alphaIsTransparency)
						{
							format = TextureFormat.ARGB32;
						}
						RenderTexture tmpBuffer = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
						Graphics.Blit(texture, tmpBuffer);
						RenderTexture.active = tmpBuffer;
						Texture2D tex = new Texture2D(width, height, format, false);
						tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
						tex.Apply();
						RenderTexture.ReleaseTemporary(tmpBuffer);
						File.WriteAllBytes(Path.Combine(outputDir, texturePath), tex.EncodeToPNG());
						Object.DestroyImmediate(tex);
						texturePaths.Add(texture, texturePath);
					}
					textures.Add(texturePath);
				}

				PrimitiveMeta primitiveMeta = new PrimitiveMeta()
				{
					position = primitive.position,
					rotation = primitive.rotation,
					scale = primitive.scale,
					model = model,
					textures = textures.ToArray()
				};
				primitiveMetas.Add(primitiveMeta);
			}

			PrefabMeta prefabMeta = new PrefabMeta()
			{
				prefabPath = prefabPath,
				primitives = primitiveMetas.ToArray()
			};
			string json = JsonUtility.ToJson(prefabMeta);
			string fileName = $"{Path.GetFileNameWithoutExtension(prefabPath)}.prefabmeta";
			string metaPath = Path.Combine(outputDir, fileName);
			File.WriteAllText(metaPath, json);
		}

		public static Primitive[] GetPrefabMeta(this GameObject prefab)
		{
			MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();
			List<Texture2D> textures = new List<Texture2D>();
			List<Primitive> primitives = new List<Primitive>();
			foreach (var renderer in renderers)
			{
				string name = renderer.gameObject.name;
				if (name.Contains("LOD") && !name.Contains("LOD0"))
					continue;
				if (name.ToLower().Contains("collision"))
					continue;
				Transform transform = renderer.transform;
				MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
				Mesh mesh = meshFilter.sharedMesh;
				if (mesh == null)
					continue;
				string model = AssetDatabase.GetAssetPath(mesh);
				if (string.IsNullOrEmpty(model))
					continue;
				Material[] materials = renderer.sharedMaterials;
				if (materials == null || materials.Length == 0)
					continue;
				foreach (var mat in materials)
				{
					Texture2D mainTex = mat.mainTexture as Texture2D;
					textures.Add(mainTex);
				}

				Primitive primitive = new Primitive()
				{
					position = transform.position,
					rotation = transform.rotation.eulerAngles,
					scale = transform.lossyScale,
					mesh = mesh,
					textures = textures.ToArray(),
				};
				primitives.Add(primitive);
				textures.Clear();
			}
			if (primitives.Count == 0)
				return null;
			return primitives.ToArray();
		}
	}
}
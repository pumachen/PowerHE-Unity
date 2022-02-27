using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PowerHE
{
	public static class MeshExportExtension
	{
		public static string EncodeToOBJ(this Mesh mesh)
		{
			StringBuilder sb = new StringBuilder();

			Vector3[] vertices = mesh.vertices;
			Color[] colors = mesh.colors;
			int numVertices = vertices.Length;
			for (int vtxnum = 0; vtxnum < numVertices; ++vtxnum)
			{
				Vector3 v = vertices[vtxnum];
				Color c = Color.white;
				if (colors.Length > 0)
				{
					c = colors[vtxnum];
				}
				sb.AppendLine($"v {-v.x} {v.y} {-v.z} {c.a} {c.a} {c.a}");
			}
			foreach (Vector3 v in mesh.vertices)
			{
			}
			sb.Append("\n");
			
			foreach (Vector3 n in mesh.normals)
			{
				sb.AppendLine($"vn {-n.x} {n.y} {-n.z}");
			}
			sb.Append("\n");

			foreach (Vector2 uv in mesh.uv)
			{
				sb.AppendLine($"vt {uv.x} {uv.y}");
			}
			sb.Append("\n");

			for (int submeshIdx = 0; submeshIdx < mesh.subMeshCount; ++submeshIdx)
			{
				sb.AppendLine($"usemtl {submeshIdx}");
				sb.AppendLine($"usemap {submeshIdx}");

				int[] triangles = mesh.GetTriangles(submeshIdx);
				for (int tri = 0; tri < triangles.Length; tri += 3)
				{
					sb.AppendLine(
						string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
							triangles[tri + 2] + 1, 
							triangles[tri + 1] + 1, 
							triangles[tri] + 1));
				}
			}
			return sb.ToString();
		}
	}
}
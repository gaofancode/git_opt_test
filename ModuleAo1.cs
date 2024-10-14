using UnityEngine;

namespace Voxels
{
	public static class ModuleAo
	{
		public static void BakeAo(Mesh mesh, MeshCollider mc)
		{
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;
			Color[] array = new Color[vertices.Length];
			float[] array2 = new float[vertices.Length];
			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 normalized = Vector3.Cross(vertices[triangles[i + 1]] - vertices[triangles[i]], vertices[triangles[i + 2]] - vertices[triangles[i]]).normalized;
				for (int j = 0; j < 16; j++)
				{
					Vector3 vector = Random.onUnitSphere;
					if (Vector3.Dot(vector, normalized) < 0f)
					{
						vector = -vector;
					}
					Vector3 vector2 = new Vector3(Random.value, Random.value, Random.value);
					vector2 /= vector2.x + vector2.y + vector2.z;
					vector2 = Vector3.one / 3f;
					Vector3 position = vertices[triangles[i]] * vector2.x + vertices[triangles[i + 1]] * vector2.y + vertices[triangles[i + 2]] * vector2.z;
					position = mc.transform.TransformPoint(position);
					Ray ray = new Ray(position, vector);
					float num = 1f;
					RaycastHit hitInfo;
					Color a = (!mc.Raycast(ray, out hitInfo, num)) ? Color.white : (Color.white * (hitInfo.distance / num));
					array[triangles[i]] += a * vector2.x;
					array[triangles[i + 1]] += a * vector2.y;
					array[triangles[i + 2]] += a * vector2.z;
					array2[triangles[i]] += vector2.x;
					array2[triangles[i + 1]] += vector2.y;
					array2[triangles[i + 2]] += vector2.z;
				}
			}
			for (int k = 0; k < array.Length; k++)
			{
				array[k] /= array2[k];
			}
			for (int l = 0; l < array.Length; l++)
			{
				array[l].a = 1f;
			}
			Color32[] array3 = new Color32[array.Length];
			for (int m = 0; m < array.Length; m++)
			{
				array3[m] = array[m];
			}
			mesh.colors32 = array3;
		}
	}
}

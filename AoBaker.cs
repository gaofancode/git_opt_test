using System.Collections.Generic;
using UnityEngine;
using Voxels.TowerDefense;

namespace Voxels
{
	public class AoBaker : IslandComponent, IIslandEnter, IIslandFirstEnter
	{
		public int loops;

		public int radius = 5;

		public int bakerLoops = 10;

		public Fake3dTex textureAO;

		public Fake3dTex textureNormal;

		private static Stack<Marcher> marcherStack = new Stack<Marcher>(2);

		public Matrix4x4 world2voxel
		{
			get
			{
				return voxelSpace.world2Voxel;
			}
		}

		public VoxelSpace voxelSpace
		{
			get
			{
				return base.island.voxelSpace;
			}
		}

		[ContextMenu("Insta bake")]
		private void InstaBake()
		{
			IEnumerator<GenInfo> enumerator = BakeAo(base.island);
			while (enumerator.MoveNext())
			{
			}
		}

		public Vector2 DirToLatLong(Vector3 dir)
		{
			Vector2 result = default(Vector2);
			result.y = (dir.y + 1f) / 2f;
			result.x = (Mathf.Atan2(dir.x, dir.z) * 57.29578f / 360f + 1f) % 1f;
			return result;
		}

		public Vector4 GetColor(Vector3 normal)
		{
			Color color = default(Color);
			color.r = Mathf.Clamp01(Vector3.Dot(normal, new Vector3(1f, 0.2f, 0f).normalized));
			color.g = Mathf.Clamp01(normal.y);
			color.b = Mathf.Clamp01(Vector3.Dot(normal, new Vector3(-1f, 0.2f, 0f).normalized));
			color.a = 1f;
			color = color * color * color * color * color;
			color.a = (normal.y + 1f) / 2f;
			return color;
		}

		private void PropagateColor(Marcher.Node node, float alpha = 1f)
		{
			node.hasColor = true;
			if (node.nodes.Count == 0)
			{
				node.color = GetColor(node.normal) * alpha;
				return;
			}
			node.color = Vector4.zero;
			alpha /= (float)node.nodes.Count;
			foreach (Marcher.Node child in node.nodes)
			{
				PropagateColor(child, alpha);
				node.color += child.color;
			}
		}

		private Vector4 GetAo(Marcher.Node node, Vector3 offset)
		{
			Vector3 vector = node.pos + offset;
			vector.y = Mathf.Abs(vector.y);
			if (!voxelSpace.voxelBounds.Contains(vector))
			{
				return node.color;
			}
			VoxelSpace.CornerVoxel cornerVoxel = voxelSpace.GetCornerVoxel(vector);
			if (cornerVoxel.inside)
			{
				return Vector4.zero;
			}
			if (node.nodes.Count == 0)
			{
				return node.color;
			}
			float d = 1f - cornerVoxel.coveredArea;
			Vector4 zero = Vector4.zero;
			for (int i = 0; i < node.nodes.Count; i++)
			{
				Marcher.Node node2 = node.nodes[i];
				zero += GetAo(node2, offset);
			}
			return zero * d;
		}

		IEnumerator<GenInfo> IIslandFirstEnter.OnIslandFirstEnter(Island island)
		{
			textureAO = new Fake3dTex(voxelSpace.size + Vector3.one, new Color(0f, 0f, 0f, 1f), false);
			textureAO.Apply();
			yield return default(GenInfo);
			textureNormal = new Fake3dTex(voxelSpace.size, new Color(0.5f, 0.5f, 0.5f, 0f), false);
			textureNormal.Apply();
			yield return default(GenInfo);
			IEnumerator<GenInfo> bakeBoth = BakeBoth(island);
			bool moving = true;
			while (moving)
			{
				using (new ScopedProfiler("Baking Both"))
				{
					moving = bakeBoth.MoveNext();
				}
				yield return bakeBoth.Current;
			}
			yield return default(GenInfo);
		}

		public Color GetColorLinear(Vector3 wPos)
		{
			Vector3 coordinate = world2voxel.MultiplyPoint(wPos) + Vector3.one / 2f;
			return textureAO.GetPixelLinear(coordinate);
		}

		public void SetColorComponent(Vector3 wPos, int componet, float value)
		{
			Vector3 coordinate = world2voxel.MultiplyPoint(wPos) + Vector3.one;
			Color pixel = textureAO.GetPixel(coordinate);
			pixel = pixel.SetComponent(componet, value);
			textureAO.SetPixel(coordinate, pixel);
		}

		public void SetColorComponentLinear(Vector3 wPos, int componet, float value, float interpolator = 1f)
		{
			Vector3 coordinate = world2voxel.MultiplyPoint(wPos) - Vector3.one / 2f;
			textureAO.SetComponentLinear(coordinate, componet, value, interpolator);
		}

		public IEnumerator<GenInfo> BakeBoth(Island island)
		{
			IEnumerator<GenInfo> bakeAO = BakeAo(island);
			bool moving2 = true;
			while (moving2)
			{
				using (new ScopedProfiler("Baking AO"))
				{
					moving2 = bakeAO.MoveNext();
				}
				yield return bakeAO.Current;
			}
			yield return new GenInfo("Updating Normals");
			voxelSpace.UpdateNormals();
			yield return new GenInfo("Updating Normals");
			IEnumerator<GenInfo> normalRoutine = BakeNormal();
			bool moving = true;
			while (moving)
			{
				using (new ScopedProfiler("Baking Normals"))
				{
					moving = normalRoutine.MoveNext();
				}
				yield return normalRoutine.Current;
			}
		}

		public IEnumerator<GenInfo> BakeAo(Island island)
		{
			Marcher marcher = (marcherStack.Count != 0) ? marcherStack.Pop() : new Marcher(5);
			GenInfo genInfo = new GenInfo("Baking light");
			yield return genInfo;
			PropagateColor(marcher.center);
			for (int i = 0; i < voxelSpace.cornerVoxels.Length; i++)
			{
				using ((ScopedProfiler)("BakeAO For Loop " + i))
				{
					VoxelSpace.CornerVoxel cornerVoxel = voxelSpace.cornerVoxels[i];
					Vector4 ao = GetAo(marcher.center, cornerVoxel.pos);
					Color gamma = ((Color)ao).gamma;
					textureAO.SetPixel(cornerVoxel.pos, gamma);
				}
				yield return genInfo;
			}
			textureAO.Apply();
			marcherStack.Push(marcher);
			yield return genInfo;
		}

		private IEnumerator<GenInfo> BakeNormal()
		{
			for (int i = 0; i < voxelSpace.voxels.Length; i++)
			{
				VoxelSpace.Voxel voxel = voxelSpace.voxels[i];
				Vector3 pos = voxel.pos;
				Vector3 normal2 = voxel.normal;
				normal2 = (normal2 + Vector3.one) / 2f;
				Color color = new Color(normal2.x, normal2.y, normal2.z, voxel.openness);
				textureNormal.SetPixel(pos, color);
				yield return new GenInfo("Baking Normal");
			}
			textureNormal.Apply();
			yield return new GenInfo("Baking Normal");
		}

		public void ApplyShaderVariables()
		{
			textureAO.SetShaderVariables("_AoTex", "_AoTexVolume", "_AoTexSize");
			textureNormal.SetShaderVariables("_NormalTex", "_NormalTexVolume", "_NormalTexSize");
			Shader.SetGlobalMatrix("_World2Voxel", world2voxel);
			Shader.SetGlobalMatrix("_Voxel2World", base.island.voxelSpace.voxel2world);
		}

		public IEnumerator<GenInfo> OnIslandEnter(Island island)
		{
			ApplyShaderVariables();
			yield return default(GenInfo);
		}
	}
}

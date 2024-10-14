using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
	/// <summary>
	///     在计算AO贴图时，Marcher通常是指Marching算法中的步长或步进值。
	///     Marching算法是一种常用的体积渲染算法，用于从三维数据中生成二维图像。
	///     在计算AO贴图时，Marcher可以控制光线与表面之间的距离，从而影响AO的强度和分辨率。
	///     通常，较小的Marcher值会产生更准确的AO效果，但也会增加计算量。
	/// </summary>
	public class Marcher
	{
		public class Node
		{
			public float alpha;

			public Vector4 color = Vector4.zero;

			public bool hasColor;

			public Vector3 pos;

			public Vector3 normal;

			public List<Node> nodes = new List<Node>();

			public Node(Vector3 pos)
			{
				this.pos = pos;
				normal = pos.normalized;
			}

			public void PropagateAlpha()
			{
				float num = alpha / (float)nodes.Count;
				for (int i = 0; i < nodes.Count; i++)
				{
					nodes[i].alpha = num;
					nodes[i].PropagateAlpha();
				}
			}
		}

		public List<Node> nodes = new List<Node>();

		public Node center;

		public Marcher(int radius)
		{
			//左排半径格子，右一排半径格子，加中间的自己
			int num = radius * 2 + 1;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num; k++)
					{
						Vector3 vector = new Vector3(i, j, k) - Vector3.one * radius;
						//vd.magnitude是每一个点到中心点的距离
						float dt = 1f - vector.magnitude / (float)radius;
						//dt越接近1，说明离中心点越近
						if (dt > 0f)
						{
							Node item = new Node(vector);
							nodes.Add(item);
							if (vector == Vector3.zero)
							{
								center = item;
							}
						}
					}
				}
			}
			nodes.TrimExcess();
			Setup();
		}

		public Marcher(Vector3 extents)
		{
			Vector3 vector = extents * 2f + Vector3.one;
			for (int i = 0; (float)i < vector.x; i++)
			{
				for (int j = 0; (float)j < vector.y; j++)
				{
					for (int k = 0; (float)k < vector.z; k++)
					{
						Vector3 vector2 = new Vector3(i, j, k) - extents;
						Node item = new Node(vector2);
						nodes.Add(item);
						if (vector2 == Vector3.zero)
						{
							center = item;
						}
					}
				}
			}
			nodes.TrimExcess();
			Setup();
		}

		private void Setup()
		{
			Dictionary<Vector3, Node> dictionary = new Dictionary<Vector3, Node>();
			for (int i = 0; i < nodes.Count; i++)
			{
				dictionary.Add(nodes[i].pos, nodes[i]);
			}
			for (int j = 0; j < nodes.Count; j++)
			{
				Node node = nodes[j];
				if (node == null)
				{
					continue;
				}
				int[] array = new int[3]
				{
					(int)node.pos.x,
					(int)node.pos.y,
					(int)node.pos.z
				};
				int[] array2 = new int[3]
				{
					Mathf.Abs(array[0]),
					Mathf.Abs(array[1]),
					Mathf.Abs(array[2])
				};
				int num = Mathf.Max(array2);
				if (num == 0)
				{
					continue;
				}
				int num2 = (int)Mathf.Pow(2f, num) % num;
				int[] array3 = new int[3];
				for (int k = 0; k < 3; k++)
				{
					if (array[k] != 0 && (array2[k] == num || array2[k] > num2))
					{
						array3[k] = (int)Mathf.Sign(array[k]);
					}
				}
				Vector3 b = new Vector3(-array3[0], -array3[1], -array3[2]);
				Vector3 key = node.pos + b;
				dictionary[key].nodes.Add(node);
			}
			center.alpha = 1f;
			center.PropagateAlpha();
		}

		public void RemoveBranch(Node node)
		{
			for (int i = 0; i < node.nodes.Count; i++)
			{
				RemoveBranch(node.nodes[i]);
			}
			nodes.Remove(node);
		}
	}
}

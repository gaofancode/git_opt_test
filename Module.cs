using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
	[SelectionBase]
	public class Module : MonoBehaviour
	{
		[Serializable]
		public class Cell
		{
			/// <summary>
			/// 相对位置
			/// </summary>
			public Vector3 pos;

			/// <summary>
			/// 边的信息
			/// </summary>
			public List<Edge> edges = new List<Edge>();

			/// <summary>
			/// 用于记录八个角的情况，某些角是不是内部的
			/// 参考cConstants.corners
			/// </summary>
			public Corner[] corners = new Corner[8];

			/// <summary>
			/// 有一些模块是由多个cell组成的，这里用于标识这个cell哪个面是内部的
			/// 面的朝赂参考Constants.directions
			/// </summary>
			public bool[] internalSide = new bool[6];

			/// <summary>
			/// 哪个面是可以通行的
			/// </summary>
			public bool[] navigable = new bool[6];

			public Cell(Vector3 pos)
			{
				this.pos = pos;
				for (int i = 0; i < corners.Length; i++)
				{
					corners[i] = new Corner(Constants.corners[i]);
				}
			}
		}

		public bool isNull;

		public Bounds bounds;

		public List<Cell> cells = new List<Cell>();

		public int uses;

		public GameObject objectToCopy;

		public List<OrientedModule> orientations = new List<OrientedModule>();

		public List<ModuleSet> sets = new List<ModuleSet>();

		public int placementToShow;

		public bool navigable;

		public bool house;

		public bool forcedNavigability;

		public Mesh debugMesh;

		public int GetSetKey()
		{
			string text = string.Empty;
			for (int i = 0; i < sets.Count; i++)
			{
				text += sets[i].name;
			}
			return text.GetHashCode();
		}

		public bool Contains(Vector3 pos)
		{
			pos = base.transform.worldToLocalMatrix.MultiplyPoint(pos);
			pos = ExtraMath.Round(pos);
			for (int i = 0; i < cells.Count; i++)
			{
				if (cells[i].pos == pos)
				{
					return true;
				}
			}
			return false;
		}

		public GameObject GetGameObject(TransformSettings settings)
		{
			if ((bool)objectToCopy)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objectToCopy);
				settings.ApplyLocal(gameObject.transform);
				return gameObject;
			}
			return null;
		}
		
#if UNITY_EDITOR
		private static Color[] g_pointColors = new Color[8]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.cyan,
			Color.magenta,
			Color.white,
			Color.black
		};
		private void OnDrawGizmos()
		{
			if (UnityEditor.Selection.activeTransform != transform)
			{
				return;
			}

			var yOffset = new Vector3(0, 0.01f, 0f);
			for (int i = 0; i < cells.Count; i++)
			{
				var t = cells[i];
				var p = transform.position + t.pos;
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(p, new Vector3(1,1,1));
				Gizmos.color = Color.green;
				for (int j = 0; j < t.edges.Count; j++)
				{
					var edge = t.edges[j];
					var start = p + edge.v0 + yOffset;
					var end = p + edge.v1 + yOffset;
					if (j < g_pointColors.Length)
					{
						Gizmos.color = g_pointColors[j];
					}
					else
					{
						Gizmos.color = g_pointColors[g_pointColors.Length - 1];
					}
					Gizmos.DrawLine(start, end);
					Gizmos.DrawSphere(start, 0.05f);
				}
			}
		}
#endif
	}
}

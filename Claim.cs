using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Voxels
{
	[Serializable]
	public class Claim
	{
		[Serializable]
		public class Edges
		{
			public List<Edge> edges = new List<Edge>();
		}

		public enum Mode
		{
			Outside,
			Inside,
			Internal,
			Standard
		}

		public Vector3 pos;

		public int[] keys = new int[6];

		public bool[] navigable = new bool[6];

		public bool anyNavigable;

		public string[] keyStrings = new string[6];

		public Edges[] edges = new Edges[6];

		public Vector3 normal;

		public Vector3 navigationNormal;

		public bool[] cornersInside = new bool[8];

		public Mode[] mode = new Mode[6];

		public Claim(Claim srcClaim)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i] = srcClaim.keys[i];
			}
			for (int j = 0; j < cornersInside.Length; j++)
			{
				cornersInside[j] = srcClaim.cornersInside[j];
			}
		}

		public Claim(Module.Cell cell, TransformSettings setting)
		{
			Matrix4x4 matrix = setting.matrix;
			pos = ExtraMath.Round(matrix.MultiplyPoint(cell.pos));
			List<string>[] array = new List<string>[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = new List<string>();
				edges[i] = new Edges();
				mode[i] = Mode.Standard;
			}
			
			//因为只是水平转动, 所以对于同样的角坐标，他该是内部的，还会是内部
			for (int i = 0; i < cell.corners.Length; i++)
			{
				Vector3 cornerPos = matrix.MultiplyVector(Constants.corners[i]);
				for (int j = 0; j < 8; j++)
				{
					if (ExtraMath.CloseEnough(cornerPos, Constants.corners[j]))
					{
						cornersInside[j] = cell.corners[i].inside;
					}
				}
			}
			
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					Vector3 lhs = ExtraMath.Round(matrix.MultiplyVector(Constants.directions[j]));
					if (lhs == Constants.directions[i])
					{
						if (cell.internalSide[j])
						{
							mode[i] = Mode.Internal;
						}
						navigable[i] = cell.navigable[j];
						break;
					}
				}
			}
			
			anyNavigable = navigable.Contains(true);
			bool flipped = setting.GetFlipped();
			for (int i = 0; i < cell.edges.Count; i++)
			{
				int vertexIndex = flipped ? 1 : 0;
				var edge = cell.edges[i];
				Vector3 vertexStart = matrix.MultiplyVector(edge.GetVertex(vertexIndex));
				Vector3 vertexEnd = matrix.MultiplyVector(edge.GetVertex(1 - vertexIndex));
				Vector3 tNormal = matrix.MultiplyVector(edge.normal);
				Vector3 tSide = ExtraMath.Round(matrix.MultiplyVector(edge.side));
				for (int j = 0; j < 6; j++)
				{
					if (mode[j] == Mode.Standard && tSide == Constants.directions[j])
					{
						vertexStart -= Constants.directions[j] * 0.5f;
						vertexEnd -= Constants.directions[j] * 0.5f;
						string startStr = GetPositionString(vertexStart);
						string endStr = GetPositionString(vertexEnd);
						string text = "{" + ((string.Compare(startStr, endStr) != 1) ? (endStr + startStr) : (startStr + endStr)) + GetNormalString(tNormal, edge.normalPrecision) + edge.extraString + "}";
						edges[j].edges.Add(new Edge(vertexStart, vertexEnd, tNormal, tSide, edge.extraString, edge.normalPrecision, text));
						array[j].Add(text);
						break;
					}
				}
			}
			for (int i = 0; i < 6; i++)
			{
				if (edges[i].edges.Count > 0)
				{
					continue;
				}
				bool inside = true;
				bool outside = true;
				for (int j = 0; j < cell.corners.Length; j++)
				{
					Vector3 lhs2 = Constants.corners[j];
					if (Vector3.Dot(lhs2, Constants.directions[i]) > 0f)
					{
						lhs2 -= Constants.directions[i] * 0.5f;
						if (cornersInside[j])
						{
							outside = false;
						}
						if (!cornersInside[j])
						{
							inside = false;
						}
					}
				}
				if (inside)
				{
					mode[i] = Mode.Inside;
				}
				if (outside)
				{
					mode[i] = Mode.Outside;
				}
			}
			
			for (int i = 0; i < 6; i++)
			{
				if (mode[i] == Mode.Standard)
				{
					array[i].Sort();
					keyStrings[i] = string.Empty;
					int num8 = 0;
					string[] array2 = null;
					(array2 = keyStrings)[num8 = i] = array2[num8] + array[i].Count;
					for (int j = 0; j < array[i].Count; j++)
					{
						int num10 = 0;
						(array2 = keyStrings)[num10 = i] = array2[num10] + array[i][j];
					}
					for (int j = 0; j < cell.corners.Length; j++)
					{
						Vector3 vector5 = Constants.corners[j];
						if (Vector3.Dot(vector5, Constants.directions[i]) > 0f)
						{
							vector5 -= Constants.directions[i] * 0.5f;
							int num12 = 0;
							(array2 = keyStrings)[num12 = i] = array2[num12] + (vector5 * 2f).ToString("F0") + cornersInside[j];
						}
					}
				}
				else
				{
					keyStrings[i] = mode[i].ToString();
				}
			}
			for (int i = 0; i < 6; i++)
			{
				if (mode[i] == Mode.Standard)
				{
					keys[i] = keyStrings[i].GetHashCode();
				}
				else
				{
					keys[i] = (int)mode[i];
				}
			}
			for (int i = 0; i < cornersInside.Length; i++)
			{
				normal += ((!cornersInside[i]) ? Constants.corners[i] : (-Constants.corners[i]));
			}
			normal.Normalize();
		}

		private string GetNormalString(Vector3 normal, float mul)
		{
			normal *= mul;
			return normal.ToString("F0");
		}

		private string GetPositionString(Vector3 position)
		{
			return (position * 100f).ToString("F0");
		}

		public int GetSumKey()
		{
			string str = string.Empty;
			for (int i = 0; i < 6; i++)
			{
				str = str + keys[i].ToString() + "_";
			}
			str += pos.ToString();
			return str.GetHashCode();
		}
	}
}

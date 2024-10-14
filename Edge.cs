using System;
using UnityEngine;

namespace Voxels
{
	[Serializable]
	public class Edge
	{
		public Vector3 v0;

		public Vector3 v1;

		public Vector3 normal;

		public string name;

		public string extraString;

		public float normalPrecision;

		public Vector3 side;

		public Edge(Vector3 v0, Vector3 v1, Vector3 normal, Vector3 side, string extraString, float normalPrecision, string name = "")
		{
			this.v0 = v0;
			this.v1 = v1;
			this.normal = normal;
			this.extraString = extraString;
			this.normalPrecision = normalPrecision;
			this.name = name;
			this.side = side;
		}

		public Vector3 GetVertex(int index)
		{
			return index == 0 ? v0 : v1;
		}
	}
}

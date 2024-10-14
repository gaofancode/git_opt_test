using System;
using UnityEngine;

namespace Voxels
{
	[Serializable]
	public class Corner
	{
		public Vector3 pos;

		public bool inside;

		public Corner(Vector3 pos)
		{
			this.pos = pos;
		}

		public Corner(Vector3 pos, bool inside)
		{
			this.pos = pos;
			this.inside = inside;
		}
	}
}

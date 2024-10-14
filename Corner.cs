using System;
using UnityEngine;
落荒而走基
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

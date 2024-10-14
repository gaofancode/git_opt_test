using System;
using UnityEngine;

namespace Voxels
{
	[Serializable]
	public class LevelSettings
	{
		public Vector3 size = new Vector3(7f, 11f, 7f);

		public string key
		{
			get
			{
				return size.ToString("F0");
			}
		}
	}
}

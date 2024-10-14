using System;
using UnityEngine;

namespace Voxels
{
	public static class Constants
	{
		public static Vector3[] directions = new Vector3[6]
		{
			Vector3.forward,
			Vector3.right,
			Vector3.up,
			Vector3.back,
			Vector3.left,
			Vector3.down
		};

		public static Quaternion[] rotations = new Quaternion[6]
		{
			Quaternion.Euler(0f, 0f, 0f),
			Quaternion.Euler(0f, 90f, 0f),
			Quaternion.Euler(0f, 180f, 0f),
			Quaternion.Euler(0f, 270f, 0f),
			Quaternion.Euler(270f, 0f, 0f),
			Quaternion.Euler(90f, 0f, 0f)
		};

		public static int[] opposites = new int[6]
		{
			3,
			4,
			5,
			0,
			1,
			2
		};

		public static int[] absolutes = new int[6]
		{
			0,
			1,
			2,
			0,
			1,
			2
		};

		public static int[] components = new int[6]
		{
			2,
			0,
			1,
			2,
			0,
			1
		};

		/// <summary>
		/// 一个cell八个角的偏移值
		/// </summary>
		public static Vector3[] corners = new Vector3[8]
		{
			new Vector3(-1f, -1f, -1f) / 2f,
			new Vector3(1f, -1f, -1f) / 2f,
			new Vector3(-1f, 1f, -1f) / 2f,
			new Vector3(1f, 1f, -1f) / 2f,
			new Vector3(-1f, -1f, 1f) / 2f,
			new Vector3(1f, -1f, 1f) / 2f,
			new Vector3(-1f, 1f, 1f) / 2f,
			new Vector3(1f, 1f, 1f) / 2f
		};

		public static Vector3[] diagonals = new Vector3[4]
		{
			new Vector3(-1f, 0f, -1f),
			new Vector3(1f, 0f, -1f),
			new Vector3(-1f, 0f, 1f),
			new Vector3(1f, 0f, 1f)
		};

		public const float waterLevel = -0.105f;

		public const float cameraAngle = 30f;

		public static float upScale = 1f / Mathf.Sin((float)Math.PI / 3f);
	}
}

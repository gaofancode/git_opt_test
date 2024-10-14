using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
	public class CornerSlot
	{
		public enum Mode
		{
			any,
			inside,
			outside
		}

		public enum State
		{
			Outside,
			Inside,
			Unclear
		}

		public Vector3 pos;

		public Mode mode;

		public List<Domino>[,] dominos;

		private List<Domino>[,] savedDominos;

		public int[] stateCount = new int[2];

		public int[] savedStateCount = new int[2];

		private bool collapsed;

		private bool savedCollapsed;

		public bool forcedVisible;

		public int occlusionCount;

		public int savedOcclusionCount;

		public bool[] occludedAngles;

		public bool[] savedOccludedAngles;

		public float visiblity
		{
			get
			{
				return 1f - (float)occlusionCount / (float)occludedAngles.Length;
			}
		}

		public State state
		{
			get
			{
				if (stateCount[1] == 0)
				{
					return State.Outside;
				}
				if (stateCount[0] == 0)
				{
					return State.Inside;
				}
				return State.Unclear;
			}
		}

		public bool inside
		{
			get
			{
				return stateCount[0] == 0;
			}
		}

		public CornerSlot(Vector3 pos, int angles)
		{
			this.pos = pos;
			occludedAngles = new bool[angles];
			savedOccludedAngles = new bool[angles];
			occlusionCount = 0;
			dominos = new List<Domino>[8, 2];
			savedDominos = new List<Domino>[8, 2];
			for (int i = 0; i < 8; i++)
			{
				dominos[i, 0] = new List<Domino>();
				dominos[i, 1] = new List<Domino>();
				savedDominos[i, 0] = new List<Domino>();
				savedDominos[i, 1] = new List<Domino>();
			}
		}

		public bool Occlude(int i)
		{
			if (occludedAngles[i])
			{
				return false;
			}
			occludedAngles[i] = true;
			occlusionCount++;
			return true;
		}

		public Color GetColor()
		{
			return (!inside) ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0f);
		}

		public void AddDomino(Domino domino, int index, bool inside)
		{
			List<Domino> list = dominos[index, inside ? 1 : 0];
			stateCount[inside ? 1 : 0]++;
			list.Add(domino);
		}

		public State RemoveDomino(Domino domino, int index, bool inside)
		{
			using (new ScopedProfiler("Remove Domino from Corner Slot"))
			{
				List<Domino> list = dominos[index, inside ? 1 : 0];
				if (list.Remove(domino))
				{
					stateCount[inside ? 1 : 0]--;
					if (list.Count == 0 && !collapsed)
					{
						collapsed = true;
						return inside ? State.Inside : State.Outside;
					}
				}
				return State.Unclear;
			}
		}

		public void SaveState()
		{
			savedCollapsed = collapsed;
			savedStateCount[0] = stateCount[0];
			savedStateCount[1] = stateCount[1];
			for (int i = 0; i < occludedAngles.Length; i++)
			{
				savedOccludedAngles[i] = occludedAngles[i];
			}
			savedOcclusionCount = occlusionCount;
			for (int j = 0; j < 8; j++)
			{
				savedDominos[j, 0].Clear();
				savedDominos[j, 1].Clear();
				savedDominos[j, 0].AddRange(dominos[j, 0]);
				savedDominos[j, 1].AddRange(dominos[j, 1]);
			}
		}

		public void Reset()
		{
			collapsed = savedCollapsed;
			forcedVisible = false;
			for (int i = 0; i < occludedAngles.Length; i++)
			{
				occludedAngles[i] = savedOccludedAngles[i];
			}
			occlusionCount = savedOcclusionCount;
			stateCount[0] = savedStateCount[0];
			stateCount[1] = savedStateCount[1];
			for (int j = 0; j < 8; j++)
			{
				dominos[j, 0].Clear();
				dominos[j, 1].Clear();
				dominos[j, 0].AddRange(savedDominos[j, 0]);
				dominos[j, 1].AddRange(savedDominos[j, 1]);
			}
		}
	}
}

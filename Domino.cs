using UnityEngine;
测试提交3
namespace Voxels
{
	public class Domino
	{
		public enum State
		{
			idle,
			done,
			removing
		}

		public Wrapper placementWrapper;

		public Vector3 offset;

		public State state;

		public State savedState;

		public OrientedModule placedModule;

		public float centerness;

		public float defaultScore;

		public float score;

		public float fraction;

		public Placement placement
		{
			get
			{
				return placementWrapper.placement;
			}
		}

		public float outwardness
		{
			get
			{
				return 1f - centerness;
			}
		}

		public float cost
		{
			get
			{
				return 0f - score;
			}
		}

		public Domino(Wrapper placementWrapper, Vector3 offset)
		{
			this.placementWrapper = placementWrapper;
			this.offset = offset;
		}

		public Claim GetClaim(Vector3 pos)
		{
			return placement.GetClaimAt(pos - offset);
		}

		public Bounds GetBounds()
		{
			return new Bounds(placement.bounds.center + offset, placement.bounds.size);
		}

		public void Reset()
		{
			state = savedState;
		}

		public void SaveState()
		{
			savedState = state;
		}
	}
}

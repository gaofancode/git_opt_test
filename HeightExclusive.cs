using UnityEngine;

namespace Voxels
{
	[RequireComponent(typeof(ModuleSet))]
	public class HeightExclusive : SetRule
	{
		public class HeightExclusiveRule : Rule
		{
			public HeightExclusiveRule(SetRule setRule)
				: base(setRule)
			{
			}

			public override bool OnPlaced(Domino domino, MultiWave multiWave)
			{
				for (int i = 0; i < multiWave.allDominos.Count; i++)
				{
					Domino domino2 = multiWave.allDominos[i];
					if (domino2.state == Domino.State.idle && 
					    domino2.placementWrapper.rules.Contains(this) && 
					    domino2.offset.y == domino.offset.y && 
					    !multiWave.RemoveDomino(domino2))
					{
						return false;
					}
				}
				return true;
			}
		}

		public override Rule GetRule()
		{
			return new HeightExclusiveRule(this);
		}
	}
}

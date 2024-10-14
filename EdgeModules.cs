using UnityEngine;

namespace Voxels
{
	[RequireComponent(typeof(ModuleSet))]
	public class EdgeModules : ModuleProcessor
	{
		public int[] keys;

		public bool Fits(int key, int direction)
		{
			return keys[direction] == key;
		}

		public override void PostProcessModules(Module[] modules)
		{
			ModuleSet component = GetComponent<ModuleSet>();
			Module module = component.modules[0];
			Claim claim = module.orientations[0].claims[0];
			keys = new int[6];
			for (int i = 0; i < 6; i++)
			{
				keys[i] = claim.keys[i];
			}
		}
	}
}

using UnityEngine;

namespace Voxels
{
	public class ChildAssigner : ModuleProcessor
	{
		public override void PreProcessModules(Module[] modules)
		{
			base.PreProcessModules(modules);
			for (int num = base.transform.childCount - 1; num >= 0; num--)
			{
				Transform child = base.transform.GetChild(num);
				for (int i = 0; i < modules.Length; i++)
				{
					var module = modules[i];
					if (module.Contains(child.transform.position) && (bool)module.objectToCopy)
					{
						child.SetParent(module.objectToCopy.transform, true);
					}
				}
			}
		}
	}
}

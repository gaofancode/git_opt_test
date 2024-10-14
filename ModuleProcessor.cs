using UnityEngine;

namespace Voxels
{
	public class ModuleProcessor : MonoBehaviour
	{
		public virtual void PostProcessModules(Module[] modules)
		{
		}

		public virtual void PreProcessModules(Module[] modules)
		{
		}
	}
}

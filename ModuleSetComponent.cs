using UnityEngine;

namespace Voxels
{
	[RequireComponent(typeof(ModuleSet))]
	public abstract class ModuleSetComponent : MonoBehaviour
	{
		private ModuleSet _moduleSet;

		public ModuleSet moduleSet
		{
			get
			{
				if (_moduleSet == null)
				{
					_moduleSet = GetComponent<ModuleSet>();
				}
				return _moduleSet;
			}
		}
	}
}

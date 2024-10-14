using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
	public class ModuleSet : ModuleProcessor
	{
		public enum Mode
		{
			Enabled,
			Disabled
		}

		[Header("Editor")]
		public Color color = Color.white;

		public Mode defaultMode;

		[HideInInspector]
		[SerializeField]
		public string cachedName;

		[Space]
		public List<Bounds> bounds = new List<Bounds>
		{
			new Bounds(Vector3.zero, Vector3.one * 2f)
		};

		[Header("Runtime")]
		public List<Module> modules;

		public SetRule[] rules;

		public bool enabledByDefault
		{
			get
			{
				return defaultMode == Mode.Enabled;
			}
		}

		private void OnValidate()
		{
			base.transform.position = ExtraMath.Round(base.transform.position);
		}

		public override void PreProcessModules(Module[] modules)
		{
			base.PreProcessModules(modules);
			cachedName = base.name;
			rules = GetComponents<SetRule>();
			foreach (Module module in modules)
			{
				if (module.isNull)
				{
					continue;
				}
				bool flag = false;
				Matrix4x4 matrix4x = base.transform.worldToLocalMatrix * module.transform.localToWorldMatrix;
				for (int j = 0; j < module.cells.Count; j++)
				{
					Vector3 point = matrix4x.MultiplyPoint(module.cells[j].pos);
					for (int k = 0; k < bounds.Count; k++)
					{
						if (bounds[k].Contains(point))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					this.modules.Add(module);
					module.sets.Add(this);
					for (int l = 0; l < rules.Length; l++)
					{
						rules[l].OnPreProcess(module);
					}
				}
			}
		}

		public bool ContainsModule(Module module)
		{
			if (module.isNull)
			{
				return false;
			}
			return modules.Contains(module);
		}

		[ContextMenu("Center Pivot")]
		private void CenterPivot()
		{
			Bounds bounds = this.bounds[0];
			for (int i = 1; i < this.bounds.Count; i++)
			{
				bounds.Encapsulate(this.bounds[i]);
			}
			Vector3 vector = ExtraMath.Round(bounds.center);
			base.transform.position += vector;
			for (int j = 0; j < this.bounds.Count; j++)
			{
				this.bounds[j] = new Bounds(this.bounds[j].center - vector, this.bounds[j].size);
			}
		}
		
#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			if (UnityEditor.Selection.activeObject == gameObject)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = color * new Color(1f, 1f, 1f, 0.2f);
			}
			
			for (int i = 0; i < this.bounds.Count; i++)
			{
				Gizmos.DrawWireCube(this.bounds[i].center, this.bounds[i].extents * 2f - Vector3.one * 0.04f);
			}
			if (defaultMode == Mode.Disabled)
			{
				for (int j = 0; j < this.bounds.Count; j++)
				{
					Bounds bounds = this.bounds[j];
					Vector3 min = bounds.min;
					Vector3 to = bounds.max.SetY(min.y);
					Gizmos.DrawLine(min, to);
					Vector3 max = bounds.max;
					min.x = max.x;
					Vector3 min2 = bounds.min;
					to.x = min2.x;
					Gizmos.DrawLine(min, to);
				}
			}
		}
#endif
	}
}

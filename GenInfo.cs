namespace Voxels
{
	public struct GenInfo
	{
		public enum Mode
		{
			interruptable,
			nonInterupptable,
			forceInterrupt,
			broken
		}

		public string text;

		public Mode mode;

		public bool interruptable
		{
			get
			{
				return mode == Mode.interruptable;
			}
		}

		public bool nonInterupptable
		{
			get
			{
				return mode == Mode.nonInterupptable;
			}
		}

		public bool forceInterrupt
		{
			get
			{
				return mode == Mode.forceInterrupt;
			}
		}

		public bool broken
		{
			get
			{
				return mode == Mode.broken;
			}
		}

		public GenInfo(string text, Mode mode = Mode.interruptable)
		{
			this.text = text;
			this.mode = mode;
		}

		public override string ToString()
		{
			return string.Format("GenInfo - {0} ({1})", text, mode);
		}
	}
}

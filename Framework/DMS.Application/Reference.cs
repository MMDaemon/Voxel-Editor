using System;

namespace DMS.Application
{
	public class Reference<TYPE>
	{
		public event EventHandler<TYPE> Change;

		public TYPE Value
		{
			get { return value; }
			set { this.value = value; Change?.Invoke(this, value); }
		}

		public Reference(TYPE value)
		{
			Value = value;
		}

		protected TYPE value;
	}
}

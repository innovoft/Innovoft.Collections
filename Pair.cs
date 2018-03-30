using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	public class Pair<TKey, TValue>
	{
		#region Fields
		protected readonly TKey key;
		protected TValue value;
		#endregion //Fields

		#region Constructors
		public Pair(TKey key)
		{
			this.key = key;
		}

		public Pair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}

		public Pair(Pair<TKey, TValue> copy)
		{
			this.key = copy.key;
			this.value = copy.value;
		}
		#endregion //Constructors

		#region Properties
		public TKey Key => key;
		public TValue Value { get => this.value; set => this.value = value; }
		#endregion //Properties

		#region Methods
		public override string ToString()
		{
			return string.Join("|",	key, value);
		}
		#endregion //Methods
	}
}

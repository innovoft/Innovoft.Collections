using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	partial class RedBlackTree<TKey, TValue>
	{
		private sealed class Node
		{
			#region Fields
			private readonly TKey key;
			private TValue value;
			private bool red;
			private Node less;
			private Node more;
			#endregion Fields

			#region Constructors
			public Node(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}
			#endregion Constructors

			#region Properties
			public TKey Key => key;
			public TValue Value { get => this.value; set => this.value = value; }
			public bool Red { get => red; set => red = value; }
			public Node Less { get => less; set => less = value; }
			public Node More { get => more; set => more = value; }
			#endregion Properties
		}
	}
}

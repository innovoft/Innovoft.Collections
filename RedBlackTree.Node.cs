﻿using System;
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
			private Node parent;
			private Node less;
			private Node more;
			#endregion Fields

			#region Constructors
			/// <summary>
			/// Creates a Black root
			/// </summary>
			public Node(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}

			/// <summary>
			/// Creates a Red child
			/// </summary>
			public Node(TKey key, TValue value, Node parent)
			{
				this.key = key;
				this.value = value;
				this.red = true;
				this.parent = parent;
			}
			#endregion Constructors

			#region Properties
			public TKey Key => key;
			public TValue Value { get => this.value; set => this.value = value; }
			public bool Red { get => red; set => red = value; }
			public Node Parent { get => parent; set => parent = value; }
			public Node Less { get => less; set => less = value; }
			public Node More { get => more; set => more = value; }
			#endregion Properties

			#region Methods
			public override string ToString()
			{
				object lessKey;
				if (less != null)
				{
					lessKey = less.key;
				}
				else
				{
					lessKey = null;
				}
				object moreKey;
				if (more != null)
				{
					moreKey = more.key;
				}
				else
				{
					moreKey = null;
				}
				object parentKey;
				if (parent != null)
				{
					parentKey = parent.key;
				}
				else
				{
					parentKey = null;
				}
				return string.Join("|",	key, value, red ? "R" : "B", lessKey, moreKey, parentKey);
			}
			#endregion Methods
		}
	}
}

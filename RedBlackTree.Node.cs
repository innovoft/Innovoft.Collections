using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	partial class RedBlackTree<TKey, TValue>
	{
		public sealed class Node : Pair<TKey, TValue>
		{
			#region Fields
			private Node parent;
			private Node less;
			private Node more;
			private bool red;
			#endregion //Fields

			#region Constructors
			/// <summary>
			/// Construct NILL
			/// </summary>
			internal Node()
				: base(default(TKey), default(TValue))
			{
				this.parent = this;
				this.less = this;
				this.more = this;
				this.red = false;
			}

			internal Node(TKey key, TValue value, Node parent, Node less, Node more, bool red)
				: base(key, value)
			{
				this.parent = parent;
				this.less = less;
				this.more = more;
				this.red = red;
			}
			#endregion //Constructors

			#region Properties
			public Node Parent { get => parent; internal set => parent = value; }
			public Node Less { get => less; internal set => less = value; }
			public Node More { get => more; internal set => more = value; }
			public bool Red { get => red; internal set => red = value; }
			#endregion //Properties

			#region Methods
			#region Operators
			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			public static bool operator==(Node x, Node y)
			{
				return object.ReferenceEquals(x, y);
			}

			[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
			public static bool operator!=(Node x, Node y)
			{
				return !object.ReferenceEquals(x, y);
			}
			#endregion //Operators

			#region Object
#if DEBUG
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
				object direction;
				if (parent != null)
				{
					parentKey = parent.key;
					direction = parent.Less == this ? "L" : "M";
				}
				else
				{
					parentKey = null;
					direction = null;
				}
				return string.Join("|", key, value, red ? "R" : "B", lessKey, moreKey, parentKey, direction);
			}
#else //DEBUG
			public override string ToString()
			{
				return string.Join("|", key, value);
			}
#endif //DEBUG
			#endregion //Object
			#endregion //Methods
		}
	}
}

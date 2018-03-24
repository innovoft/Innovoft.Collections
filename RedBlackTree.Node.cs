using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	partial class RedBlackTree<TKey, TValue>
	{
		public sealed class Node : TreeNode<TKey, TValue, Node>
		{
			#region Fields
			private bool red;
			#endregion //Fields

			#region Constructors
			/// <summary>
			/// Creates a Black root
			/// </summary>
			internal Node(TKey key, TValue value)
				: base(key, value)
			{
			}

			/// <summary>
			/// Creates a Red child
			/// </summary>
			internal Node(TKey key, TValue value, Node parent)
				: base(key, value, parent)
			{
				this.red = true;
			}
			#endregion //Constructors

			#region Properties
			internal bool Red { get => red; set => red = value; }
			#endregion //Properties

			#region Methods
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
				return string.Join("|",	key, value, red ? "R" : "B", lessKey, moreKey, parentKey, direction);
			}
#endif //DEBUG
			#endregion //Methods
		}
	}
}

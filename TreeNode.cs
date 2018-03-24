using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	public class TreeNode<TKey, TValue, TNode> : Node<TKey, TValue>
		where TNode : TreeNode<TKey, TValue, TNode>
	{
		#region Fields
		protected TNode parent;
		protected TNode less;
		protected TNode more;
		#endregion //Fields

		#region Constructors
		internal TreeNode(TKey key, TValue value)
			: base(key, value)
		{
		}

		internal TreeNode(TKey key, TValue value, TNode parent)
			: base(key, value)
		{
			this.parent = parent;
		}

		internal TreeNode(TKey key, TValue value, TNode parent, TNode less, TNode more)
			: base(key, value)
		{
			this.parent = parent;
			this.less = less;
			this.more = more;
		}
		#endregion //Constructors

		#region Properties
		public TNode Parent { get => parent; set => parent = value; }
		public TNode Less { get => less; set => less = value; }
		public TNode More { get => more; set => more = value; }
		#endregion //Properties

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
			return string.Join("|",	key, value, lessKey, moreKey, parentKey, direction);
		}
		#endregion //Methods
	}
}

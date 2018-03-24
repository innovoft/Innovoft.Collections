using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	public class TreeNode<TKey, TValue> : Node<TKey, TValue>
	{
		#region Fields
		protected TreeNode<TKey, TValue> parent;
		protected TreeNode<TKey, TValue> less;
		protected TreeNode<TKey, TValue> more;
		#endregion //Fields

		#region Constructors
		internal TreeNode(TKey key, TValue value)
			: base(key, value)
		{
		}

		internal TreeNode(TKey key, TValue value, TreeNode<TKey, TValue> parent)
			: base(key, value)
		{
			this.parent = parent;
		}

		internal TreeNode(TKey key, TValue value, TreeNode<TKey, TValue> parent, TreeNode<TKey, TValue> less, TreeNode<TKey, TValue> more)
			: base(key, value)
		{
			this.parent = parent;
			this.less = less;
			this.more = more;
		}
		#endregion //Constructors

		#region Properties
		public TreeNode<TKey, TValue> Parent { get => parent; set => parent = value; }
		public TreeNode<TKey, TValue> Less { get => less; set => less = value; }
		public TreeNode<TKey, TValue> More { get => more; set => more = value; }
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

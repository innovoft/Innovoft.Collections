using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	partial class RedBlackTree<TKey, TValue>
	{
		public sealed class Node : Node<TKey, TValue>
		{
			#region Fields
			private Node parent;
			private Node less;
			private Node more;
			private bool red;
			#endregion //Fields

			#region Constructors
			/// <summary>
			/// Creates a Black root
			/// </summary>
			internal Node(TKey key)
				: base(key)
			{
			}

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
			internal Node(TKey key, Node parent)
				: base(key)
			{
				this.parent = parent;
				this.red = true;
			}

			/// <summary>
			/// Creates a Red child
			/// </summary>
			internal Node(TKey key, TValue value, Node parent)
				: base(key, value)
			{
				this.parent = parent;
				this.red = true;
			}
			#endregion //Constructors

			#region Properties
			public Node Parent { get => parent; internal set => parent = value; }
			public Node Less { get => less; internal set => less = value; }
			public Node More { get => more; internal set => more = value; }
			internal bool Red { get => red; set => red = value; }
			#endregion //Properties

			#region Methods
			#region Object
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
#if DEBUG
				return string.Join("|", key, value, red ? "R" : "B", lessKey, moreKey, parentKey, direction);
#else //DEBUG
				return string.Join("|",	key, value, lessKey, moreKey, parentKey, direction);
#endif //DEBUG
			}
			#endregion //Object

			public Node Next()
			{
				Node node;
				if (more != null)
				{
					node = more;
					while (true)
					{
						if (node.less != null)
						{
							node = node.less;
							continue;
						}

						break;
					}
					return node;
				}
				node = this;
				var parent = this.parent;
				while (true)
				{
					if (parent == null)
					{
						return null;
					}
					if (parent.more == node)
					{
						node = parent;
						parent = parent.Parent;
						continue;
					}
					return parent;
				}
			}

			public bool TryNext(out Node next)
			{
				Node node;
				if (more != null)
				{
					node = more;
					while (true)
					{
						if (node.less != null)
						{
							node = node.less;
							continue;
						}

						break;
					}
					next = node;
					return true;
				}
				node = this;
				var parent = this.parent;
				while (true)
				{
					if (parent == null)
					{
						next = null;
						return false;
					}
					if (parent.more == node)
					{
						node = parent;
						parent = parent.Parent;
						continue;
					}
					next = parent;
					return true;
				}
			}

			public Node Prev()
			{
				Node node;
				if (less != null)
				{
					node = less;
					while (true)
					{
						if (node.more != null)
						{
							node = node.more;
							continue;
						}

						break;
					}
					return node;
				}
				node = this;
				var parent = this.parent;
				while (true)
				{
					if (parent == null)
					{
						return null;
					}
					if (parent.less == node)
					{
						node = parent;
						parent = parent.Parent;
						continue;
					}
					return parent;
				}
			}

			public bool TryPrev(out Node prev)
			{
				Node node;
				if (less != null)
				{
					node = less;
					while (true)
					{
						if (node.more != null)
						{
							node = node.more;
							continue;
						}

						break;
					}
					prev = node;
					return true;
				}
				node = this;
				var parent = this.parent;
				while (true)
				{
					if (parent == null)
					{
						prev = null;
						return false;
					}
					if (parent.less == node)
					{
						node = parent;
						parent = parent.Parent;
						continue;
					}
					prev = parent;
					return true;
				}
			}
			#endregion //Methods
		}
	}
}

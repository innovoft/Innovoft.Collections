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
			public bool Red { get => red; internal set => red = value; }
			#endregion //Properties

			#region Methods
			#region Object
#if DEBUG
			public override string ToString()
			{
				var state = "";
				if (red)
				{
					if ((less == null) != (more == null))
					{
						state += "RC";
					}
				}
				else
				{
					if (less == null && more != null)
					{
						if (!more.Red)
						{
							state += "BMR";
						}
					}
					if (more == null && less != null)
					{
						state += "BML";
					}
					if (parent != null)
					{
						if (parent.less == null)
						{
							state += "BPL";
						}
						if (parent.more == null)
						{
							state += "BPM";
						}
					}
				}
				if (parent != null)
				{
					if (parent == this)
					{
						state += "PT";
					}
					if (parent == less)
					{
						state += "PL";
					}
					if (parent == more)
					{
						state += "PM";
					}
				}
				if (less != null)
				{
					if (red && less.red)
					{
						state += "RL";
					}
					if (less == this)
					{
						state += "LT";
					}
					if (less.parent != this)
					{
						state += "LPT";
					}
				}
				if (more != null)
				{
					if (red && more.red)
					{
						state += "RM";
					}
					if (more == this)
					{
						state += "MT";
					}
					if (more.parent != this)
					{
						state += "MPT";
					}
				}
				if (less != null && more != null)
				{
					if (less == more)
					{
						state += "CS";
					}
				}
				if (state.Length <= 0)
				{
					state = "G";
				}
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
				return string.Join("|", state, key, value, red ? "R" : "B", lessKey, moreKey, parentKey, direction);
			}
#endif //DEBUG
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
				while (true)
				{
					var parent = node.parent;
					if (parent == null)
					{
						return null;
					}
					if (parent.more == node)
					{
						node = parent;
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
				while (true)
				{
					var parent = node.parent;
					if (parent == null)
					{
						next = null;
						return false;
					}
					if (parent.more == node)
					{
						node = parent;
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
				while (true)
				{
					var parent = node.parent;
					if (parent == null)
					{
						return null;
					}
					if (parent.less == node)
					{
						node = parent;
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
				while (true)
				{
					var parent = node.parent;
					if (parent == null)
					{
						prev = null;
						return false;
					}
					if (parent.less == node)
					{
						node = parent;
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

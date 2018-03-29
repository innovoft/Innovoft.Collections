using System;
using System.Collections.Generic;
using System.Text;

namespace Innovoft.Collections
{
	partial class RedBlackNILLTree<TKey, TValue>
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

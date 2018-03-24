using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Innovoft.Collections
{
	public sealed partial class RedBlackTree<TKey, TValue>
	{
		#region Fields
		private readonly Comparison<TKey> comparer;

		private Node tree;
		private int count;
		#endregion //Fields

		#region Constructors
		public RedBlackTree(Comparison<TKey> comparer)
		{
			this.comparer = comparer;
		}
		#endregion //Constructors

		#region Properties
		public Comparison<TKey> Comparer => comparer;

		public int Count => count;
		#endregion //Properties

		#region Methods
		public void Clear()
		{
			tree = null;
			count = 0;
		}

		public void Add(TKey key, TValue value)
		{
			if (tree == null)
			{
				tree = new Node(key, value);
				count = 1;
				return;
			}

			var node = tree;
			var parent = default(Node);
			int compared;
			do
			{
				compared = comparer(key, node.Key);
				if (compared == 0)
				{
					throw new ArgumentException("key already exists", nameof(key));
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != null);
			node = new Node(key, value, parent);
			bool nodeDirection;
			if (compared < 0)
			{
				nodeDirection = true;
				parent.Less = node;
			}
			else
			{
				nodeDirection = false;
				parent.More = node;
			}
			++count;

			ResolveAdd(node, nodeDirection, parent);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			if (tree == null)
			{
				tree = new Node(key, value);
				count = 1;
				return true;
			}

			var node = tree;
			var parent = default(Node);
			int compared;
			do
			{
				compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return false;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != null);
			node = new Node(key, value, parent);
			bool nodeDirection;
			if (compared < 0)
			{
				nodeDirection = true;
				parent.Less = node;
			}
			else
			{
				nodeDirection = false;
				parent.More = node;
			}
			++count;

			ResolveAdd(node, nodeDirection, parent);

			return true;
		}

		public bool Set(TKey key, TValue value)
		{
			if (tree == null)
			{
				tree = new Node(key, value);
				count = 1;
				return true;
			}

			var node = tree;
			var parent = default(Node);
			int compared;
			do
			{
				compared = comparer(key, node.Key);
				if (compared == 0)
				{
					node.Value = value;
					return false;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != null);
			bool nodeDirection;
			if (compared < 0)
			{
				nodeDirection = true;
				parent.Less = node;
			}
			else
			{
				nodeDirection = false;
				parent.More = node;
			}
			++count;

			ResolveAdd(node, nodeDirection, parent);

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ResolveAdd(Node node, bool nodeDirection, Node parent)
		{
			while (true)
			{
				if (!parent.Red)
				{
					return;
				}

				var grand = parent.Parent;
				var great = grand.Parent;
				var parentDirection = grand.Less == parent;
				var uncle = parentDirection ? grand.More : grand.Less;

				if (uncle != null && uncle.Red)
				{
					uncle.Red = false;
					parent.Red = false;
					if (great != null)
					{
						grand.Red = true;
						node = grand;
						parent = grand.Parent;
						nodeDirection = parent.Less == node;
						continue;
					}
					else
					{
						return;
					}
				}
				else
				{
					if (parentDirection == nodeDirection)
					{
						if (parentDirection)
						{
							RotateMore(great, grand, parent);
						}
						else
						{
							RotateLess(great, grand, parent);
						}
					}
					else
					{
						if (parentDirection)
						{
							RotateLessMore(great, grand, parent, node);
						}
						else
						{
							RotateMoreLess(great, grand, parent, node);
						}
					}
					return;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateLess(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			grand.Parent = parent;
			var parentLess = parent.Less;
			grand.More = parentLess;
			if (parentLess != null)
			{
				parentLess.Parent = grand;
			}
			parent.Red = false;
			parent.Less = grand;
			parent.Parent = great;
			if (great != null)
			{
				if (grand == great.Less)
				{
					great.Less = parent;
				}
				else
				{
					great.More = parent;
				}
			}
			else
			{
				tree = parent;
			}

#if ASSERT
			Assert(tree);
#endif //ASSERT
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateMore(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			grand.Parent = parent;
			var parentMore = parent.More;
			grand.Less = parentMore;
			if (parentMore != null)
			{
				parentMore.Parent = grand;
			}
			parent.Red = false;
			parent.More = grand;
			parent.Parent = great;
			if (great != null)
			{
				if (grand == great.Less)
				{
					great.Less = parent;
				}
				else
				{
					great.More = parent;
				}
			}
			else
			{
				tree = parent;
			}

#if ASSERT
			Assert(tree);
#endif //ASSERT
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateLessMore(Node great, Node grand, Node parent, Node node)
		{
			grand.Red = true;
			grand.Parent = node;
			var nodeMore = node.More;
			grand.Less = nodeMore;
			if (nodeMore != null)
			{
				nodeMore.Parent = grand;
			}
			parent.Parent = node;
			var nodeLess = node.Less;
			parent.More = nodeLess;
			if (nodeLess != null)
			{
				nodeLess.Parent = parent;
			}
			node.Red = false;
			node.Less = parent;
			node.More = grand;
			node.Parent = great;
			if (great != null)
			{
				if (grand == great.Less)
				{
					great.Less = node;
				}
				else
				{
					great.More = node;
				}
			}
			else
			{
				tree = node;
			}

#if ASSERT
			Assert(tree);
#endif //ASSERT
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateMoreLess(Node great, Node grand, Node parent, Node node)
		{
			grand.Red = true;
			grand.Parent = node;
			var nodeLess = node.Less;
			grand.More = nodeLess;
			if (nodeLess != null)
			{
				nodeLess.Parent = grand;
			}
			parent.Parent = node;
			var nodeMore = node.More;
			parent.Less = nodeMore;
			if (nodeMore != null)
			{
				nodeMore.Parent = parent;
			}
			node.Red = false;
			node.Less = grand;
			node.More = parent;
			node.Parent = great;
			if (great != null)
			{
				if (grand == great.Less)
				{
					great.Less = node;
				}
				else
				{
					great.More = node;
				}
			}
			else
			{
				tree = node;
			}

#if ASSERT
			Assert(tree);
#endif //ASSERT
		}

#if ASSERT
		private void Assert(Node node)
		{
			if (node.Parent != null)
			{
				System.Diagnostics.Debug.Assert(node.Parent != node.Less);
				System.Diagnostics.Debug.Assert(node.Parent != node.More);
			}
			if (node.Less != null)
			{
				System.Diagnostics.Debug.Assert(node.Less.Parent == node);
				Assert(node.Less);
			}
			if (node.More != null)
			{
				System.Diagnostics.Debug.Assert(node.More.Parent == node);
				Assert(node.More);
			}
			if (node.Less != null && node.More != null)
			{
				System.Diagnostics.Debug.Assert(node.Less != node.More);
			}
		}
#endif //ASSERT

		#region Min
		public void GetMin(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return;
			}
		}

		public TKey GetMinKey()
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				return node.Key;
			}
		}

		public TValue GetMinValue()
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				return node.Value;
			}
		}

		public bool TryGetMin(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return true;
			}
		}

		public bool TryGetMin(out TKey key)
		{
			if (tree == null)
			{
				key = default(TKey);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				return true;
			}
		}

		public bool TryGetMin(out TValue value)
		{
			if (tree == null)
			{
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				value = node.Value;
				return true;
			}
		}
		#endregion //Min

		#region Max
		public void GetMax(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return;
			}
		}

		public TKey GetMaxKey()
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				return node.Key;
			}
		}

		public TValue GetMaxValue()
		{
			if (tree == null)
			{
				throw new InvalidOperationException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				return node.Value;
			}
		}

		public bool TryGetMax(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return true;
			}
		}

		public bool TryGetMax(out TKey key)
		{
			if (tree == null)
			{
				key = default(TKey);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				return true;
			}
		}

		public bool TryGetMax(out TValue value)
		{
			if (tree == null)
			{
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				value = node.Value;
				return true;
			}
		}
		#endregion //Max

		public bool ContainsKey(TKey key)
		{
			if (tree == null)
			{
				return false;
			}

			var node = tree;
			while (true)
			{
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return true;
				}
				node = compared < 0 ? node.Less : node.More;
				if (node == null)
				{
					return false;
				}
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (tree == null)
			{
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					value = node.Value;
					return true;
				}
				node = compared < 0 ? node.Less : node.More;
				if (node == null)
				{
					value = default(TValue);
					return false;
				}
			}
		}

		public void CopyKeysAscending(TKey[] values, int offset)
		{
			if (tree == null)
			{
				return;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				break;
			}
			while (true)
			{
				values[offset++] = node.Key;
				if (node.More != null)
				{
					node = node.More;
					while (true)
					{
						if (node.Less != null)
						{
							node = node.Less;
							continue;
						}

						break;
					}
					continue;
				}
				var parent = node.Parent;
				while (true)
				{
					if (parent == null)
					{
						return;
					}
					if (parent.More == node)
					{
						node = parent;
						parent = parent.Parent;
						continue;
					}
					break;
				}
				node = parent;
			}
		}
		#endregion //Methods
	}
}

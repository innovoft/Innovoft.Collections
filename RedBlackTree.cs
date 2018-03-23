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
		#endregion Fields

		#region Constructors
		public RedBlackTree(Comparison<TKey> comparer)
		{
			this.comparer = comparer;
		}
		#endregion Constructors

		#region Properties
		public Comparison<TKey> Comparer => comparer;

		public int Count => count;
		#endregion Properties

		#region Methods
		public void Clear()
		{
			tree = null;
			count = 0;
		}

		public void Add(TKey key, TValue value)
		{
			if (!TryAdd(key, value))
			{
				throw new Exception();
			}
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
			if (compared < 0)
			{
				parent.Less = node;
			}
			else
			{
				parent.More = node;
			}
			++count;

			ResolveAdd(node, parent);

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
			node = new Node(key, value, parent);
			if (compared < 0)
			{
				parent.Less = node;
			}
			else
			{
				parent.More = node;
			}
			++count;

			ResolveAdd(node, parent);

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ResolveAdd(Node node, Node parent)
		{
			while (true)
			{
				if (!parent.Red)
				{
					return;
				}

				var grand = parent.Parent;
				var great = grand.Parent;
				var parentCompared = grand.Less == parent;
				var uncle = parentCompared ? grand.More : grand.Less;

				if (uncle == null)
				{
					if (parentCompared)
					{
						RotateMore(great, grand, parent);
					}
					else
					{
						RotateLess(great, grand, parent);
					}
					return;
				}

				if (uncle.Red)
				{
					uncle.Red = false;
					parent.Red = false;
					if (great != null)
					{
						grand.Red = true;
						node = grand;
						parent = grand.Parent;
						continue;
					}
					else
					{
						return;
					}
				}

				throw new NotImplementedException();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateLess(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			parent.Red = false;
			grand.Parent = parent;
			grand.More = parent;
			parent.Less = grand;
			parent.Parent = great;
			if (great != null)
			{
				parent.Parent = great;
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RotateMore(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			parent.Red = false;
			grand.Parent = parent;
			grand.Less = parent;
			parent.More = grand;
			parent.Parent = great;
			if (great != null)
			{
				parent.Parent = great;
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
		}

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
		#endregion Min

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
		#endregion Max

		public bool Contains(TKey key)
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
		#endregion Methods
	}
}

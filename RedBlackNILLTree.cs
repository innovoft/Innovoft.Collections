using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Innovoft.Collections
{
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public sealed partial class RedBlackNILLTree<TKey, TValue>
	{
		#region Fields
		private readonly Comparison<TKey> comparer;
		private readonly Node nill;

		private Node tree;
		private int count;
		#endregion //Fields

		#region Constructors
		public RedBlackNILLTree(Comparison<TKey> comparer)
		{
			this.comparer = comparer;

			this.nill = new Node();

			this.tree = nill;
		}
		#endregion //Constructors

		#region Properties
		public Comparison<TKey> Comparer => comparer;
		public Node Tree => tree;

		public int Count => count;

		public IEnumerable<Node> Nodes => GetNodesAscendingEnumerable();
		public IEnumerable<Pair<TKey, TValue>> Pairs => GetPairsAscendingEnumerable();
		public IEnumerable<KeyValuePair<TKey, TValue>> KVPs => GetKVPsAscendingEnumerable();
		public IEnumerable<TKey> Keys => GetKeysAscendingEnumerable();
		public IEnumerable<TValue> Values => GetValuesAscendingEnumerable();
		#endregion //Properties

		#region Methods
		public void Clear()
		{
			tree = nill;
			count = 0;
		}

		#region Add
		public void Add(TKey key, TValue value)
		{
			if (count <= 0)
			{
				tree = new Node(key, value, nill, nill, nill, false);
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
			while (node != nill);
			node = new Node(key, value, parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}
		}

		public bool TryAdd(TKey key, TValue value)
		{
			if (count <= 0)
			{
				tree = new Node(key, value, nill, nill, nill, false);
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
			while (node != nill);
			node = new Node(key, value, parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return true;
		}

		public bool AddSet(TKey key, Action<bool, Node> value)
		{
			if (count <= 0)
			{
				tree = new Node(key, default(TValue), nill, nill, nill, false);
				count = 1;
				value(true, tree);
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
					value(false, node);
					return false;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != nill);
			node = new Node(key, default(TValue), parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}
			value(true, node);

			return true;
		}

		public bool AddSet(TKey key, Func<bool, TValue, TValue> value)
		{
			if (count <= 0)
			{
				tree = new Node(key, value(true, default(TValue)), nill, nill, nill, false);
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
					node.Value = value(false, node.Value);
					return false;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != nill);
			node = new Node(key, value(true, default(TValue)), parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return true;
		}

		public bool Set(TKey key, TValue value)
		{
			if (count <= 0)
			{
				tree = new Node(key, value, nill, nill, nill, false);
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
			while (node != nill);
			node = new Node(key, value, parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return true;
		}

		public TValue GetValue(TKey key, Func<TKey, TValue> create)
		{
			TValue value;

			if (count <= 0)
			{
				value = create(key);
				tree = new Node(key, value, nill, nill, nill, false);
				count = 1;
				return value;
			}

			var node = tree;
			var parent = default(Node);
			int compared;
			do
			{
				compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return node.Value;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != nill);
			value = create(key);
			node = new Node(key, value, parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return value;
		}

		public TValue GetValue(TKey key, Func<TValue> create)
		{
			TValue value;

			if (count <= 0)
			{
				value = create();
				tree = new Node(key, value, nill, nill, nill, false);
				count = 1;
				return value;
			}

			var node = tree;
			var parent = default(Node);
			int compared;
			do
			{
				compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return node.Value;
				}
				parent = node;
				node = compared < 0 ? node.Less : node.More;
			}
			while (node != nill);
			value = create();
			node = new Node(key, value, parent, nill, nill, true);
			if (compared < 0)
			{
				parent.Less = node;
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddResolve(Node node, bool nodeDirection, Node parent)
		{
			++count;

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

				if (uncle.Red)
				{
					uncle.Red = false;
					parent.Red = false;
					if (great != nill)
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
							AddRotateMore(great, grand, parent);
						}
						else
						{
							AddRotateLess(great, grand, parent);
						}
					}
					else
					{
						if (parentDirection)
						{
							AddRotateLessMore(great, grand, parent, node);
						}
						else
						{
							AddRotateMoreLess(great, grand, parent, node);
						}
					}
					return;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateLess(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			grand.Parent = parent;
			var parentLess = parent.Less;
			grand.More = parentLess;
			parentLess.Parent = grand;
			parent.Red = false;
			parent.Less = grand;
			parent.Parent = great;
			if (great != nill)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateMore(Node great, Node grand, Node parent)
		{
			grand.Red = true;
			grand.Parent = parent;
			var parentMore = parent.More;
			grand.Less = parentMore;
			parentMore.Parent = grand;
			parent.Red = false;
			parent.More = grand;
			parent.Parent = great;
			if (great != nill)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateLessMore(Node great, Node grand, Node parent, Node node)
		{
			grand.Red = true;
			grand.Parent = node;
			var nodeMore = node.More;
			grand.Less = nodeMore;
			nodeMore.Parent = grand;
			parent.Parent = node;
			var nodeLess = node.Less;
			parent.More = nodeLess;
			nodeLess.Parent = parent;
			node.Red = false;
			node.Less = parent;
			node.More = grand;
			node.Parent = great;
			if (great != nill)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateMoreLess(Node great, Node grand, Node parent, Node node)
		{
			grand.Red = true;
			grand.Parent = node;
			var nodeLess = node.Less;
			grand.More = nodeLess;
			nodeLess.Parent = grand;
			parent.Parent = node;
			var nodeMore = node.More;
			parent.Less = nodeMore;
			nodeMore.Parent = parent;
			node.Red = false;
			node.Less = grand;
			node.More = parent;
			node.Parent = great;
			if (great != nill)
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
		}
		#endregion //Add

		#region Remove
		public bool Remove(TKey key)
		{
			if (TryGetNode(key, out var node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, out Node node)
		{
			if (TryGetNode(key, out node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, out Pair<TKey, TValue> pair)
		{
			if (TryGetNode(key, out var node))
			{
				pair = new Pair<TKey, TValue>(node.Key, node.Value);
				RemoveResolve(node);
				return true;
			}
			else
			{
				pair = default(Pair<TKey, TValue>);
				return false;
			}
		}

		public bool Remove(TKey key, out TValue value)
		{
			if (TryGetNode(key, out var node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<Node> predicate)
		{
			if (TryGetNode(key, out var node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<Node> predicate, out Node node)
		{
			if (TryGetNode(key, out node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<Node> predicate, out TValue value)
		{
			if (TryGetNode(key, out var node) && predicate(node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<TValue> predicate)
		{
			if (TryGetNode(key, out var node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<TValue> predicate, out Node node)
		{
			if (TryGetNode(key, out node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(TKey key, Predicate<TValue> predicate, out TValue value)
		{
			if (TryGetNode(key, out var node) && predicate(node.Value))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin()
		{
			if (TryGetMinNode(out var node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(out Node node)
		{
			if (TryGetMinNode(out node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(out TKey key, out TValue value)
		{
			if (TryGetMinNode(out var node))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(out TKey key)
		{
			if (TryGetMinNode(out var node))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMin(out TValue value)
		{
			if (TryGetMinNode(out var node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<Node> predicate)
		{
			if (TryGetMinNode(out var node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<Node> predicate, out Node node)
		{
			if (TryGetMinNode(out node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<Node> predicate, out TKey key, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<Node> predicate, out TKey key)
		{
			if (TryGetMinNode(out var node) && predicate(node))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMin(Predicate<Node> predicate, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TKey> predicate)
		{
			if (TryGetMinNode(out var node) && predicate(node.Key))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<TKey> predicate, out Node node)
		{
			if (TryGetMinNode(out node) && predicate(node.Key))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<TKey> predicate, out TKey key, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node.Key))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TKey> predicate, out TKey key)
		{
			if (TryGetMinNode(out var node) && predicate(node.Key))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TKey> predicate, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node.Key))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TValue> predicate)
		{
			if (TryGetMinNode(out var node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<TValue> predicate, out Node node)
		{
			if (TryGetMinNode(out node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMin(Predicate<TValue> predicate, out TKey key, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node.Value))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TValue> predicate, out TKey key)
		{
			if (TryGetMinNode(out var node) && predicate(node.Value))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMin(Predicate<TValue> predicate, out TValue value)
		{
			if (TryGetMinNode(out var node) && predicate(node.Value))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax()
		{
			if (TryGetMaxNode(out var node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(out Node node)
		{
			if (TryGetMaxNode(out node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(out TKey key, out TValue value)
		{
			if (TryGetMaxNode(out var node))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(out TKey key)
		{
			if (TryGetMaxNode(out var node))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMax(out TValue value)
		{
			if (TryGetMaxNode(out var node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<Node> predicate)
		{
			if (TryGetMaxNode(out var node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<Node> predicate, out Node node)
		{
			if (TryGetMaxNode(out node) && predicate(node))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<Node> predicate, out TKey key, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<Node> predicate, out TKey key)
		{
			if (TryGetMaxNode(out var node) && predicate(node))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMax(Predicate<Node> predicate, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TKey> predicate)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Key))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<TKey> predicate, out Node node)
		{
			if (TryGetMaxNode(out node) && predicate(node.Key))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<TKey> predicate, out TKey key, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Key))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TKey> predicate, out TKey key)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Key))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TKey> predicate, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Key))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TValue> predicate)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<TValue> predicate, out Node node)
		{
			if (TryGetMaxNode(out node) && predicate(node.Value))
			{
				RemoveResolve(node);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RemoveMax(Predicate<TValue> predicate, out TKey key, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Value))
			{
				key = node.Key;
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TValue> predicate, out TKey key)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Value))
			{
				key = node.Key;
				RemoveResolve(node);
				return true;
			}
			else
			{
				key = default(TKey);
				return false;
			}
		}

		public bool RemoveMax(Predicate<TValue> predicate, out TValue value)
		{
			if (TryGetMaxNode(out var node) && predicate(node.Value))
			{
				value = node.Value;
				RemoveResolve(node);
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveResolve(Node node)
		{
			--count;

			Node work;
			bool red;

			if (node.Less == nill)
			{
				red = node.Red;
				work = node.More;
				RemoveReplace(node, work);
			}
			else if (node.More == nill)
			{
				red = node.Red;
				work = node.Less;
				RemoveReplace(node, work);
			}
			else
			{
				var next = node.More;
				while (next.Less != nill)
				{
					next = next.Less;
				}
				red = next.Red;
				work = next.More;
				if (next.Parent == node)
				{
					work.Parent = next;
				}
				else
				{
					RemoveReplace(next, next.More);
					next.More = node.More;
					next.More.Parent = next;
				}
				RemoveReplace(node, next);
				next.Less = node.Less;
				next.Less.Parent = next;
				next.Red = node.Red;
			}

			if (red)
			{
				return;
			}

			while (work != tree && !work.Red)
			{
				if (work == work.Parent.Less)
				{
					var sibling = work.Parent.More;
					if (sibling.Red)
					{
						sibling.Red = false;
						work.Parent.Red = true;
						RemoveRotateLess(work.Parent);
						sibling = work.Parent.More;
					}
					if (!sibling.Less.Red && !sibling.More.Red)
					{
						sibling.Red = true;
						work = work.Parent;
					}
					else
					{
						if (!sibling.More.Red)
						{
							sibling.Less.Red = false;
							sibling.Red = true;
							RemoveRotateMore(sibling);
							sibling = work.Parent.More;
						}
						sibling.Red = work.Parent.Red;
						work.Parent.Red = false;
						sibling.More.Red = false;
						RemoveRotateLess(work.Parent);
						work = tree;
					}
				}
				else
				{
					var sibling = work.Parent.Less;
					if (sibling.Red)
					{
						sibling.Red = false;
						work.Parent.Red = true;
						RemoveRotateMore(work.Parent);
						sibling = work.Parent.Less;
					}
					if (!sibling.More.Red && !sibling.Less.Red)
					{
						sibling.Red = true;
						work = work.Parent;
					}
					else
					{
						if (!sibling.Less.Red)
						{
							sibling.More.Red = false;
							sibling.Red = true;
							RemoveRotateLess(sibling);
							sibling = work.Parent.Less;
						}
						sibling.Red = work.Parent.Red;
						work.Parent.Red = false;
						sibling.Less.Red = false;
						RemoveRotateMore(work.Parent);
						work = tree;
					}
				}
			}
			work.Red = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveReplace(Node ing, Node ment)
		{
			var parent = ing.Parent;
			ment.Parent = parent;
			if (parent != nill)
			{
				if (parent.Less == ing)
				{
					parent.Less = ment;
				}
				else
				{
					parent.More = ment;
				}
			}
			else
			{
				tree = ment;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveRotateLess(Node node)
		{
			var temp = node.More;
			node.More = temp.Less;
			if (temp.Less != nill)
			{
				temp.Less.Parent = node;
			}
			temp.Parent = node.Parent;
			if (node.Parent != nill)
			{
				if (node.Parent.Less == node)
				{
					node.Parent.Less = temp;
				}
				else
				{
					node.Parent.More = temp;
				}
			}
			else
			{
				tree = temp;
			}
			temp.Less = node;
			node.Parent = temp;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveRotateMore(Node node)
		{
			var temp = node.Less;
			node.Less = temp.More;
			if (temp.More != nill)
			{
				temp.More.Parent = node;
			}
			temp.Parent = node.Parent;
			if (node.Parent != nill)
			{
				if (node.Parent.More == node)
				{
					node.Parent.More = temp;
				}
				else
				{
					node.Parent.Less = temp;
				}
			}
			else
			{
				tree = temp;
			}
			temp.More = node;
			node.Parent = temp;
		}
		#endregion //Remove

		#region Min
		public void GetMin(out TKey key, out TValue value)
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return;
			}
		}

		public Node GetMinNode()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				return node;
			}
		}

		public TKey GetMinKey()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				return node.Key;
			}
		}

		public TValue GetMinValue()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				return node.Value;
			}
		}

		public bool TryGetMin(out TKey key, out TValue value)
		{
			if (count <= 0)
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return true;
			}
		}

		public bool TryGetMinNode(out Node node)
		{
			if (count <= 0)
			{
				node = nill;
				return false;
			}

			var crnt = tree;
			while (true)
			{
				if (crnt.Less != nill)
				{
					crnt = crnt.Less;
					continue;
				}

				node = crnt;
				return true;
			}
		}

		public bool TryGetMinKey(out TKey key)
		{
			if (count <= 0)
			{
				key = default(TKey);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					continue;
				}

				key = node.Key;
				return true;
			}
		}

		public bool TryGetMinValue(out TValue value)
		{
			if (count <= 0)
			{
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.Less != nill)
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
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return;
			}
		}

		public Node GetMaxNode()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				return node;
			}
		}

		public TKey GetMaxKey()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				return node.Key;
			}
		}

		public TValue GetMaxValue()
		{
			if (count <= 0)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				return node.Value;
			}
		}

		public bool TryGetMax(out TKey key, out TValue value)
		{
			if (count <= 0)
			{
				key = default(TKey);
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				value = node.Value;
				return true;
			}
		}

		public bool TryGetMaxNode(out Node node)
		{
			if (count <= 0)
			{
				node = nill;
				return false;
			}

			var crnt = tree;
			while (true)
			{
				if (crnt.More != nill)
				{
					crnt = crnt.More;
					continue;
				}

				node = crnt;
				return true;
			}
		}

		public bool TryGetMaxKey(out TKey key)
		{
			if (count <= 0)
			{
				key = default(TKey);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
				{
					node = node.More;
					continue;
				}

				key = node.Key;
				return true;
			}
		}

		public bool TryGetMaxValue(out TValue value)
		{
			if (count <= 0)
			{
				value = default(TValue);
				return false;
			}

			var node = tree;
			while (true)
			{
				if (node.More != nill)
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
			var node = tree;
			while (true)
			{
				if (node == nill)
				{
					return false;
				}
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return true;
				}
				node = compared < 0 ? node.Less : node.More;
			}
		}

		#region Get
		public Node GetNodeFromMin(int i)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt;
				}
				if (!TryNext(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetNodeFromMin(int i, out Node node)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					node = crnt;
					return true;
				}
				if (!TryNext(crnt, out crnt))
				{
					node = nill;
					return false;
				}
			}
		}

		public TKey GetKeyFromMin(int i)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt.Key;
				}
				if (!TryNext(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetKeyFromMin(int i, out TKey key)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					key = crnt.Key;
					return true;
				}
				if (!TryNext(crnt, out crnt))
				{
					key = default(TKey);
					return false;
				}
			}
		}

		public TValue GetValueFromMin(int i)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt.Value;
				}
				if (!TryNext(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetValueFromMin(int i, out TValue value)
		{
			for (var crnt = GetMinNode(); ; --i)
			{
				if (i == 0)
				{
					value = crnt.Value;
					return true;
				}
				if (!TryNext(crnt, out crnt))
				{
					value = default(TValue);
					return false;
				}
			}
		}

		public Node GetNodeFromMax(int i)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt;
				}
				if (!TryPrev(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetNodeFromMax(int i, out Node node)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					node = crnt;
					return true;
				}
				if (!TryPrev(crnt, out crnt))
				{
					node = nill;
					return false;
				}
			}
		}

		public TKey GetKeyFromMax(int i)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt.Key;
				}
				if (!TryPrev(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetKeyFromMax(int i, out TKey key)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					key = crnt.Key;
					return true;
				}
				if (!TryPrev(crnt, out crnt))
				{
					key = default(TKey);
					return false;
				}
			}
		}

		public TValue GetValueFromMax(int i)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					return crnt.Value;
				}
				if (!TryPrev(crnt, out crnt))
				{
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool TryGetValueFromMax(int i, out TValue value)
		{
			for (var crnt = GetMaxNode(); ; --i)
			{
				if (i == 0)
				{
					value = crnt.Value;
					return true;
				}
				if (!TryPrev(crnt, out crnt))
				{
					value = default(TValue);
					return false;
				}
			}
		}

		public TValue GetValue(TKey key)
		{
			var node = tree;
			while (true)
			{
				if (node == nill)
				{
					throw new KeyNotFoundException(key.ToString());
				}
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return node.Value;
				}
				node = compared < 0 ? node.Less : node.More;
			}
		}

		public Node GetNode(TKey key)
		{
			var node = tree;
			while (true)
			{
				if (node == nill)
				{
					throw new KeyNotFoundException(key.ToString());
				}
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					return node;
				}
				node = compared < 0 ? node.Less : node.More;
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var node = tree;
			while (true)
			{
				if (node == nill)
				{
					value = default(TValue);
					return false;
				}
				var compared = comparer(key, node.Key);
				if (compared == 0)
				{
					value = node.Value;
					return true;
				}
				node = compared < 0 ? node.Less : node.More;
			}
		}

		public bool TryGetNode(TKey key, out Node node)
		{
			var crnt = tree;
			while (true)
			{
				if (crnt == nill)
				{
					node = nill;
					return false;
				}
				var compared = comparer(key, crnt.Key);
				if (compared == 0)
				{
					node = crnt;
					return true;
				}
				crnt = compared < 0 ? crnt.Less : crnt.More;
			}
		}
		#endregion //Get

		public int Height()
		{
			if (count <= 0)
			{
				return 0;
			}

			var height = 1;
			var max = 1;
			var node = tree;
			while (true)
			{
				if (node.Less != nill)
				{
					node = node.Less;
					++height;
					continue;
				}
				if (node.More != nill)
				{
					node = node.More;
					++height;
					continue;
				}

				if (height > max)
				{
					max = height;
				}

				while (true)
				{
					var parent = node.Parent;
					if (parent == nill)
					{
						return max;
					}

					var more = parent.More;
					if (more != nill && more != node)
					{
						node = more;
						break;
					}
					else
					{

						node = node.Parent;
						--height;
						continue;
					}
				}
			}
		}

		#region Copy
		public void CopyAscending(out TKey[] keys, out TValue[] values)
		{
			keys = new TKey[count];
			values = new TValue[count];
			CopyAscending(keys, values, 0);
		}

		public Node[] CopyNodesAscending()
		{
			var nodes = new Node[count];
			CopyNodesAscending(nodes, 0);
			return nodes;
		}

		public Pair<TKey, TValue>[] CopyPairsAscending()
		{
			var nodes = new Pair<TKey, TValue>[count];
			CopyPairsAscending(nodes, 0);
			return nodes;
		}

		public KeyValuePair<TKey, TValue>[] CopyKVPsAscending()
		{
			var nodes = new KeyValuePair<TKey, TValue>[count];
			CopyKVPsAscending(nodes, 0);
			return nodes;
		}

		public TKey[] CopyKeysAscending()
		{
			var keys = new TKey[count];
			CopyKeysAscending(keys, 0);
			return keys;
		}

		public TValue[] CopyValuesAscending()
		{
			var values = new TValue[count];
			CopyValuesAscending(values, 0);
			return values;
		}

		public void CopyAscending(TKey[] keys, TValue[] values, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys[offset] = node.Key;
				values[offset] = node.Value;
				++offset;
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(Node[] nodes, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = node;
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsAscending(Pair<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new Pair<TKey, TValue>(node);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsAscending(KeyValuePair<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysAscending(TKey[] keys, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys[offset++] = node.Key;
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesAscending(TValue[] values, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				values[offset++] = node.Value;
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(ICollection<Node> nodes)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(node);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsAscending(ICollection<Pair<TKey, TValue>> nodes)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new Pair<TKey, TValue>(node));
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsAscending(ICollection<KeyValuePair<TKey, TValue>> nodes)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysAscending(ICollection<TKey> keys)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys.Add(node.Key);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesAscending(ICollection<TValue> values)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				values.Add(node.Value);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(Action<Node> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsAscending(Action<Pair<TKey, TValue>> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new Pair<TKey, TValue>(node));
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsAscending(Action<KeyValuePair<TKey, TValue>> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysAscending(Action<TKey> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node.Key);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesAscending(Action<TValue> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node.Value);
				if (!TryNext(node, out node))
				{
					return;
				}
			}
		}

		public void CopyDescending(out TKey[] keys, out TValue[] values)
		{
			keys = new TKey[count];
			values = new TValue[count];
			CopyDescending(keys, values, 0);
		}

		public Node[] CopyNodesDescending()
		{
			var nodes = new Node[count];
			CopyNodesDescending(nodes, 0);
			return nodes;
		}

		public Pair<TKey, TValue>[] CopyPairsDescending()
		{
			var nodes = new Pair<TKey, TValue>[count];
			CopyPairsDescending(nodes, 0);
			return nodes;
		}

		public KeyValuePair<TKey, TValue>[] CopyKVPsDescending()
		{
			var nodes = new KeyValuePair<TKey, TValue>[count];
			CopyKVPsDescending(nodes, 0);
			return nodes;
		}

		public TKey[] CopyKeysDescending()
		{
			var keys = new TKey[count];
			CopyKeysDescending(keys, 0);
			return keys;
		}

		public TValue[] CopyValuesDescending()
		{
			var values = new TValue[count];
			CopyValuesDescending(values, 0);
			return values;
		}

		public void CopyDescending(TKey[] keys, TValue[] values, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys[offset] = node.Key;
				values[offset] = node.Value;
				++offset;
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(Node[] nodes, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = node;
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsDescending(Pair<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new Pair<TKey, TValue>(node);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsDescending(KeyValuePair<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysDescending(TKey[] keys, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys[offset++] = node.Key;
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesDescending(TValue[] values, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				values[offset++] = node.Value;
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(ICollection<Node> nodes)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(node);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsDescending(ICollection<Pair<TKey, TValue>> nodes)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new Pair<TKey, TValue>(node));
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsDescending(ICollection<KeyValuePair<TKey, TValue>> nodes)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysDescending(ICollection<TKey> keys)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				keys.Add(node.Key);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesDescending(ICollection<TValue> values)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				values.Add(node.Value);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(Action<Node> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyPairsDescending(Action<Pair<TKey, TValue>> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new Pair<TKey, TValue>(node));
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKVPsDescending(Action<KeyValuePair<TKey, TValue>> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyKeysDescending(Action<TKey> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node.Key);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}

		public void CopyValuesDescending(Action<TValue> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node.Value);
				if (!TryPrev(node, out node))
				{
					return;
				}
			}
		}
		#endregion //Copy

		#region Enumerable
		public IEnumerable<Node> GetNodesAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node;
				if (!TryNext(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<Pair<TKey, TValue>> GetPairsAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return new Pair<TKey, TValue>(node);
				if (!TryNext(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetKVPsAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
				if (!TryNext(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<TKey> GetKeysAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node.Key;
				if (!TryNext(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<TValue> GetValuesAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node.Value;
				if (!TryNext(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<Node> GetNodesDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node;
				if (!TryPrev(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<Pair<TKey, TValue>> GetPairsDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return new Pair<TKey, TValue>(node);
				if (!TryPrev(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetKVPsDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
				if (!TryPrev(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<TKey> GetKeysDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node.Key;
				if (!TryPrev(node, out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<TValue> GetValuesDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node.Value;
				if (!TryPrev(node, out node))
				{
					yield break;
				}
			}
		}
		#endregion //Enumerable

		#region Node
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Terminal(Node node)
		{
			return node == nill;
		}

		public Node Next(Node node)
		{
			if (node.More != nill)
			{
				node = node.More;
				while (true)
				{
					if (node.Less != nill)
					{
						node = node.Less;
						continue;
					}

					break;
				}
				return node;
			}
			while (true)
			{
				var parent = node.Parent;
				if (parent == nill)
				{
					return nill;
				}
				if (parent.More == node)
				{
					node = parent;
					continue;
				}
				return parent;
			}
		}

		public bool TryNext(Node node, out Node next)
		{
			if (node.More != nill)
			{
				node = node.More;
				while (true)
				{
					if (node.Less != nill)
					{
						node = node.Less;
						continue;
					}

					break;
				}
				next = node;
				return true;
			}
			while (true)
			{
				var parent = node.Parent;
				if (parent == nill)
				{
					next = nill;
					return false;
				}
				if (parent.More == node)
				{
					node = parent;
					continue;
				}
				next = parent;
				return true;
			}
		}

		public Node Prev(Node node)
		{
			if (node.Less != nill)
			{
				node = node.Less;
				while (true)
				{
					if (node.More != nill)
					{
						node = node.More;
						continue;
					}

					break;
				}
				return node;
			}
			while (true)
			{
				var parent = node.Parent;
				if (parent == nill)
				{
					return nill;
				}
				if (parent.Less == node)
				{
					node = parent;
					continue;
				}
				return parent;
			}
		}

		public bool TryPrev(Node node, out Node prev)
		{
			if (node.Less != nill)
			{
				node = node.Less;
				while (true)
				{
					if (node.More != nill)
					{
						node = node.More;
						continue;
					}

					break;
				}
				prev = node;
				return true;
			}
			while (true)
			{
				var parent = node.Parent;
				if (parent == nill)
				{
					prev = nill;
					return false;
				}
				if (parent.Less == node)
				{
					node = parent;
					continue;
				}
				prev = parent;
				return true;
			}
		}
		#endregion //Node
		#endregion //Methods
	}
}

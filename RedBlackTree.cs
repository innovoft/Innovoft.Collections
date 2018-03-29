using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Innovoft.Collections
{
	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
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
		public Node Tree => tree;

		public int Count => count;
		#endregion //Properties

		#region Methods
		public void Clear()
		{
			tree = null;
			count = 0;
		}

		#region Add
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
			if (tree == null)
			{
				tree = new Node(key);
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
			while (node != null);
			node = new Node(key, parent);
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
			if (tree == null)
			{
				tree = new Node(key, value(true, default(TValue)));
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
			while (node != null);
			node = new Node(key, value(true, default(TValue)), parent);
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
				AddResolve(node, true, parent);
			}
			else
			{
				parent.More = node;
				AddResolve(node, false, parent);
			}

			return true;
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateMore(Node great, Node grand, Node parent)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateLessMore(Node great, Node grand, Node parent, Node node)
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
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddRotateMoreLess(Node great, Node grand, Node parent, Node node)
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
			Node parent;
			bool workDirection;
			Node sibling;

			if (node.Less == null && node.More == null)
			{
				parent = node.Parent;
				if (parent == null)
				{
					tree = null;
					return;
				}
				if (node.Red)
				{
					if (parent.Less == node)
					{
						parent.Less = null;
					}
					else
					{
						parent.More = null;
					}
					return;
				}
				if (parent.Red)
				{
					if (parent.Less == node)
					{
						parent.Less = null;
						sibling = parent.More;
						var nephew = sibling.Less;
						if (nephew == null)
						{
							if (sibling.More == null)
							{
								parent.Red = false;
								sibling.Red = true;
								return;
							}
							else
							{
								RemoveRotateLess(parent.Parent, parent, sibling);
								return;
							}
						}
						else
						{
							var grand = parent.Parent;
							nephew.Parent = grand;
							if (grand != null)
							{
								if (parent == grand.Less)
								{
									grand.Less = nephew;
								}
								else
								{
									grand.More = nephew;
								}
							}
							else
							{
								tree = nephew;
							}
							nephew.More = sibling;
							nephew.Less = parent;
							parent.Red = false;
							parent.Parent = nephew;
							parent.More = null;
							sibling.Parent = nephew;
							sibling.Less = null;
							return;
						}
					}
					else
					{
						parent.More = null;
						sibling = parent.Less;
						var nephew = sibling.More;
						if (nephew == null)
						{
							if (sibling.Less == null)
							{
								parent.Red = false;
								sibling.Red = true;
								return;
							}
							else
							{
								RemoveRotateMore(parent.Parent, parent, sibling);
								return;
							}
						}
						else
						{
							var grand = parent.Parent;
							nephew.Parent = grand;
							if (grand != null)
							{
								if (parent == grand.Less)
								{
									grand.Less = nephew;
								}
								else
								{
									grand.More = nephew;
								}
							}
							else
							{
								tree = nephew;
							}
							nephew.Less = sibling;
							nephew.More = parent;
							parent.Red = false;
							parent.Parent = nephew;
							parent.Less = null;
							sibling.Parent = nephew;
							sibling.More = null;
							return;
						}
					}
				}
				if (parent.Less == node)
				{
					parent.Less = null;
					sibling = parent.More;
					if (sibling.Red)
					{
						sibling.Red = false;
						var nephew = sibling.Less;
						if (nephew.Less == null && nephew.More == null)
						{
							nephew.Red = false;
							RemoveRotateLess(parent.Parent, parent, sibling);
							return;
						}
						else
						{
							var grandParent = parent.Parent;
							sibling.Parent = grandParent;
							if (grandParent != null)
							{
								if (grandParent.Less == parent)
								{
									grandParent.Less = sibling;
								}
								else
								{
									grandParent.More = sibling;
								}
							}
							else
							{
								tree = sibling;
							}
							parent.More = null;
							var grandNephew = nephew.Less;
							if (grandNephew == null)
							{
								parent.Parent = nephew;
								nephew.Less = parent;
								nephew.Red = true;
								nephew.More.Red = false;
								return;
							}
							else
							{
								parent.Parent = grandNephew;
								sibling.Less = grandNephew;
								nephew.Parent = grandNephew;
								nephew.Less = null;
								grandNephew.Parent = sibling;
								grandNephew.Less = parent;
								grandNephew.More = nephew;
								return;
							}
						}
					}
					else
					{
						sibling.Red = true;
						work = parent;
						parent = parent.Parent;
						if (parent == null)
						{
							return;
						}
						workDirection = parent.Less == work;
					}
				}
				else
				{
					parent.More = null;
					sibling = parent.Less;
					if (sibling.Red)
					{
						sibling.Red = false;
						var nephew = sibling.More;
						if (nephew.Less == null && nephew.More == null)
						{
							nephew.Red = false;
							RemoveRotateMore(parent.Parent, parent, sibling);
							return;
						}
						else
						{
							var grandParent = parent.Parent;
							sibling.Parent = grandParent;
							if (grandParent != null)
							{
								if (grandParent.Less == parent)
								{
									grandParent.Less = sibling;
								}
								else
								{
									grandParent.More = sibling;
								}
							}
							else
							{
								tree = sibling;
							}
							parent.Less = null;
							var grandNephew = nephew.More;
							if (grandNephew == null)
							{
								parent.Parent = nephew;
								nephew.More = parent;
								nephew.Red = true;
								nephew.Less.Red = false;
								return;
							}
							else
							{
								parent.Parent = grandNephew;
								sibling.More = grandNephew;
								nephew.Parent = grandNephew;
								nephew.More = null;
								grandNephew.Parent = sibling;
								grandNephew.More = parent;
								grandNephew.Less = nephew;
								return;
							}
						}
					}
					else
					{
						sibling.Red = true;
						work = parent;
						parent = parent.Parent;
						if (parent == null)
						{
							return;
						}
						workDirection = parent.Less == work;
					}
				}
			}
			else
			{
				bool red;
				if (node.Less == null)
				{
					red = node.Red;
					work = node.More;
					RemoveReplace(node, node.More);
				}
				else if (node.More == null)
				{
					red = node.Red;
					work = node.Less;
					RemoveReplace(node, node.Less);
				}
				else
				{
					var next = node.More;
					while (next.Less != null)
					{
						next = next.Less;
					}
					red = next.Red;
					work = next.More;
					if (next.Parent == node)
					{
						if (work != null)
						{
							work.Parent = next;
						}
					}
					else
					{
						RemoveReplace(next, work);
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
				if (work.Red)
				{
					work.Red = false;
					return;
				}
				parent = work.Parent;
				if (parent == null)
				{
					return;
				}
				workDirection = false;
			}

			while (true)
			{
				if (workDirection)
				{
					sibling = parent.More;
					if (sibling.Red)
					{
						sibling.Red = false;
						parent.Red = true;
						RemoveRotateLess(parent.Parent, parent, sibling);
						//sibling = parent.More;
					}
					if ((sibling.Less == null || !sibling.Less.Red) && (sibling.More == null || !sibling.More.Red))
					{
						sibling.Red = true;
						work = parent;
						if (work.Red)
						{
							work.Red = false;
							return;
						}
						parent = work.Parent;
						if (parent == null)
						{
							return;
						}
						workDirection = parent.Less == work;
					}
					else
					{
						if (sibling.More == null || !sibling.More.Red)
						{
							sibling.Less.Red = false;
							sibling.Red = true;
							RemoveRotateMore(parent.Parent.Parent, parent.Parent, parent);
							sibling = parent.More;
						}
						sibling.Red = parent.Red;
						parent.Red = false;
						sibling.More.Red = false;
						RemoveRotateLess(parent.Parent.Parent, parent.Parent, parent);
						work.Red = false;
						return;
					}
				}
				else
				{
					sibling = parent.Less;
					if (sibling.Red)
					{
						sibling.Red = false;
						parent.Red = true;
						RemoveRotateMore(parent.Parent, parent, sibling);
						//sibling = parent.Less;
					}
					if ((sibling.Less == null || !sibling.Less.Red) && (sibling.More == null || !sibling.More.Red))
					{
						sibling.Red = true;
						work = parent;
						if (work.Red)
						{
							work.Red = false;
							return;
						}
						parent = work.Parent;
						if (parent == null)
						{
							return;
						}
						workDirection = parent.Less == work;
					}
					else
					{
						if (sibling.Less == null || !sibling.Less.Red)
						{
							sibling.More.Red = false;
							sibling.Red = true;
							RemoveRotateLess(parent.Parent.Parent, parent.Parent, parent);
							sibling = parent.More;
						}
						sibling.Red = parent.Red;
						parent.Red = false;
						sibling.Less.Red = false;
						RemoveRotateMore(parent.Parent.Parent, parent.Parent, parent);
						work.Red = false;
						return;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveReplace(Node ing, Node ment)
		{
			var parent = ing.Parent;
			ment.Parent = parent;
			if (parent != null)
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
		private void RemoveRotateLess(Node grand, Node parent, Node node)
		{
			parent.Parent = node;
			var parentLess = node.Less;
			parent.More = parentLess;
			if (parentLess != null)
			{
				parentLess.Parent = parent;
			}
			node.Less = parent;
			node.Parent = grand;
			if (grand != null)
			{
				if (parent == grand.Less)
				{
					grand.Less = node;
				}
				else
				{
					grand.More = node;
				}
			}
			else
			{
				tree = node;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RemoveRotateMore(Node grand, Node parent, Node node)
		{
			parent.Parent = node;
			var parentMore = node.More;
			parent.Less = parentMore;
			if (parentMore != null)
			{
				parentMore.Parent = parent;
			}
			node.More = parent;
			node.Parent = grand;
			if (grand != null)
			{
				if (parent == grand.Less)
				{
					grand.Less = node;
				}
				else
				{
					grand.More = node;
				}
			}
			else
			{
				tree = node;
			}
		}
		#endregion //Remove

		#region Min
		public Node GetMinNode()
		{
			if (tree == null)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					continue;
				}

				return node;
			}
		}

		public void GetMin(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				throw new KeyNotFoundException();
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
				throw new KeyNotFoundException();
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
				throw new KeyNotFoundException();
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

		public bool TryGetMinNode(out Node node)
		{
			if (tree == null)
			{
				node = null;
				return false;
			}

			var crnt = tree;
			while (true)
			{
				if (crnt.Less != null)
				{
					crnt = crnt.Less;
					continue;
				}

				node = crnt;
				return true;
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

		public bool TryGetMinKey(out TKey key)
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

		public bool TryGetMinValue(out TValue value)
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
		public Node GetMaxNode()
		{
			if (tree == null)
			{
				throw new KeyNotFoundException();
			}

			var node = tree;
			while (true)
			{
				if (node.More != null)
				{
					node = node.More;
					continue;
				}

				return node;
			}
		}

		public void GetMax(out TKey key, out TValue value)
		{
			if (tree == null)
			{
				throw new KeyNotFoundException();
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
				throw new KeyNotFoundException();
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
				throw new KeyNotFoundException();
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

		public bool TryGetMaxNode(out Node node)
		{
			if (tree == null)
			{
				node = null;
				return false;
			}

			var crnt = tree;
			while (true)
			{
				if (crnt.More != null)
				{
					crnt = crnt.More;
					continue;
				}

				node = crnt;
				return true;
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

		public bool TryGetMaxKey(out TKey key)
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

		public bool TryGetMaxValue(out TValue value)
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
			var node = tree;
			while (true)
			{
				if (node == null)
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
		public TValue GetValue(TKey key)
		{
			var node = tree;
			while (true)
			{
				if (node == null)
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
				if (node == null)
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
				if (node == null)
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
				if (crnt == null)
				{
					node = null;
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
			if (tree == null)
			{
				return 0;
			}

			var height = 1;
			var max = 1;
			var node = tree;
			while (true)
			{
				if (node.Less != null)
				{
					node = node.Less;
					++height;
					continue;
				}
				if (node.More != null)
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
					if (parent == null)
					{
						return max;
					}

					var more = parent.More;
					if (more != null && more != node)
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

		public Node<TKey, TValue>[] CopyNodesAscending()
		{
			var nodes = new Node<TKey, TValue>[count];
			CopyNodesAscending(nodes, 0);
			return nodes;
		}

		public Node[] CopyReferencesAscending()
		{
			var nodes = new Node[count];
			CopyReferencesAscending(nodes, 0);
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
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(Node<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new Node<TKey, TValue>(node);
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyReferencesAscending(Node[] nodes, int offset)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = node;
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(ICollection<Node<TKey, TValue>> nodes)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new Node<TKey, TValue>(node));
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyRefencesAscending(ICollection<Node> nodes)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(node);
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesAscending(Action<Node<TKey, TValue>> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new Node<TKey, TValue>(node));
				if (!node.TryNext(out node))
				{
					return;
				}
			}
		}

		public void CopyReferencesAscending(Action<Node> copy)
		{
			if (!TryGetMinNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node);
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
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

		public Node<TKey, TValue>[] CopyNodesDescending()
		{
			var nodes = new Node<TKey, TValue>[count];
			CopyNodesDescending(nodes, 0);
			return nodes;
		}

		public Node[] CopyReferencesDescending()
		{
			var nodes = new Node[count];
			CopyReferencesDescending(nodes, 0);
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
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(Node<TKey, TValue>[] nodes, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = new Node<TKey, TValue>(node);
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyReferencesDescending(Node[] nodes, int offset)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes[offset++] = node;
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(ICollection<Node<TKey, TValue>> nodes)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(new Node<TKey, TValue>(node));
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyReferencesDescending(ICollection<Node> nodes)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				nodes.Add(node);
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyNodesDescending(Action<Node<TKey, TValue>> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(new Node<TKey, TValue>(node));
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}

		public void CopyReferencesDescending(Action<Node> copy)
		{
			if (!TryGetMaxNode(out var node))
			{
				return;
			}
			while (true)
			{
				copy(node);
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
				{
					return;
				}
			}
		}
		#endregion //Copy

		#region Enumerable
		public IEnumerable<Node> GetAscendingEnumerable()
		{
			if (!TryGetMinNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node;
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
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
				if (!node.TryNext(out node))
				{
					yield break;
				}
			}
		}

		public IEnumerable<Node> GetDescendingEnumerable()
		{
			if (!TryGetMaxNode(out var node))
			{
				yield break;
			}
			while (true)
			{
				yield return node;
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
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
				if (!node.TryPrev(out node))
				{
					yield break;
				}
			}
		}
		#endregion //Enumerable
		#endregion //Methods
	}
}

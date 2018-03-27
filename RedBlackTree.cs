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
			ModifiedAssert(tree);
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
			ModifiedAssert(tree);
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
			ModifiedAssert(tree);
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
			ModifiedAssert(tree);
#endif //ASSERT
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

			var working = node;
			var red = node.Red;

			var parent = node.Parent;
			Node sibling;

			Node x;
			if (node.Less == null && node.More == null)
			{
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
				else
				{
					if (parent.Red)
					{
						parent.Red = false;
						if (parent.Less == node)
						{
							parent.Less = null;
							sibling = parent.More;
							if (sibling != null)
							{
								sibling.Red = true;
							}
						}
						else
						{
							parent.More = null;
							sibling = parent.Less;
							if (sibling != null)
							{
								sibling.Red = true;
							}
						}
						return;
					}
					else
					{
						throw new NotImplementedException();
					}
				}
			}
			else if (node.Less == null)
			{
				x = node.More;
				Transplant(node, node.More);
			}
			else if (node.More == null)
			{
				x = node.Less;
				Transplant(node, node.Less);
			}
			else
			{
				throw new NotImplementedException();
			}

			if (red)
			{
				return;
			}

			node = x;
			parent = node.Parent;
			while (!node.Red && parent != null)
			{
				if (node == parent.Less)
				{
					sibling = parent.More;
					if (sibling.Red)
					{
						throw new NotImplementedException();
					}
					throw new NotImplementedException();
				}
				else
				{
					sibling = parent.More;
					if (sibling.Red)
					{
						throw new NotImplementedException();
					}
					throw new NotImplementedException();
				}
			}

			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Transplant(Node parent, Node node)
		{
			var grand = parent.Parent;
			node.Parent = grand;
			if (grand != null)
			{
				if (grand.Less == parent)
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

		#region Assert
#if ASSERT
		private void ModifiedAssert(Node node)
		{
			if (node.Parent != null)
			{
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.Parent, node));
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.Parent, node.Less));
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.Parent, node.More));
			}
			if (node.Less != null)
			{
				System.Diagnostics.Debug.Assert(!(node.Red && node.Less.Red));
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.Less, node));
				System.Diagnostics.Debug.Assert(object.ReferenceEquals(node.Less.Parent, node));
				Assert(node.Less);
			}
			if (node.More != null)
			{
				System.Diagnostics.Debug.Assert(!(node.Red && node.More.Red));
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.More, node));
				System.Diagnostics.Debug.Assert(object.ReferenceEquals(node.More.Parent, node));
				Assert(node.More);
			}
			if (node.Less != null && node.More != null)
			{
				System.Diagnostics.Debug.Assert(!object.ReferenceEquals(node.Less, node.More));
			}
		}
#endif //ASSERT
		#endregion //Assert

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

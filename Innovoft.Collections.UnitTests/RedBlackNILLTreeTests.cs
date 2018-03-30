using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Innovoft.Collections;

namespace Innovoft.Collections.UnitTests
{
	[TestClass]
	public class RedBlackNILLTreeTests
	{
		[TestMethod]
		public void RedBlackNILLTreeAddAscendingTest()
		{
			var tree = Create();

			var count = 0;
			for (var i = 0; i < 1024; ++i)
			{
				tree.Add(i, i);
				++count;

				Assert.AreEqual(count, tree.Count);
				Assert.AreEqual(0, tree.GetMinKey());
				Assert.AreEqual(i, tree.GetMaxKey());
				for (var j = i; j >= 0; --j)
				{
					Assert.IsTrue(tree.ContainsKey(j));
				}
				var keys = tree.CopyKeysAscending();
				var values = tree.CopyValuesAscending();
				for (var j = i; j > 0; --j)
				{
					Assert.AreEqual(j, keys[j]);
					Assert.AreEqual(j, values[j]);
				}
				Tests(tree);
			}
		}

		[TestMethod]
		public void RedBlackNILLTreeAddDescendingTest()
		{
			var tree = Create();

			var count = 0;
			for (var i = 1023; i >= 0; --i)
			{
				tree.Add(i, i);
				++count;

				Assert.AreEqual(count, tree.Count);
				Assert.AreEqual(i, tree.GetMinKey());
				Assert.AreEqual(1023, tree.GetMaxKey());
				for (var j = i; j < 1024; ++j)
				{
					Assert.IsTrue(tree.ContainsKey(j));
				}
				var keys = tree.CopyKeysAscending();
				var values = tree.CopyValuesAscending();
				for (var j = i; j < 1024; ++j)
				{
					Assert.AreEqual(j, keys[j - i]);
					Assert.AreEqual(j, values[j - i]);
				}
				Tests(tree);
			}
		}

		[TestMethod]
		public void RedBlackNILLTreeAddRandomTest()
		{
			var tree = Create();

			var random = new Random();
			var count = 0;
			while (tree.Count < 4096)
			{
				while (true)
				{
					var key = random.Next(4096);
					if (tree.ContainsKey(key))
					{
						continue;
					}
					tree.Add(key, key);
					break;
				}
				++count;

				Tests(tree);
			}
		}

		private static RedBlackNILLTree<int, int> Create()
		{
			var tree = new RedBlackNILLTree<int, int>((x, y) => x - y);
			Assert.IsNull(tree.Tree);
			Assert.AreEqual(0, tree.Count);
			Assert.AreEqual(0, tree.Height());
			return tree;
		}

		private static void TestsAdd(RedBlackNILLTree<int, int> tree, int add)
		{
			tree.Add(add, add);
			Tests(tree);
		}

		private static void Tests(RedBlackNILLTree<int, int> tree)
		{
			TestNodes(tree);
			TestBlackCount(tree);
			TestHeight(tree);
			TestMinMaxKey(tree);
			TestNext(tree);
			TestTryNext(tree);
			TestPrev(tree);
			TestTryPrev(tree);
			TestCopyKeysAscending(tree);
			TestCopyKeysDescending(tree);
		}

		private static void TestNodes(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.Tree;
			Assert.IsFalse(node.Red);
			TestNode(node);
		}

		private static void TestNode(RedBlackNILLTree<int, int>.Node node)
		{
			if (node.Red)
			{
				Assert.IsTrue((node.Less == null) == (node.More == null));
			}
			else
			{
				if (node.Less == null && node.More != null)
				{
					Assert.IsTrue(node.More.Red);
				}
				if (node.More == null && node.Less != null)
				{
					Assert.IsTrue(node.Less.Red);
				}
				if (node.Parent != null)
				{
					Assert.IsNotNull(node.Parent.Less);
					Assert.IsNotNull(node.Parent.More);
				}
			}
			if (node.Parent != null)
			{
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.Less));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.More));
			}
			if (node.Less != null)
			{
				Assert.IsFalse(node.Red && node.Less.Red);
				Assert.IsFalse(object.ReferenceEquals(node.Less, node));
				Assert.IsTrue(object.ReferenceEquals(node.Less.Parent, node));
				TestNode(node.Less);
			}
			if (node.More != null)
			{
				Assert.IsFalse(node.Red && node.More.Red);
				Assert.IsFalse(object.ReferenceEquals(node.More, node));
				Assert.IsTrue(object.ReferenceEquals(node.More.Parent, node));
				TestNode(node.More);
			}
			if (node.Less != null && node.More != null)
			{
				Assert.IsFalse(object.ReferenceEquals(node.Less, node.More));
			}
		}

		private static void TestBlackCount(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			var blacks = GetBlackCount(node);
			while (true)
			{
				node = tree.Next(node);
				if (tree.NILL(node))
				{
					break;
				}
				if (node.Less == null || node.More == null)
				{
					Assert.AreEqual(blacks, GetBlackCount(node));
				}
			}
		}

		private static int GetBlackCount(RedBlackNILLTree<int, int>.Node node)
		{
			var count = 0;
			while (true)
			{
				if (!node.Red)
				{
					++count;
				}
				node = node.Parent;
				if (node == null)
				{
					return count;
				}
			}
		}

		private static void TestHeight(RedBlackNILLTree<int, int> tree)
		{
			var max = 2 * Math.Log(tree.Count + 1, 2);
			var height = tree.Height();
			Assert.IsTrue(height <= max);
		}

		private static void TestMinMaxKey(RedBlackNILLTree<int, int> tree)
		{
			var minKey = tree.GetMinKey();
			var maxKey = tree.GetMaxKey();
			if (tree.Count > 1)
			{
				Assert.IsTrue(minKey < maxKey);
			}
			else
			{
				Assert.IsTrue(minKey == maxKey);
			}
		}

		private static void TestNext(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				var next = tree.Next(node);
				if (tree.NILL(next))
				{
					break;
				}
				++count;
				Assert.IsNotNull(next);
				Assert.IsTrue(node.Key < next.Key);
				node = next;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestTryNext(RedBlackNILLTree<int, int> tree)
		{
			if (!tree.TryGetMinNode(out var node))
			{
				Assert.Fail();
			}
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				if (!tree.TryNext(node, out var next))
				{
					break;
				}
				++count;
				Assert.IsNotNull(next);
				Assert.IsTrue(node.Key < next.Key);
				node = next;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestPrev(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.GetMaxNode();
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				var prev = tree.Prev(node);
				if (tree.NILL(prev))
				{
					break;
				}
				++count;
				Assert.IsNotNull(prev);
				Assert.IsTrue(node.Key > prev.Key);
				node = prev;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestTryPrev(RedBlackNILLTree<int, int> tree)
		{
			if (!tree.TryGetMaxNode(out var node))
			{
				Assert.Fail();
			}
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				if (!tree.TryPrev(node, out var prev))
				{
					break;
				}
				++count;
				Assert.IsNotNull(prev);
				Assert.IsTrue(node.Key > prev.Key);
				node = prev;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestCopyKeysAscending(RedBlackNILLTree<int, int> tree)
		{
			var keys = tree.CopyKeysAscending();
			Assert.AreEqual(tree.Count, keys.Length);
			for (var i = keys.Length - 1; i > 0; --i)
			{
				Assert.IsTrue(keys[i - 1] < keys[i]);
			}
		}

		private static void TestCopyKeysDescending(RedBlackNILLTree<int, int> tree)
		{
			var keys = tree.CopyKeysDescending();
			Assert.AreEqual(tree.Count, keys.Length);
			for (var i = keys.Length - 1; i > 0; --i)
			{
				Assert.IsTrue(keys[i - 1] > keys[i]);
			}
		}
	}
}

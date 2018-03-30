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
				TestsAdd(tree, i);
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
			}
		}

		[TestMethod]
		public void RedBlackNILLTreeAddDescendingTest()
		{
			var tree = Create();

			var count = 0;
			for (var i = 1023; i >= 0; --i)
			{
				TestsAdd(tree, i);
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
					TestsAdd(tree, key);
					break;
				}
				++count;
			}
		}

		[TestMethod]
		public void RedBlackNILLTreeRemoveOnlyTest()
		{
			var tree = Create();

			TestsAdd(tree, 1);
			Assert.AreEqual(1, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(0, tree.Count);
		}

		[TestMethod]
		public void RedBlackNILLTreeRemoveLeafRedLessTest()
		{
			var tree = Create();

			TestsAdd(tree, 2);
			TestsAdd(tree, 1);
			Assert.AreEqual(2, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(1, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackNILLTreeRemoveLeafRedMoreTest()
		{
			var tree = Create();

			TestsAdd(tree, 1);
			TestsAdd(tree, 2);
			Assert.AreEqual(2, tree.Count);
			Assert.IsTrue(tree.Remove(2));
			Assert.AreEqual(1, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackNILLTreeRemoveRandomTest()
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
					TestsAdd(tree, key);
					break;
				}
				++count;
				Assert.AreEqual(count, tree.Count);
			}

			while (true)
			{
				var key = tree.GetKeyFromMin(random.Next(tree.Count));
				Assert.IsTrue(tree.Remove(key));
				--count;

				Assert.AreEqual(count, tree.Count);

				if (tree.Count <= 0)
				{
					break;
				}

				Tests(tree);
			}
		}

		private static RedBlackNILLTree<int, int> Create()
		{
			var tree = new RedBlackNILLTree<int, int>((x, y) => x - y);
			Assert.IsTrue(tree.NILL(tree.Tree));
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
			TestNode(tree, node);
		}

		private static void TestNode(RedBlackNILLTree<int, int> tree, RedBlackNILLTree<int, int>.Node node)
		{
			if (node.Red)
			{
				Assert.IsTrue(tree.NILL(node.Less) == tree.NILL(node.More));
			}
			else
			{
				if (tree.NILL(node.Less) && !tree.NILL(node.More))
				{
					Assert.IsTrue(node.More.Red);
				}
				if (tree.NILL(node.More) && !tree.NILL(node.Less))
				{
					Assert.IsTrue(node.Less.Red);
				}
				if (!tree.NILL(node.Parent))
				{
					Assert.IsFalse(tree.NILL(node.Parent.Less));
					Assert.IsFalse(tree.NILL(node.Parent.More));
				}
			}
			if (!tree.NILL(node.Parent))
			{
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.Less));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.More));
			}
			if (!tree.NILL(node.Less))
			{
				Assert.IsFalse(node.Red && node.Less.Red);
				Assert.IsFalse(object.ReferenceEquals(node.Less, node));
				Assert.IsTrue(object.ReferenceEquals(node.Less.Parent, node));
				TestNode(tree, node.Less);
			}
			if (!tree.NILL(node.More))
			{
				Assert.IsFalse(node.Red && node.More.Red);
				Assert.IsFalse(object.ReferenceEquals(node.More, node));
				Assert.IsTrue(object.ReferenceEquals(node.More.Parent, node));
				TestNode(tree, node.More);
			}
			if (!tree.NILL(node.Less) && !tree.NILL(node.More))
			{
				Assert.IsFalse(object.ReferenceEquals(node.Less, node.More));
			}
		}

		private static void TestBlackCount(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			var blacks = GetBlackCount(tree, node);
			while (true)
			{
				node = tree.Next(node);
				if (tree.NILL(node))
				{
					break;
				}
				if (tree.NILL(node.Less) || tree.NILL(node.More))
				{
					Assert.AreEqual(blacks, GetBlackCount(tree, node));
				}
			}
		}

		private static int GetBlackCount(RedBlackNILLTree<int, int> tree, RedBlackNILLTree<int, int>.Node node)
		{
			var count = 0;
			while (true)
			{
				if (!node.Red)
				{
					++count;
				}
				node = node.Parent;
				if (tree.NILL(node))
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
			Assert.IsFalse(tree.NILL(node));
			var count = 1;
			while (true)
			{
				var next = tree.Next(node);
				if (tree.NILL(next))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.NILL(next));
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
			Assert.IsFalse(tree.NILL(node));
			var count = 1;
			while (true)
			{
				if (!tree.TryNext(node, out var next))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.NILL(next));
				Assert.IsTrue(node.Key < next.Key);
				node = next;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestPrev(RedBlackNILLTree<int, int> tree)
		{
			var node = tree.GetMaxNode();
			Assert.IsFalse(tree.NILL(node));
			var count = 1;
			while (true)
			{
				var prev = tree.Prev(node);
				if (tree.NILL(prev))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.NILL(prev));
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
			Assert.IsFalse(tree.NILL(node));
			var count = 1;
			while (true)
			{
				if (!tree.TryPrev(node, out var prev))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.NILL(prev));
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

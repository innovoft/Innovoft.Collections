using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Innovoft.Collections;

namespace Innovoft.Collections.UnitTests
{
	[TestClass]
	public class RedBlackTreeTests
	{
		[TestMethod]
		public void RedBlackTreeAddAscendingTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			Assert.IsNull(tree.Tree);
			Assert.AreEqual(0, tree.Count);

			for (var i = 0; i < 1024; ++i)
			{
				tree.Add(i, i);

				Assert.AreEqual(i + 1, tree.Count);
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
		public void RedBlackTreeAddDescendingTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			Assert.IsNull(tree.Tree);
			Assert.AreEqual(0, tree.Count);

			for (var i = 1023; i >= 0; --i)
			{
				tree.Add(i, i);

				Assert.AreEqual(1024 - i, tree.Count);
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
		public void RedBlackTreeAddRandomTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			Assert.IsNull(tree.Tree);
			Assert.AreEqual(0, tree.Count);

			var random = new Random();
			var count = 0;
			while (tree.Count < 1024)
			{
				while (true)
				{
					var key = random.Next(1024);
					if (tree.ContainsKey(key))
					{
						continue;
					}
					tree.Add(key, key);
					break;
				}
				++count;

				Assert.AreEqual(count, tree.Count);
				Assert.IsTrue(tree.GetMinKey() <=  tree.GetMaxKey());
				var keys = tree.CopyKeysAscending();
				var values = tree.CopyValuesAscending();
				for (var i = keys.Length - 1; i > 0; --i)
				{
					Assert.IsTrue(keys[i - 1] < keys[i]);
					Assert.IsTrue(values[i - 1] < values[i]);
				}
				Tests(tree);
			}
		}

		[TestMethod]
		public void RedBlackTreeRemoveTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			Assert.AreEqual(0, tree.Count);

			Assert.IsFalse(tree.Remove(1));

			tree.Add(1, 1);
			Assert.AreEqual(1, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(0, tree.Count);
		}

		private static void Tests(RedBlackTree<int, int> tree)
		{
			TestNode(tree.Tree);
			TestNext(tree);
			TestTryNext(tree);
			TestPrev(tree);
			TestTryPrev(tree);
			TestCopyKeysAscending(tree);
			TestCopyKeysDescending(tree);
		}

		private static void TestNode(RedBlackTree<int, int>.Node node)
		{
			if (node.Parent != null)
			{
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node));
				Assert.IsTrue(node.Parent != node.Less);
				Assert.IsTrue(node.Parent != node.More);
			}
			if (node.Less != null)
			{
				Assert.IsFalse(node.Red && node.Less.Red);
				Assert.IsFalse(object.ReferenceEquals(node.Less, node));
				Assert.IsTrue(node.Less.Parent == node);
				TestNode(node.Less);
			}
			if (node.More != null)
			{
				Assert.IsFalse(node.Red && node.More.Red);
				Assert.IsFalse(object.ReferenceEquals(node.More, node));
				Assert.IsTrue(node.More.Parent == node);
				TestNode(node.More);
			}
			if (node.Less != null && node.More != null)
			{
				Assert.IsFalse(object.ReferenceEquals(node.Less, node.More));
			}
		}

		private static void TestNext(RedBlackTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				var next = node.Next();
				if (next == null)
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

		private static void TestTryNext(RedBlackTree<int, int> tree)
		{
			if (!tree.TryGetMinNode(out var node))
			{
				Assert.Fail();
			}
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				if (!node.TryNext(out var next))
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

		private static void TestPrev(RedBlackTree<int, int> tree)
		{
			var node = tree.GetMaxNode();
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				var prev = node.Prev();
				if (prev == null)
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

		private static void TestTryPrev(RedBlackTree<int, int> tree)
		{
			if (!tree.TryGetMaxNode(out var node))
			{
				Assert.Fail();
			}
			Assert.IsNotNull(node);
			var count = 1;
			while (true)
			{
				if (!node.TryPrev(out var prev))
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

		private static void TestCopyKeysAscending(RedBlackTree<int, int> tree)
		{
			var keys = tree.CopyKeysAscending();
			Assert.AreEqual(tree.Count, keys.Length);
			for (var i = keys.Length - 1; i > 0; --i)
			{
				Assert.IsTrue(keys[i - 1] < keys[i]);
			}
		}

		private static void TestCopyKeysDescending(RedBlackTree<int, int> tree)
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

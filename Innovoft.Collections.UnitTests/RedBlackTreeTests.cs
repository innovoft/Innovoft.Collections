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
		public void RedBlackTreeAddDescendingTest()
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
		public void RedBlackTreeAddRandomTest()
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

		[TestMethod]
		public void RedBlackTreeRemoveOnlyTest()
		{
			var tree = Create();

			TestsAdd(tree, 1);
			Assert.AreEqual(1, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(0, tree.Count);
		}

		[TestMethod]
		public void RedBlackTreeRemoveLeafRedLessTest()
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
		public void RedBlackTreeRemoveLeafRedMoreTest()
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
		public void RedBlackTreeRemoveLeafBlackMoreParentRedNephewNoneTest()
		{
			var tree = Create();

			TestsAdd(tree, 7);//B
			TestsAdd(tree, 3);//R
			TestsAdd(tree, 8);//B
			TestsAdd(tree, 2);//B
			TestsAdd(tree, 5);//B
			TestsAdd(tree, 4);//R
			//              B7
			//      R3              B8
			//  B2      B5
			//        R4

			Assert.AreEqual(6, tree.Count);
			Assert.IsTrue(tree.Remove(4));
			Assert.AreEqual(5, tree.Count);
			Tests(tree);
			Assert.IsTrue(tree.Remove(5));
			Assert.AreEqual(4, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveLeafBlackMoreParentRedNephewLessTest()
		{
			var tree = Create();

			TestsAdd(tree, 7);//B
			TestsAdd(tree, 3);//R
			TestsAdd(tree, 8);//B
			TestsAdd(tree, 2);//B
			TestsAdd(tree, 5);//B
			TestsAdd(tree, 1);//R
			//              B7
			//      R3              B8
			//  B2      B5
			//R1

			Assert.AreEqual(6, tree.Count);
			Assert.IsTrue(tree.Remove(5));
			Assert.AreEqual(5, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveLeafBlackMoreParentRedNephewMoreTest()
		{
			var tree = Create();

			TestsAdd(tree, 7);//B
			TestsAdd(tree, 3);//R
			TestsAdd(tree, 8);//B
			TestsAdd(tree, 1);//B
			TestsAdd(tree, 5);//B
			TestsAdd(tree, 2);//R
			//              B7
			//      R3              B8
			//  B1      B5
			//    R2

			Assert.AreEqual(6, tree.Count);
			Assert.IsTrue(tree.Remove(5));
			Assert.AreEqual(5, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveLeafBlackMoreParentBlackTest()
		{
			var tree = Create();

			TestsAdd(tree, 7);//B
			TestsAdd(tree, 3);//R
			TestsAdd(tree, 8);//B
			TestsAdd(tree, 2);//B
			TestsAdd(tree, 5);//B
			TestsAdd(tree, 1);//R
			TestsAdd(tree, 4);//R
			TestsAdd(tree, 6);//R
			//              B7
			//      R3              B8
			//  B2      B5
			//R1      R4  R6

			Assert.AreEqual(8, tree.Count);
			Assert.IsTrue(tree.Remove(8));
			Assert.AreEqual(7, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveRootWithChildMoreTest()
		{
			var tree = Create();

			TestsAdd(tree, 1);
			TestsAdd(tree, 2);
			Assert.AreEqual(2, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(1, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveRootWithChildLessTest()
		{
			var tree = Create();

			TestsAdd(tree, 2);
			TestsAdd(tree, 1);
			Assert.AreEqual(2, tree.Count);
			Assert.IsTrue(tree.Remove(2));
			Assert.AreEqual(1, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveRootWithChildrenTest()
		{
			var tree = Create();

			TestsAdd(tree, 2);
			TestsAdd(tree, 1);
			TestsAdd(tree, 3);
			Assert.AreEqual(3, tree.Count);
			Assert.IsTrue(tree.Remove(2));
			Assert.AreEqual(2, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveStemRedTest()
		{
			var tree = Create();

			TestsAdd(tree, 7);//B
			TestsAdd(tree, 3);//R
			TestsAdd(tree, 8);//B
			TestsAdd(tree, 2);//B
			TestsAdd(tree, 5);//B
			TestsAdd(tree, 4);//R
			//              B7
			//      R3              B8
			//  B2      B5
			//        R4

			Assert.AreEqual(6, tree.Count);
			Assert.IsTrue(tree.Remove(4));
			Assert.AreEqual(5, tree.Count);
			Tests(tree);
			Assert.IsTrue(tree.Remove(3));
			Assert.AreEqual(4, tree.Count);
			Tests(tree);
		}

		[TestMethod]
		public void RedBlackTreeRemoveRandomTest()
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
				var keys = tree.CopyKeysAscending();
				var key = keys[random.Next(keys.Length)];
				tree.Remove(key);
				--count;

				Assert.AreEqual(count, tree.Count);

				if (tree.Count <= 0)
				{
					break;
				}

				Tests(tree);
			}
		}

		private static RedBlackTree<int, int> Create()
		{
			var tree = new RedBlackTree<int, int>((x, y) => x - y);
			Assert.IsNull(tree.Tree);
			Assert.AreEqual(0, tree.Count);
			Assert.AreEqual(0, tree.Height());
			return tree;
		}

		private static void TestsAdd(RedBlackTree<int, int> tree, int add)
		{
			tree.Add(add, add);
			Tests(tree);
		}

		private static void Tests(RedBlackTree<int, int> tree)
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

		private static void TestNodes(RedBlackTree<int, int> tree)
		{
			var node = tree.Tree;
			Assert.IsFalse(node.Red);
			TestNode(node);
		}

		private static void TestNode(RedBlackTree<int, int>.Node node)
		{
			if (node.Red)
			{
				Assert.IsTrue((node.Less == null) == (node.More == null));
			}
			else
			{
				if (node.Less == null && node.More !=null)
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

		private static void TestBlackCount(RedBlackTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			var blacks = GetBlackCount(node);
			while (true)
			{
				node = node.Next();
				if (node == null)
				{
					break;
				}
				if (node.Less == null || node.More == null)
				{
					Assert.AreEqual(blacks, GetBlackCount(node));
				}
			}
		}

		private static int GetBlackCount(RedBlackTree<int, int>.Node node)
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

		private static void TestHeight(RedBlackTree<int, int> tree)
		{
			var max = 2 * Math.Log(tree.Count + 1, 2);
			var height = tree.Height();
			Assert.IsTrue(height <= max);
		}

		private static void TestMinMaxKey(RedBlackTree<int, int> tree)
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

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Innovoft.Collections;

namespace Innovoft.Collections
{
	[TestClass]
	public class RedBlackTreeTests
	{
		[TestMethod]
		public void RedBlackTreeAddAscendingTest()
		{
			var tree = CreateInt32();

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
		public void RedBlackTreeAddDescendingTest()
		{
			var tree = CreateInt32();

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
		public void RedBlackTreeAddRandomTest()
		{
			var tree = CreateInt32();

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
		public void RedBlackTreeCopyKeysAscending()
		{
			const int count = 4096;
			var tree = CreateInt32();
			for (var i = 0; i < count; ++i)
			{
				TestsAdd(tree, i);
			}
			var keys = tree.CopyKeysAscending();
			for (var i = count - 1; i > 0; --i)
			{
				Assert.IsTrue(keys[i] > keys[i - 1]);
			}
		}

		[TestMethod]
		public void RedBlackTreeAddRemoveTest()
		{
			const int count = 4096;
			var tree = CreateInt32();
			var es = CreateInt32();
			var os = CreateInt32();
			for (var i = 0; i < count; ++i)
			{
				if ((i & 1) == 0)
				{
					TestsAdd(es, i);
				}
				else
				{
					TestsAdd(os, i);
				}
			}
			Assert.AreEqual(count, es.Count + os.Count);
			var merge = new Action<int, int>((x, y) => Assert.Fail("No merging should be happening"));
			tree.AddRemove(es, merge);
			Assert.AreEqual(0, es.Count);
			tree.AddRemove(os, merge);
			Assert.AreEqual(0, os.Count);
			Assert.AreEqual(count, tree.Count);
		}

		[TestMethod]
		public void RedBlackTreeRemoveOnlyTest()
		{
			var tree = CreateInt32();

			TestsAdd(tree, 1);
			Assert.AreEqual(1, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(0, tree.Count);
		}

		[TestMethod]
		public void RedBlackTreeRemoveLeafRedLessTest()
		{
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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
			var tree = CreateInt32();

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

		[TestMethod]
		public void ReadBlackTreeMoreAndLess()
		{
			const double min = 1.0;
			const double max = 1024.0;
			const double step = 1.0;
			const double half = 0.5;
			const int digits = 1;

			var tree = CreateDouble();

			double i;

			//Add
			for (i = min; i <= max; i += step)
			{
				tree.Add(i, i);
			}

			//Tests
			RedBlackTree<double, double>.Node node;
			i = Math.Round(min - half, digits);
			Assert.IsFalse(tree.TryGetNodeOrLess(i, out node), "TryGetNodeOrLess");
			Assert.IsTrue(tree.TryGetNodeOrMore(i, out node), "TryGetNodeOrMore");
			Assert.AreEqual(min, node.Key);
			for (i = Math.Round(min + half, digits); i < max; i = Math.Round(i + step, digits))
			{
				Assert.IsTrue(tree.TryGetNodeOrLess(i, out node), "TryGetNodeOrLess");
				Assert.AreEqual(Math.Round(i - half), node.Key);
				Assert.IsTrue(tree.TryGetNodeOrMore(i, out node), "TryGetNodeOrMore");
				Assert.AreEqual(Math.Round(i + half), node.Key);
			}
			i = Math.Round(max + half, digits);
			Assert.IsTrue(tree.TryGetNodeOrLess(i, out node), "TryGetNodeOrLess");
			Assert.AreEqual(max, node.Key);
			Assert.IsFalse(tree.TryGetNodeOrMore(i, out node), "TryGetNodeOrMore");
		}

		private static RedBlackTree<int, int> CreateInt32()
		{
			var tree = new RedBlackTree<int, int>(Int32AscendingComparison.Comparison);
			Assert.IsTrue(tree.Terminal(tree.Tree));
			Assert.AreEqual(0, tree.Count);
			Assert.AreEqual(0, tree.Height());
			return tree;
		}

		private static RedBlackTree<double, double> CreateDouble()
		{
			var tree = new RedBlackTree<double, double>(DoubleAscendingComparison.Comparison);
			Assert.IsTrue(tree.Terminal(tree.Tree));
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
			TestNode(tree, node);
		}

		private static void TestNode(RedBlackTree<int, int> tree, RedBlackTree<int, int>.Node node)
		{
			if (node.Red)
			{
				Assert.IsTrue(tree.Terminal(node.Less) == tree.Terminal(node.More));
				if (!tree.Terminal(node.Less))
				{
					Assert.IsFalse(node.Less.Red);
				}
				if (!tree.Terminal(node.More))
				{
					Assert.IsFalse(node.More.Red);
				}
			}
			else
			{
				if (tree.Terminal(node.Less) && !tree.Terminal(node.More))
				{
					Assert.IsTrue(node.More.Red);
				}
				if (tree.Terminal(node.More) && !tree.Terminal(node.Less))
				{
					Assert.IsTrue(node.Less.Red);
				}
				if (!tree.Terminal(node.Parent))
				{
					Assert.IsFalse(tree.Terminal(node.Parent.Less));
					Assert.IsFalse(tree.Terminal(node.Parent.More));
				}
			}
			if (!tree.Terminal(node.Parent))
			{
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.Less));
				Assert.IsFalse(object.ReferenceEquals(node.Parent, node.More));
			}
			if (!tree.Terminal(node.Less))
			{
				Assert.IsFalse(object.ReferenceEquals(node.Less, node));
				Assert.IsTrue(object.ReferenceEquals(node.Less.Parent, node));
				TestNode(tree, node.Less);
			}
			if (!tree.Terminal(node.More))
			{
				Assert.IsFalse(object.ReferenceEquals(node.More, node));
				Assert.IsTrue(object.ReferenceEquals(node.More.Parent, node));
				TestNode(tree, node.More);
			}
			if (!tree.Terminal(node.Less) && !tree.Terminal(node.More))
			{
				Assert.IsFalse(object.ReferenceEquals(node.Less, node.More));
			}
		}

		private static void TestBlackCount(RedBlackTree<int, int> tree)
		{
			var node = tree.GetMinNode();
			var blacks = GetBlackCount(tree, node);
			while (true)
			{
				node = tree.Next(node);
				if (tree.Terminal(node))
				{
					break;
				}
				if (tree.Terminal(node.Less) || tree.Terminal(node.More))
				{
					Assert.AreEqual(blacks, GetBlackCount(tree, node));
				}
			}
		}

		private static int GetBlackCount(RedBlackTree<int, int> tree, RedBlackTree<int, int>.Node node)
		{
			var count = 0;
			while (true)
			{
				if (!node.Red)
				{
					++count;
				}
				node = node.Parent;
				if (tree.Terminal(node))
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
			Assert.IsFalse(tree.Terminal(node));
			var count = 1;
			while (true)
			{
				var next = tree.Next(node);
				if (tree.Terminal(next))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.Terminal(next));
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
			Assert.IsFalse(tree.Terminal(node));
			var count = 1;
			while (true)
			{
				if (!tree.TryNext(node, out var next))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.Terminal(next));
				Assert.IsTrue(node.Key < next.Key);
				node = next;
			}
			Assert.AreEqual(tree.Count, count);
		}

		private static void TestPrev(RedBlackTree<int, int> tree)
		{
			var node = tree.GetMaxNode();
			Assert.IsFalse(tree.Terminal(node));
			var count = 1;
			while (true)
			{
				var prev = tree.Prev(node);
				if (tree.Terminal(prev))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.Terminal(prev));
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
			Assert.IsFalse(tree.Terminal(node));
			var count = 1;
			while (true)
			{
				if (!tree.TryPrev(node, out var prev))
				{
					break;
				}
				++count;
				Assert.IsFalse(tree.Terminal(prev));
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

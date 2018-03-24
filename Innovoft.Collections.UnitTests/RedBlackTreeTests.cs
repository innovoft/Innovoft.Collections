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
			}
		}

		[TestMethod]
		public void RedBlackTreeAddDescendingTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);

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
			}
		}

		[TestMethod]
		public void RedBlackTreeAddRandomTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);

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
			}
		}

		[TestMethod]
		public void RedBlackTreeRemoveTest()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			Assert.AreEqual(0, tree.Count);

			tree.Add(1, 1);
			Assert.AreEqual(1, tree.Count);
			Assert.IsTrue(tree.Remove(1));
			Assert.AreEqual(0, tree.Count);
		}
	}
}

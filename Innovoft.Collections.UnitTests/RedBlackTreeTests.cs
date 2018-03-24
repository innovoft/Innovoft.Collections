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
					Assert.IsTrue(tree.Contains(j));
				}
				var keys = new int[tree.Count];
				tree.CopyKeysAscending(keys, 0);
				for (var j = i; j > 0; --j)
				{
					Assert.AreEqual(j, keys[j]);
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
					Assert.IsTrue(tree.Contains(j));
				}
				var keys = new int[tree.Count];
				tree.CopyKeysAscending(keys, 0);
				for (var j = i; j < 1024; ++j)
				{
					Assert.AreEqual(j, keys[j - i]);
				}
			}
		}
	}
}

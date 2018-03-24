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

			for (var i = 1; i <= 1024; ++i)
			{
				tree.Add(i, i);

				Assert.AreEqual(i, tree.Count);
				Assert.AreEqual(1, tree.GetMinKey());
				Assert.AreEqual(i, tree.GetMaxKey());
				for (var j = i; j > 0; --j)
				{
					Assert.IsTrue(tree.Contains(j));
				}
				var keys = new int[tree.Count];
				tree.CopyKeysAscendingTo(keys, 0);
				for (var j = i; j > 0; --j)
				{
					Assert.AreEqual(j, keys[j - 1]);
				}
			}
		}

		[TestMethod]
		public void RedBlackTree321Test()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			tree.Add(3, 3);
			tree.Add(2, 2);
			tree.Add(1, 1);

			Assert.AreEqual(3, tree.Count);
			Assert.AreEqual(1, tree.GetMinValue());
			Assert.AreEqual(3, tree.GetMaxValue());
			Assert.IsTrue(tree.Contains(2));
		}

		[TestMethod]
		public void RedBlackTree4321Test()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			tree.Add(4, 4);
			tree.Add(3, 3);
			tree.Add(2, 2);
			tree.Add(1, 1);

			Assert.AreEqual(4, tree.Count);
			Assert.AreEqual(1, tree.GetMinValue());
			Assert.AreEqual(4, tree.GetMaxValue());
			Assert.IsTrue(tree.Contains(2));
			Assert.IsTrue(tree.Contains(3));
		}
	}
}

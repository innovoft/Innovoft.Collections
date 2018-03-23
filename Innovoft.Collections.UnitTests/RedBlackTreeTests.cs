using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Innovoft.Collections;

namespace Innovoft.Collections.UnitTests
{
	[TestClass]
	public class RedBlackTreeTests
	{
		[TestMethod]
		public void RedBlackTree123Test()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			tree.Add(1, 1);
			tree.Add(2, 2);
			tree.Add(3, 3);

			Assert.AreEqual(1, tree.GetMinValue());
			Assert.AreEqual(3, tree.GetMaxValue());
			Assert.IsTrue(tree.Contains(2));
		}

		[TestMethod]
		public void RedBlackTree321Test()
		{
			var tree = new RedBlackTree<int, int>(new ComparableAscendingComparer<int>().Compare);
			tree.Add(3, 3);
			tree.Add(2, 2);
			tree.Add(1, 1);

			Assert.AreEqual(1, tree.GetMinValue());
			Assert.AreEqual(3, tree.GetMaxValue());
			Assert.IsTrue(tree.Contains(2));
		}
	}
}

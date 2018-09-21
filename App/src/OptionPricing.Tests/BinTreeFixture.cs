using System;
using NUnit.Framework;
using OptionPricing.Core;

namespace OptionPricing.Tests
{
	[TestFixture]
	public class BinTreeFixture
	{
		[Test]
		public void CalculateTotalNodes()
		{
			Assert.AreEqual(1, BinTree.CalculateTotalNodes(1));
			Assert.AreEqual(3, BinTree.CalculateTotalNodes(2));
			Assert.AreEqual(6, BinTree.CalculateTotalNodes(3));
			Assert.AreEqual(10, BinTree.CalculateTotalNodes(4));		
		}
		
		[Test]
		public void GetFirstNodeInStep()
		{
			Assert.AreEqual(4, BinTree.GetIndexFirstNodeInStep(3));
			Assert.AreEqual(2, BinTree.GetIndexFirstNodeInStep(2));
		}
		
		[Test]
		public void GetLastNodeInStep()
		{
			Assert.AreEqual(6, BinTree.GetIndexLastNodeInStep(3));
			Assert.AreEqual(3, BinTree.GetIndexLastNodeInStep(2));
		}
		
		[Test]
		public void RecombineNodesInTree()
		{
			var tree = new CapmBinTree(4);
			
			var node2 = tree.GetUpDependantNode(tree.GetRootNode());
			var node3 = tree.GetDownDependantNode(tree.GetRootNode());
			Assert.AreEqual(2, node2.NodeIndex);
			Assert.AreEqual(3, node3.NodeIndex);
			
			var node5A = tree.GetDownDependantNode(node2);
			var node5B = tree.GetUpDependantNode(node3);
			Assert.AreSame(node5A, node5B);
		}
		
		[Test]
		public void IterateNodes()
		{
			var tree = new CapmBinTree(4);
			var nodes = new System.Collections.Generic.List<BinTree.Node>(tree.IterateNodesOfStep(4));
			Assert.AreEqual(4, nodes.Count);
			Assert.AreEqual(7, nodes[0].NodeIndex);
			Assert.AreEqual(8, nodes[1].NodeIndex);
			Assert.AreEqual(9, nodes[2].NodeIndex);
			Assert.AreEqual(10, nodes[3].NodeIndex);
		}
	}
}


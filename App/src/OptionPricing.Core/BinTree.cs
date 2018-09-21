using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace OptionPricing.Core
{
	public abstract class BinTree
	{
		private List<Node> nodeList;
		public int NumberOfSteps { get; set; }
        private int NodeCount { get; set; }
		
		#region Tree Properties
				
		public double Volatility { get; set; }
		public double InitialStockPrice { get; set; }
		public double StrikePrice { get; set; }
		public double TotalTimeYears { get; set; }
		public double RiskFree { get; set; }
		public double MarketRiskPremium { get; set; }
		public double StockBeta { get; set; }
		
		private double? deltaT;
		public double DeltaT
		{
			get{
				if (deltaT == null) deltaT = TotalTimeYears / NumberOfSteps;
				return deltaT.Value;
			}
		}
		
		private double? mu;
		public double Mu
		{
			get{
				if (mu == null) mu = (double)Math.Exp((double)Volatility * Math.Sqrt((double)DeltaT));
				return mu.Value;
			}
		}
		
		private double? de;
		public double De
		{
			get{
				if (de == null) de = 1/mu;
				return de.Value;
			}
		}

        public void ResetTreeParameterCalcs()
        {
            de = null;
            mu = null;
            deltaT = null;
        }
		
		#endregion
		
		public BinTree (int numberOfSteps)
		{
			if (numberOfSteps < 2)
				throw new ArgumentException("Number of steps must be 2 or greater.", "numberOfSteps");
		
			NumberOfSteps = numberOfSteps;
			nodeList = new List<Node>(CalculateTotalNodes(numberOfSteps));
			
			BuildTree();
		}
		
		private void BuildTree()
		{
			for (int step = 1; step <= NumberOfSteps; step++) {
				
				int firstNode = CalculateTotalNodes(step-1)+1;
				int lastNode = CalculateTotalNodes(step);
				for (int idx = firstNode; idx <= lastNode; idx++) {
					
					var node = new BinTree.Node();
					node.StepNumber = step;
					node.NodeIndex = idx;
					nodeList.Add(node);
					
					if (idx != nodeList.Count) throw new Exception("Node was added to the wrong possition");
				}
			}
            NodeCount = nodeList.Count;
		}
		
		public void CalculateStockPrices()
        {
            var rootNode = GetRootNode();
            rootNode.StockPrice = InitialStockPrice;

            for (int step = 2; step <= NumberOfSteps; step++)
            {
                bool isFirstNodeInStep = true;
                foreach (var node in IterateNodesOfStep(step))
                {
                    if (isFirstNodeInStep)
                    {
                        // If it's the first node, the value is S*u
                        isFirstNodeInStep = false;

                        if (step == 2)
                        {
                            node.StockPrice = rootNode.StockPrice * Mu;
                        }
                        else
                        {
                            var downParentNode = CalculateTotalNodes(step-2)+1;
                            node.StockPrice = nodeList[downParentNode-1].StockPrice * Mu;
                        }
                    }
                    else
                    {
                        // Nodes other than the first one can be calculated as S*d
                        int nodeOffset = CalculateTotalNodes(step) - node.NodeIndex;
                        int upParentNode = CalculateTotalNodes(step - 1) - nodeOffset;
                        node.StockPrice = nodeList[upParentNode-1].StockPrice * De;
                    }
                }
            }
            //var rootNode = GetRootNode();
            //rootNode.StockPrice = InitialStockPrice;
            //CalculateSubtreeStockPrices(rootNode);
		}

		public abstract void CalculateOptionPrices();
		
		#region Static Index Based Helper Methods

		public static int CalculateTotalNodes(int numberOfSteps)
		{
			if (numberOfSteps <= 0) return 0;
			return (numberOfSteps + 1) * numberOfSteps / 2;
		}
		
		public static int GetIndexFirstNodeInStep(int stepNumber)
		{
			return CalculateTotalNodes(stepNumber-1) + 1;
		}
		
		public static int GetIndexLastNodeInStep(int stepNumber)
		{
			return CalculateTotalNodes(stepNumber);
		}
			
		#endregion
		
		public Node GetRootNode()
		{
			return nodeList[0];
		}
		
		public Node GetFirstNodeInStep(int stepNumber)
		{
			if (stepNumber > NumberOfSteps) return null;
			return nodeList[GetIndexFirstNodeInStep(stepNumber)-1];
		}
		
		public Node GetLastNodeInStep(int stepNumber)
		{
			if (stepNumber > NumberOfSteps) return null;
			return nodeList[GetIndexLastNodeInStep(stepNumber)-1];
		}
		
		public int GetStepFromNode(Node node)
		{
			return node.StepNumber;
		}
		
		public Node GetUpDependantNode(Node node)
		{
			int idx = node.NodeIndex + CalculateTotalNodes(node.StepNumber) - CalculateTotalNodes(node.StepNumber - 1);
            if (NodeCount < idx) return null;
			return nodeList[idx-1];
		}
		
		public Node GetDownDependantNode(Node node)
		{
			int idx = node.NodeIndex + CalculateTotalNodes(node.StepNumber) - CalculateTotalNodes(node.StepNumber - 1) + 1;
            if (NodeCount < idx) return null;
			return nodeList[idx-1];
		}
		
		public IEnumerable<Node> IterateNodesOfStep(int stepNumber)
		{
			int firstNode = CalculateTotalNodes(stepNumber-1)+1;
			int lastNode = CalculateTotalNodes(stepNumber);
			for (int idx = firstNode; idx <= lastNode; idx++) {
				yield return nodeList[idx-1];
			}
		}
			
		public class Node
		{
			public int NodeIndex { get; set; }
			public int StepNumber { get; set; }
			
			public double StockPrice { get; set; }
            public double OptionPrice { get; set; }
            
			public double OptionBeta { get; set; }
			
			public double Rc { get; set; }
            public double UpwardProb { get; set; }
            public double ReplPortDelta { get; set; }
            public double ReplPortBond { get; set; }
		}
	}
}


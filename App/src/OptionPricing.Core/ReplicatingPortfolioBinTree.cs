using System;

namespace OptionPricing.Core
{
	public class ReplicatingPortfolioBinTree : BinTree
	{
		public ReplicatingPortfolioBinTree (int numberOfSteps) : base(numberOfSteps)
		{
		}
		
		public override void CalculateOptionPrices()
		{
			// First, calculate last step prices
			foreach (var node in IterateNodesOfStep(NumberOfSteps)) {
				node.OptionPrice = Math.Max(node.StockPrice - StrikePrice, 0);
			}
			
			double rfTimesDelta = (double)Math.Exp((double)RiskFree*(double)DeltaT);
			
			// Now, in previous steps... 
			for (int step = NumberOfSteps-1; step >= 1; step--) {
				// calculate the beta and then the option price
				foreach (var node in IterateNodesOfStep(step)) {
					
					var upDep = GetUpDependantNode(node);
					var downDep = GetDownDependantNode(node);
					
					// Delta OK!
					double delta = (upDep.OptionPrice - downDep.OptionPrice) / 
									(upDep.StockPrice - downDep.StockPrice);
					
					// Bond OK!
					double bond = (Mu * downDep.OptionPrice - De * upDep.OptionPrice) /
									((Mu-De)*rfTimesDelta);
					/*
					node.ReplPortDelta = delta;
					node.ReplPortBond = bond;
					*/
					node.OptionPrice = delta * node.StockPrice + bond;
				}
			}
		}
	}
}


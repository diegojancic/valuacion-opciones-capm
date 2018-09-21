using System;

namespace OptionPricing.Core
{
	public class RiskNeutralBinTree : BinTree
	{
		public RiskNeutralBinTree (int numberOfSteps) : base(numberOfSteps)
		{
		}
		
		public override void CalculateOptionPrices()
		{
			// First, calculate last step prices
			foreach (var node in IterateNodesOfStep(NumberOfSteps)) {
				node.OptionPrice = Math.Max(node.StockPrice - StrikePrice, 0);
			}
			
			
			double p = ((double)Math.Exp((double)RiskFree*(double)DeltaT)-De)/(Mu-De);
					
			// Now, in previous steps... 
			for (int step = NumberOfSteps-1; step >= 1; step--) {
				// calculate the beta and then the option price
				foreach (var node in IterateNodesOfStep(step)) {
					
					var upDep = GetUpDependantNode(node);
					var downDep = GetDownDependantNode(node);
					
					//node.UpwardProb = p;
					
					node.OptionPrice = (double)Math.Exp((double)-RiskFree*(double)DeltaT) * 
						(p*upDep.OptionPrice+(1-p)*downDep.OptionPrice);
					
					//node.OptionPrice = delta * node.StockPrice + bond; ==>> WORKS! =)
				}
			}
		}
	}
}


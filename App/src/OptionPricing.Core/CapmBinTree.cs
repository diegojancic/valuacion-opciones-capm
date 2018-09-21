using System;

namespace OptionPricing.Core
{
	public class CapmBinTree : BinTree
	{
		public CapmBinTree (int numberOfSteps) : base(numberOfSteps)
		{
		}
		
		public override void CalculateOptionPrices()
		{
            var _de = De;
            var _mu = Mu;
            var _deltaT = DeltaT;

			// First, calculate last step prices
			foreach (var node in IterateNodesOfStep(NumberOfSteps)) {
				node.OptionPrice = Math.Max(node.StockPrice - StrikePrice, 0);
			}

            double rfTimesDelta = (double)Math.Exp((double)RiskFree * (double)_deltaT);
			
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
                    double bond = (_mu * downDep.OptionPrice - _de * upDep.OptionPrice) /
                                    ((_mu - _de) * rfTimesDelta);
					
					double totalReplPortfolio = (delta * node.StockPrice + bond);
                    double optionBeta = 0;
                    if (totalReplPortfolio < 0)
                    {
                        // Do to precision concerns, if the portfolio value is zero, the beta tends to inf
                        optionBeta = double.MaxValue;
                    }
                    else if (totalReplPortfolio != 0)
                    {
                        optionBeta = delta * node.StockPrice / totalReplPortfolio * StockBeta;
                    }

                    node.OptionBeta = optionBeta;

                    double rc = RiskFree + optionBeta * MarketRiskPremium;
					double rs = RiskFree + StockBeta * MarketRiskPremium;
                    double p = (rs * _deltaT - _de + 1) / (_mu - _de);
					
					node.Rc = rc;
					
					node.UpwardProb = p;
					node.ReplPortDelta = delta;	// OK!
					node.ReplPortBond = bond;	// OK!
                    
                    node.OptionPrice = (p * upDep.OptionPrice + (1 - p) * downDep.OptionPrice) / (1 + rc * _deltaT);
				}
			}
		}
	}
}


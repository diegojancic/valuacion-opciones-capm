using System;
using OptionPricing.Core;
using System.IO;
using System.Collections.Generic;

namespace OptionPricing.UI.Cons
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//IterateByAmountOfSteps();
            //SingleConvergeTest();
            //BetaGraph1();
			//BetaGraph2();
			//BetaGraph3();
			
			SmallTreeBetaGraph();
			
			
			Console.ReadKey();
			
		}
		
		/// <summary>
		/// Datos de las betas segun el percentil del valor del arbol (beta en 10% sup, 20% sup, ...)
		/// </summary>
        private static void BetaGraph1()
        {
            int numberOfSteps = 2000;
            double InitialStockPrice = 100;
            double Volatility = 0.2;
            double StrikePrice = 105;
            double TotalTimeYears = 0.5;
            double RiskFree = 0.03;
            double MarketRiskPremium = 0.06;
            double StockBeta = 1;

            var tree = new CapmBinTree(numberOfSteps);

            tree.InitialStockPrice = InitialStockPrice;
            tree.Volatility = Volatility;
            tree.StrikePrice = StrikePrice;
            tree.TotalTimeYears = TotalTimeYears;
            tree.RiskFree = RiskFree;
            tree.MarketRiskPremium = MarketRiskPremium;
            tree.StockBeta = StockBeta;

            Console.WriteLine("Calculating stock prices...");
            tree.CalculateStockPrices();

            Console.WriteLine("Calculating option prices...");
            tree.CalculateOptionPrices();

            Console.WriteLine("Outputing the betas...");

            int divisions = 70;
            using (var writer = new StreamWriter("beta_graph_1.csv", false))
            {
                writer.Write("Step");
                for (int i = 1; i <= divisions; i++)
                {
                    writer.Write(",n" + i);
                }
                writer.WriteLine();


                var indexes = new List<int>(divisions);
                for (int step = 1; step < numberOfSteps; step++)
                {
                    int nodeCount = BinTree.GetIndexLastNodeInStep(step) - BinTree.GetIndexFirstNodeInStep(step);
                    indexes.Clear();
                    for (int stp = 0; stp < divisions; stp++)
                    {
                        indexes.Add(nodeCount * stp / divisions);
                    }
                    
                    writer.Write(step + ",");

                    int i = 0;
                    foreach (var node in tree.IterateNodesOfStep(step))
                    {
                        if (indexes.Contains(i))
                        {
                            if (node.OptionBeta <= 0) break;
                            writer.Write(node.OptionBeta + ",");
                        }
                        i++;
                    }
                    writer.WriteLine();
                }
            }

            Console.WriteLine("Done. See: beta_graph_1.csv");
        }
		
		/// <summary>
		/// Grafico en 3D de la beta en cada nodo, de forma que salga un cuadrado
		/// correspondiente al centro del arbol
		/// </summary>
        private static void BetaGraph2()
        {
            int numberOfSteps = 100; //7000
            double InitialStockPrice = 100;
            double Volatility = 0.2;
            double StrikePrice = 105;
            double TotalTimeYears = 0.5;
            double RiskFree = 0.03;
            double MarketRiskPremium = 0.06;
            double StockBeta = 1;
			
			
			const int minPrice = 60;
			const int maxPrice = 140;
			const int showOneEveryNSteps = 2; //50
			const int showOneEveryNNodes = 1; //4

            var tree = new CapmBinTree(numberOfSteps);

            tree.InitialStockPrice = InitialStockPrice;
            tree.Volatility = Volatility;
            tree.StrikePrice = StrikePrice;
            tree.TotalTimeYears = TotalTimeYears;
            tree.RiskFree = RiskFree;
            tree.MarketRiskPremium = MarketRiskPremium;
            tree.StockBeta = StockBeta;

            Console.WriteLine("Calculating stock prices...");
            tree.CalculateStockPrices();

            Console.WriteLine("Calculating option prices...");
            tree.CalculateOptionPrices();

            Console.WriteLine("Outputing the betas...");
			
			
            //OutputTree(tree, delegate(BinTree.Node node) { return node.StockPrice.ToString("##0.##"); });	return;
			
						
            Console.WriteLine("The format of the output file will be (col and row headers excl):");
			Console.WriteLine("      P=120 P=119 ... P=100 ... P=80");
			Console.WriteLine("t=1     B     B         B         B ");
			Console.WriteLine("t=2     B     B         B         B ");
			Console.WriteLine("t=...   B     B         B         B ");
			Console.WriteLine("(where B is the beta of that node)");
			
			List<double> pricesSaved = new List<double>(500);
			List<int> stepNumbers = new List<int>(500);
			
            using (var writer = new StreamWriter("beta_graph_2.matrix", false))
            {
				bool headersPrinted = false;
				bool pricesAlreadyStoredInList = false;
				
				// only output half of the nodes, para que los nodos coincidan (por ser recombinante)!
                for (int step = 1; step < numberOfSteps; step+=showOneEveryNSteps)
                {
					int idx = 0;
					if (!headersPrinted)
					{
						// Skip first steps where are only a few nodes.
						if (tree.GetFirstNodeInStep(step).StockPrice <= maxPrice) continue;
						if (tree.GetLastNodeInStep(step).StockPrice >= minPrice) continue;
	                    
						// I cannot do the last node index less the first index because it
						// might happen that the upper node is OK while the bottom node
						// is still not down enough
						int inboundNodes = 0;
						foreach (var node in tree.IterateNodesOfStep(step))
	                    {
							// skip higher order and less order nodes
							if (node.StockPrice > maxPrice) continue;
							if (node.StockPrice < minPrice) continue;
							
							if ((idx++ % showOneEveryNNodes) != 0) continue;
							
							inboundNodes++;
						}
						
						int rows = (int)Math.Ceiling((double)(numberOfSteps - step + 1)/showOneEveryNSteps);
						PrintMatrixFileHeaders(writer, "betas", rows, inboundNodes);
						
						headersPrinted = true;
					}
					
					bool firstNode = true;
					idx = 0;
                    foreach (var node in tree.IterateNodesOfStep(step))
                    {
						if (firstNode) stepNumbers.Add(step);
						firstNode = false;
						
						// skip higher order and less order nodes
						if (node.StockPrice > maxPrice) continue;
						if (node.StockPrice < minPrice) continue;
						
						if ((idx++ % showOneEveryNNodes) != 0) continue;
							
						if (!pricesAlreadyStoredInList)
						{
							pricesSaved.Add(node.StockPrice);
						}
						
                        if (node.OptionBeta <= 0 || node.OptionBeta > 999999) node.OptionBeta = 999999;
                        writer.Write(" " + node.OptionBeta.ToString("0.##"));
                    }
					if (!firstNode) writer.WriteLine();
					
					if (pricesSaved.Count > 0) pricesAlreadyStoredInList = true;
                }
				writer.Flush();
            }

            Console.WriteLine("Output saved. See: beta_graph_2.matrix");
			Console.WriteLine("Now saving price list and step list.");
			
			
            using (var writer = new StreamWriter("beta_graph_2-prices.matrix", false))
            {
				PrintMatrixFileHeaders(writer, "prices", pricesSaved.Count, 1);
				foreach (var p in pricesSaved)
				{
					writer.WriteLine(" " + p.ToString("0.##"));
				}
			}
			
            using (var writer = new StreamWriter("beta_graph_2-steps.matrix", false))
            {
				PrintMatrixFileHeaders(writer, "steps", stepNumbers.Count, 1);
				foreach (var p in stepNumbers)
				{
					writer.WriteLine(" " + p);
				}
			}
			
            Console.WriteLine("Now done! See: <beta_graph_2-prices.matrix> and <beta_graph_2-steps.matrix>");
        }
		
		/// <summary>
		/// Grafico en 3D de la beta en cada nodo. Grafica todo el arbol. 3D TREE!
		/// </summary>
        private static void BetaGraph3()
        {
			const string graphPrefix = "beta_graph_3";
			
            int numberOfSteps = 200; //7000
            double InitialStockPrice = 100;
            double Volatility = 0.2;
            double StrikePrice = 105;
            double TotalTimeYears = 0.5;
            double RiskFree = 0.03;
            double MarketRiskPremium = 0.06;
            double StockBeta = 1;
			
			const int showOneEveryNSteps = 2; //50 (QUE SEA PAR!)
			const int showOneEveryNNodes = 1; //4 (QUE SEA PAR!)

            var tree = new CapmBinTree(numberOfSteps);

            tree.InitialStockPrice = InitialStockPrice;
            tree.Volatility = Volatility;
            tree.StrikePrice = StrikePrice;
            tree.TotalTimeYears = TotalTimeYears;
            tree.RiskFree = RiskFree;
            tree.MarketRiskPremium = MarketRiskPremium;
            tree.StockBeta = StockBeta;

            Console.WriteLine("Calculating stock prices...");
            tree.CalculateStockPrices();

            Console.WriteLine("Calculating option prices...");
            tree.CalculateOptionPrices();

            Console.WriteLine("Outputing the betas...");
			
			
            //OutputTree(tree, delegate(BinTree.Node node) { return node.StockPrice.ToString("##0.##"); });	return;
			
						
            Console.WriteLine("The format of the output file will be (col and row headers excl):");
			Console.WriteLine("      P=120 P=119 ... P=100 ... P=80");
			Console.WriteLine("t=1     B     B         B         B ");
			Console.WriteLine("t=2     B     B         B         B ");
			Console.WriteLine("t=...   B     B         B         B ");
			Console.WriteLine("(where B is the beta of that node)");
			
			List<double> pricesSaved = new List<double>(500);
			List<int> stepNumbers = new List<int>(500);
			
            using (var writer = new StreamWriter(graphPrefix + ".matrix", false))
            {
				int lastStepNumber;
				
				// Find the last step, and the steps to print
				for (int s = 1 ; s<numberOfSteps ; s+=showOneEveryNSteps)
				{
					stepNumbers.Add(s);
				}
				lastStepNumber = stepNumbers[stepNumbers.Count - 1];
				
				// Find the prices that will be outputed
				int idx = 0;
				foreach (var node in tree.IterateNodesOfStep(lastStepNumber))
                {
					if ((idx++ % showOneEveryNNodes) != 0) continue;
					pricesSaved.Add(node.StockPrice);
				}
				
				PrintMatrixFileHeaders(writer, "betas", stepNumbers.Count, pricesSaved.Count);
						
				// only output half of the nodes, para que los nodos coincidan (por ser recombinante)!
				int totalPrices = pricesSaved.Count;
				double beta;
                foreach (int step in stepNumbers)
                {
					var nodes = new List<BinTree.Node>(tree.IterateNodesOfStep(step));
					var nodesMatched = nodes.FindAll(n => pricesSaved.FindIndex(p => AreDoublesEqual(p, n.StockPrice)) >= 0);
					
					int nodesRemainingEachSide = (totalPrices - nodesMatched.Count)/2;
					
					// Print left missing nodes
					for (var n = 0 ; n<nodesRemainingEachSide; n++)
					{
						writer.Write(" 999999");
					}
					
					// Print existent nodes
                	foreach (var node in nodesMatched)
					{
						beta = node.OptionBeta;
						if (beta <= 0 || beta > 999999) beta = 999999;
                   		writer.Write(" " + beta.ToString("0.##"));
					}
					
					// Print right missing nodes
					for (var n = 0 ; n<nodesRemainingEachSide; n++)
					{
						writer.Write(" 999999");
					}
					
					writer.WriteLine();
                }
				writer.Flush();
            }

            Console.WriteLine("Output saved. See: " + graphPrefix + ".matrix");
			Console.WriteLine("Now saving price list and step list.");
			
			
            using (var writer = new StreamWriter(graphPrefix + "-prices.matrix", false))
            {
				PrintMatrixFileHeaders(writer, "prices", pricesSaved.Count, 1);
				foreach (var p in pricesSaved)
				{
					writer.WriteLine(" " + p.ToString("0.##"));
				}
			}
			
            using (var writer = new StreamWriter(graphPrefix + "-steps.matrix", false))
            {
				PrintMatrixFileHeaders(writer, "steps", stepNumbers.Count, 1);
				foreach (var p in stepNumbers)
				{
					writer.WriteLine(" " + p);
				}
			}
			
            Console.WriteLine("Now done! See: <" + graphPrefix + "-prices.matrix> and <" + graphPrefix + "-steps.matrix>");
        }
		
		private static void IterateByAmountOfSteps()
		{
			double InitialStockPrice = 100;
			double Volatility = 0.2;
			double StrikePrice = 105;
			double TotalTimeYears = 0.5;
			double RiskFree = 0.03;
			double MarketRiskPremium = 0.06;
			double StockBeta = 1;
			double callPriceBS = BlackScholes.CallPrice((double)InitialStockPrice,
														(double)StrikePrice,
														(double)TotalTimeYears,
														(double)RiskFree,
														(double)Volatility);
						
			int minNumberOfSteps = 7000;
			int maxNumberOfSteps = 9000;
			
			bool createHeaders = !File.Exists("output.csv");
			
						
			using (var writer = new StreamWriter("output.csv", true))
			{
				if (createHeaders)
				{
					writer.WriteLine("Steps,CallValueBS,CallValueCAPM");
                }

                var tree = new CapmBinTree(maxNumberOfSteps);

                tree.InitialStockPrice = InitialStockPrice;
                tree.Volatility = Volatility;
                tree.StrikePrice = StrikePrice;
                tree.TotalTimeYears = TotalTimeYears;
                tree.RiskFree = RiskFree;
                tree.MarketRiskPremium = MarketRiskPremium;
                tree.StockBeta = StockBeta;					
					
                for (int steps = minNumberOfSteps; steps < maxNumberOfSteps; steps += (int)Math.Pow(Math.Log10(steps), 3))
                {
                    tree.NumberOfSteps = steps;
                    tree.ResetTreeParameterCalcs();
					tree.CalculateStockPrices();
					tree.CalculateOptionPrices();
					
					writer.WriteLine(steps + "," + callPriceBS.ToString("##0.#############################") + 
						", " + tree.GetRootNode().OptionPrice.ToString("##0.#############################"));
					
					writer.Flush();
					
					Console.WriteLine("Step Number: " + steps);
				}
			}

		}
		
		private static void SingleConvergeTest()
		{
			int numberOfSteps = 2000;
			var tree = new CapmBinTree(numberOfSteps);
			
			tree.InitialStockPrice = 100;
			tree.Volatility = 0.2;
			tree.StrikePrice = 105;
			tree.TotalTimeYears = 0.5;
			tree.RiskFree = 0.03;
			tree.MarketRiskPremium = 0.06;
			tree.StockBeta = 1;
			// B&S = call: 4.17 (con strike =105)
			
			double callPriceBS = BlackScholes.CallPrice((double)tree.InitialStockPrice,
														(double)tree.StrikePrice,
														(double)tree.TotalTimeYears,
														(double)tree.RiskFree,
														(double)tree.Volatility);
			Console.WriteLine("call B&S: " + callPriceBS.ToString("##0.######"));
			
			
			tree.CalculateStockPrices();
			tree.CalculateOptionPrices();
		
            Console.WriteLine();
            //OutputTree(tree, delegate(BinTree.Node node) { return node.StockPrice.ToString("##0.##"); });
            Console.WriteLine("call: " + tree.GetRootNode().OptionPrice.ToString("##0.######"));
		}
		
		public static void SmallTreeBetaGraph()
		{
			int numberOfSteps = 10;
			var tree = new CapmBinTree(numberOfSteps);
			
			tree.InitialStockPrice = 100;
			tree.Volatility = 0.2;
			tree.StrikePrice = 105;
			tree.TotalTimeYears = 0.5;
			tree.RiskFree = 0.03;
			tree.MarketRiskPremium = 0.06;
			tree.StockBeta = 1;
			// B&S = call: 4.17 (con strike =105)
			
			double callPriceBS = BlackScholes.CallPrice((double)tree.InitialStockPrice,
														(double)tree.StrikePrice,
														(double)tree.TotalTimeYears,
														(double)tree.RiskFree,
														(double)tree.Volatility);
			Console.WriteLine("call B&S: " + callPriceBS.ToString("##0.######"));
			
			
			tree.CalculateStockPrices();
			tree.CalculateOptionPrices();
		
            Console.WriteLine();
            OutputTree(tree, delegate(BinTree.Node node) { 
					return (-node.ReplPortBond/(node.StockPrice*node.ReplPortDelta+node.ReplPortBond)).ToString("##0.##"); 
					});
		}
		
		private delegate string GetNodeValue(BinTree.Node node);
		private static void OutputTree(BinTree tree, GetNodeValue func)
		{
			var initialPos = Console.CursorTop+1;
			var numberOfSteps = tree.NumberOfSteps;
				
			// cannot print more than 10 steps
			if (numberOfSteps>10) numberOfSteps = 10;
			
			for (int s = 1; s <= numberOfSteps; s++) {
				
				int stepNodeIndex = 0;
				foreach (var node in tree.IterateNodesOfStep(s))
				{
					try{
					Console.SetCursorPosition((s-1)*8, initialPos + numberOfSteps - s + stepNodeIndex);
					Console.Write(func(node));
					}
					catch
					{
						// ignore
					}
					
					stepNodeIndex+=2;
				}
			}
		}
		
		private static void PrintMatrixFileHeaders(StreamWriter writer, string matrixName, int rows, int columns)
		{
			// This is the header file format:
			// # Created by Octave 3.2.4, Wed Jan 18 01:11:20 2012 ART <diego@notebook2>
			// # name: data
			// # type: matrix
			// # rows: 1370
			// # columns: 3
			
			writer.WriteLine("# Created by Octave 3.2.4, Wed Jan 18 01:11:20 2012 ART <diego@notebook2>");
			writer.WriteLine("# name: " + matrixName);
			writer.WriteLine("# type: matrix");
			writer.WriteLine("# rows: " + rows);
			writer.WriteLine("# columns: " + columns);
		}
		
		private static bool AreDoublesEqual(double a, double b)
		{
			return Math.Abs(a - b) < 0.00000001;
		}
		
	}
}


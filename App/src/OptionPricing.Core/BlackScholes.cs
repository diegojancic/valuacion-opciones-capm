using System;

namespace OptionPricing.Core
{
	// Source adapted from: http://www.espenhaug.com/black_scholes.html
	public static class BlackScholes
	{
		/* The Black and Scholes (1973) Stock option formula
		 * C# Implementation
		*/
		public static double CallPrice(double stockPrice, double strikePrice, double timeYears, double riskFreeRate, double volatility)
		{
			double d1 = 0.0;
			double d2 = 0.0;
			double dBlackScholes = 0.0;
			
			d1 = (Math.Log(stockPrice / strikePrice) + (riskFreeRate + volatility * volatility / 2.0) * timeYears) / (volatility * Math.Sqrt(timeYears));
			d2 = d1 - volatility * Math.Sqrt(timeYears);
			dBlackScholes = stockPrice * CND(d1) - strikePrice * Math.Exp(-riskFreeRate * timeYears) * CND(d2);
			return dBlackScholes;
		}
		
		public static double PutPrice(double stockPrice, double strikePrice, double timeYears, double riskFreeRate, double volatility)
		{
			double d1 = 0.0;
			double d2 = 0.0;
			double dBlackScholes = 0.0;
			
			d1 = (Math.Log(stockPrice / strikePrice) + (riskFreeRate + volatility * volatility / 2.0) * timeYears) / (volatility * Math.Sqrt(timeYears));
			d2 = d1 - volatility * Math.Sqrt(timeYears);
			dBlackScholes = strikePrice * Math.Exp(-riskFreeRate * timeYears) * CND(-d2) - stockPrice * CND(-d1);				
			return dBlackScholes;
		}
		
		private static double CND(double X)
		{
			double L = 0.0;
			double K = 0.0;
			double dCND = 0.0;
			const double a1 = 0.31938153; 
			const double a2 = -0.356563782; 
			const double a3 = 1.781477937;
			const double a4 = -1.821255978;
			const double a5 = 1.330274429;
			L = Math.Abs(X);
			K = 1.0 / (1.0 + 0.2316419 * L);
			dCND = 1.0 - 1.0 / Math.Sqrt(2 * Convert.ToDouble(Math.PI.ToString())) * 
				Math.Exp(-L * L / 2.0) * (a1 * K + a2 * K  * K + a3 * Math.Pow(K, 3.0) + 
				a4 * Math.Pow(K, 4.0) + a5 * Math.Pow(K, 5.0));
			
			if (X < 0) 
			{
				return 1.0 - dCND;
			}
			else
			{
				return dCND;
			}
		}
	}
}


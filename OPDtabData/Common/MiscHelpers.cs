using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class MiscHelpers
	{
		/// <summary>
		/// Number of decimal places used in inexact calculations.
		/// This specifically concerns the calculation of
		/// average scores from the individual scores awarded by
		/// multiple judges. Must be non-negative.
		/// </summary>
		public const int Accuracy = 2;
		
		public static void PrintList<T>(List<T> list) {
			foreach(T item in list)
				Console.WriteLine(item);
		}
			
		public static string SanitizeString(string str) {
			return String.Join(" ", StringToWords(str));
		}
		
		public static string[] StringToWords(string str) {
			return str.Split(new char[] {' '},
				StringSplitOptions.RemoveEmptyEntries);
		}
		
		public static int CalcSum(ICollection<int> l) {
			int sum = 0;
			foreach(int i in l) {
				if(i<0)
					return -1;
				else
					sum += i;
			}
			return sum;
		}
		
		public static double CalcSum(ICollection<double> values) {
			var sum = 0.0;
			foreach (var i in values)
				if (i < 0)
					return -1;
				else
					sum += i;
			
			return sum;
		}
			   
		public static string IntArrayToString(List<int> l, string concat) {
			List<string> tmp = new List<string>();
			foreach(int i in l) 
				tmp.Add(IntToStr(i));
			return String.Join(concat,tmp.ToArray());
		}
		
		public static string IntToStr(int i) {
			return i<0 ? "?" : i.ToString();
		}
		
		public static string DoubleToStr(double d) {
			return d < 0 ? "?" : d.ToString ("F" + Accuracy);
		}
		
		public static double CalcAverage(ICollection<int> l) {
			int sum = CalcSum(l);	
			int count = l.Count;
			if(sum < 0 || count == 0)
				return -1;
			else
				return Math.Round((double)sum/count, Accuracy, MidpointRounding.AwayFromZero);
		}
		
		public static double CalcAverage (ICollection<double> values) {
			var count = values.Count;
			if (count == 0)
				return -1;
			
			var sum = CalcSum (values);
			if (sum < 0)
				return -1;
			
			return Math.Round (sum / count, Accuracy, MidpointRounding.AwayFromZero);
		}
		
		public static double CalcExactAverage(ICollection<int> l) {
			int sum = CalcSum(l);	
			if(sum<0 || l.Count==0)
				return -1;
			else
				return (double)sum/l.Count;	
		}
		
		public static double CalcExactAverage(ICollection<double> l) {
			if(l.Count==0)
				return double.NaN;
			double sum = 0;
			foreach(double i in l) 
				sum += i;
			return sum/l.Count;
		}
				
		public static string FmtDecimal(double n) {
			return string.Format(
				"{0:F" + OPDtabData.MiscHelpers.Accuracy + "}", n);
		}
		
		public static string FmtDecimal(double n, int alignment) {
			return string.Format(
				"{0," + alignment + ":F" + OPDtabData.MiscHelpers.Accuracy + "}", n);
		}
	}
}
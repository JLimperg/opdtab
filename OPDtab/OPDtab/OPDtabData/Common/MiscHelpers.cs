using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class MiscHelpers
	{
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
			   
		public static string IntArrayToString(List<int> l, string concat) {
			List<string> tmp = new List<string>();
			foreach(int i in l) 
				tmp.Add(IntToStr(i));
			return String.Join(concat,tmp.ToArray());
		}
		
		public static string IntToStr(int i) {
			return i<0 ? "?" : i.ToString();
		}
		
		public static int CalcAverage(ICollection<int> l) {
			int sum = CalcSum(l);		
			if(sum<0 || l.Count==0)
				return -1;
			else
				return (int)Math.Round((double)sum/l.Count, MidpointRounding.AwayFromZero);
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
		
		/*public static double[] CalcMuAndErr(ICollection<double> l) {
			if(l.Count==0)
				return new double[] {-1, -1};
			
		}*/
	}
}


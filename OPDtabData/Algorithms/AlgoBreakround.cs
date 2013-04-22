using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class AlgoBreakround
	{
		static int[][] placing;		
		static int max;
		
		public static int ParsePlacing(string p) {
			// try to parse placing
			string[] tmp = p.Split(new string[] {"\n\n"}, StringSplitOptions.RemoveEmptyEntries);
			placing = new int[tmp.Length][]; 
			max = 0;
			for(int i=0;i<tmp.Length;i++) {
				string[] tmp_ = tmp[i].Split(new string[] {"\n","-"}, StringSplitOptions.RemoveEmptyEntries); 
				if(tmp_.Length % 2 != 0)
					throw new Exception("Rooms of Round "+(i+1)+" not even");
				placing[i] = new int[tmp_.Length];
				for(int j=0;j<tmp_.Length;j++) {
					placing[i][j] = (int)uint.Parse(tmp_[j]);
					max = max<placing[i][j] ? placing[i][j] : max;
				}
			}
			// return number of rounds
			return tmp.Length;
		}
		
		public static List<RoomData>[] Generate(List<TeamData> bestTeams) {
			Console.WriteLine("===== BreakRound started =====");
			Console.WriteLine("Rounds: "+placing.Length);
			
			if(max>bestTeams.Count)
				throw new Exception("Not enough teams in ranking, needed "+max);	
			List<RoomData>[] rounds = new List<RoomData>[placing.Length];
			for(int i=0;i<placing.Length;i++) {
				rounds[i] = new List<RoomData>();
				for(int j=0;j<placing[i].Length/2;j++) {
					rounds[i].Add(new RoomData(j, 
					                           bestTeams[placing[i][2*j]-1], 
					                           bestTeams[placing[i][2*j+1]-1]));	
				}	
			}
			return rounds;
		}		
	}
}


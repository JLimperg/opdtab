using System;
using System.Collections.Generic;
using System.Linq;
using OPDtabData;
namespace OPDtabData
{
	public class Algorithms
	{
		
		public static void RandomizeArray<T>(List<T> arr, Random _random, int count)
    	{
			List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>>();
        	// Add all strings from array
        	// Add new random int each time
        	for(int i=0;i<count;i++) {
            	list.Add(new KeyValuePair<int, T>(_random.Next(), arr[i]));
        	}
				
			list.Sort(delegate(KeyValuePair<int, T> a, KeyValuePair<int, T> b) {
				return a.Key.CompareTo(b.Key);
			});
        	
        	// set values
			int index = 0;
        	foreach (KeyValuePair<int, T> pair in list)
        	{
            	arr[index] = pair.Value;
            	index++;
        	}
    	}
		
		public static void RandomizeArray<T>(List<T> arr, Random _random) {
			RandomizeArray<T>(arr, _random, arr.Count);	
		}
		
		public static int CalcInterRoundConflict(List<RoomData>[] rounds) {
			// calc inter-Round conflicts manually
			// this is expensive...
			int sum=0;
			for(int round=0;round<rounds.Length;round++) {
				foreach(RoomData room in rounds[round]) {
					sum += WorkOnRoom(room, round, rounds);
				}
			}	
			return sum;
		}
		
		static int WorkOnRoom(RoomData room, int round, List<RoomData>[] rounds) {
			int sum=0;
			List<RoundDebater> rd = room.GetRoomMembers();
			// since teamMembers don't conflict each other, 
			// we can start with j=3
			for(int j=3;j<rd.Count;j++) {
				for(int i=0;i<j;i++) {
						if(rd[i] == null)
							goto inner;
						if(rd[j] == null)
							goto outer;
						// teamMembers don't conflict each other
						if(j<6 && i>2)
							continue;
						int nConflicts = SearchConflicts(rounds, round, rd[i], rd[j]);
						int[] penalties = AppSettings.I.GenerateRound.conflictPenalties;
						// factor 2 since reflexivity
						sum += 2*nConflicts*penalties[penalties.Length-1];
					inner:;
					}
				outer:;							
				}
			return sum;
		}
		
		static int SearchConflicts(List<RoomData>[] rounds, int round, RoundDebater rd1, RoundDebater rd2) {
			int conflicts = 0;
			for(int otherRound=0;otherRound<rounds.Length;otherRound++) {
				if(otherRound==round)
					continue;
				foreach(RoomData otherRoom in rounds[otherRound]) {
					List<RoundDebater> rd = otherRoom.GetRoomMembers();
					int n=0;
					foreach(RoundDebater rd_ in rd) {
						if(rd_==null)
							continue;
						if(rd_.Equals(rd1))
							n++;
						if(rd_.Equals(rd2))
							n++;
						if(n==2) {
							conflicts++;
							goto nextRound;
						}
					}
				}
			nextRound: ;				
			}
			return conflicts;
		}
		
	}
}


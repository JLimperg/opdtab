using System;

namespace OPDtabData
{
	public static class AlgoShuffle
	{
		
		public static void Rooms(int randomSeed, RoundData round) {
			Random random = new Random(randomSeed);
			// check structure of round
			bool flag = false;
			int count = 0;
			foreach(RoomData room in round.Rooms) {
				if(!room.IsEmpty) {
					if(flag)
						throw new Exception("Rooms not consecutive non-empty from beginning.");
					else
						count++;
				}
				else
					flag = true;
			}
			if(count==0)
				throw new Exception("No non-empty rooms found.");
			// shuffle rooms from 0 to count-1
			Algorithms.RandomizeArray<RoomData>(round.Rooms, random, count);
			// keep rounddata consistent
			for(int i=0;i<count;i++) {
				round.Rooms[i].Index = i;	
			}
			round.UpdateAllArrays();
		}
		
		public static void GovOpp(int randomSeed, RoundData round) {
			Random random = new Random(randomSeed);
			foreach(RoomData room in round.Rooms) {
				if(random.NextDouble()<0.5) {
					// swap with probability of 1/2...
					TeamData tmp = room.Opp;
					room.Opp = room.Gov;
					room.Gov = tmp;
				}
			}
		}
	}
}


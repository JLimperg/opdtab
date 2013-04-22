using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class AlgoJudges
	{
		static int steps;
		static Random random;
		static int nRooms;
		//static int nMinJudgesPerRoom;
		static List<RoomData> bestConfig;
		static List<RoomData> startConfig;
		static List<RoundDebater> firstJudges;
		static List<RoundDebater> otherJudges;
		static List<RoundDebater> chairJudges;
		static AlgoData data;	
			
		public static void Prepare(int randomSeed, int mcSteps,
		                           RoundData rd) {
			// copy startConfig from roundData
			startConfig = new List<RoomData>();
			foreach(RoomData room in rd.Rooms) 
				startConfig.Add(new RoomData(room, true));		
			
			nRooms = startConfig.Count;
			// determine avail judges from roundData, 
			// only the ones shown in pool
			firstJudges = new List<RoundDebater>();
			otherJudges = new List<RoundDebater>();
			chairJudges = new List<RoundDebater>();
			//int nNotAvail = 0;
			foreach(RoundDebater judge in rd.AllJudges) {
				if(judge.IsShown) {
					switch(judge.JudgeState) {
					case RoundDebater.JudgeStateType.FirstJudge:
						firstJudges.Add(judge);
						break;
					case RoundDebater.JudgeStateType.Judge:
						otherJudges.Add(judge);
						break;
					case RoundDebater.JudgeStateType.Chair:
						chairJudges.Add(judge);
						break;	
					//case RoundDebater.JudgeStateType.NotAvail:
					//	nNotAvail++;
					//	break;
					}					
				}
			}
			//nMinJudgesPerRoom = (rd.AllJudges.Count-nNotAvail)/nRooms; // int division!
			// some other config
			steps = mcSteps;
			random = new Random(randomSeed);			
			// init bestConfig
			bestConfig = DoMonteCarloStep();
		}
		
		
		public static void Generate(object p) {
			data = (AlgoData)p;
			Console.WriteLine("===== Judges started =====");
			Console.WriteLine("Judges: "+firstJudges.Count+" / "
				+otherJudges.Count+" / "+chairJudges.Count);
			OptimizeConfig(); 			
			data.Finished(bestConfig);
			//return  bestConfig;
		}
		
		static void OptimizeConfig() {
		
			// startVal (therefore bestconfig should be well-defined...)
			int val = CalcConflictPenalty(bestConfig);
			Console.WriteLine("Initial config: Total penalty "+val);
			
			if(val>0) {
				for(int i=0;i<steps;i++) {
					List<RoomData> newConfig = DoMonteCarloStep();
					int newVal = CalcConflictPenalty(newConfig);
					if(newVal<val) {
						Console.WriteLine("Step "+i+": Accepted total penalty "+newVal);
						val = newVal;
						bestConfig = newConfig;
					}
					if(newVal==0) {
						Console.WriteLine("INFO: Config without any penalty found.");
						break;
					}	
					data.Update((double)(i+1)/steps, "Best: "+val+" Curr: "+newVal);
				}
			}
			else
				Console.WriteLine("INFO: Config without any penalty given.");
			data.Update(1, "Best: "+val);
		}
		
		static int CalcConflictPenalty(List<RoomData> round) {
			int sum = 0;
			foreach(RoomData room in round) 
				sum += room.CalcTotalPenalty(false, true);
			
			return sum;
		}
		
		static List<RoomData> DoMonteCarloStep() {			
						
			// make a copy of judges arrays and rooms (use startConfig)
			List<RoomData> rooms = new List<RoomData>(nRooms);
			for(int i=0;i<nRooms;i++) 
				rooms.Add(new RoomData(startConfig[i], true));
			
			List<RoundDebater> first = new List<RoundDebater>(firstJudges.Count);
			List<RoundDebater> other = new List<RoundDebater>(otherJudges.Count);
			List<RoundDebater> chair = new List<RoundDebater>(chairJudges.Count);
			foreach(RoundDebater judge in firstJudges)
				first.Add(new RoundDebater(judge, true));
			foreach(RoundDebater judge in otherJudges)
				other.Add(new RoundDebater(judge, true));
			foreach(RoundDebater judge in chairJudges)
				chair.Add(new RoundDebater(judge, true));
			
			// fill chairs and firstjudges
			// we use a lot of arrays to cover all possible cases...
			// shuffle the available firstjudges and chair
			Algorithms.RandomizeArray<RoundDebater>(first, random);
			Algorithms.RandomizeArray<RoundDebater>(chair, random);
			
			// determine rooms to be filled
			List<int> indicesFirst = new List<int>();
			List<int> indicesChair = new List<int>();
			
			for(int i=0;i<nRooms;i++) {
				if(rooms[i].Chair==null) 
					indicesChair.Add(i);
				if(rooms[i].Judges.Count==0) // no judges => first judge is to be filled 
					indicesFirst.Add(i);
			}
			
			Algorithms.RandomizeArray<int>(indicesChair, random);
			Algorithms.RandomizeArray<int>(indicesFirst, random);
			
			foreach(int i in indicesFirst) {
				if(first.Count==0)
					break;
				rooms[i].Judges.Add(first[0]);
				first.RemoveAt(0);
			}
			foreach(int i in indicesChair) {
				if(chair.Count==0)
					break;
				rooms[i].Chair = chair[0];
				chair.RemoveAt(0);
			}
			
			// put rest judges in other and shuffle
			other.AddRange(first);
			other.AddRange(chair);
			Algorithms.RandomizeArray<RoundDebater>(other, random);
			
			
			// fill rooms with judges, first the ones with too little judges
			//bool flag = true;				
			while(other.Count>0) {
				
				// choose which rooms to fill, first the ones with 
				// too little judges
				List<int> indicesOther = new List<int>(rooms.Count); 
				// determine minimum number of judges in room
				int minNumOfJudges = rooms[0].Judges.Count;
				for(int k=1;k<rooms.Count;k++) 
					if(minNumOfJudges>rooms[k].Judges.Count)
						minNumOfJudges = rooms[k].Judges.Count;
				// add all rooms with that minimum number
				for(int k=0;k<rooms.Count;k++) 
					if(minNumOfJudges==rooms[k].Judges.Count)
						indicesOther.Add(k);				
				
				/*if(flag) {
					indicesOther = new List<int>(rooms.Count);
					flag = false;
					for(int k=0;k<rooms.Count;k++) {
						if(rooms[k].Judges.Count<nMinJudgesPerRoom) {
							flag = true;
							indicesOther.Add(k);
						}	
					}
				}
				else {
					indicesOther = new List<int>(System.Linq.Enumerable.Range(0, rooms.Count));
				}*/
				
				// shuffle!
				Algorithms.RandomizeArray<int>(indicesOther, random);
				// fill 
				while(other.Count>0 && indicesOther.Count>0) {
					rooms[indicesOther[0]].Judges.Add(other[0]);
					indicesOther.RemoveAt(0);
					other.RemoveAt(0);
				}
			}	
			
			return rooms;
		}
		
	}
}


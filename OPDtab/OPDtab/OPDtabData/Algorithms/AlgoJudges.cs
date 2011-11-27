using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class AlgoJudges
	{
		static int steps;
		static Random random;
		static int nRooms;
		static int nMinJudgesPerRoom;
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
			nMinJudgesPerRoom = rd.AllJudges.Count/nRooms; // int division!
			// determine avail judges from roundData, 
			// only the ones shown in pool
			firstJudges = new List<RoundDebater>();
			otherJudges = new List<RoundDebater>();
			chairJudges = new List<RoundDebater>();
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
					}
				}
			}
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
				if(rooms[i].Judges.Count==0) 
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
			int j = 0;
			while(other.Count>0) {
				// choose which room to fill
				bool flag = true;
				for(int k=0;k<rooms.Count;k++) {
					if(rooms[k].Judges.Count<nMinJudgesPerRoom) {
						flag = false;
						break;
					}	
				}
				// fill 
				if(rooms[j].Judges.Count<nMinJudgesPerRoom || flag) {
					rooms[j].Judges.Add(other[0]);
					other.RemoveAt(0);
				}
				// always go to next room, cyclic
				j = (j+1) % nRooms;   
			}	
			
			return rooms;
		}
		
	}
}


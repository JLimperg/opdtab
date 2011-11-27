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
		static List<RoundDebater> availJudges; 
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
			availJudges = new List<RoundDebater>();
			foreach(RoundDebater judge in rd.AllJudges) {
				if(judge.IsShown && judge.JudgeAvail) {
					availJudges.Add(judge);	
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
			Console.WriteLine("Available judges: "+availJudges.Count);
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
			
			// shuffle the available judges
			Algorithms.RandomizeArray<RoundDebater>(availJudges, random);
			
			// make a copy of availJudges and rooms (use startConfig)
			List<RoomData> rooms = new List<RoomData>(nRooms);
			for(int i=0;i<nRooms;i++) 
				rooms.Add(new RoomData(startConfig[i], true));
			
			List<RoundDebater> judges = new List<RoundDebater>(availJudges.Count);
			foreach(RoundDebater judge in availJudges)
				judges.Add(new RoundDebater(judge, true));
			
			// fill rooms with judges
			int j = 0;
			while(judges.Count>0) {
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
					rooms[j].Judges.Add(judges[judges.Count-1]);
					judges.RemoveAt(judges.Count-1);
				}
				// always go to next room, cyclic
				j = (j+1) % nRooms;   
			}	
			return rooms;
		}
		
	}
}


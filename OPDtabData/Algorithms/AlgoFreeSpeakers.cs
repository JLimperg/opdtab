using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class AlgoFreeSpeakers
	{
		static int steps;
		static Random random;
		static int nRooms;
		static List<RoomData> bestConfig;
		static List<RoomData> startConfig;
		static List<RoundDebater> availSpeakers; 
		static AlgoData data;		
			
		public static void Prepare(int randomSeed, int mcSteps,
		                           RoundData roundData, 
								   List<RoundDebater> bestSpeakers) {
			// copy startConfig from given/current roundData
			startConfig = new List<RoomData>();
			availSpeakers = new List<RoundDebater>();
			foreach(RoomData room in roundData.Rooms) { 
				// only consider non-empty
				if(room.IsEmpty)
					continue;
				// already set FreeSpeakers are added to 
				// availSpeakers (no ordering by Ranking)
				foreach(RoundDebater rd in room.FreeSpeakers)
					availSpeakers.Add(rd);
				// make a hard copy, and clear freespeakers
				RoomData newRoom = new RoomData(room, true);
				newRoom.FreeSpeakers.Clear();
				startConfig.Add(newRoom);		
			}
			nRooms = startConfig.Count;
			if(nRooms==0)
				throw new Exception("No non-empty Rooms found.");
			
			// determine avail teams from roundData, 
			// only the ones completely shown in pool
			// and ordered by ranking!
			
			foreach(RoundDebater rd in bestSpeakers) {
				TeamData td = roundData.AllTeams.Find(delegate(TeamData td_) {
					return td_.Equals(rd.Role.TeamName);	
				});
				if(td==null)
					continue;
				// check if debater is shown!
				foreach(RoundDebater rd_ in td)
					if(rd.Equals(rd_) && !rd_.IsShown)
						goto outer;		
				// stop if we have enough free speakers
				if(availSpeakers.Count==3*nRooms)
					break;
				// debater is shown in pool, so add it
				availSpeakers.Add(rd);
				
			outer: ;
			}
			
			if(availSpeakers.Count<3*nRooms)
				throw new Exception("Found "+availSpeakers.Count+" available Speakers, not enough for "+nRooms+" rooms.");
			
			// sort availSpeakers again by Ranking, since the ones already set in rooms come first in availSpeakers
			// ByRanking relies on correct ordering in availSpeakers
			availSpeakers.Sort(delegate(RoundDebater rd1, RoundDebater rd2) {
				int i1 = bestSpeakers.FindIndex(delegate(RoundDebater rd) {
					return rd.Equals(rd1);	
				});
				int i2 = bestSpeakers.FindIndex(delegate(RoundDebater rd) {
					return rd.Equals(rd2);	
				});
				return i1.CompareTo(i2);
			});			
			// some other config, not used by ByRanking
			steps = mcSteps;
			random = new Random(randomSeed);
		}
		
		public static List<RoomData> ByRanking() {
			// no MonteCarlo optimization, simply place freespeakers by ranking
			bestConfig = new List<RoomData>(nRooms);	
			for(int i=0;i<nRooms;i++) {
				RoomData newRoom = new RoomData(startConfig[i], true);
				for(int j=0;j<3;j++)
					newRoom.FreeSpeakers.Add(availSpeakers[i+j*nRooms]);
				bestConfig.Add(newRoom);
			}
			
			return bestConfig;
		}
		
		public static void Generate(object p) {		
			data = (AlgoData)p;
			Console.WriteLine("===== FreeSpeakers started =====");
			Console.WriteLine("Available Speakers: "+availSpeakers.Count);
			
			// init bestConfig
			bestConfig = DoMonteCarloStep();
			// start steps
			OptimizeConfig(); 			
			data.Finished(bestConfig);
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
			
			// shuffle the available speakers
			Algorithms.RandomizeArray<RoundDebater>(availSpeakers, random);
			
			// use startConfig
			List<RoomData> rooms = new List<RoomData>(nRooms);
			for(int i=0;i<nRooms;i++) {
				RoomData newRoom = new RoomData(startConfig[i], true);
				for(int j=0;j<3;j++)
					newRoom.FreeSpeakers.Add(availSpeakers[3*i+j]);
				rooms.Add(newRoom);
			}
			
			return rooms;
		}
		
	}
}


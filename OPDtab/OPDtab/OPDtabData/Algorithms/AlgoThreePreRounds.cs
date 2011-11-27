using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public static class AlgoThreePreRounds
	{
		static RoundDebater[] chairs;
		static List<RoundDebater>[] judges;
		static List<TeamData> teams;
		static List<RoomData>[] bestConfig;
		static int nRooms;		
		static Random random;
		static int steps;
		static bool cyclicFree;
		static List<RoomData>[] existingRounds;
		static AlgoData data;	
		
		
		public static void Prepare(int randomSeed, int mcSteps, 
								   bool cyclic,
		                           RoundData[] rds) {
			// check rounddata
			if(rds.Length != 3)
				throw new Exception("No correct RoundData given.");
			
			for(int j=0;j<rds.Length;j++) {
				for(int i=0;i<j;i++) {
					if(rds[i].AllTeams.Count != rds[j].AllTeams.Count)
						throw new Exception("AllTeams in given RoomData inconsistent.");
					if(rds[i].AllJudges.Count != rds[j].AllJudges.Count)
						throw new Exception("AllJudges in given RoomData inconsistent.");
				}
			}
			// hardcopy the first teams, assuming all rounds have the same teams 
			SetTeams(rds[0].AllTeams);
			// set the judges
			for(int j=0;j<rds.Length;j++) {
				for(int i=0;i<nRooms;i++) {
					chairs[j*nRooms+i] = rds[j].Rooms[i].Chair;
					judges[j*nRooms+i] = rds[j].Rooms[i].Judges;
				}
			}			
			SetOthers(randomSeed, mcSteps, false, cyclic);
		}
		
		
		public static void Prepare(int randomSeed, int mcSteps, bool cyclic, List<TeamData> t) {
			SetTeams(t);
			// just make empty chairs and judges
			
			for(int i=0;i<3*nRooms;i++) {
				chairs[i] = new RoundDebater();
				judges[i] = new List<RoundDebater>();
			}
			SetOthers(randomSeed, mcSteps, true, cyclic);
		}
		
		static void SetTeams(List<TeamData> t) {
			
			// check data
			if(t.Count % 3 != 0 || t.Count<9)
				throw new Exception("Number of teams ("+t.Count+") not a multiple of 3 (or not enough teams)");
			
			teams = new List<TeamData>();
			// check and make a hard copy
			foreach(TeamData td in t) { 
				if(td.NTeamMembers != 3)
					throw new Exception("Team '"+td+"' has not exactly 3 members.");
				// teams are going to be in rooms, so all are shown
				teams.Add(new TeamData(td, true));
			}	
			nRooms = teams.Count/3;
			chairs = new RoundDebater[3*nRooms];
			judges = new List<RoundDebater>[3*nRooms];
				
		}
		            
		static void SetOthers(int randomSeed, int mcSteps, bool useExisting, bool cyclic) {
			// some other config
			steps = mcSteps;
			random = new Random(randomSeed);
			cyclicFree = cyclic;
			
			if(useExisting)
				existingRounds = new List<RoomData>[Tournament.I.Rounds.Count];
			else
				existingRounds = new List<RoomData>[] {};
			
			for(int i=0;i<existingRounds.Length;i++) {
				existingRounds[i] = Tournament.I.Rounds[i].Rooms;
			}
			// bestConfig is just a try...but Generate() needs a inited bestConfig!
			bestConfig = DoMonteCarloStep();	
				
		}
		
		public static void Generate(object p) {				
			data = (AlgoData)p;
			Console.WriteLine("===== Three PreRounds started =====");
			Console.WriteLine("Teams: "+teams.Count);
			OptimizeConfig(); 	
			data.Finished(bestConfig);
			//return  bestConfig;
		}
		
		/// <summary>
		/// teams is divided in three pools, bestConfig is expected to be well-defined
		/// </summary>
		static void OptimizeConfig() {
		
			// startVal (therefore bestconfig should be well-defined...)
			int val = CalcConflictPenalty(bestConfig);
			Console.WriteLine("Initial config: Total penalty "+val);
			
			if(val>0) {
				for(int i=0;i<steps;i++) {
					//Console.Write("Step "+i+": ");
					List<RoomData>[] newConfig = DoMonteCarloStep();
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
		
		static int CalcConflictPenalty(List<RoomData>[] rounds) {
			int sum = 0;
			foreach(List<RoomData> round in rounds) 	
				foreach(RoomData room in round) 
					sum += room.CalcTotalPenalty(false, true);
			// use also existing rounds
			List<RoomData>[] tmp = new List<RoomData>[rounds.Length+existingRounds.Length];
			Array.Copy(existingRounds, tmp, existingRounds.Length);
			Array.Copy(rounds, 0, tmp, existingRounds.Length, rounds.Length);
			sum += Algorithms.CalcInterRoundConflict(tmp);
			
			return sum;
		}
		
		
		
		static List<RoomData>[] DoMonteCarloStep() {			
			
			// shuffle the teams
			Algorithms.RandomizeArray<TeamData>(teams, random);
			
			List<RoomData>[] rounds = {new List<RoomData>(nRooms), 
				new List<RoomData>(nRooms), 
				new List<RoomData>(nRooms)};
								
			// set rounds with gov/opp 
			for(int i=0;i<nRooms;i++) {
				// pools are defined by slices
				TeamData pool1 = teams[3*i];
				TeamData pool2 = teams[3*i+1];
				TeamData pool3 = teams[3*i+2];
				// Round 1
				rounds[0].Add(new RoomData(i, pool3, pool2, chairs[i+0*nRooms], judges[i+0*nRooms]));
				// Round 2 
				rounds[1].Add(new RoomData(i, pool1, pool3, chairs[i+1*nRooms], judges[i+1*nRooms]));
				// Round 3
				rounds[2].Add(new RoomData(i, pool2, pool1, chairs[i+2*nRooms], judges[i+2*nRooms]));
			}
			
			if(cyclicFree)
				CyclicFreeSpeakers(teams, rounds, nRooms);
			else
				NonCyclicFreeSpeakers(teams, rounds, nRooms);
			
			return rounds;
		}
		
		static void NonCyclicFreeSpeakers(List<TeamData> teams, List<RoomData>[] rounds, 
			int nRooms) {
			
			// a permutation of 012 ensures that 
			// the same team member does not always speak on the
			// same free speaker position
			List<int> i012 = new List<int>(new int[] {0,1,2});
			Algorithms.RandomizeArray<int>(i012, random);
			
			// cannot work in parallel...?!
			for(int round=0;round<rounds.Length;round++) {
				List<RoundDebater>[] freeSpeakers = {
					new List<RoundDebater>(), // first freeSpeakers for this round
					new List<RoundDebater>(), // second 
					new List<RoundDebater>()  // third
				};
				for(int room=0;room<nRooms;room++) {
					// the pools are ordered such that the order of freeSpeaker pools
					// correspond to the order of rounds, so slicing a la 
					// 3*room+round works (see above when setting Gov/Opp)
					for(int i=0;i<3;i++) 
						freeSpeakers[i].Add(teams[3*room+round][i012[i]]);					
				}
				// this makes the freeSpeakers really random without touching
				// the constraint that each team has equal positions
				for(int i=0;i<3;i++)
					Algorithms.RandomizeArray<RoundDebater>(freeSpeakers[i], random);
				// set the speakers
				for(int room=0;room<nRooms;room++) {
					for(int i=0;i<3;i++)
						rounds[round][room].FreeSpeakers[i] = freeSpeakers[i][room];
				}
			}	
			
		}
		
		static void CyclicFreeSpeakers(List<TeamData> teams, List<RoomData>[] rounds, 
			int nRooms) {
			
			// and distribute freeSpeakers for all three rounds in parallel
			int offset1 = random.Next(nRooms);
			int offset2 = random.Next(nRooms);
			int offset3 = random.Next(nRooms);
			
			// a permutation of 012 ensures that 
			// the same team member does not always speak on the
			// same free speaker position
			List<int> i012 = new List<int>(new int[] {0,1,2});
			Algorithms.RandomizeArray<int>(i012, random);
			
			for(int i=0;i<nRooms;i++) {
 				TeamData pool1 = teams[3*i];
				TeamData pool2 = teams[3*i+1];
				TeamData pool3 = teams[3*i+2];		
				
				// Round 1
				SetFreeSpeakers(i, offset1, rounds[0], pool1, i012);
				// Round 2
				SetFreeSpeakers(i, offset2, rounds[1], pool2, i012);
				// Round 3
				SetFreeSpeakers(i, offset3, rounds[2], pool3, i012);
			}	
		}
		
		static void SetFreeSpeakers(int i, int off, List<RoomData> rooms, 
			TeamData freeSpeakerTeam, List<int> i012) {
			
			// cyclic boundary constraints
			int l = (i+0+3*off) % nRooms;   
			int m = (i+1+3*off) % nRooms;   
			int n = (i+2+3*off) % nRooms;   
				
			rooms[l].FreeSpeakers[0] = freeSpeakerTeam[i012[0]];
			rooms[m].FreeSpeakers[1] = freeSpeakerTeam[i012[1]];
			rooms[n].FreeSpeakers[2] = freeSpeakerTeam[i012[2]];	
		}
	}
}


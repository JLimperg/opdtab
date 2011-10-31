using System;
using System.Collections.Generic;

namespace OPDtabData
{
	public static class AlgoBreakroundOnRound
	{
		public static List<RoomData> Generate(RoundData roundData, 
										      List<TeamData> bestTeams,
			                                  List<RoundDebater> bestSpeakers
			) {
			List<RoomData> round = new List<RoomData>();
			List<TeamData> breakingTeams = new List<TeamData>();
			List<RoundDebater> breakingSpeakers = new List<RoundDebater>();
			int nonEmptyRooms = 0;
			foreach(RoomData room in roundData.Rooms) {
				// only rooms with valid bestSpeaker and bestTeam (set by RoundResultSheet)
				if(room.ValidBest) {
					// teams
					if(room.BestTeam==0)
						breakingTeams.Add(room.Gov);	
					else
						breakingTeams.Add(room.Opp);
					// best non-team breaking Speaker
					breakingSpeakers.Add((RoundDebater)room.AsOrderedObjects()[room.BestSpeaker]);
				}
				if(!room.IsEmpty)
					nonEmptyRooms++;
			}
			if(nonEmptyRooms%2 != 0 
				|| breakingTeams.Count == 0
				|| nonEmptyRooms != breakingTeams.Count) {
				throw new Exception("Selected round not valid for creating breakround.");	
			}			
			// sort teams by ranking
			breakingTeams.Sort(delegate(TeamData td1, TeamData td2) {
				int i1 = bestTeams.FindIndex(delegate(TeamData td) {
					return td.Equals(td1);	
				});
				int i2 = bestTeams.FindIndex(delegate(TeamData td) {
					return td.Equals(td2);	
				});
				return i1.CompareTo(i2);
			});	
			// create round by power pairing
			// position of freeSpeakers is rather irrelevant
			for(int i=0;i<breakingTeams.Count/2;i++) {
				RoomData room = new RoomData(i, 
					breakingTeams[i], 
					breakingTeams[breakingTeams.Count-1-i]);
				// add two free speakers
				room.FreeSpeakers.Add(breakingSpeakers[2*i]);
				room.FreeSpeakers.Add(breakingSpeakers[2*i+1]);
				round.Add(room);
			}
			return round;
		}
	}
}


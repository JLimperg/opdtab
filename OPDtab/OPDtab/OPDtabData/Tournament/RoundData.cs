using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace OPDtabData
{
	[Serializable]
	public class RoundData : IRoundEditCbItem
	{
		string roundName;
		string motion;
				
		List<RoomData> rooms;
		List<RoundDebater> allJudges;
		List<TeamData> allTeams;
		
		public RoundData() {
			// empty (for xml serialization)
			roundName = "";
			motion = "";
			allTeams = new List<TeamData>();
			allJudges = new List<RoundDebater>();			
			rooms = new List<RoomData>();
		}
		
		public RoundData (string n, List<Debater> debaters) : this()
		{
			RoundName = n;
			
			foreach(Debater d in debaters) {
				d.SetRoom(RoundName, RoomData.Dummy()); 
				if(d.Role.IsTeamMember) 
					AddMemberToTeamList(allTeams, new RoundDebater(d, true));
				else if(d.Role.IsJudge)
					allJudges.Add(new RoundDebater(d, true));
			}
			allJudges.Sort();
			allTeams.Sort();
			
			// empty rooms			
			AddEmptyRooms(allTeams.Count);			
		}
		
		
		
		public RoundData (string n, List<RoomData> r,
			List<TeamData> aT, List<RoundDebater> aJ) : this()
		{
			RoundName = n;
			
			// make a hard copy, by default, all are in pool, but keep JudgeAvail
			foreach(RoundDebater d in aJ) 
				allJudges.Add(new RoundDebater(d, true, d.JudgeAvail));
			
			foreach(TeamData td in aT)
				allTeams.Add(new TeamData(td, true));
			
			allJudges.Sort();
			allTeams.Sort();      	
			
			// hard copy the rooms
			foreach(RoomData room in r) {
				RoomData newRoom = new RoomData(room, true);
				// tell the room the roundName
				newRoom.RoundName = RoundName;
				rooms.Add(newRoom);
			}
			
			UpdateAllArrays();
			
			// pad empty rooms
			AddEmptyRooms(allTeams.Count-rooms.Count*3);		
		}
		
		// this is really bad coding.
		// but it seems to keep the arrays consistent!
		public void UpdateAllArrays() {
			// set all to shown
			foreach(RoundDebater rd in allJudges) {
				rd.IsShown = true;
				Debater d = Tournament.I.FindDebater(rd);
				if(d != null)
					d.SetRoom(roundName, RoomData.Dummy());	
			}
			
			foreach(TeamData td in allTeams) {
				foreach(RoundDebater rd in td) {
					rd.IsShown = true;
					Debater d = Tournament.I.FindDebater(rd);
					if(d != null)
						d.SetRoom(roundName, RoomData.Dummy());
				}
			}
			
			// then set roomMembers to false
			foreach(RoomData room in rooms)	{
				foreach(RoundDebater rd in room.GetRoomMembers()) {
					if(rd==null)
						continue;
					// check both cases
					if(rd.Role.IsJudge) {
						RoundDebater judge = allJudges.Find(delegate(RoundDebater rd_) {
							return rd_.Equals(rd);	
						});						
						// judge should always be found, 
						// is not shown in pool
						judge.IsShown = false;
					}
					else if (rd.Role.IsTeamMember) {
						// we need to find the reference of rd in allTeams, 
						// set this one to not shown in pool, since it is in room 
						TeamData td = allTeams.Find(delegate(TeamData td_) {
							return td_.Contains(rd);	
						});
						foreach(RoundDebater rd_ in td) {
							if(rd_.Equals(rd))
								rd_.IsShown = false;	
						}
					}
					// update visited rooms
					Debater d = Tournament.I.FindDebater(rd);
					if(d != null)
						d.SetRoom(roundName, room);
				}	
			}
		}
		
		public bool ReplaceInAllArrays(RoundDebater old, RoundDebater rd) {
			// don't rely on the role of old, might have changed.
			
			// search in AllTeams
			foreach(TeamData team in allTeams) {
				for(int i=0;i<team.Count;i++) {
					if(old.Equals(team[i])) { 
						team[i] = rd;
						return true;
					}
				}
			}
			// search in AllJudges
			for(int i=0;i<allJudges.Count;i++) {
				if(old.Equals(allJudges[i])) {
					allJudges[i] = rd;
					return true;
				}
			}			
			// none found
			return false;
		}
		
		public void MergeDebaters(List<Debater> debaters) {
			int num = 0;
			foreach(Debater d in debaters) {
				if(d.Role.IsTeamMember) { 
					if(!IsDebaterInList(allTeams, d)) {
						AddMemberToTeamList(allTeams, new RoundDebater(d, true));
						num++;
					}
				}
				else if(d.Role.IsJudge) {
					if(!IsDebaterInList(allJudges, d))
						allJudges.Add(new RoundDebater(d, true));
				}
			}
			// sort after merging, does it break anything?
			allJudges.Sort();
			allTeams.Sort(); 			
			// add some empty rooms
			AddEmptyRooms(num);			
		}
		
		void AddEmptyRooms(int count) {
			int start = rooms.Count;
			if(count>0) 
				for(int i=0;i<=(count-1)/3;i++) 
					rooms.Add(new RoomData(roundName, start+i));		
		}
	
			   
		bool IsDebaterInList(List<TeamData> list, Debater d) {
			return list.FindLastIndex(delegate(TeamData td) {
				foreach(RoundDebater de in td) 
					if(de.Equals(d))
						return true;
				return false;
			}) != -1;
		}
		
		bool IsDebaterInList(List<RoundDebater> list, Debater d) {
			return list.FindLastIndex(delegate(RoundDebater rd) {
				return rd.Equals(d);
			}) != -1;
		}
		
		public List<RoomData> Rooms {
			get {
				return this.rooms;
			}
			set {
				rooms = value;	
			}
		}

		public List<RoundDebater> AllJudges {
			get {
				return allJudges;
			}
			set {
				allJudges = value;
			}
		}
		
		public List<TeamData> AllTeams {
			get {
				return allTeams;
			}
			set {
				allTeams = value;	
			}
		}
		
		public static void AddMemberToTeamList(List<TeamData> teams, RoundDebater d) {
			TeamData teamData = 
				teams.FindLast(delegate(TeamData td) {
					return td.Equals(d.Role.TeamName);	
				}); 
			if(teamData != null) 
				// this might throw an exception
				teamData.AddMember(d);					
			else
				teams.Add(new TeamData(d));	
		}
		
		public string Motion {
			get {
				return this.motion;
			}
			set {
				motion = value;
			}
		}

		public string RoundName {
			get {
				return this.roundName;
			}
			set {
				roundName = value;
			}
		}
		
		public string CbText {
			get {
				return RoundName;	
			}
		}
		
		public override bool Equals (object obj)
		{
			if(obj is RoundData) {
				RoundData rd = (RoundData)obj;
				return RoundName.Equals(rd.RoundName);
			}
			else if(obj is string) {
				return RoundName.Equals(obj);
			}
			else
				throw new NotImplementedException();
				
		}
		
		public override int GetHashCode ()
		{
			return RoundName.GetHashCode();
		}
	

		#region IItemCompleted implementation
		[System.Xml.Serialization.XmlIgnore]
		public bool ItemCompleted
		{
			get {
				foreach(IRoundEditCbItem i in rooms)
					if(!i.ItemCompleted)
						return false;
				return true;
			}
		}
		#endregion
}
}


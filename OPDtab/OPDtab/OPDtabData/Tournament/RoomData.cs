using System;
using System.Collections.Generic;
namespace OPDtabData
{
	[Serializable]
	public class RoomData : IRoundEditCbItem
	{
		int index;
		string roundName;
		TeamData gov;
		TeamData opp;
		List<RoundDebater> freeSpeakers;
		RoundDebater chair;
		List<RoundDebater> judges;
		bool itemCompleted;
		int bestTeam;
		int bestSpeaker;
		
		public static RoomData Dummy() {
			return new RoomData();
		}
		
		public RoomData () {
			index = -1;
			itemCompleted = false;
			gov = new TeamData();
			opp = new TeamData();
			chair = new RoundDebater();
			freeSpeakers = new List<RoundDebater>();
			judges = new List<RoundDebater>();		
			bestTeam = -1;
			bestSpeaker = -1;
		}
		
		public RoomData(string rN, int i) : this() {			
			roundName = rN;
			index = i;
		}
		
		public RoomData(int i, TeamData g, TeamData o, 
		                RoundDebater c, List<RoundDebater> j) : this() {
			index = i;
			gov = g;
			opp = o;
			// expected to be completely set by Algorithm!
			freeSpeakers.Add(null);
			freeSpeakers.Add(null);
			freeSpeakers.Add(null);		
			judges = j;
			if(c != null)
				chair = c;
		}
		
		public RoomData(int i, TeamData g, TeamData o) : this() {
			index = i;
			gov = g;
			opp = o;
		}
		
		public RoomData(RoomData room, bool isShown) : this() {
			index = room.Index;
			if(!room.Gov.IsEmpty)
				gov = new TeamData(room.Gov, isShown);
			if(!room.Opp.IsEmpty)
				opp = new TeamData(room.Opp, isShown);
			if(room.Chair != null) {
				chair = new RoundDebater(room.Chair, isShown);
			}	
			
			foreach(RoundDebater rd in room.FreeSpeakers) {
				freeSpeakers.Add(new RoundDebater(rd, isShown));
			}
			foreach(RoundDebater rd in room.Judges)
				judges.Add(new RoundDebater(rd, isShown));	
		}
		
		public bool IsDummy {
			get {
				return index == -1;	
			}
		}
		
		public bool IsEmpty {
			get {
				foreach(RoundDebater rd in GetRoomMembers()) 
					if(rd != null)
						return false;
				return true;
			}				
		}
		
		public int Index {
			get {
				return this.index;
			}
			set {
				index = value;
			}
		}
		
		string RoomNameFromIndex(int i) {
			return "Room "+(i+1);	
		}
		
		[System.Xml.Serialization.XmlIgnore]
		public string RoomName {
			get {
				return RoomNameFromIndex(Index);
			}
		}
	
		public string CbText {
			get {
				string s1 = Gov==null ? "?" : Gov.TeamName;
				string s2 = Opp==null ? "?" : Opp.TeamName;
				return RoomName+" ("+s1+" vs. "+s2+")";	
			}
		}

		public TeamData Gov {
			get {
				if(gov.IsEmpty)
					return null;
				else
					return this.gov;
			}
			set {
				if(value == null)
					gov.SetEmpty();
				else
					gov = value;
			}
		}
		
		public TeamData Opp {
			get {
				if(opp.IsEmpty)
					return null;
				else
					return this.opp;
			}
			set {
				if(value == null)
					opp.SetEmpty();
				else
					opp = value;
			}
		}
		
		public List<RoundDebater> FreeSpeakers {
			get {
				return this.freeSpeakers;
			}
			set {
				freeSpeakers = value;
			}
		}
		
		public RoundDebater Chair {
			get {
				if(chair.IsEmpty)
					return null;
				else
					return this.chair;
			}
			set {
				if(value==null)
					chair.SetEmpty();
				else
					this.chair = value;
			}
		}
		
		
		
		public List<RoundDebater> Judges {
			get {
				return this.judges;
			}
			set {
				judges = value;
			}
		}
		
		public List<object> AsOrderedObjects() {
			List<object> tmp = new List<object>();
			tmp.Add(gov[0]);
			tmp.Add(opp[0]);
			tmp.Add(gov[1]);
			tmp.Add(opp[1]);
			foreach(RoundDebater d in freeSpeakers)
				tmp.Add(d);
			for(int i=freeSpeakers.Count;i<3;i++)
				tmp.Add(null);
			tmp.Add(opp[2]);
			tmp.Add(gov[2]);
			// this is a bit tricky
			tmp.Add(Gov);
			tmp.Add(Opp);
			return tmp;
		}
			
		public string RoundName {
			get {
				return this.roundName;
			}
			set {
				roundName = value;
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("{1}, {0}: ({2}, {3}) {4} vs {5}, {6} Free, {7} Judges", RoomName, RoundName,
			                      IsDummy, Index, Gov, Opp, FreeSpeakers.Count, Judges.Count);
		}
		
		public bool ItemCompleted
		{
			get {
				if(IsEmpty)
					return true;
				return itemCompleted;
			}
			set {
				itemCompleted = value;
			}
		}
		
		public bool ValidBest 
		{
			get {
				return !IsEmpty && ItemCompleted 
					&& !(BestTeam<0) && !(BestSpeaker<0);	
			}
		}
		
		// in order of AsOrderedObjects()
		public int BestSpeaker {
			get {
				return this.bestSpeaker;
			}
			set {
				bestSpeaker = value;
			}
		}
		
		// 0 = Gov
		// 1 = Opp
		public int BestTeam {
			get {
				return this.bestTeam;
			}
			set {
				bestTeam = value;
			}
		}		
		
		public List<RoundDebater> GetRoomMembers() {
			List<RoundDebater> roomMembers = new List<RoundDebater>();
			for(int i=0;i<3;i++)
				roomMembers.Add(gov[i]);	
			for(int i=0;i<3;i++)
				roomMembers.Add(opp[i]);
			for(int i=0;i<freeSpeakers.Count;i++)
				roomMembers.Add(freeSpeakers[i]);
			for(int i=freeSpeakers.Count;i<3;i++)
				roomMembers.Add(null);
			// use property here to get null if empty
			roomMembers.Add(Chair);
			foreach(RoundDebater rd in judges)
				roomMembers.Add(rd);
			return roomMembers;	
		}
		
		public bool ReplaceRoomMember(RoundDebater old, RoundDebater rd) {
			// don't rely on the role of old, might have changed.
			// search gov/opp
			
			for(int i=0;i<3;i++) {
				if(old.Equals(gov[i])) {
					Console.WriteLine("Gov: "+old+" -> "+rd);
					gov[i] = rd;
					return true;
				}
				if(old.Equals(opp[i])) {
					Console.WriteLine("Opp: "+old+" -> "+rd);
					opp[i] = rd;
					return true;
				}
			}
			// search free
			for(int i=0;i<freeSpeakers.Count;i++) {
				if(old.Equals(freeSpeakers[i])) {
					Console.WriteLine("Free: "+old+" -> "+rd);
					freeSpeakers[i] = rd;
					return true;
				}
			}
			// search chair and judges
			if(old.Equals(chair)) {
				Console.WriteLine("Chair: "+old+" -> "+rd);
				chair = rd;	
			}
			for(int i=0;i<judges.Count;i++) {
				if(old.Equals(judges[i])) {
					Console.WriteLine("Judge: "+old+" -> "+rd);
					judges[i] = rd;
					return true;
				}	
			}
			return false;
		}
		
		#region Conflict Algorithms
		/// <summary>
		/// This updates also the RoomConflicts in RoundDebater references...
		/// </summary>
		public int CalcTotalPenalty(bool checkOtherRounds, bool calcPenalty) {
			List<RoundDebater> roomMembers = GetRoomMembers();
			// Get Debater Pointers, can have null items...
			List<Debater> dRoomMembers = new List<Debater>();
			
			foreach(RoundDebater rd in roomMembers) {
				// FindDebater can handle rd==null
				dRoomMembers.Add(Tournament.I.FindDebater(rd));	
				// clear the conflicts
				if(rd != null)
					rd.SetConflict(new RoomConflict(rd));
			}
			int max = roomMembers.Count-1;
			// check Gov
		 	CheckRange(roomMembers, dRoomMembers, 0, 2, 3, max);
			
			// check Opp
			CheckRange(roomMembers, dRoomMembers, 3, 5, 0, 2, 6, max);
			
			// check rest
			CheckRange(roomMembers, dRoomMembers, 6, max, 0, max);
			
			if(checkOtherRounds) 
				CheckOtherRounds(roomMembers, dRoomMembers);
			
			if(calcPenalty) 
				return CalcTotalPenalty(roomMembers);
			else 
				return -1;
		}
		
		int CalcTotalPenalty(List<RoundDebater> rds) {
			int sum = 0;
			foreach(RoundDebater rd in rds) {
				if(rd==null)
					continue;
				int pen = rd.GetConflictPenalty();
				if(pen<0)
					continue;
				sum += pen;
			}
			return sum;
		}
		
		
		void CheckOtherRounds(List<RoundDebater> rd, List<Debater> d) {
			int roundIndex=0;
			foreach(RoundData roundData in Tournament.I.Rounds) {
				if(roundData.RoundName == roundName)
					break;
				roundIndex++;		
			}
			// we iterate over all (i, j) with i<j
			// and find pairs of Debaters in the same room (this is reflexive, in contrast to
			// sameRound blackList conflicts...
			// since teamMembers don't conflict each other, 
			// we can start with j=3
			for(int j=3;j<rd.Count;j++) {
				for(int i=0;i<j;i++) {
					if(rd[i] == null || d[i] == null)
						goto inner;
					if(rd[j] == null || d[j] == null)
						goto outer;
					// teamMembers don't conflict each other
					if(j<6 && i>2)
						continue;
					MyDictionary<string, int> vr1 = d[i].VisitedRooms;
					MyDictionary<string, int> vr2 = d[j].VisitedRooms;
					// we are only interested in the intersection of roundNames
					foreach(KVP<string, int> kvp1 in vr1.Store) {
						// skip if this room is in the round...
						if(roundName==kvp1.Key)
							continue;	
						// skip if debater isn't in room
						if(kvp1.Val<0)
							continue;
						KVP<string, int> kvp2;
						if(vr2.GetKVP(kvp1.Key, out kvp2)) {
							// skip if debater isn't in room
							if(kvp2.Val<0)
								continue;
							// same roomIndex?
							if(kvp1.Val == kvp2.Val) {
								string room = RoomNameFromIndex(kvp1.Val);
								// store it in both debaters
								// using a lot of gets in order to prevent null pointers...
								rd[i].GetConflict().GetPartners2(kvp1.Key).GetList(room).Add(rd[j]);
								rd[j].GetConflict().GetPartners2(kvp1.Key).GetList(room).Add(rd[i]);
							}	
						}							
					}
									
				inner: ;
				}
			outer: ;
			}	
		}
		                      
				    
				    
		void CheckRange(List<RoundDebater> rd, List<Debater> d, int oStart, int oStop, params int[] innerParts) {
			// innerParts should have even length
			for(int part=0;part<innerParts.Length/2;part++) {
				int iStart = innerParts[2*part];
				int iStop = innerParts[2*part+1];
				// outer loop
				for(int o=oStart;o<=oStop;o++) {
					// inner loop
					for(int i=iStart;i<=iStop;i++) {
						if(i==o || rd[i] == null || d[i] == null)
							goto inner;
						if(rd[o] == null || d[o] == null)
							goto outer;
						if(d[o].ConflictsWith(d[i])) {
							RoomConflict.Type t = o<i ? GetConflictType(o, i) : GetConflictType(i, o);						
							RoomConflict rc = rd[o].GetConflict();
							rc.Partners1[t].Add(rd[i]);
						}
						
					inner: ;
					}		
				outer: ;
				}
			}
		}
		
		// handy lookup table for many cases...
		// length=11, first 3 should never be accessed!
		static int[] lookupJ = new int[] {-1, -1, -1, 
			(int)RoomConflict.Type.TeamVsTeam, 
			(int)RoomConflict.Type.TeamVsTeam, 
			(int)RoomConflict.Type.TeamVsTeam, 
			(int)RoomConflict.Type.TeamVsFree, 
			(int)RoomConflict.Type.TeamVsFree, 
			(int)RoomConflict.Type.TeamVsFree, 
			(int)RoomConflict.Type.ChairVsAny, 
			(int)RoomConflict.Type.JudgeVsTeam}; 
		
		RoomConflict.Type GetConflictType(int i, int j) {
			// assuming i<j
			// this is tedious to think about, but minimizes computing time...
			if(i<6) {
				if(j>10)
					j=10;
				return (RoomConflict.Type)lookupJ[j];	
			}
			else if(j<9) {
				return RoomConflict.Type.FreeVsFree;	
			}
			else if(j==9) {
				return RoomConflict.Type.ChairVsAny;	
			}
			else if(i<9) {
				return RoomConflict.Type.JudgeVsFree;	
			}
			else if(i==9) {
				return RoomConflict.Type.ChairVsAny;	
			}
			else 
				return RoomConflict.Type.JudgeVsJudge;
		}
		
		void AddToPartners(List<RoundDebater> l, Debater d, List<Debater> o1, IList<RoundDebater> o2) {
			for(int j=0;j<o1.Count;j++) {
				if(o1[j] == null || o2[j] == null || d==o1[j])
					continue;
				if(d.ConflictsWith(o1[j]))
					l.Add(o2[j]);
			}	
		}
		
		/*List<Debater> GetDebaters(IEnumerable<RoundDebater> rd) {
			List<Debater> l = new List<Debater>();
			foreach(RoundDebater d in rd) 
				l.Add(Tournament.I.FindDebater(d));
			return l;
		}*/
		
		#endregion
		
}
}


using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace OPDtabData
{
	[Serializable]
	public class Debater : AbstractDebater
	{
		DebaterPattern blackList;
		DebaterPattern whiteList;	
		List<RoundResultData> roundResults;
		// judges have an average of their stats
		// speakers have the same performance in all rounds...
		List<double> statsAvg;
		// Dictionary is not XML Serializable...:(
		MyDictionary<string, int> visitedRooms;
		string extraInfo;
		
		public Debater() : base() {			
			InitFields();
		}
		
		public Debater(Debater d) : base(d)  {
			InitFields();	
			// ALWAYS COPY!
			// TODO implement proper copying
			extraInfo = d.ExtraInfo;
			roundResults = new List<RoundResultData>(d.RoundResults);
			visitedRooms = new MyDictionary<string, int>(d.VisitedRooms);
			blackList = DebaterPattern.Parse(d.BlackList.ToString());
			whiteList = DebaterPattern.Parse(d.WhiteList.ToString());
		}
		
		void InitFields() {
			extraInfo = "";
			roundResults = new List<RoundResultData>();
			statsAvg = new List<double>();
			visitedRooms = new MyDictionary<string, int>();
			blackList = new DebaterPattern();
			whiteList = new DebaterPattern();	
		}
		
		
	    public RoundResultData GetRoundResult(string roundName, int pos, int nJudges) {			
			RoundResultData rr = roundResults.Find(delegate(RoundResultData d) {
				return d.Equals(roundName);
			});
			if(rr == null) {
				if(Role.IsJudge)
					rr = new RoundResultData(roundName, pos);
				else 
					rr = new RoundResultData(roundName, pos, nJudges);
				rr.ClearStats();
				roundResults.Add(rr);
			}
			else {
				// update judges...
				rr.SetNJudges(nJudges);
			}
			return rr;
		}
	
		public string RoundResultsAsMarkup() {
			
			if(roundResults.Count==0)
				return "No Results";
			List<string> l = new List<string>();
			if(Role.IsTeamMember) {
				l.Add("<i>Performance:</i> "+roundResults[0].PerformanceAsMarkup());
			}
			else if(Role.IsJudge) {
				l.Add("<i>Average:</i> "+RoundResultData.JudgeStatsAsMarkup(statsAvg));
			}	
			foreach(RoundResultData rr in roundResults) {
				int roomIndex = GetRoomIndex(rr.RoundName);
				string room = roomIndex<0 ? "?" : (roomIndex+1).ToString();
				l.Add(rr.RoundName+", "+room+": "+rr.AsMarkup());	
			}			
			return String.Join("\n", l.ToArray());
		}
		
		public void UpdateStatsAvg() {
			statsAvg = new List<double>(new double[] {0, 0, 0});
			int[] n = new int[] {0,0,0};
			foreach(RoundResultData rr in roundResults) {
				for(int i=0;i<rr.Stats.Count;i++) {
					if(!double.IsNaN(rr.Stats[i])) {
						statsAvg[i] += rr.Stats[i];	
						n[i]++;
					}
				}
			}
			for(int i=0;i<statsAvg.Count;i++) {
				statsAvg[i] = n[i]==0 ? double.NaN : statsAvg[i]/n[i]; 	
			}
		}
		
		public void ClearResultStats() {
			statsAvg = new List<double>(new double[] {double.NaN, double.NaN, double.NaN});
			foreach(RoundResultData rr in roundResults) {
				rr.ClearStats();
			}
		}

		public void SetRoom (string roundName, RoomData rd)
		{
			// this is not well-tested for awkward cases
			// visitedRooms should always have the same length as Tournament.I.Rounds,
			// (* still true? if Debater didn't participate in round, then RoomData.IsDummy is true
			if(rd==null) {
				visitedRooms.Remove(roundName);
			}
			else {
				visitedRooms[roundName] = rd.Index;
			}
			// delete RoundResults...
			if(rd==null || rd.IsDummy) {
				RoundResultData rr = roundResults.Find(delegate(RoundResultData d) {
					return d.Equals(roundName);
				});	
				if(rr != null) {
					Console.WriteLine("WARNING: Results removed for Round "+roundName+" ("+this+")");
					roundResults.Remove(rr);
				}
			}
		}
		
		public List<RoundResultData> RoundResults {
			get {
				return this.roundResults;
			}
			set {
				roundResults = value;
			}
		}

		public MyDictionary<string, int> VisitedRooms {
			get {
				return this.visitedRooms;
			}
			set {
				visitedRooms = value;
			}
		}
		
		public int GetRoomIndex(string roundName) {
			int roomIndex = -1;
			foreach(KVP<string, int> kvp in visitedRooms.Store) {
				if(kvp.Key == roundName) {
					roomIndex = kvp.Val;
					break;
				}
			}
			return roomIndex;
		}	
		
		public List<double> StatsAvg {
			get {
				return this.statsAvg;
			}
			set {
				statsAvg = value;
			}
		}

		public bool ConflictsWith(AbstractDebater d) {
			if(whiteList.Matches(d))
				return false;
			if(blackList.Matches(d))
				return true;
			return false;
		}
		
		public DebaterPattern BlackList {
			get {
				return this.blackList;
			}
			set {
				blackList = value;	
			}
		}
		
		public DebaterPattern WhiteList {
			get {
				return this.whiteList;
			}
			set {
				whiteList = value;
			}
		}	

		public string ExtraInfo {
			get {
				return this.extraInfo;
			}
			set {
				extraInfo = value;
			}
		}
	
		
}
}


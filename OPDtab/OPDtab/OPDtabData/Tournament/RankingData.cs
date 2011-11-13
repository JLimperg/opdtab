using System;
using System.Collections.Generic;
namespace OPDtabData
{
	public class RankingDataItem : IComparable<RankingDataItem> {
		
		object data;
		List<int> points;
		int totalPoints;
		double avgPoints;
		List<RoundResultData> results;	
		// do not use SortedDictionary for roundPoints
		// this keeps the added entries in the correct order (in Ranking PDF)
		Dictionary<string, int> roundPoints;
		SortedDictionary<string, bool> availRounds;
		bool resolved;
		
		public RankingDataItem(object d, List<RoundResultData> r) {
			data = d;
			results = r;
			points = null;
			availRounds = null;
			resolved = true;
		}
		
		public void AddResults(List<RoundResultData> r) {
			results.AddRange(r);
		}
		
		public void CalculatePoints(SortedDictionary<string, bool> aR, bool includeTeam) {
			// store pointer to available rounds for sorting...
			// and also for UpdatePointsArray
			availRounds = aR;
			// we expect a Sort() after the recalculation of points
			resolved = true;
			// calculate...
			UpdatePointsArray(includeTeam);
			if(points == null) {
				totalPoints = -1;
				avgPoints = -1;	
			}
			else {
				totalPoints =0;
				foreach(int i in points)
					totalPoints += i;
				avgPoints = (double)totalPoints/points.Count;
			}
		}
		void UpdatePointsArray(bool includeTeam) {
			points = new List<int>();
			SortedDictionary<string, int> teamPoints = new SortedDictionary<string, int>();
			roundPoints = new Dictionary<string, int>();
			int n = 0;
			foreach(RoundResultData rr in results) {
				if(!availRounds.ContainsKey(rr.RoundName))
					continue;
				if(!availRounds[rr.RoundName])
					continue;
				if(rr.AvgSpeakerScore<0) {
					points = null;
					return;
				}
				points.Add(rr.AvgSpeakerScore);
				// try get it...
				int sum = 0;
				roundPoints.TryGetValue(rr.RoundName, out sum);
				sum += rr.AvgSpeakerScore;
				roundPoints[rr.RoundName] = sum;
				// there are no team points if the speaker was free speakers
				if(includeTeam && rr.Role != RoundResultData.RoleType.Free)
					teamPoints[rr.RoundName] = rr.AvgTeamScore;		
				n++;
			}
			if(n==0) {
				points = null;
				return;
			}
			
			if(!includeTeam)
				return;		
			
			foreach(string key in teamPoints.Keys) {
				if(teamPoints[key]<0) {
					points = null;				
					roundPoints[key] = -1;
					continue;
				}
				if(points != null)
					points.Add(teamPoints[key]);
				roundPoints[key] += teamPoints[key];
			}			
		}
	
		public object Data {
			get {
				return this.data;
			}
		}

		public double AvgPoints {
			get {
				return this.avgPoints;
			}
		}

		public List<int> Points {
			get {
				return this.points;
			}
		}
		
		public List<int> RoundPoints {
			get {				
				return new List<int>(roundPoints.Values);	
			}
		}
		
		public int TotalPoints {
			get {
				return this.totalPoints;
			}
		}

		public bool Resolved {
			get {
				return this.resolved;
			}
		}	
		
		#region IComparable[T] implementation
		public int CompareTo (RankingDataItem other)
		{
			if(other==this)
				return 0;
			int ret = other.TotalPoints.CompareTo(TotalPoints);
			if(ret==0) {
				// mark as unresolved firstly...
				resolved = false;
				other.resolved = false;
				EqualPointsResolver epr = Tournament.I.FindResolver(
					EqualPointsResolver.IdRanking(availRounds, this));
				EqualPointsResolver eprOther = Tournament.I.FindResolver(
					EqualPointsResolver.IdRanking(availRounds, other));
				if(epr==null || eprOther==null) {
					// mark if we have resolution available
					resolved = epr != null;
					other.resolved = eprOther != null;
					// still unresolved, sort alphabetically
					// this makes the sorting stable since Data.ToString() should be unique
					ret = Data.ToString().CompareTo(other.Data.ToString());
				}
				else {
					resolved = true;
					other.resolved = true;
					// take resolution into account
					ret = epr.Resolution.CompareTo(eprOther.Resolution);
				}
			}
			return ret;
		}
		#endregion
	}
				            
	public class RankingData {
		
		List<RankingDataItem> teams;
		List<RankingDataItem> speakers;
		Dictionary<string, bool> availRounds;
		
		public RankingData() {
			Update();
		}
		
		public void Update() {
			availRounds = new Dictionary<string, bool>();
			teams = new List<RankingDataItem>();
			speakers = new List<RankingDataItem>();
			// temporary aggregation for teams
			List<TeamData> tmp1 = new List<TeamData>();
			List<List<RoundResultData>> tmp2 = new List<List<RoundResultData>>();
			
			foreach(Debater d in Tournament.I.Debaters) {
				if(!d.Role.IsTeamMember)
					continue;
				RoundDebater rd = new RoundDebater(d, true);
				speakers.Add(new RankingDataItem(rd, d.RoundResults));
				
				// aggregate teams
				int i = tmp1.FindLastIndex(delegate(TeamData td) {
					return td.Equals(d.Role.TeamName);
				});
				
				if(i<0) {
					tmp1.Add(new TeamData(rd));
					tmp2.Add(new List<RoundResultData>(d.RoundResults));
				}
				else {
					tmp1[i].AddMember(rd);
					tmp2[i].AddRange(d.RoundResults);
				}
			}
			// aggregate availRounds
			foreach(RoundData rd in Tournament.I.Rounds)
				availRounds[rd.RoundName] = true;
			
			for(int i=0;i<tmp1.Count;i++)
				teams.Add(new RankingDataItem(tmp1[i], tmp2[i]));
		}
		
		public List<KeyValuePair<string, bool>> GetAvailRounds() {
			// return an independent pointer
			List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
			foreach(string key in availRounds.Keys) 
				list.Add(new KeyValuePair<string, bool>(key, availRounds[key]));
			
			return list;
		}	
		
		public List<RankingDataItem> GetTeamRanking(List<KeyValuePair<string, bool>> aR) {
			
			foreach(RankingDataItem item in teams) 
				item.CalculatePoints(MakeDic(aR), true);
			teams.Sort();
			return new List<RankingDataItem>(teams);
		}
		
		public List<RankingDataItem> GetSpeakerRanking(List<KeyValuePair<string, bool>> aR) {
			foreach(RankingDataItem item in speakers) 
				item.CalculatePoints(MakeDic(aR), false);
			speakers.Sort();
			return new List<RankingDataItem>(speakers);
		}
		
		public static SortedDictionary<string, bool> MakeDic(List<KeyValuePair<string, bool>> list) {
			SortedDictionary<string, bool> dic = new SortedDictionary<string, bool>();
			foreach(KeyValuePair<string, bool> kvp in list)
				dic[kvp.Key] = kvp.Value;
			return dic;
		}
		                                
	}
		
		
		
		
	
}


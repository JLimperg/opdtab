using System;
using System.Collections.Generic;
namespace OPDtabData
{
	[Serializable]
	public class RoundResultData
	{
		public enum RoleType {
			Gov=0, Opp=1, Free=2, Judge=3	
		};
		
		// Gov/Opp/Free, including teams
		public static uint[] PosToRoleType = 
			new uint[] {0, 1, 0, 1, 2, 2, 2, 1, 0, 0, 1}; 
		
		
		// for Speakers, the index denotes the judge
		// for Judges, the index denotes the speaker 
		// in room (from 0 to 8) resp. the team (0=Gov, 1=Opp)
		List<int> speakerScores;
		List<int> teamScores;
		// for speakers, this array has two elements,
		// for judges, it has three
		List<double> stats;
		RoleType role;
		int index;		
		string roundName;
		
		public RoundResultData() {
			speakerScores = new List<int>();
			teamScores = new List<int>();	
			stats = new List<double>();
		}
		
		public RoundResultData(string rN) : this() {
			roundName = rN;			
		}
		
		public RoundResultData(string rN, int i) : this(rN) {
			index = i;
			role = RoundResultData.RoleType.Judge;
			SetListSize(speakerScores, 9);
			SetListSize(teamScores, 2);
		}
		
		public RoundResultData(string rN, int pos, int nJudges) : this(rN) {
			ParsePosition(pos);
			SetListSize(speakerScores, nJudges);
			SetListSize(teamScores, nJudges);
		}
		
		public void SetNJudges(int nJudges) {
			// adjust nJudges, only for speakers
			if(role != RoundResultData.RoleType.Judge) {
				SetListSize(speakerScores, nJudges);
				SetListSize(teamScores, nJudges);
			}
		}
		
		public void ClearJudgeData() {
			if(role == RoundResultData.RoleType.Judge) {
				speakerScores = new List<int>();
				teamScores = new List<int>();
				SetListSize(speakerScores, 9);
				SetListSize(teamScores, 2);
			}
		}		
		
		void SetListSize(List<int> l, int n) {
			// pad with -1
			for(int i=l.Count;i<n;i++)
				l.Add(-1);
			l.RemoveRange(n, l.Count-n);
		}
		
				
		public List<int> SpeakerScores {
			get {
				return speakerScores;	
			}
			set {
				speakerScores = value;
			}
		}
		
		public List<int> TeamScores {
			get {
				return teamScores;	
			}
			set {
				speakerScores = value;
			}
		}	
		
		public List<double> Stats {
			get {
				return this.stats;
			}
			set {
				stats = value;
			}
		}	
		
		public void ClearStats() {
			if(role!=RoleType.Judge) {
				stats = new List<double>(new double[] {double.NaN, double.NaN});
			}
			else {
				stats = new List<double>(new double[] {double.NaN, double.NaN, double.NaN});	
			}
		}
		
		public int AvgSpeakerScore {
			get {
				if(role == RoundResultData.RoleType.Judge)
					throw new Exception("Judges don't have averages");
				return MiscHelpers.CalcAverage(speakerScores);
			}
		}

		public int AvgTeamScore {
			get {
				if(role == RoundResultData.RoleType.Judge)
					throw new Exception("Judges don't have averages");
				return MiscHelpers.CalcAverage(teamScores);
			}
		}	
		
		public int GetPosition() {
			switch(role) {
			case RoleType.Gov:
				if(index<2)
					return index*2;
				else
					return 8;
			case RoleType.Opp:
				if(index<2)
					return index*2+1;
				else
					return 7;
			case RoleType.Free:
				return index+4;
			case RoleType.Judge:
				return index+9;
			default:
				return -1;
			}
		}
	
		public static void ParsePosition(int pos, out RoleType r, out int i) {
			if(pos>8) {
				r = RoleType.Judge;
				i = pos-9;
			} else 
				// use a helper array
				r = (RoundResultData.RoleType)PosToRoleType[pos];
				if(r == RoundResultData.RoleType.Free) {
					i = pos-4;
				} 
				else {
					// integer division works...
					i = pos/2;
					// but not for last speakers
					if(pos>3)
						i=2;
			}
		}
			
		public void ParsePosition(int pos) {
			ParsePosition(pos, out role, out index);	
		}
		
		public int Index {
			get {
				return index;
			}
			set {
				index = value;
			}
		}
		
		public RoleType Role {
			get {
				return this.role;
			}
			set {				
				role = value;
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
		
		public override string ToString ()
		{
			return AsMarkup(); 
		}
		
		
		public string AsMarkup()
		{			
			if(speakerScores.Count==0 || teamScores.Count==0)
				return "No complete Data";
			
			string tmp = "";
			if(Role != RoundResultData.RoleType.Judge) {
				tmp = "<big><b>"+MiscHelpers.IntToStr(AvgSpeakerScore)+
					"</b></big> ("+MiscHelpers.IntArrayToString(speakerScores,"+")+") ";
				
				if(Role != RoundResultData.RoleType.Free)
					tmp+= " <b>"+MiscHelpers.IntToStr(AvgTeamScore)+
					"</b> ("+MiscHelpers.IntArrayToString(teamScores,"+")+") ";
				tmp += GetPosAsString();
			}
			else
				tmp = "Judge"+(Index+1)+" "+
					JudgeStatsAsMarkup(stats)
					+" ["+MiscHelpers.IntArrayToString(speakerScores,", ")
					+"] ["+MiscHelpers.IntArrayToString(teamScores,", ")+"]";
			return tmp;
		}
		
		public string PerformanceAsMarkup() {
			return "<big><b>"+
				      StatAsString(stats[0])+
				      " "+
				      StatAsString(stats[1])
				      +"</b></big>";	
		}
		
		public static string JudgeStatsAsMarkup(List<double> l) {
			return "<big><b>"+
					StatAsString(l[0])+"&#177;"+
					ErrAsString(l[1])+" "+
					StatAsString(l[2])
					+"</b></big>";	
		}
		
		
		
		public static string StatAsString(double d) {
			return double.IsNaN(d) ?"?":
				String.Format("{0:+0.0;-0.0;0}", d);
		}
		
		public static string ErrAsString(double d) {
			return double.IsNaN(d)?"?":
						String.Format("{0:0.0}", d);
		}
		
		public string GetPosAsString() {
			if(Role == RoundResultData.RoleType.Judge)
				return "Judge";
			else {
				string tmp = Role.ToString();
				tmp += (Index+1).ToString();
				return tmp;
			}
		}
		
		public override bool Equals (object obj)
		{
			if(obj is RoundResultData)
				return RoundName.Equals((obj as RoundResultData).RoundName);
			else if(obj is string) 
				return RoundName.Equals(obj);
			else
				throw new NotImplementedException();
		}
		
		public override int GetHashCode ()
		{
			return RoundName.GetHashCode();
		}
		
		public static void UpdateStats() {		
			
			// store for roundAvg, 0...8=speakers, 9/10=gov/opp
			Dictionary<string, List<int>[]> store1 = new Dictionary<string, List<int>[]>();
			foreach(RoundData rd in Tournament.I.Rounds) {
				store1[rd.RoundName] = new List<int>[] {
					new List<int>(), new List<int>(), new List<int>(), 
					new List<int>(), new List<int>(), new List<int>(),
					new List<int>(), new List<int>(), new List<int>(),
					new List<int>(), new List<int>()
				};	
			}
			
			// stores for avgSpeaker/avgTeam
			Dictionary<Debater, List<double>> store2 = new Dictionary<Debater, List<double>>();
			Dictionary<string, List<double>> store3 = new Dictionary<string, List<double>>();
			
			foreach(Debater d in Tournament.I.Debaters) {
				if(d.Role.IsTeamMember) {
					foreach(RoundResultData rr in d.RoundResults) {
						List<int>[] val;						
						if(store1.TryGetValue(rr.RoundName, out val)) {
							int pos = rr.GetPosition();
							foreach(int p in rr.SpeakerScores) {
								if(p>=0) {
									val[pos].Add(p);
								}
							}
							pos = (int)rr.Role+9;
							foreach(int p in rr.TeamScores) {
								if(p>=0) {
									val[pos].Add(p);
								}
							}
						}
					}	
					// just init, used in following loops
					store2[d] = new List<double>();
					store3[d.Role.TeamName] = new List<double>();
				}
				// clear stats
				d.ClearResultStats();	
			}			
			
			// calculate averages: per round and per position
			Dictionary<string, double[]> roundAvg = new Dictionary<string, double[]>();
			foreach(string rN in store1.Keys) {
				roundAvg[rN] = new double[11];
				// i iterates over positions and teams
				for(int i=0;i<11;i++) {
					roundAvg[rN][i] = OPDtabData.MiscHelpers.CalcExactAverage(store1[rN][i]);
				}
			}
			
			// collect performance of speaker/team per round
			foreach(Debater d in store2.Keys) {
				foreach(RoundResultData rr in d.RoundResults) {
					double[] val;						
					if(roundAvg.TryGetValue(rr.RoundName, out val)) {
						int pos = rr.GetPosition();
						double avg = 
							OPDtabData.MiscHelpers.CalcExactAverage(rr.SpeakerScores);
						if(avg>=0 && val[pos]>=0)
							store2[d].Add(avg-val[pos]);
						pos = (int)rr.Role+9;
						avg = 
							OPDtabData.MiscHelpers.CalcExactAverage(rr.TeamScores);
						if(avg>=0 && val[pos]>=0)
							store3[d.Role.TeamName].Add(avg-val[pos]);
					}
				}			
			}
			
			// calculate averages of performance,
			// is used to predict performance in selected round
			// compare it with result of judge
			Dictionary<RoundDebater, double> speakerAvg = new Dictionary<RoundDebater, double>();
			Dictionary<string, double> teamAvg = new Dictionary<string, double>();			
			
			foreach(string t in store3.Keys) {
				teamAvg[t] = OPDtabData.MiscHelpers.CalcExactAverage(store3[t]);
			}				
			foreach(Debater d in store2.Keys) {
				double avg = OPDtabData.MiscHelpers.CalcExactAverage(store2[d]);
				speakerAvg[new RoundDebater(d)] = avg;
				foreach(RoundResultData rr in d.RoundResults) {
					rr.Stats[0] = avg;
					rr.Stats[1] = teamAvg[d.Role.TeamName];
				}
			}
			
			// iterate over rounds and rooms
			// and compare expected points with judge score			
			Dictionary<Debater, Dictionary<string, List<double>[]>> store4 = 
				new Dictionary<Debater, Dictionary<string, List<double>[]>>();
			
			foreach(RoundData rd in Tournament.I.Rounds) {
				foreach(RoomData room in rd.Rooms) {
					List<object> data = room.AsOrderedObjects();
					
					foreach(RoundDebater j in room.Judges) {
						Debater judge = Tournament.I.FindDebater(j);
						if(judge==null)
							continue;
						RoundResultData rr = judge.RoundResults.Find(delegate(RoundResultData rr_) {
							return rr_.Equals(rd.RoundName);
						});
						if(rr==null)
							continue;
						
						// init store4 on the fly
						Dictionary<string, List<double>[]> dic;
						if(!store4.TryGetValue(judge, out dic)) {
							dic = new Dictionary<string, List<double>[]>();
							store4[judge] = dic;
						}
						List<double>[] list;
						if(!dic.TryGetValue(rd.RoundName, out list)) {
							list = new List<double>[] {
								new List<double>(), new List<double>()
							};
							dic[rd.RoundName] = list; 
						}
						
						for(int i=0;i<11;i++) {
							if(data[i]==null)
								continue;
							
							if(i<9) {
								double correct;
								if(speakerAvg.TryGetValue((RoundDebater)data[i], out correct)) {
									double expected = correct+roundAvg[rd.RoundName][i];
									int score = rr.SpeakerScores[i];
									if(score<0)
										continue;
									list[0].Add(score-expected);
								}
							}
							else {
								double correct;
								if(teamAvg.TryGetValue((data[i] as TeamData).TeamName, out correct)) {
									double expected = correct+roundAvg[rd.RoundName][i];
									int score = rr.TeamScores[i-9];
									if(score<0)
										continue;
									list[1].Add(score-expected);
								}
							}
						}		
					}					
				}
			}
			
			// the final result is the avg/sigma in store4
			foreach(Debater d in store4.Keys) {
				foreach(string rN in store4[d].Keys) { 
					double sum = 0;
					double sum2 = 0;
					foreach(double num in store4[d][rN][0]) {
						sum += num;
						sum2 += num*num;
					}
					int count = store4[d][rN][0].Count;
					double mu = count==0?double.NaN:sum/count;
					double mu_err = count==0?double.NaN:
						Math.Sqrt((sum2/count-mu*mu)/count);
					// search roundResult and set stats
					foreach(RoundResultData rr in d.RoundResults) {
						if(rr.RoundName==rN) {
							rr.Stats[0]=mu;
							rr.Stats[1]=mu_err;
							rr.Stats[2]=OPDtabData.MiscHelpers.CalcExactAverage(store4[d][rN][1]);
						}
					}
				}
				// update averages
				d.UpdateStatsAvg();
			}	
		}
		
	}
}


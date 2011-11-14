using System;
using System.IO;
using System.Collections.Generic;
using OPDtabData;
using Gtk;

namespace OPDtabGui
{
	public partial class ShowRanking : Gtk.Window
	{
		public static ShowRanking I;
		RankingData ranking;
		List<RankingDataItem> teams;
		List<RankingDataItem> speakers;
		List<KeyValuePair<string, bool>> availRounds;
		
		bool isConsistent;
		
		// some arrays for marking
		// used in ranking export
		List<int> mBreakingTeams = new List<int>();
		List<int> mTeamSpeakers = new List<int>();
		List<int> mBestSpeakers = new List<int>();
		List<int> mFreeSpeakers = new List<int>();
		
		// this is not the same as in RoundResultSheet
		Gdk.Color[] colBg = new Gdk.Color[] {new Gdk.Color(213,229,255), // blueish (break as team)
			new Gdk.Color(255,238,170), // yellowish (break as free speaker)
			new Gdk.Color(255,128,128), // reddish (conflict)
			new Gdk.Color(215,244,215)  // greenish (break as best speaker in room)
			};
		
		public ShowRanking () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			// make this instance public so ranking can be shown from GenerateRound
			DeleteEvent += delegate(object o, DeleteEventArgs args) {
				args.RetVal = HideOnDelete();	
			}; 			
			I = this;	
			// setup GUI
			UpdateCbSelectMarking();
			SetupFilterEntry(cTeamsFilter, tableTeams, cbTeamsOnlyHighlight);
			SetupFilterEntry(cSpeakersFilter, tableSpeakers, cbSpeakersOnlyHighlight);		
			// build the ranking, last step so that GUI is ready
			UpdateAll();
			// by default, no consistent ranking. is set by MarkBreakround routines
			isConsistent = false;
		}
		
		void SetupFilterEntry(Alignment c, Table table, CheckButton cb) {
			Entry e = MiscHelpers.MakeFilterEntry();
			e.Changed += delegate(object sender, EventArgs a) {		
				UpdateSearchFilter(e, table, cb.Active);
			};
			c.Add(e);
			c.ShowAll();	
		}
		
		void UpdateSearchFilter(Entry e, Table table, bool onlyHighlight) {
			string key = e.Text;
			List<uint> toBeHiddenRows = new List<uint>();
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;
				// by default show all widgets
				w.ShowAll();
				// search in second column
				if(c.LeftAttach == 1 && c.TopAttach>0) {
					IMySearchable s = (IMySearchable)(w as EventBox).Child;
					if(!s.MatchesSearchString(key) && !onlyHighlight) {
						toBeHiddenRows.Add(c.TopAttach);
					}
				}
			}
			// hide rows
			if(toBeHiddenRows.Count>0) {
				foreach(Widget w in table) {
					Table.TableChild c = table[w] as Table.TableChild;
					// hide all conflict resolve buttons if filtering...
					if(c.LeftAttach == 3 || c.LeftAttach == 4 || toBeHiddenRows.Contains(c.TopAttach)) {
						w.HideAll();
					}
				}
			}
		}
		
		public void UpdateCbSelectMarking() {
			// try to save old selected marking
			TreeIter oldIter;
			string oldPath;
			if(cbSelectMarking.GetActiveIter(out oldIter)) 
				oldPath = cbSelectMarking.Model.GetStringFromIter(oldIter);
			else 
				oldPath = "0"; // root by default
			// create new store
			TreeStore store = new TreeStore(typeof(string));
			TreeIter iter0 = store.AppendValues("None");
			TreeIter iter1 = store.AppendValues("BreakroundOnRanking");
			string[][] pre = AppSettings.I.GenerateRound.breakConfigPresets;
			for(int i=0;i<pre.Length;i++) {
				store.AppendValues(iter1, pre[i][0]);	
			}
			TreeIter iter2 = store.AppendValues("BreakroundOnRound");
			foreach(RoundData rd in Tournament.I.Rounds)
				store.AppendValues(iter2, rd.RoundName);
			
			cbSelectMarking.Model = store;
			// try to set to old marking
			TreeIter newIter;
			if(store.GetIterFromString(out newIter, oldPath))
				cbSelectMarking.SetActiveIter(newIter);
			else
				cbSelectMarking.SetActiveIter(iter0); // root by default
		}
		
		public void UpdateAll() {
			// first update judges
			UpdateJudges();
			// then teams and speakers stuff
			MiscHelpers.ClearTable(tableRounds);
			ranking = new RankingData();
			availRounds = ranking.GetAvailRounds();
			tableRounds.NRows = (uint)availRounds.Count+1;
			for(uint i=0;i<availRounds.Count;i++) {
				KeyValuePair<string, bool> kvp = availRounds[(int)i];
				RoundData rd = Tournament.I.FindRound(kvp.Key);
				if(rd==null)
					continue;
				kvp = new KeyValuePair<string, bool>(kvp.Key, rd.ItemCompleted); 
				availRounds[(int)i] = kvp;
				// checkBox
				tableRounds.Attach(SetupCheckbutton(kvp, (int)i), 
				                   0, 1, i+1, i+2, 
				                   AttachOptions.Shrink, AttachOptions.Shrink, 0,0);
					
				// Image completed?
				string imageStr = rd.ItemCompleted ? "gtk-yes" : "gtk-no";
				tableRounds.Attach(new Image(imageStr, IconSize.Menu), 
				                   1, 2, i+1, i+2,
				                   AttachOptions.Shrink, AttachOptions.Shrink, 0,0);
				// roundName
				Label lblName = new Label(kvp.Key);
				lblName.Xalign = 0;
				tableRounds.Attach(lblName, 
				                   2, 3, i+1, i+2,
				                   AttachOptions.Fill, AttachOptions.Fill, 0, 0);
				// Stats: Gov/Opp wins
				int nGov = 0;
				int nOpp = 0;
				foreach(RoomData room in rd.Rooms) {
					if(room.BestTeam==0)
						nGov++;
					else if(room.BestTeam==1)
						nOpp++;
				}
				Label lblStats = new Label(nGov+" / "+nOpp);
				tableRounds.Attach(lblStats,
					3, 4, i+1, i+2,
					AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			}
			tableRounds.ShowAll();
			UpdateTeamsAndSpeakers();
		}
		
		CheckButton SetupCheckbutton(KeyValuePair<string, bool> kvp, int i) {
			CheckButton cb = new CheckButton();
			// order Active/Clicked is important: 
			// so UpdateTeamsAndSpeakers() is not fired unnecessarily
			cb.Active = kvp.Value;
			cb.Clicked += delegate(object sender, EventArgs e) {
				availRounds[i] = 
					new KeyValuePair<string, bool>(kvp.Key, 
					                               (sender as CheckButton).Active);
				UpdateTeamsAndSpeakers();                                                
			};			
			return cb;
		}		
		
		void UpdateJudges() {
			RoundResultData.UpdateStats();
			MiscHelpers.ClearTable(tableJudges);
			List<Debater> judges = new List<Debater>();
			foreach(Debater d in Tournament.I.Debaters) {
				if(d.Role.IsJudge) {
					judges.Add(d);
				}
			}
			judges.Sort(delegate(Debater a, Debater b) {
				return b.StatsAvg[0].CompareTo(a.StatsAvg[0]);	
			});
			for(uint pos=0;pos<judges.Count;pos++) {
				Debater j = judges[(int)pos];
				tableJudges.Attach(new DebaterWidget(new RoundDebater(j)),
					                   0, 1,
					                  2*pos+1, 2*pos+2,
					                   AttachOptions.Fill, AttachOptions.Fill, 0, 0);	
				Label lbl1 = new Label();
				lbl1.Markup = "<big><b>"+
					RoundResultData.StatAsString(j.StatsAvg[0])+"&#177;"+
					RoundResultData.ErrAsString(j.StatsAvg[1])+"</b></big>";
				lbl1.Xalign = 0;
				tableJudges.Attach(lbl1,
					               1, 2,
					               2*pos+1, 2*pos+2,
					               AttachOptions.Fill, AttachOptions.Fill, 0, 0);
				Label lbl2 = new Label();
				lbl2.Markup = "<big><b>"+
					RoundResultData.StatAsString(j.StatsAvg[2])+"</b></big>";
				lbl2.Xalign = 0;
				tableJudges.Attach(lbl2,
					               2, 3,
					               2*pos+1, 2*pos+2,
					               AttachOptions.Fill, AttachOptions.Fill, 0, 0);				
				if(pos<judges.Count-1) {
					Debater jNext = judges[(int)(pos+1)];
					double diff = j.StatsAvg[0]-jNext.StatsAvg[0];
					if(double.IsNaN(diff))
						diff=0;
					int pixels = 5*(int)Math.Round(diff*10);
					tableJudges.Attach(MyGridLine.H(pixels+3),
				    	               0, 3,
				        	           2*pos+2, 2*pos+3,
				            	       AttachOptions.Fill,AttachOptions.Fill,0,0);
				}
			}
			
			tableJudges.ShowAll();
		}
		
		void UpdateTeamsAndSpeakers() {			
			// teams
			teams = ranking.GetTeamRanking(availRounds);
			MiscHelpers.ClearTable(tableTeams);
			for(int i=0;i<teams.Count;i++) {
				SetupTableRow(tableTeams, 
				              new Team((TeamData)teams[i].Data), 
				              teams, 
				              i);
			}
			tableTeams.ShowAll();
			(cTeamsFilter.Child as Entry).Text="";
			
			// speakers
			speakers = ranking.GetSpeakerRanking(availRounds);			
			MiscHelpers.ClearTable(tableSpeakers);
			for(int i=0;i<speakers.Count;i++) {
				SetupTableRow(tableSpeakers, 
				              new DebaterWidget((RoundDebater)speakers[i].Data), 
				              speakers, 
				              i);
			}
			tableSpeakers.ShowAll();
			(cSpeakersFilter.Child as Entry).Text="";
			
			// reset marking, this also rebuilds the combobox, just to be sure
			UpdateCbSelectMarking();
		}
		
		void SetupTableRow(Table table, Widget w, List<RankingDataItem> items, int i) {
			RankingDataItem item = items[i];
			uint row = (uint)i+1;	
			string pos = row.ToString();
				
			// check for equal points
			if(i>0 && items[i-1].TotalPoints == item.TotalPoints) {
				uint off = row % 2;
				table.Attach(MakeResolveBtn(table, i),
				             3+off, 3+off+1,
				             row-1,row+1,
				             AttachOptions.Fill, AttachOptions.Fill, 0,0);
				
			}
			if(!item.Resolved)                     
				pos += "?";
								
			// insert widgets in table,
			// always in EventBox for marking with Backgrounds...
			Label lblPos = new Label(pos);
			lblPos.Yalign = 0.5f;
			table.Attach(MiscHelpers.MakeBackground(lblPos),
			             0, 1, row, row+1, 
			             AttachOptions.Fill, AttachOptions.Fill, 0, 0);
			table.Attach(MiscHelpers.MakeBackground(w),
			             1, 2, row, row+1, 
			             AttachOptions.Fill, AttachOptions.Fill, 0, 0);
			Label lblPoints = new Label();
			lblPoints.Markup = item.TotalPoints<0 ? "<big><b>?</b></big>" : "<b><big>"+item.TotalPoints+"</big></b>";
			table.Attach(MiscHelpers.MakeBackground(lblPoints), 
			             2, 3, row, row+1,
			             AttachOptions.Fill, AttachOptions.Fill, 0, 0);
		}
		
		Widget MakeResolveBtn(Table table, int i) {
			Button btn = new Button();
			btn.Relief = ReliefStyle.None;
			btn.FocusOnClick = false;
			btn.Clicked += delegate(object sender, EventArgs e) {
				ResolveConflict(table, i);				
			};
			btn.Add(new Image("gtk-refresh", IconSize.LargeToolbar));
			return btn;
		}
		
		void ResolveConflict(Table table, int i) {
			// swap it in data
			if(table == tableTeams) 
				SwapItems(teams, i);	
			else
				SwapItems(speakers, i);
			// i is one less than row (since header)
			MiscHelpers.SwapTableRows(table, (uint)i, (uint)i+1);
			// update pos labels
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				if(c.LeftAttach==0 && c.TopAttach==i)
					((Label)(w as EventBox).Child).Text = i.ToString();
				else if(c.LeftAttach==0 && c.TopAttach==i+1)
					((Label)(w as EventBox).Child).Text = (i+1).ToString();
			}
			// update all marking, this affects also speakers 
			// if teams are switched...
			UpdateBreakroundMarking();
		}
		
	
		
		public List<TeamData> GetBestTeams(int nTeams) {
			List<TeamData> list = new List<TeamData>();
			for(int i=0;i<nTeams;i++)
				list.Add((TeamData)teams[i].Data);
			return list;
		}
		
		public List<TeamData> GetBestTeams() {
			return GetBestTeams(teams.Count);	
		}
		
		public List<RoundDebater> GetBestFreeSpeakers(int nTeams) {
			// determine speakers in teams first	
			List<RoundDebater> teamSpeakers = new List<RoundDebater>();
			foreach(TeamData td in GetBestTeams(nTeams)) {
				teamSpeakers.AddRange(td);	
			}
			List<RoundDebater> freeSpeakers = new List<RoundDebater>();
			// speakers are ordered as in shown ranking
			foreach(RankingDataItem item in speakers) {
				RoundDebater rd = (RoundDebater)item.Data;
				if(!teamSpeakers.Contains(rd)) {
					freeSpeakers.Add(rd);
				}
			}			
			return freeSpeakers;
		}
		
		public bool IsConsistent {
			get {				
				return isConsistent;
			}
		}
			
		void SwapItems(List<RankingDataItem> list, int i) {
			// swap them in list
			RankingDataItem item = list[i-1];
			list[i-1] = list[i];
			list[i] = item;
			// save this order for both items
			SaveResolution(list[i-1], i-1);
			SaveResolution(list[i], i);
		}
		
		void SaveResolution(RankingDataItem item, int i) {
			string id = EqualPointsResolver.IdRanking(availRounds, item);
			EqualPointsResolver epr = Tournament.I.FindResolver(id);
			if(epr==null) {
				epr = new EqualPointsResolver(id, i);
				Tournament.I.Resolvers.Add(epr);
			}
			else {
				epr.Resolution = i;	
			}
		}
		
		protected void OnBtnExportCSVClicked (object sender, System.EventArgs e)
		{
			DoTheExport(MiscHelpers.TemplateType.CSV);
		}
		
		protected virtual void OnBtnExportPDFClicked (object sender, System.EventArgs e)
		{
			DoTheExport(MiscHelpers.TemplateType.PDF);		
		}
			
		void DoTheExport(MiscHelpers.TemplateType type) {
			try {
				ITemplate tmpl = MiscHelpers.GetTemplate("ranking", type);
				string separator = type == MiscHelpers.TemplateType.PDF ? 
					"+" : "\",\"";
				ITmplBlock tmplTitle = tmpl.ParseBlock("TITLE");
				tmplTitle.Assign("V",Tournament.I.Title);
				tmplTitle.Out();
				
				ITmplBlock tmplTeams = tmpl.ParseBlock("TEAMS");
				int pos=1;
				int realPos=1;
				foreach(RankingDataItem item in teams) {
					TeamData td = (TeamData)item.Data;
					if(!(pos>1 && teams[pos-2].TotalPoints==item.TotalPoints)) {
						realPos = pos;
					}
					tmplTeams.Assign("POS", realPos.ToString());
					tmplTeams.Assign("NAME", td.TeamName);
					if(mBreakingTeams.Contains(pos-1))
						tmplTeams.Assign("BREAKMARK", "Break");
					else
						tmplTeams.Assign("BREAKMARK", "");
					tmplTeams.Assign("POINTS", OPDtabData.MiscHelpers.IntToStr(item.TotalPoints));
					ITmplBlock tmplPointsPerRound = tmpl.ParseBlock("POINTSPERROUNDTEAM");
					int nPoints=0;
					if(item.RoundPoints.Count==0) {
						tmplPointsPerRound.Assign("POINTS","?");
						tmplPointsPerRound.Assign("SEP","");
						tmplPointsPerRound.Out();
					}
					else {
						for(int i=0;i<item.RoundPoints.Count;i++) {
							tmplPointsPerRound.Assign("POINTS",OPDtabData.MiscHelpers.IntToStr(item.RoundPoints[i]));
							tmplPointsPerRound.Assign("SEP",i==item.RoundPoints.Count-1?"":separator);
							tmplPointsPerRound.Out();	
							nPoints++;
						}
					}
					if(type == MiscHelpers.TemplateType.CSV)  {
						// in CSV mode pad with more separators	
						for(int i=nPoints;i<Tournament.I.Rounds.Count;i++) {
							tmplPointsPerRound.Assign("POINTS","");
							tmplPointsPerRound.Assign("SEP",separator);
							tmplPointsPerRound.Out();
						}
					}
					tmplTeams.Out();
					pos++;
				}
				
				ITmplBlock tmplSpeakers = tmpl.ParseBlock("SPEAKERS");
				pos=1;
				realPos=1;
				foreach(RankingDataItem item in speakers) {
					RoundDebater rd = (RoundDebater)item.Data;
					if(!(pos>1 && speakers[pos-2].TotalPoints==item.TotalPoints)) {
						realPos = pos;
					}
					tmplSpeakers.Assign("POS", realPos.ToString());
					tmplSpeakers.Assign("NAME", rd.Name.FirstName+" "+rd.Name.LastName);
					tmplSpeakers.Assign("POINTS", OPDtabData.MiscHelpers.IntToStr(item.TotalPoints));		
					if(mTeamSpeakers.Contains(pos-1)) 
						tmplSpeakers.Assign("BREAKMARK", "Team");
					else if (mFreeSpeakers.Contains(pos-1))
						tmplSpeakers.Assign("BREAKMARK", "Tab");
					else if (mBestSpeakers.Contains(pos-1))
						tmplSpeakers.Assign("BREAKMARK", "Raum");
					else
						tmplSpeakers.Assign("BREAKMARK", "");
					tmplSpeakers.Assign("TEAMNAME",rd.Role.TeamName);
					
					ITmplBlock tmplPointsPerRound = tmpl.ParseBlock("POINTSPERROUNDSPEAKER");
					int nPoints = 0;
					if(item.Points==null) {
						tmplPointsPerRound.Assign("POINTS","?");
						tmplPointsPerRound.Assign("SEP","");
						tmplPointsPerRound.Out();
					}
					else {
						for(int i=0;i<item.Points.Count;i++) {
							tmplPointsPerRound.Assign("POINTS",
								OPDtabData.MiscHelpers.IntToStr(item.Points[i]));
							tmplPointsPerRound.Assign("SEP",i==item.Points.Count-1?"":separator);
							tmplPointsPerRound.Out();
							nPoints++;
						}
					}
					if(type == MiscHelpers.TemplateType.CSV)  {
						// in CSV mode pad with more separators	
						for(int i=nPoints;i<Tournament.I.Rounds.Count;i++) {
							tmplPointsPerRound.Assign("POINTS","");
							tmplPointsPerRound.Assign("SEP",separator);
							tmplPointsPerRound.Out();
						}
					}
					tmplSpeakers.Assign("AVERAGEPOINTS",item.AvgPoints<0?"?":String.Format("{0:0.00}",item.AvgPoints));
					tmplSpeakers.Out();
					pos++;
				}	
				MiscHelpers.AskShowTemplate(this, 
					"Ranking Export successfully generated, see pdfs/ranking.(pdf|csv).",
					MiscHelpers.MakeExportFromTemplate()
					);			
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not export Ranking: "+ex.Message, MessageType.Error);
			}	
		}
		
		protected virtual void OnBtnUpdateClicked (object sender, System.EventArgs e)
		{
			UpdateAll();
		}
				
		
		protected void OnCbTeamsOnlyHighlightToggled (object sender, System.EventArgs e)
		{
			UpdateSearchFilter(cTeamsFilter.Child as Entry, 
				tableTeams, cbTeamsOnlyHighlight.Active);			
		}

		protected void OnCbSpeakersOnlyHighlightToggled (object sender, System.EventArgs e)
		{
			UpdateSearchFilter(cSpeakersFilter.Child as Entry, 
				tableSpeakers, cbSpeakersOnlyHighlight.Active);
		}

		protected void OnCbSelectMarkingChanged (object sender, System.EventArgs e)
		{
			UpdateBreakroundMarking();
		}
		
		void UpdateBreakroundMarking() {
			ClearMarking();
			TreeIter iter = TreeIter.Zero;
			if(cbSelectMarking.GetActiveIter(out iter)) {
				TreeStore store = cbSelectMarking.Model as TreeStore;
				if(store.IterDepth(iter)>0) {
					string[] tmp = store.GetStringFromIter(iter).Split(':');	
					int i = int.Parse(tmp[1]);
					if(tmp[0]=="1") {
						MarkBreakroundOnRanking(i);
					}
					else if(tmp[0]=="2") {
						MarkBreakroundOnRound(i);
					}
				}
			}	
		}
		
		public void SetBreakroundOnRanking(int preset) {
			SetMarkingByPath("1:"+preset.ToString());
			MarkBreakroundOnRanking(preset);
		}
		
		public void SetBreakroundOnRound(int roundIndex) {
			SetMarkingByPath("2:"+roundIndex.ToString());
			MarkBreakroundOnRound(roundIndex);
		}
		
		void SetMarkingByPath(string path) {
			TreeIter iter;
			if(cbSelectMarking.Model.GetIterFromString(out iter, path)) {
				cbSelectMarking.SetActiveIter(iter);
			}
		}
		
		void MarkBreakroundOnRanking(int preset) {
			// by default it's consistent
			isConsistent = true;			
			string pre = AppSettings.I.GenerateRound.breakConfigPresets[preset][1];
			// determine only max pos of team which breaks
			string[] nums = pre.Split(new char[] {'-','\n'},StringSplitOptions.RemoveEmptyEntries);
			int max = 0;
			foreach(string s in nums) { 
				int n = int.Parse(s);
				if(max<n) { 
					max=n;
				}
			}
			// max is at least 2 and dividable by 2
			if(max==0)
				return;
			if(max%2!=0)
				return;
			// mark first teams until max in teamList,
			// also gather breaking teams as speakers in teamSpeakers 
			// this foreach loop does not iterate from top to bottom!
			foreach(Widget w in tableTeams) {
				Table.TableChild c = tableTeams[w] as Table.TableChild;
				int i = (int)c.TopAttach-1;
				// colour first three columns, exclude header 
				if(i>=0 && c.LeftAttach < 3) {
					// rows 1 to max
					if(i<max) {
						w.ModifyBg(StateType.Normal, colBg[0]);
						// only for one column: add to teamSpeakers
						if(c.LeftAttach==0) {
							mBreakingTeams.Add(i);
							foreach(RoundDebater rd in (TeamData)teams[i].Data) {
								// find position in speakers and add this
								AddToPositionList(speakers, mTeamSpeakers, rd);
							}
						}
					}
					// other rows
					else {
						// check for equalPoints,
						// iteration order is not defined, so use this
						// to determine if points are equal
						if(max<teams.Count &&
							teams[i].TotalPoints 
							== teams[max-1].TotalPoints) {
							w.ModifyBg(StateType.Normal, colBg[2]);
							if(!teams[i].Resolved)
								isConsistent = false;
						}
					}
				}
				
			}
			// determine freeSpeakers
			for(int i=0;i<speakers.Count;i++) {
				// only add as freeSpeakers if not breaking as TeamSpeaker and
				// still some Speakers needed
				if(mFreeSpeakers.Count<mTeamSpeakers.Count/2 && 
					!mTeamSpeakers.Contains(i)) {
					mFreeSpeakers.Add(i);
				}
			}
			
			int minPointsForBreak = mFreeSpeakers.Count>0 ? 
				speakers[mFreeSpeakers[mFreeSpeakers.Count-1]].TotalPoints :
					-1;
			
			// mark speakers 	
			foreach(Widget w in tableSpeakers) {
				Table.TableChild c = tableSpeakers[w] as Table.TableChild;
				// colour first three columns, exclude header 
				if(c.TopAttach>0 && c.LeftAttach < 3) {
					int i = (int)c.TopAttach-1;
					if(mTeamSpeakers.Contains(i)) {
						w.ModifyBg(StateType.Normal, colBg[0]);
					}
					else if(mFreeSpeakers.Contains(i)) {
						w.ModifyBg(StateType.Normal, colBg[1]);
					}
					else if(speakers[i].TotalPoints == minPointsForBreak) {
						w.ModifyBg(StateType.Normal, colBg[2]);
						if(!speakers[i].Resolved)
							isConsistent = false;
					}
				}
			}
		}
		
		void MarkBreakroundOnRound(int roundIndex) {
			// by default it's consistent
			isConsistent = true;			
			RoundData rd = Tournament.I.Rounds[roundIndex];
			foreach(RoomData room in rd.Rooms) {
				if(room.ValidBest) {
					TeamData winner = room.BestTeam==0 ? room.Gov : room.Opp;
					// find position in team ranking
					AddToPositionList(teams, mBreakingTeams, winner);
					// find position in speaker ranking
					foreach(RoundDebater d in winner) 
						AddToPositionList(speakers, mTeamSpeakers, d);
					// find position of best speaker, this is by definition not a
					// member of winning team
					AddToPositionList(speakers, mBestSpeakers, 
						room.AsOrderedObjects()[room.BestSpeaker]); 
				}
			}
			// mark teams, there are no conflicts here
			foreach(Widget w in tableTeams) {
				Table.TableChild c = tableTeams[w] as Table.TableChild;
				// colour first three columns, exclude header 
				if(c.TopAttach>0 && c.LeftAttach < 3) {
					int i = (int)c.TopAttach-1;
					if(mBreakingTeams.Contains(i)) 
						w.ModifyBg(StateType.Normal, colBg[0]);
				}
			}
						
			// determine freeSpeakers
			for(int i=0;i<speakers.Count;i++) {
				// only add as freeSpeaker if not breaking 
				// as TeamSpeaker or BestSpeaker and
				// still some Speakers needed
				if(mFreeSpeakers.Count<mBreakingTeams.Count/2 && 
					!mTeamSpeakers.Contains(i) &&
					!mBestSpeakers.Contains(i)) {
					mFreeSpeakers.Add(i);
				}
			}	
			int minPointsForBreak = mFreeSpeakers.Count>0 ? 
				speakers[mFreeSpeakers[mFreeSpeakers.Count-1]].TotalPoints :
					-1;
			
			// mark speakers 	
			foreach(Widget w in tableSpeakers) {
				Table.TableChild c = tableSpeakers[w] as Table.TableChild;
				// colour first three columns, exclude header 
				if(c.TopAttach>0 && c.LeftAttach < 3) {
					int i = (int)c.TopAttach-1;
					if(mTeamSpeakers.Contains(i)) {
						w.ModifyBg(StateType.Normal, colBg[0]);
					}
					else if(mFreeSpeakers.Contains(i)) {
						w.ModifyBg(StateType.Normal, colBg[1]);
					}
					else if(mBestSpeakers.Contains(i)) {
						w.ModifyBg(StateType.Normal, colBg[3]);
					}
					else if(speakers[i].TotalPoints == minPointsForBreak) {
						w.ModifyBg(StateType.Normal, colBg[2]);
						if(!speakers[i].Resolved)
							isConsistent = false;
					}
				}
			}
		}
		
		void AddToPositionList(List<RankingDataItem> list1, List<int> list2, object item) {
			int i = list1.FindIndex(delegate(RankingDataItem item_) {
				return item_.Data.Equals(item); 	
			});
			if(i>=0) 
				list2.Add(i);
		}
		
		void ClearMarking() {
			// by default not consistent
			isConsistent = false;
			// clear arrays
			mBreakingTeams = new List<int>();
			mTeamSpeakers = new List<int>();
			mBestSpeakers = new List<int>();
			mFreeSpeakers = new List<int>();
			// clear table
			foreach(Widget w in tableTeams) {
				if(w is EventBox) {
					w.ModifyBg(StateType.Normal);	
				}
			}
			foreach(Widget w in tableSpeakers) {
				if(w is EventBox) {
					w.ModifyBg(StateType.Normal);	
				}
			}
		}

		
	}
}


using System;
using OPDtabData;
using Gtk;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace OPDtabGui
{
	public partial class GenerateRound : Gtk.Window
	{
		
		ListStore storeRounds;
		AppSettings.GenerateRoundClass settings;
		AlgoProgress algoProgress;
		DebaterPool debaterpool;
				
		public GenerateRound () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();	
			RoundResultData.UpdateStats();
					
			settings = AppSettings.I.GenerateRound;
			// small setups
			textRoomDetails.Buffer.Text = Tournament.I.RoomDetails;
			sbRandomSeed.Value = settings.randomSeed;
			sbMonteCarloSteps.Value = settings.monteCarloSteps;
			
			// algoProgress
			algoProgress = new AlgoProgress();
			ebAlgoProgress.Add(algoProgress);
			MiscHelpers.SetIsShown(ebAlgoProgress, false);
			
			// debaterpool Widget
			debaterpool = new DebaterPool();
			ebDebaterPool.Add(debaterpool);
			ebDebaterPool.ShowAll();
			
			// more complex setups
			SetupCbRoundName();
			SetupEntryFilter();
			SetupConflictExpander();
			SetupAlgorithmSb();
			SetupBreakroundPresets();
			// use full screen, hopefully that's enough :)
			Maximize();
		}	
		
		void SetupBreakroundPresets() {
			string[][] pre = settings.breakConfigPresets;
			for(int i=0;i<pre.Length;i++) {
				cbBreakPresets.AppendText(pre[i][0]);	
			}
			cbBreakPresets.Changed += delegate(object sender, EventArgs a) {
				int i = (sender as ComboBox).Active;
				textviewBreakConfig.Buffer.Text = pre[i][1];	
			};
		}
		
		void SetupAlgorithmSb() {
			sbRandomSeed.Alignment = 1f;
			sbMonteCarloSteps.Alignment = 1f;
			sbAlgoRoundStart.Alignment = 1f;
	
		}
		
		
		void SetupEntryFilter() {			
			Entry e = MiscHelpers.MakeFilterEntry();
			e.Changed += delegate(object sender, EventArgs a) {
				UpdateSearchFilter();
			};
			cEntryFilter.Add(e);
			cEntryFilter.ShowAll();
		}
		
		void UpdateSearchFilter() {
			Entry e = (Entry)cEntryFilter.Child; 
			string key = e.Text;
			debaterpool.SetFilter(key);
			// if only in pool, clear other searches
			if(cbSearchOnlyInPool.Active) {				
				key="";	
			}
			vboxRooms.ShowAll();
			int hidden = 0;
			foreach(IMySearchable r in vboxRooms) {
				if(!r.MatchesSearchString(key)) {
					hidden++;
			   		(r as Widget).HideAll();
				}
			}
			// show all rooms again if all hidden
			// so we can drag drop from pool search result...nice!
			if(hidden==vboxRooms.Children.Length)
				vboxRooms.ShowAll();
				
		}
		
		protected virtual void OnCbSearchOnlyInPoolToggled (object sender, System.EventArgs e)
		{
			UpdateSearchFilter();
		}
		
		void SetupCbRoundName() {
			storeRounds = new ListStore(typeof(string), typeof(RoundData));
			foreach(RoundData rd in Tournament.I.Rounds)
				storeRounds.AppendValues(rd.RoundName, rd);			
			
			cbRoundName.Model = storeRounds;
			cbRoundName.TextColumn = 0;
			// synchronized Combobox for selection of Round for next Breakround
			cbBreakroundRounds.Model = storeRounds;
		}
		
		void SetGuiToRound(RoundData rd) {
			// Setup Pool
			Pool.SetGuiToRound(rd);
			
			// Widgets for Rooms			
			MiscHelpers.ClearContainer(vboxRooms);
			foreach(RoomData roomData in rd.Rooms) 
				vboxRooms.PackStart(new Room(roomData, cbCompactView.Active), 
				                    false, false, 0);
			vboxRooms.ShowAll();
		}
		
		public DebaterPool Pool {
			get {
				return debaterpool;
			}
		}
		
		public Room FindRoom(int i) {
			return (Room)vboxRooms.Children[i];	
		}
		
		protected virtual void OnBtnRoundNewClicked (object sender, EventArgs e)
		{
			if(cbRoundName.ActiveText == "") {
				MiscHelpers.ShowMessage(this, "Please enter RoundName.",MessageType.Error);
				CheckRoundSelected();
				return;
			}
			
			if(!CheckRoundName(cbRoundName.ActiveText)) {
				MiscHelpers.ShowMessage(this, "Name is identical "+
				                        "to existing Round.", MessageType.Error);
				CheckRoundSelected();
				return;
			}
					
			try { 
				if(Tournament.I.Debaters.Count==0)
					throw new Exception("No Debaters found.");
				RoundData rd = new RoundData(cbRoundName.ActiveText, Tournament.I.Debaters);
				// tell all the debaters we know the new round with a dummy room
				foreach(Debater d in Tournament.I.Debaters)
					d.SetRoom(rd.RoundName, new RoomData());
				// add and select the new round
				AddRound(rd, true);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Cannot create new Round.\n"
				                        +ex.Message,MessageType.Error);	
				
			}
			CheckRoundSelected();
		}
		
		void AddRound(RoundData rd, bool selectRound) {
			if(selectRound)
				SetGuiToRound(rd);
			rd.Motion = OPDtabData.MiscHelpers.SanitizeString(textMotion.Buffer.Text);
			// Append in ComboBox and select
			TreeIter iter = storeRounds.AppendValues(rd.RoundName, rd);
			if(selectRound)
				cbRoundName.SetActiveIter(iter); 	
			SynchronizeData();
		}
		
		protected virtual void OnCbRoundNameChanged (object sender, System.EventArgs e)
		{
			CheckRoundSelected();
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {           
				RoundData rd = (RoundData)storeRounds.GetValue(iter,1); 
				try {
					rd.MergeDebaters(Tournament.I.Debaters);
					SetGuiToRound(rd);
					
					textMotion.Buffer.Text = rd.Motion;
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Cannot load Round.\n"
					                        +ex.Message,MessageType.Error);	
					cbRoundName.Entry.Text = "";					                         
				}
			}			
		}
		
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			// save data on close
			SynchronizeData();
		}
		
		void SynchronizeData() {
			// rounds
			List<RoundData> list = new List<RoundData>();
			foreach(object[] row in storeRounds) 
				list.Add((RoundData)row[1]);
			Tournament.I.Rounds = list;
			ShowRanking.I.UpdateCbSelectMarking();
			// roomDetails
			Tournament.I.RoomDetails = textRoomDetails.Buffer.Text;
		}
		
		protected virtual void OnBtnRoundDeleteClicked (object sender, System.EventArgs e)
		{
			CheckRoundSelected();
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {           
				RoundData rd = (RoundData)storeRounds.GetValue(iter,1); 
				if(MiscHelpers.AskYesNo(this, "Really delete Round '"+rd.RoundName+"'? "+
					"This will also clear all entered results for this round.")
				   == ResponseType.Yes) {
					foreach(Debater d in Tournament.I.Debaters) {
						// remove Debater from affected room...
						d.SetRoom(rd.RoundName, null);
						// remove results, if any
						RoundResultData rr = d.RoundResults.Find(delegate(RoundResultData rr_) {
							return rr_.Equals(rd.RoundName);
						});
						if(rr!=null) {
							d.RoundResults.Remove(rr);	
						}
					}
					storeRounds.Remove(ref iter);
					SynchronizeData();
					ClearRound();
				}
			}
			else 
				MiscHelpers.ShowMessage(this, "No Round selected.", MessageType.Error);
		}
		
		void ClearRound() {
			textMotion.Buffer.Clear();
			cbRoundName.Entry.Text = "";
			MiscHelpers.ClearContainer(vboxRooms);
			debaterpool.ClearRound();
			CheckRoundSelected();
		}
		
		
		protected virtual void OnTextMotionKeyReleaseEvent (object o, Gtk.KeyReleaseEventArgs args)
		{
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {           
				RoundData rd = (RoundData)storeRounds.GetValue(iter,1); 
				rd.Motion = OPDtabData.MiscHelpers.SanitizeString(textMotion.Buffer.Text);
			}
		}	
		
		
		[GLib.ConnectBefore]
		protected virtual void OnCbRoundNameFocused (object o, Gtk.FocusedArgs args)
		{
			CheckRoundSelected();
		}
		
		void CheckRoundSelected() {
			TreeIter iter = TreeIter.Zero;
			textMotion.Sensitive = cbRoundName.GetActiveIter(out iter);
		}
		
		protected virtual void OnCbRoundNameFocusChildSet (object o, Gtk.FocusChildSetArgs args)
		{
			CheckRoundSelected();			
		}
		
		
		#region conflict expander
		void SetupConflictExpander() {
			cbConflictIncludeOther.Active = settings.includeOtherRounds;
			sbConflictThresh.Value = settings.conflictThresh;
			sbConflictThresh.Alignment = 1f;
			// one more for header labels
			uint n = (uint)settings.conflictPenalties.Length;
			tableConflicts.NRows = n+1;
			
			for(int i=0;i<n;i++) {
				Label lbl = new Label(((RoomConflict.Type)i).ToString());
				if(i==n-1)
					lbl.Markup = "<small><i>From other Rounds</i></small>";
				lbl.Xalign = 0;
				tableConflicts.Attach(lbl,
				                      0, 1,
				                      (uint)i+1, (uint)i+2, 
				                      AttachOptions.Fill, AttachOptions.Fill,
				                      0, 0);
				tableConflicts.Attach(GetConflictIconCb(i),
				                      1, 2,
				                      (uint)i+1, (uint)i+2, 
				                      AttachOptions.Fill, AttachOptions.Fill,
				                      0, 0);
				tableConflicts.Attach(GetConflictPenaltySb(i),
				                      2, 3,
				                      (uint)i+1, (uint)i+2, 
				                      AttachOptions.Fill, AttachOptions.Fill,
				                      0, 0);
				
			}
			
			tableConflicts.ShowAll();
		}
		
		SpinButton GetConflictPenaltySb(int i) {
			SpinButton sb = new SpinButton(0, 1000, 10);
			sb.Value = settings.conflictPenalties[i];
			sb.Alignment = 1f;
			sb.ValueChanged += delegate(object s, EventArgs a) {
				settings.conflictPenalties[i] = sb.ValueAsInt;	
				UpdateAllConflicts();
			};
			return sb;
		}
		
		ComboBox GetConflictIconCb(int i) {
			ListStore store = new ListStore(typeof(string));
			store.AppendValues("none");
			for(int j=0;j<settings.possibleIcons.Length;j++)
				store.AppendValues(settings.possibleIcons[j]);
			
			ComboBox cb = new ComboBox(store);
			
			TreeIter valIter;
			store.GetIterFromString(out valIter, (settings.conflictIcons[i]+1).ToString());
			cb.SetActiveIter(valIter);
			
			CellRendererPixbuf cellPixbuf = new CellRendererPixbuf();
			cb.PackStart(cellPixbuf, false);
			cb.SetCellDataFunc(cellPixbuf, ComboboxCellDataFunc);
			
			cb.Changed += delegate(object s, EventArgs a) {
				TreeIter iter;
				if(cb.GetActiveIter(out iter)) {
					settings.conflictIcons[i] = int.Parse(store.GetStringFromIter(iter))-1;		
					UpdateAllConflicts();
				}
			};			
			return cb;
		}
		
		void ComboboxCellDataFunc(CellLayout layout,
		                          CellRenderer cell,
		                          TreeModel model,
		                          TreeIter iter) {
			string str = (string)model.GetValue(iter, 0);
			if(str == null)
				return;
			if(str == "none") 
				(cell as CellRendererPixbuf).Pixbuf = MiscHelpers.LoadIcon("weather-clear-night"); 	
			else
				(cell as CellRendererPixbuf).Pixbuf = MiscHelpers.LoadIcon(str);			
			
		}
		
		protected virtual void OnCbConflictIncludeOtherToggled (object sender, System.EventArgs e)
		{
			settings.includeOtherRounds = cbConflictIncludeOther.Active;
			UpdateAllConflicts();
		}
		
		
		protected virtual void OnSbConflictThreshValueChanged (object sender, System.EventArgs e)
		{
			settings.conflictThresh = sbConflictThresh.ValueAsInt;
			UpdateAllConflicts();
		}
		
		void UpdateAllConflicts() {
			foreach(Room r in vboxRooms)
				r.UpdateConflicts();
		}
		#endregion
		
		protected void OnBtnShuffleRoomsClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);	
				try {
					AlgoShuffle.Rooms(sbRandomSeed.ValueAsInt, rd);
					SetGuiToRound(rd);
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Cannot shuffle rooms: "+ex.Message, MessageType.Error);	
				}
			}
		}
		
		protected void OnBtnShuffleGovOppClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);	
				AlgoShuffle.GovOpp(sbRandomSeed.ValueAsInt, rd);
				SetGuiToRound(rd);
			}
		}
		
		void StartAlgoAsThread(ParameterizedThreadStart algo,
					           AlgoDataFinishedHandler finished) {
			MiscHelpers.SetIsShown(ebAlgoProgress,true);
			Sensitive = false;
			AlgoData algoData = new AlgoData();
			algoData.OnUpdate += delegate(double percentage, string infoMarkup) {
				algoProgress.Update(percentage,infoMarkup);
			};
			algoData.OnFinished += delegate(object data) {
				finished(data);
				Sensitive = true;
			};
			Thread thr = 
				new Thread(algo);
              						
        	thr.Start(algoData);	
		}
		
		protected virtual void OnBtnAlgoThreePreRoundsClicked (object sender, System.EventArgs e)
		{				
			try {			
				bool overwrite = cbAlgoOverwriteExisting.Active && storeRounds.IterNChildren()>0;
				List<string> roundNames = MakeAlgoRoundNames(3, overwrite);				
				// pointers used for generating new round in the end
				List<TeamData> teams;
				List<RoundDebater> judges;				
				
				if(overwrite && cbUseChairsAndJudges.Active) {
					// we need to give three rounds of judges and chairs
					RoundData[] rds = new RoundData[] {
						GetRoundByName(roundNames[0]),
						GetRoundByName(roundNames[1]),
						GetRoundByName(roundNames[2])};
					
					AlgoThreePreRounds.Prepare(sbRandomSeed.ValueAsInt, 
					                           sbMonteCarloSteps.ValueAsInt,
											   cbCyclicFreeSpeakers.Active,
			                           		   rds); 
					// all rounds should have the same teams, checked by Prepare
					teams = rds[0].AllTeams; 
					// and the same all judges, checked by Prepare
					judges = rds[0].AllJudges;	
				}
				else {
					teams = new List<TeamData>();
					judges = new List<RoundDebater>();
					foreach(Debater d in Tournament.I.Debaters) {
						if(d.Role.IsTeamMember)
							RoundData.AddMemberToTeamList(teams, new RoundDebater(d, true));	
						else if(d.Role.IsJudge)
							judges.Add(new RoundDebater(d, true));
					}
					AlgoThreePreRounds.Prepare(sbRandomSeed.ValueAsInt,
					                           sbMonteCarloSteps.ValueAsInt,
											   cbCyclicFreeSpeakers.Active,
					                           teams);
					
						
				}
				
				StartAlgoAsThread(AlgoThreePreRounds.Generate,
					              delegate(object data) {
					List<RoomData>[] rounds = (List<RoomData>[])data;
					AlgoThreePreRoundsFinished(rounds, overwrite, 
				                               roundNames, teams, judges);
				});
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Cannot generate Rounds: "+ex.Message, MessageType.Error);	
			}
		}
		
		void AlgoThreePreRoundsFinished(object data, 
		                                bool overwrite, 
		                                List<string> roundNames,
		                                List<TeamData> teams,
										List<RoundDebater> judges) {
			List<RoomData>[] rounds = (List<RoomData>[])data;
			// store in cbRoundNames
			for(int i=0;i<rounds.Length;i++)
				if(overwrite)					
					SetRoundByName(new RoundData(roundNames[i], rounds[i], teams, judges));	
				else		
					AddRound(new RoundData(roundNames[i], rounds[i], teams, judges), false);
			// update view if necessary
			TreeIter iter;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
				SetGuiToRound(rd);
			}	
		}
		
		protected virtual void OnBtnAlgoJudgesClicked (object sender, System.EventArgs e)
		{
			if(debaterpool.NumJudges==0) {
				MiscHelpers.ShowMessage(this, "There are no judges to distribute.", MessageType.Error);
				return;
			}
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);	
				try {
					AlgoJudges.Prepare(sbRandomSeed.ValueAsInt, 
					                   sbMonteCarloSteps.ValueAsInt,
			                           rd);
					
					StartAlgoAsThread(AlgoJudges.Generate,
					                  delegate(object data) {
						List<RoomData> rooms = (List<RoomData>)data;
						RoundData newRd = new RoundData(rd.RoundName, rooms, 
				        		                        rd.AllTeams, rd.AllJudges);
						SetRoundByName(newRd);
						SetGuiToRound(newRd);
					});		
				}	
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Cannot distribute judges: "+ex.Message, MessageType.Error);	
				}
			}			
		}
		
		
		
		protected virtual void OnBtnAlgoJudgesClearClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
				foreach(RoomData room in rd.Rooms) {
					// clear rooms
					room.Judges = new List<RoundDebater>();
					room.Chair = null;
				}
				rd.UpdateAllArrays();
				SetGuiToRound(rd);
			}
		}
		
		protected virtual void OnBtnAlgoFreeSpeakersClicked (object sender, System.EventArgs e)
		{
			if(debaterpool.NumTeams==0) {
				MiscHelpers.ShowMessage(this, "There are no teams to distribute.", MessageType.Error);
				return;
			}
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);	
				try {
					AlgoFreeSpeakers.Prepare(sbRandomSeed.ValueAsInt, 
					                   sbMonteCarloSteps.ValueAsInt,
			                           rd, ShowRanking.I.GetBestFreeSpeakers(0)); 
					
					if(cbOnRankingFreeSpeakers.Active) {
						// just use ByRanking
						RoundData newRd = new RoundData(rd.RoundName, 
							AlgoFreeSpeakers.ByRanking(), 
				        	rd.AllTeams, rd.AllJudges);
						SetRoundByName(newRd);
						SetGuiToRound(newRd);
					}
					else {
						// use monte carlo to optimize free speakers
						StartAlgoAsThread(AlgoFreeSpeakers.Generate,
					    	              delegate(object data) {
							// algo finished 
							List<RoomData> rooms = (List<RoomData>)data;
							RoundData newRd = new RoundData(rd.RoundName, rooms, 
				        		                        rd.AllTeams, rd.AllJudges);
							SetRoundByName(newRd);
							SetGuiToRound(newRd);
						});	
					}
				}	
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Cannot distribute teams as free speakers: "+ex.Message, MessageType.Error);	
				}
			}
		}
		
		protected virtual void OnBtnAlgoFreeSpeakersClearClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(cbRoundName.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
				foreach(RoomData room in rd.Rooms) {
					// clear rooms
					room.FreeSpeakers = new List<RoundDebater>();
				}
				rd.UpdateAllArrays();
				SetGuiToRound(rd);
			}	
		}
		
		protected virtual void OnBtnAlgoBreakroundClicked (object sender, System.EventArgs e)
		{
			if(OPDtabData.MiscHelpers.SanitizeString(textviewBreakConfig.Buffer.Text)=="") {
				MiscHelpers.ShowMessage(this, "Please select config.", MessageType.Error);
				return;	
			}
			// since the text buffer isn't empty, cbBreakPresets has selection.
			// IsConsistent is then valid for this preset, however, the Algo only 
			// relies on the ordering of the teams
			ShowRanking.I.SetBreakroundOnRanking(cbBreakPresets.Active);			
			if(!ShowRanking.I.IsConsistent) {
				if(MiscHelpers.AskYesNo(this, "Ranking is inconsistent. Really Continue?")
				   == ResponseType.No)
					return;
			}
			// Ask again!
			if(MiscHelpers.AskYesNo(this, "Is the Ranking <b>updated and doublechecked</b>?")
				== ResponseType.No)
				return;
			
			try {			
				bool overwrite = cbAlgoOverwriteExisting.Active && storeRounds.IterNChildren()>0;
				int nRounds = AlgoBreakround.ParsePlacing(textviewBreakConfig.Buffer.Text);
				
				List<string> roundNames = MakeAlgoRoundNames(nRounds, overwrite);	
				List<TeamData> teams = new List<TeamData>();
				List<RoundDebater> judges = new List<RoundDebater>();
				foreach(Debater d in Tournament.I.Debaters) {
					if(d.Role.IsTeamMember)
						RoundData.AddMemberToTeamList(teams, new RoundDebater(d, true));	
					else if(d.Role.IsJudge)
						judges.Add(new RoundDebater(d, true));
				}				
				List<RoomData>[] rounds = AlgoBreakround.Generate(ShowRanking.I.GetBestTeams());	
				
				// store in cbRoundNames
				for(int i=0;i<rounds.Length;i++)
					if(overwrite)					
						SetRoundByName(new RoundData(roundNames[i], rounds[i], teams, judges));	
					else		
						AddRound(new RoundData(roundNames[i], rounds[i], teams, judges), false);
				// update view if necessary
				TreeIter iter;
				if(cbRoundName.GetActiveIter(out iter)) {
					RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
					SetGuiToRound(rd);
				}				
			}	
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Cannot generate Breakround: "+ex.Message, MessageType.Error);	
			}
		}
		
		protected void OnBtnAlgoBreakroundOnRoundClicked (object sender, System.EventArgs e)
		{
			if(cbBreakroundRounds.Active<0) {
				MiscHelpers.ShowMessage(this, "Please select Round for Break", MessageType.Error);
				return;
			}
			// since the text buffer isn't empty, cbBreakPresets has selection.
			// IsConsistent is then valid for this preset
			ShowRanking.I.SetBreakroundOnRound(cbBreakroundRounds.Active);			
			if(!ShowRanking.I.IsConsistent) {
				if(MiscHelpers.AskYesNo(this, "Ranking is inconsistent. Really Continue?")
				   == ResponseType.No)
					return;
			}
			try {			
				bool overwrite = cbAlgoOverwriteExisting.Active && storeRounds.IterNChildren()>0;
				// get round which is basis for next breakround
				TreeIter iter = TreeIter.Zero;
				RoundData round;
				if(storeRounds.GetIterFromString(out iter, cbBreakroundRounds.Active.ToString()))
					round = (RoundData)storeRounds.GetValue(iter, 1);	
				else
					throw new Exception("Can't find selected Round.");
				
				string roundName = MakeAlgoRoundNames(1, overwrite)[0];	
				// aggregate teams and judges
				List<TeamData> teams = new List<TeamData>();
				List<RoundDebater> judges = new List<RoundDebater>();
				foreach(Debater d in Tournament.I.Debaters) {
					if(d.Role.IsTeamMember)
						RoundData.AddMemberToTeamList(teams, new RoundDebater(d, true));	
					else if(d.Role.IsJudge)
						judges.Add(new RoundDebater(d, true));
				}
				// generate breakround
				List<RoomData> newRound = AlgoBreakroundOnRound.Generate(round, 
					ShowRanking.I.GetBestTeams(),
					ShowRanking.I.GetBestFreeSpeakers(0));	
				
				// store in cbRoundNames
				if(overwrite)					
					SetRoundByName(new RoundData(roundName, newRound, teams, judges));	
				else		
					AddRound(new RoundData(roundName, newRound, teams, judges), false);
				// update view if necessary
				if(cbRoundName.GetActiveIter(out iter)) {
					RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
					SetGuiToRound(rd);
				}				
			}	
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Cannot generate Breakround: "+
					ex.Message, MessageType.Error);	
			}
		}
		
		RoundData GetRoundByName(string roundName) {
			int i=0;
			foreach(object[] row in storeRounds) {
				if(row[0].Equals(roundName))
					break;
				i++;
			}
			TreeIter iter = TreeIter.Zero;
			if(storeRounds.GetIterFromString(out iter, i.ToString()))
				return (RoundData)storeRounds.GetValue(iter, 1);
			else
				throw new Exception("To be overwritten Round not found");
		
		}
		
		void SetRoundByName(RoundData rd) {
			int i=0;
			foreach(object[] row in storeRounds) {
				if(row[0].Equals(rd.RoundName))
					break;
				i++;
			}
			TreeIter iter = TreeIter.Zero;
			if(storeRounds.GetIterFromString(out iter, i.ToString()))
				storeRounds.SetValues(iter, rd.RoundName, rd);
			else
				throw new Exception("To be overwritten Round not found");
		}
		
		List<string> MakeAlgoRoundNames(int count, bool overwrite) {
			string prefix = OPDtabData.MiscHelpers.SanitizeString(entryAlgoRoundPrefix.Text);
			if(prefix == "") 
				throw new Exception("No AlgoPrefix given");
			int start = sbAlgoRoundStart.ValueAsInt;
			List<string> roundNames = new List<string>();
			
			while(roundNames.Count<count) {
				string rN = prefix+" "+start;
				// little hack for final break rounds...
				// remove the unnecessary "1" in the end
				if(count==1 && rN.ToLower().Contains("final")) {
					rN = rN.Replace(" 1","");	
				}				
				if(overwrite) {
					if(CheckRoundName(rN))
						throw new Exception("Can only overwrite complete set of rounds.");
					roundNames.Add(rN);
				}
				else if(CheckRoundName(rN)) {
					roundNames.Add(rN);
				}
				start++;
			}			      
			return roundNames;           
		}
		
		
		bool CheckRoundName(string roundName) {
			
			foreach(object[] row in storeRounds) {
				if(row[0].Equals(roundName))
					return false;			
			}
			return true;
		}
		
		protected virtual void OnCbAlgoOverwriteExistingToggled (object sender, System.EventArgs e)
		{
			cbUseChairsAndJudges.Sensitive = cbAlgoOverwriteExisting.Active;
		}
		
		protected virtual void OnBtnShowRankingClicked (object sender, System.EventArgs e)
		{
			ShowRanking.I.KeepAbove = true;
			ShowRanking.I.ShowAll();			
		}
				
		protected virtual void OnBtnExportPDFClicked (object sender, System.EventArgs e)
		{			
			TreeIter iter = TreeIter.Zero;
			if(!cbRoundName.GetActiveIter(out iter))
				return;
			RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
			
			try {
				ITemplate tmplRound = MiscHelpers.GetTemplate("round");
				WorkOnTemplate(rd, tmplRound);				
				MiscHelpers.AskShowTemplate(this,
					"Round PDF successfully generated, see "+
					"pdfs/<RoundName>-round.pdf",
					MiscHelpers.MakePDFfromTemplate(rd.RoundName)
					);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not generate Round Presentation as PDF: "+ex.Message, MessageType.Error);
			}
		}
		
		void WorkOnTemplate(RoundData rd, ITemplate tmpl) {
			// parse text in textRoomDetails			
			string[] roomDesc = new string[rd.Rooms.Count];
			string[] roomTitle = new string[rd.Rooms.Count];
			string[] lines = textRoomDetails.Buffer.Text.Split(new char[] {'\n'},
				StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<rd.Rooms.Count;i++) {
				roomTitle[i] = "";
				roomDesc[i] = "";
				
				if(i<lines.Length) {
					string[] s = lines[i].Split(new char[] {'|'});
					if(s.Length==2) {
						roomTitle[i] = s[0];
						roomDesc[i] = s[1];	
					}
					else if (s.Length==1){
						roomTitle[i] = s[0];
						roomDesc[i] = "";
					}
				}
			}			
			
			ITmplBlock tmplTitle = tmpl.ParseBlock("TITLE");
			tmplTitle.Assign("V",Tournament.I.Title);
			tmplTitle.Out();
			
			ITmplBlock tmplRoundName = tmpl.ParseBlock("ROUNDNAME");
			tmplRoundName.Assign("V", rd.RoundName);
			tmplRoundName.Out();
				
			ITmplBlock tmplRooms = tmpl.ParseBlock("ROOMS");
			foreach(RoomData room in rd.Rooms) {
				if(room.IsEmpty)
					continue;
				tmplRooms.Assign("ROOMNO",(room.Index+1).ToString());
				tmplRooms.Assign("ROOMDESC",roomDesc[room.Index]);
				tmplRooms.Assign("ROOMTITLE",roomTitle[room.Index]);
					
				// Teams
				if(room.Gov==null) 
					tmplRooms.Assign("GOV","?");
				else
					tmplRooms.Assign("GOV",room.Gov.TeamName);					
				AssignExtras(tmplRooms, "GOV", room.Gov);
				
				if(room.Opp==null) 
					tmplRooms.Assign("OPP","?");
				else
					tmplRooms.Assign("OPP", room.Opp.TeamName);					
				AssignExtras(tmplRooms, "OPP", room.Opp);
					
				// Free Speakers
				int n=0;
				foreach(RoundDebater d in room.FreeSpeakers) {
					tmplRooms.Assign("FREE"+(n+1), NameToString(d.Name));
					n++;
				}
				AssignExtras(tmplRooms, "FREE", room.FreeSpeakers);
				
				for(int i=n;i<3;i++) {
					tmplRooms.Assign("FREE"+(i+1), "");	
					tmplRooms.Assign("EXTRAFREE"+(i+1), "");					
				}
					
				// Chair & Judges
				ITmplBlock tmplJudges = tmpl.ParseBlock("JUDGES");
				if(room.Chair != null) {
					Debater dChair = Tournament.I.FindDebater(room.Chair);
					tmplJudges.Assign("NAME", NameToString(room.Chair.Name));
					tmplJudges.Assign("TAG", "(Präsident)");
					tmplJudges.Assign("EXTRA", dChair.ExtraInfo);
					tmplJudges.Out();
				}
				n=0;
				foreach(RoundDebater judge in room.Judges) {
					Debater dJudge = Tournament.I.FindDebater(judge);
			
					tmplJudges.Assign("NAME", NameToString(judge.Name));
					if(n==0)
						tmplJudges.Assign("TAG", "(Hauptjuror)");
					else
						tmplJudges.Assign("TAG", "");
					tmplJudges.Assign("EXTRA", dJudge.ExtraInfo);
					tmplJudges.Out();
					n++;
				}					
				tmplRooms.Out();
			}
				
			ITmplBlock tmplMotion = tmpl.ParseBlock("MOTION");
			string[] motions = rd.Motion.Split(new char[] {'\n'},
				StringSplitOptions.RemoveEmptyEntries);
			for(int i=0;i<motions.Length;i++) {
				tmplMotion.Assign("V"+(i+1).ToString(), motions[i]);		
			}
			tmplMotion.Out();
		}
			
			
		string NameToString(Name n) {
			return n.FirstName+" "+n.LastName;
		}
						
		void AssignExtras(ITmplBlock tmpl, string sec, IEnumerable<RoundDebater> list) {
			int n=1;
			foreach(RoundDebater rd in list) {
				Debater d = Tournament.I.FindDebater(rd);
				string extra = d.ExtraInfo;
				tmpl.Assign("EXTRA"+sec+n, extra);
				n++;
			}
		}
		
		protected void OnBtnExportSheetsClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(!cbRoundName.GetActiveIter(out iter))
				return;
			RoundData rd = (RoundData)storeRounds.GetValue(iter, 1);
			
			try {
				ITemplate tmplSheets = MiscHelpers.GetTemplate("sheets");
				int n=0;
				foreach(RoomData room in rd.Rooms) {
					n++;
					if(room.IsEmpty)
						continue;
					ITmplBlock tmplRooms = tmplSheets.ParseBlock("ROOMS");
					tmplRooms.Assign("ROOMNO",n.ToString());
					tmplRooms.Assign("ROUNDNO",rd.RoundName);
					tmplRooms.Assign("MOTION",rd.Motion);
					AssignTeam(tmplRooms, "GOV", room.Gov);
					AssignSpeakers(tmplRooms, "FREE", room.FreeSpeakers);
					AssignTeam(tmplRooms, "OPP", room.Opp);
					if(room.Chair==null)
						tmplRooms.Assign("CHAIR","?");
					else
						tmplRooms.Assign("CHAIR",NameToString(room.Chair.Name));
					foreach(RoundDebater judge in room.Judges) {
						ITmplBlock tmplJudges = tmplSheets.ParseBlock("JUDGES");
						tmplJudges.Assign("V", NameToString(judge.Name));
						tmplJudges.Out();
					}
					tmplRooms.Out();
					
				}
				MiscHelpers.AskShowTemplate(this,
					"Sheets successfully generated, see "+
					"pdfs/<RoundName>-sheets.pdf",
					MiscHelpers.MakePDFfromTemplate(rd.RoundName)
					);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not generate Sheets as PDF: "+ex.Message, MessageType.Error);
			}
		}
		
		protected void OnBtnExportAllClicked (object sender, System.EventArgs e)
		{
			int numRounds = storeRounds.IterNChildren();
			if(numRounds==0) {
				MiscHelpers.ShowMessage(this, "No Rounds available.", MessageType.Error);
				return;
			}
			// determine max num of rooms
			// create temporary rounds List
			List<RoundData> rounds = new List<RoundData>();
			int numRooms = 0;
			foreach(object[] row in storeRounds) {
				RoundData rd = (RoundData)row[1];
				rounds.Add(rd);
				if(numRooms<rd.Rooms.Count)
					numRooms = rd.Rooms.Count;
			}	
			if(numRooms==0) {
				MiscHelpers.ShowMessage(this, "No Round with at least one Room available.", 
					MessageType.Error);
				return;	
			}
			
			// template stuff
			ITemplate tmplRoundsOverview = MiscHelpers.GetTemplate("roundsoverview");
			ITmplBlock tmplPages = tmplRoundsOverview.ParseBlock("PAGES");
			
			// manual pagination				
			int roomsN = 4;
			int roundsN = 6; // could be max. 7
			int pagesX = 1+(numRooms-1)/roomsN;
			int pagesY = 1+(numRounds-1)/roundsN;
			for(int x=0;x<pagesX;x++) {
				for(int y=0;y<pagesY;y++) {
					int tempX = x+1==pagesX 
						? numRooms-(pagesX-1)*roomsN
							: roomsN;
					int tempY = y+1==pagesY 
						? numRounds-(pagesY-1)*roundsN
							: roundsN;
					ExportAllCreatePage(tmplRoundsOverview, rounds, 
						x*roomsN, tempX, y*roundsN, tempY);
					tmplPages.Out();
				}
			}
			
			try {
				MiscHelpers.AskShowTemplate(this,
					"RoundsOverview PDF successfully generated, " +
				 	"see pdfs/roundsoverview.pdf",
					MiscHelpers.MakePDFfromTemplate(null,false)
					);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not generate RoundsOverview as PDF: "+ex.Message, MessageType.Error);
			}
		}
		
		void ExportAllCreatePage(ITemplate tmplRoundsOverview, List<RoundData> rounds,
			int roomsStart, int roomsN,
		    int roundsStart, int roundsN) {
			
			for(int round=roundsStart;round<roundsStart+roundsN;round++) {
				RoundData rd = rounds[round];
				ITmplBlock tmplRounds = tmplRoundsOverview.ParseBlock("ROUNDS");
				for(int roomIdx=roomsStart;roomIdx<roomsStart+roomsN;roomIdx++) {
					if(roomIdx<rd.Rooms.Count) {
						ITmplBlock tmplRooms = tmplRoundsOverview.ParseBlock("ROOMS");
						// header
						tmplRooms.Assign("ROUNDNO",(round+1).ToString());
						tmplRooms.Assign("ROOMNO",(roomIdx+1).ToString());
						// teams and freeSpeakers
						RoomData room = rd.Rooms[roomIdx];
						AssignTeam(tmplRooms, "GOV", room.Gov);
						AssignSpeakers(tmplRooms, "FREE", room.FreeSpeakers);
						AssignTeam(tmplRooms, "OPP", room.Opp);	
						// chair and judges
						if(room.Chair==null) 
							tmplRooms.Assign("CHAIR","?");
						else
							tmplRooms.Assign("CHAIR",NameToString(room.Chair.Name));
						foreach(RoundDebater judge in room.Judges) {
							ITmplBlock tmplJudges = tmplRoundsOverview.ParseBlock("JUDGES");
							tmplJudges.Assign("V", NameToString(judge.Name));
							tmplJudges.Out();
						}						
						tmplRooms.Out();
					}
				}	
				tmplRounds.Out();
			}
		}
		
		protected virtual void OnBtnVsTeamsClicked (object sender, System.EventArgs e)
		{
			TreeIter iter = TreeIter.Zero;
			if(!cbRoundName.GetActiveIter(out iter)) {           
				MiscHelpers.ShowMessage(this, "No Round selected.", MessageType.Error);
				return;
			}			
			
			RoundData rd = (RoundData)storeRounds.GetValue(iter,1);
			if(rd.Rooms.Count==0 || rd.AllJudges.Count==0)
				return;
			
			// DinA3 / DinA4
			int roomsN = 5;
			int judgesN = 26;
			ITemplate tmplJudgeMatrix = MiscHelpers.GetTemplate("judgesvsteams-dina4");
			if(rbJudgesDinA3.Active) {
				roomsN = 8;
				judgesN = 39;
				tmplJudgeMatrix = MiscHelpers.GetTemplate("judgesvsteams-dina3");
			}
			
			// only availJudges
			ITmplBlock tmplPages = tmplJudgeMatrix.ParseBlock("PAGES");
			List<RoundDebater> judges = new List<RoundDebater>();
			foreach(RoundDebater rd_ in rd.AllJudges) 
				if(rd_.JudgeAvail)
					judges.Add(rd_);
			// and sort it
			judges.Sort(delegate(RoundDebater a, RoundDebater b) {
				return a.Name.ToString().CompareTo(b.Name.ToString());	
			});
			
			// manual pagination				
			int pagesX = 1+(rd.Rooms.Count-1)/roomsN;
			int pagesY = 1+(judges.Count-1)/judgesN;
			for(int x=0;x<pagesX;x++) {
				for(int y=0;y<pagesY;y++) {
					int tempX = x+1==pagesX 
						? rd.Rooms.Count-(pagesX-1)*roomsN
							: roomsN;
					int tempY = y+1==pagesY 
						? judges.Count-(pagesY-1)*judgesN
							: judgesN;
					VsTeamsCreatePage(tmplJudgeMatrix, rd, judges, 
					                  x*roomsN, tempX, y*judgesN, tempY);
					tmplPages.Out();
				}
			}
			
			try {
				MiscHelpers.AskShowTemplate(this,
					"JudgeVsTeams PDF successfully generated, see "+
					"pdfs/<RoundName>-judgesvsteams-<size>.pdf",
					MiscHelpers.MakePDFfromTemplate(rd.RoundName,false)
					);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not generate JudgesVsTeams as PDF: "+ex.Message, MessageType.Error);
			}
		}
		
		void VsTeamsCreatePage(ITemplate tmplJudgeMatrix, RoundData rd, List<RoundDebater> judges,
		                int roomsStart, int roomsN,
		                int judgesStart, int judgesN) {
			
			// just the room titles
			ITmplBlock tmplRooms = tmplJudgeMatrix.ParseBlock("ROOMS");
			foreach(RoomData room in rd.Rooms.GetRange(roomsStart, roomsN)) {
				tmplRooms.Assign("V",(room.Index+1).ToString());
				tmplRooms.Out();
			}
			
			// the team header
			ITmplBlock tmplTeams = tmplJudgeMatrix.ParseBlock("TEAMS");
			foreach(RoomData room in rd.Rooms.GetRange(roomsStart, roomsN)) {
				AssignTeam(tmplTeams, "GOV", room.Gov);
				AssignSpeakers(tmplTeams, "FREE", room.FreeSpeakers);
				AssignTeam(tmplTeams, "OPP", room.Opp);		
				tmplTeams.Out();
			}
			
			// the rows for each judge
			ITmplBlock tmplJudges = tmplJudgeMatrix.ParseBlock("JUDGES");
			
			foreach(RoundDebater judge in judges.GetRange(judgesStart, judgesN)) {
				Debater d = Tournament.I.FindDebater(judge);
				if(d==null)
					continue;
				tmplJudges.Assign("NAME", NameToString(judge.Name));
				
				// how often which position (firstJudge / judge / chair) 
				// iterate (naively) over all other roundData,
				// and look for debater (maybe using Debater.VisitedRooms is better?)
				int NfirstJudge = 0;
				int Njudge = 0;
				int Nchair = 0;
				foreach(RoundData rd_ in Tournament.I.Rounds) {
					if(rd_.RoundName==rd.RoundName)
						continue;
					foreach(RoomData room in rd_.Rooms) {
						for(int i=0;i<room.Judges.Count;i++) {
							if(room.Judges[i].Equals(judge)) {
								if(i==0)
									NfirstJudge++;
								else
									Njudge++;
							}
						}
						if(room.Chair != null 
						   && room.Chair.Equals(judge)) 
							Nchair++;
					}
				}
				tmplJudges.Assign("NFIRSTJUDGE",NfirstJudge.ToString());
				tmplJudges.Assign("NJUDGE",Njudge.ToString());
				tmplJudges.Assign("NCHAIR",Nchair.ToString());
				
				// iterate over rooms 
				// (we assume same ordering in rd.Rooms as in vboxRooms)
				// and find conflicts/alreadySeen count         
				ITmplBlock tmplConflictInd = tmplJudgeMatrix.ParseBlock("CONFLICTIND");
				int n=0;
				foreach(Widget w in vboxRooms) {
					n++;
					if(n<=roomsStart)
						continue;
					if(n>roomsStart+roomsN)
						break;
					List<int> list = ((Room)w).GetJudgeMatrixPattern(d);
					for(int i=0;i<list.Count;i++) 
						tmplConflictInd.Assign("V"+(i+1),list[i].ToString());
					tmplConflictInd.Out();
					
				}				
				tmplJudges.Out();
			}
		}
		
		void AssignTeam(ITmplBlock tmpl, string prefix, TeamData td) {
			if(td==null) 
				tmpl.Assign(prefix,"?");
			else
				tmpl.Assign(prefix,td.TeamName);
			AssignSpeakers(tmpl,prefix,td);
		}
		
		void AssignSpeakers(ITmplBlock tmpl, string prefix, 
		                    IList<RoundDebater> speakers) {
			int i=0;
			if(speakers != null) {
				foreach(RoundDebater rd in speakers) {
					if(rd==null || rd.IsEmpty)
						continue;
					i++;	
					tmpl.Assign(prefix+i, NameToString(rd.Name));
				}
			}	
			for(int j=i+1;j<=3;j++) {
				tmpl.Assign(prefix+j,"?");	
			}
		}
		
		protected virtual void OnBtnVsJudgesClicked (object sender, System.EventArgs e)
		{
			// update stats first
			RoundResultData.UpdateStats();			
			
			// DinA3 / DinA4
			int othersN = 24-Tournament.I.Rounds.Count-1;
			int judgesN = 28;
			ITemplate tmplJudgeMatrix = MiscHelpers.GetTemplate("judgesvsjudges-dina4");
			if(rbJudgesDinA3.Active) {
				othersN = 35-Tournament.I.Rounds.Count-1;
				judgesN = 39;
				tmplJudgeMatrix = MiscHelpers.GetTemplate("judgesvsjudges-dina3");
			}
			
			// all judges 
			ITmplBlock tmplPages = tmplJudgeMatrix.ParseBlock("PAGES");
			
			List<Debater> judges = new List<Debater>();
			foreach(Debater d in Tournament.I.Debaters) 
				if(d.Role.IsJudge)
					judges.Add(d);
			// and sort it
			judges.Sort(delegate(Debater a, Debater b) {
				return a.Name.ToString().CompareTo(b.Name.ToString());	
			});
			
			// manual pagination				
			int pagesX = 1+(judges.Count-1)/othersN;
			int pagesY = 1+(judges.Count-1)/judgesN;
			for(int x=0;x<pagesX;x++) {
				for(int y=0;y<pagesY;y++) {
					int tempX = x+1==pagesX 
						? judges.Count-(pagesX-1)*othersN
							: othersN;
					int tempY = y+1==pagesY 
						? judges.Count-(pagesY-1)*judgesN
							: judgesN;
					VsJudgesCreatePage(tmplJudgeMatrix, judges, 
					                  x*othersN, tempX, y*judgesN, tempY);
					tmplPages.Out();
				}
			}
			
			try {
				MiscHelpers.AskShowTemplate(this,
					"JudgeVsJudges PDF successfully generated, see "+
					"pdfs/judgesvsjudges-<size>.pdf",
					MiscHelpers.MakePDFfromTemplate(null,false)
					);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Could not generate JudgeVsJudges as PDF: "+ex.Message, MessageType.Error);
			}
		}
		
		
		void VsJudgesCreatePage(ITemplate tmplJudgeMatrix, List<Debater> judges,
		                int othersStart, int othersN,
		                int judgesStart, int judgesN) {
			// rounds as simple numbers
			ITmplBlock tmplRoundNames = tmplJudgeMatrix.ParseBlock("ROUNDNAMES");
			tmplRoundNames.Assign("V","Avg");
			tmplRoundNames.Out();
			for(int i=0;i<Tournament.I.Rounds.Count;i++) {
				tmplRoundNames.Assign("V",(i+1).ToString());
				tmplRoundNames.Out();
			}
			
			// the judges in short form
			ITmplBlock tmplJudgesShort = tmplJudgeMatrix.ParseBlock("JUDGESSHORT");
			foreach(Debater d in judges.GetRange(othersStart, othersN)) {
				tmplJudgesShort.Assign("V", d.Name.FirstName.Substring(0,1)+". "
				                       +d.Name.LastName.Substring(0,1)+".");
				tmplJudgesShort.Out();
			}
			
			// the rows for each judge
			ITmplBlock tmplJudges = tmplJudgeMatrix.ParseBlock("JUDGES");
			
			foreach(Debater judge in judges.GetRange(judgesStart, judgesN)) {
				tmplJudges.Assign("NAME", NameToString(judge.Name));
				
				// show stats for each round (preceding average), in same order as above
				ITmplBlock tmplJudgeStats = tmplJudgeMatrix.ParseBlock("JUDGESTATS"); 
				AssignJudgeStats(tmplJudgeStats, judge.StatsAvg);
				tmplJudgeStats.Out();
				foreach(RoundData rd in Tournament.I.Rounds) {
					RoundResultData rr = judge.RoundResults.Find(delegate(RoundResultData rr_) {
						return rr_.Equals(rd.RoundName);
					});
					if(rr==null) {
						tmplJudgeStats.Assign("V1","?");
						tmplJudgeStats.Assign("V2","?");
						tmplJudgeStats.Assign("V3","?");
					}
					else {
						AssignJudgeStats(tmplJudgeStats, rr.Stats);
					}
					tmplJudgeStats.Out();	
				}
				
				// output conflicts
				ITmplBlock tmplConflictInd = tmplJudgeMatrix.ParseBlock("CONFLICTIND"); 
				foreach(Debater other in judges.GetRange(othersStart, othersN)) {
					if(other.Equals(judge)) {
						tmplConflictInd.Assign("V1","-2");
						tmplConflictInd.Assign("V2","-2");		
					}
					else {
						List<int> l = FindJudgeConflicts(judge, other);
						for(int i=0;i<l.Count;i++) 
							tmplConflictInd.Assign("V"+(i+1).ToString(), l[i].ToString());
					}
					tmplConflictInd.Out();
				}
					
				
				tmplJudges.Out();
			}
		}
		
		List<int> FindJudgeConflicts(Debater j1, Debater j2) {
			List<int> l = new List<int>(new int[] {0,0});
			// we need the info if judge was chair or not, 
			// so visitedRooms is not helpful here
			foreach(RoundData rd in Tournament.I.Rounds) {
				foreach(RoomData room in rd.Rooms) {
					bool isInRoom1 = false;
					bool isInRoom2 = false;
					foreach(RoundDebater judge in room.Judges) {
						if(!isInRoom1 && j1.Equals(judge)) 
							isInRoom1 = true;
						else if(!isInRoom2 && j2.Equals(judge))
							isInRoom2 = true;
					}
					// chair/judge case 
					if((j1.Equals(room.Chair) && isInRoom2)
					   || (j2.Equals(room.Chair) && isInRoom1)) {
						l[1]++;	
					}
					else if(isInRoom1 && isInRoom2) {
						l[0]++;	
					}
				}
			}
			return l;
		}
		
		void AssignJudgeStats(ITmplBlock tmpl, List<double> stats) {
			for(int i=0;i<stats.Count;i++) {
				if(double.IsNaN(stats[i]))
					tmpl.Assign("V"+(i+1).ToString(),"?");
				else if(i==1)
					tmpl.Assign("V"+(i+1).ToString(),
					                      RoundResultData.ErrAsString(stats[i]));
				else
					tmpl.Assign("V"+(i+1).ToString(),
					                      RoundResultData.StatAsString(stats[i]));									
			}	
		}					
	
		protected void OnSbMonteCarloStepsValueChanged (object sender, System.EventArgs e)
		{
			settings.monteCarloSteps = sbMonteCarloSteps.ValueAsInt;
		}

		protected void OnSbRandomSeedValueChanged (object sender, System.EventArgs e)
		{
			settings.randomSeed = sbRandomSeed.ValueAsInt;
		}
		
		// global flag for handling the FocusIn/FocusOut events 
		bool disableFocusEvents = false;
		
		protected void OnTextMotionFocusInEvent (object o, Gtk.FocusInEventArgs args)
		{
			if(disableFocusEvents)
				return;
			disableFocusEvents = true;
			cMotionSmall.Remove(frameMotion);
			cMotionBig.Add(frameMotion);
			// removing the widget makes it loose the focus...
			// so correct for this
			textMotion.GrabFocus();
			disableFocusEvents = false;
			// small cosmetics
			alignmentDisplay.TopPadding = 0;
			cMotionBig.BottomPadding = 6;
		}
		
		protected void OnTextMotionFocusOutEvent (object o, Gtk.FocusOutEventArgs args)
		{
			if(disableFocusEvents)
				return;
			disableFocusEvents = true;
			cMotionBig.Remove(frameMotion);
			cMotionSmall.Add(frameMotion);
			disableFocusEvents = false;
			// small cosmetics			
			alignmentDisplay.TopPadding = 6;
			cMotionBig.BottomPadding = 0;
		}


		
		protected void OnCbCompactViewToggled (object sender, System.EventArgs e)
		{
			foreach(Room room in vboxRooms) {
				room.Small = cbCompactView.Active;	
			}
			UpdateSearchFilter();
		}
	}
}


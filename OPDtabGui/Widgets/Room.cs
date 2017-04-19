using System;
using Gtk;
using OPDtabData;
using System.Collections.Generic;
using System.Reflection;


namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Room : Gtk.Bin, IMySearchable
	{
		RoomData roomData;
		static string[] sections = new string[] {"Gov", 
			 "Opp", "FreeSpeakers", "Judges", "Chair"}; 
		List<Container> widgetsContainer;
		List<Container> cBig;
		List<Container> cSmall;
		AppSettings.GenerateRoundClass settings;
		int penalty;
		bool small;
		
		
		public Room (RoomData rd, bool s)
		{
			this.Build();
			roomData = rd;
			settings = AppSettings.I.GenerateRound;
			penalty = -1;
			small = s;
			MyGridLine.H(tableSmall, 2, tableSmall.NColumns, 1, new Gdk.Color(0,0,0));
			
			// generate widget containers
			widgetsContainer = new List<Container>();
			cBig = new List<Container>();
			cSmall = new List<Container>();
			foreach(string sec in sections) { 
				widgetsContainer.Add((Container)GetField("c"+sec));
				cBig.Add((Container)GetField("c"+sec+"Big"));
				cSmall.Add((Container)GetField("c"+sec+"Small"));
			}
			
			// Update Gui after Realized for scrolled window support...
			this.Realized += delegate(object sender, EventArgs e) {
				UpdateGui();
				// done by UpdateGui: UpdateConflicts();
			};				
		}
		
		void UpdateGuiSection(string section) {
			object o = roomData.GetType().GetProperty(section).GetValue(roomData, null);
			UpdateGuiSection(section, 0, o);	
		}
		
		void UpdateGuiSection(string section, int i, object data) {
			Container c = (Container)GetField("c"+section);		
			if(data == null) { 				
				SetDummyLabel(c, section);		
			} 
			else if(data is List<RoundDebater>) {
				// Judges or FreeSpeakers (variable number...)
				MiscHelpers.ClearContainer(c);
				List<RoundDebater> list = (List<RoundDebater>)data;
				if(list.Count==0)
					SetDummyLabel(c, section);
				else {
					for(int j=0;j<list.Count;j++)
						UpdateGuiSection(section,j,list[j]);
					// append label for adding by D&D
					if(small) {
						if(section == "Judges") {
							SetDummyLabel(c, section, roomData.Judges.Count.ToString());	
						}
						else if(section == "FreeSpeakers" && list.Count<3) {
							SetDummyLabel(c, section, "F");
						}
					}
					else if(section == "Judges" || list.Count<3)
						SetDummyLabel(c, section, "Drag here to add");
				}
			}
			else { 
				IDragDropWidget w = null;
					
				if(data is TeamData) {
					if(small)
						w = Team.Small((TeamData)data);
					else		
						w = new Team((TeamData)data, null);
				}
				else if(data is RoundDebater) {
					if(small)
						w = DebaterWidget.Small((RoundDebater)data, false);
					else
						w = new DebaterWidget((RoundDebater)data);
				}
				else
					throw new NotImplementedException("Don't know how to create widget for data");
				
				// tell Debater the room for visitedRooms list..
				w.SetRoom(roomData.RoundName, roomData);				
				
				w.SetupDragDropSource(section, data);
				w.SetDataTrigger += delegate(Widget sender, object data_) {
					SetItem(section, i, data_);				
				};
				
				// Add to container after source, but before Dest
				Widget wi = (Widget)w;
				MiscHelpers.AddToContainer(c, wi, small);
								
				// Drag drop DestSet, needs Container for scrolling support...
				DragDropHelpers.DestSet(wi,
			             DestDefaults.All,
			             DragDropHelpers.TgtFromString(section),
			             Gdk.DragAction.Move);
				
				wi.DragDataReceived += delegate(object o, DragDataReceivedArgs args) {
					object data_ = 
						DragDropHelpers.Deserialize(args.SelectionData);
					// save data here
					SetItem(section, i, data_);
					// delete data there
					MyButton b = (MyButton)Drag.GetSourceWidget(args.Context);
					b.Owner.SetData(data);
				};				
			}
			// show judge quality by icons
			if(section=="Judges" && !small) {				
				int nStars = 0;
				double sumAvgSpkr = 0.0;
				double sumAvgTeam = 0.0;
				int nAvgSpkr = 0;
				int nAvgTeam = 0;
				
				foreach(RoundDebater rd in roomData.Judges) {
					Debater d = Tournament.I.FindDebater(rd);
					if(d==null)
						continue;
					nStars += d.Role.JudgeQuality;
					if(!double.IsNaN(d.StatsAvg[0])) {
						sumAvgSpkr += d.StatsAvg[0];
						nAvgSpkr++;
					}
					if(!double.IsNaN(d.StatsAvg[2])) {
						sumAvgTeam += d.StatsAvg[2];
						nAvgTeam++;
					}
				}	
				
				lblJudgeStats.Markup = "JudgeStats: "+
					JudgeStatsToMarkup(sumAvgSpkr, nAvgSpkr, 2)+" "+
						JudgeStatsToMarkup(sumAvgTeam, nAvgTeam, 5);
						
				
				MiscHelpers.ClearTable(tableJudgeStars);
				tableJudgeStars.NRows = (uint)nStars/5+1;
				
				for(int j=0;j<nStars;j++) {
					uint col = (uint)j % 5;
					uint row = (uint)j/5;
					
					tableJudgeStars.Attach(new Image(MiscHelpers.LoadIcon("face-smile")),
					                       col, col+1,
					                       row, row+1,
					                       AttachOptions.Shrink, AttachOptions.Shrink,
					                       0, 0);
				}
				if(nStars==0)
					tableJudgeStars.HideAll();
				else
					tableJudgeStars.ShowAll();
				
			}
		}
		
		string JudgeStatsToMarkup(double sum, int n, double tresh) {
			if(n==0) {
				return "?";	
			}
			else {
				double avg = sum/n;
				return Math.Abs(avg)>tresh
					? "<b><span color=\"#FF6600\">"+RoundResultData.StatAsString(avg)+"</span></b>"
						: RoundResultData.StatAsString(avg);
			}	
		}
		
		void UpdateGui() {
			// set both labels...
			labelRoomName.Markup = "<b>"+roomData.RoomName+"</b>";					
			labelRoomNo.Markup = "<big><b>"+(roomData.Index+1)+"</b></big>";
			labelRoomNoRight.Markup = "<big><b>"+(roomData.Index+1)+"</b></big>";
			// small or big?
			if(small) {
				MiscHelpers.SetIsShown(frameBig, false);
				for(int i=0;i<widgetsContainer.Count;i++) {
					if(cBig[i].Children.Length>0) {
						cBig[i].Remove(widgetsContainer[i]);
						cSmall[i].Add(widgetsContainer[i]);
					}
				}
				MiscHelpers.SetIsShown(alignSmall, true);
			}
			else {
				MiscHelpers.SetIsShown(alignSmall, false);
				for(int i=0;i<widgetsContainer.Count;i++) {
					if(cSmall[i].Children.Length>0) {
						cSmall[i].Remove(widgetsContainer[i]);
						cBig[i].Add(widgetsContainer[i]);
					}
				}
				MiscHelpers.SetIsShown(frameBig, true);
			}
			
			
			foreach(string sec in sections)
				UpdateGuiSection(sec);
			// update always conflicts, cause we don't know what can happen ;)
			UpdateConflicts();
		}	
		
		void SetDummyLabel(Container c, string section) {
			string text = "";
			switch(section) {
			case "Gov":
			case "Opp":
				text = small ? section : "Drag complete Team here";
				break;
			case "FreeSpeakers":
				text = small ? "F" : "Drag Team Member here";
				break;
			case "Chair":
				text = small ? "C" : "Drag Judge here";
				break;
			case "Judges":
				text = small ? roomData.Judges.Count.ToString() : "Drag Judge here";
				break;
			}
			SetDummyLabel(c, section, text);
		}
		
		void SetDummyLabel(Container c, string section, string text) {	
			Label lbl = new Label();
			lbl.Markup = small ? "<span color=\"#FF6600\"><b><big><big>"+text+"</big></big></b></span>" 
				: "<i>"+text+"</i>";
			lbl.SetPadding(5,5);
			MiscHelpers.AddToContainer(c, lbl, small);
			// Setup appropiate DragDest
			DragDropHelpers.DestSet(lbl,
			             DestDefaults.All,
			             DragDropHelpers.TgtFromString(section),
			             Gdk.DragAction.Move);
			// correctly count the number of "real" widgets in
			// container, since their might be some more
			// widgets for layouting in it (in small mode for example)
			int index = 0;  
			foreach(Widget w in c) 
				if(w is IDragDropWidget)
					index++;
			lbl.DragDataReceived += delegate(object o, DragDataReceivedArgs args) {
				object data = DragDropHelpers.Deserialize(args.SelectionData);
				// save data here	
				SetItem(section, index, data);
				// delete data there
				MyButton b = (MyButton)Drag.GetSourceWidget(args.Context);
				b.Owner.SetData(null);
			};
			
		}
		
		void SetItem(string section, int i, object data) {
			Type type = roomData.GetType().GetProperty(section).PropertyType;			
			if(type == typeof(List<RoundDebater>)) {
				// a judge (not as chair) is set
				List<RoundDebater> list = (List<RoundDebater>)roomData.GetType().
					GetProperty(section).GetValue(roomData, null);
				
				if(data==null)
					list.RemoveAt(i);
				else if(i<list.Count)
					list[i] = (RoundDebater)data;
				else if(i==list.Count)
					list.Add((RoundDebater)data);
				else
					throw new IndexOutOfRangeException("Index too large.");
			}
			else {
				// everything else by calling
				roomData.GetType().InvokeMember(section,
				                       	BindingFlags.SetProperty, null, roomData,
				                        new object[] {data});
			}			
			UpdateGuiSection(section);
			// update always conflicts, cause we don't know what can happen ;)
			UpdateConflicts();
			// set incomplete since room was modified
			roomData.ItemCompleted = false;
		}
		
		public void UpdateConflicts() {
			// also check other rounds, if desired
			penalty = roomData.CalcTotalPenalty(settings.includeOtherRounds, true);
			UpdateStatus();
			// tell the widgets
			foreach(Container c in widgetsContainer) {
				foreach(Widget w in c) {
					if(w is IDragDropWidget) {
						(w as IDragDropWidget).ShowNotifications();
					}
				}
			}
		}
		
		void UpdateStatus() {
			// quick and dirty...
			if(penalty<0) {
				lblConflictStatus.Markup = "<i>Unknown status</i>";
			}
			else {
				if(penalty>settings.conflictThresh && settings.conflictThresh>0) {
					lblConflictStatus.Markup = "Penalty Sum: " +
						"<b><span color=\"#FF6600\">"+penalty+"</span></b>";	
				}
				else {
					lblConflictStatus.Text = "Penalty Sum: "+penalty;	
				}	
			}
		}
		
		object GetField(string field) {
			return this.
				GetType().
					GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).
						GetValue(this);
		}
		
		// the nine integers indicate conflict (-1), nothing (0) and already seen (>0)
		public List<int> GetJudgeMatrixPattern(Debater judge) {
			List<int> list = new List<int>();
			AddSpeakersToList(judge, list, cGov.Child);
			AddSpeakersToList(judge, list, cFreeSpeakers);
			AddSpeakersToList(judge, list, cOpp.Child);
			return list;
		}
		
		void AddSpeakersToList(Debater judge, 
		                       List<int> list, 
		                       Widget e) {
			int i=0;
			if(e is Container) { // might also be a dummy label
				bool isTeam = e is Team; // might also be a table (=cFreeSpeakers
				Container c = (Container)(isTeam ? ((Team)e).GetVboxTeamMembers() : e); 
				foreach(Widget w in c) {
					if(w is DebaterWidget) {
						int conflict = GetJudgeVsSpeaker(judge, (DebaterWidget)w); 
						// cFreeSpeakers as table iterates over debater widgets in
						// reverse order than vbox/hbox containers, so correct that
						if(isTeam)
							list.Add(conflict);
						else
							list.Insert(3, conflict); // i=3 is first freeSpeaker index
						i++;	
					}
				}
			}
			// always pad with zeros...(if we have an imcomplete room)
			for(int j=i;j<3;j++) {
				list.Add(0);	
			}
		}
		
		int GetJudgeVsSpeaker(Debater d, DebaterWidget dw) {
			if(d.ConflictsWith(dw.Debater) || dw.Debater.ConflictsWith(d)) {
				return -1;
			}
			else {
				int n=0;
				MyDictionary<string, int> vr1 = d.VisitedRooms;
				MyDictionary<string, int> vr2 = dw.Debater.VisitedRooms;
				// we are only interested in the intersection of roundNames
				foreach(KVP<string, int> kvp1 in vr1.Store) {
					// skip if this room is in the round...
					if(roomData.RoundName==kvp1.Key)
						continue;	
					// skip if debater isn't in any room
					if(kvp1.Val<0)
						continue;
					KVP<string, int> kvp2;
					if(vr2.GetKVP(kvp1.Key, out kvp2)) {
						// skip if debater isn't in any room
						if(kvp2.Val<0)
							continue;
						// same roomIndex?
						if(kvp1.Val == kvp2.Val) {
							n++;
						}
					}
				}
				return n;
			}
		}
		
		#region IMySearchable implementation
		bool IMySearchable.MatchesSearchString (string key)
		{
			bool match = false;
			foreach(Container c in widgetsContainer) {
				foreach(Widget w in c) {
					if(w is IMySearchable) {
						if((w as IMySearchable).MatchesSearchString(key))
							match = true;
					}
					else if(key == "")
						match=true;
				// iterate all so all matches get red...
				}
			}
			return match;
		}
		#endregion

		public bool Small {
			get {
				return this.small;
			}
			set {
				small = value;
				UpdateGui();
			}
		}
}
}


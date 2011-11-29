using System;
using System.Collections.Generic;
using System.Reflection;
using OPDtabData;
using Gtk;


namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DebaterPool : Gtk.Bin
	{
		int numTeams;
		int numJudges;
		int[] widVboxes = new int[] {0,0};
		string roundName;
				
		public DebaterPool()
		{
			this.Build ();			
			numTeams = 0;
			numJudges = 0;			
			
			Realized += delegate(object sender, EventArgs e) {
				// make whole DebaterPool to DragDest
				SetDragDestToAllWidgets();
			};					
		}
		
		void SetDragDestToAllWidgets() {		
			DragDropHelpers.ApplyToWidget(notebookDragDrop, delegate(Widget w) {
				Drag.DestSet(w, 
				             DestDefaults.Motion | DestDefaults.Drop, 
				             DragDropHelpers.TgtAll, 
				             Gdk.DragAction.Move);
				
				w.DragDataReceived += delegate(object o, DragDataReceivedArgs args) {
					// first delete
					MyButton b = (MyButton)Drag.GetSourceWidget(args.Context);
					b.Owner.SetData(null);
					// then set...
					object data = 
						DragDropHelpers.Deserialize(args.SelectionData);
					SetItem(data);									
				};	
			});		
		}
		
		public void SetGuiToRound(RoundData rd) {
			ClearRound();
			// remember round name
			roundName = rd.RoundName;
			
			// Widgets for Teams			
			foreach(TeamData td in rd.AllTeams) {
				Team w = new Team(td, SetDataTrigger);
				w.SetupDragDropSource("Team", td);
								
				w.Shown += delegate(object sender, EventArgs e) {
					NumTeams++;					
				};
				w.Hidden += delegate(object sender, EventArgs e) {
					NumTeams--;
				};
				w.SizeRequested += VboxChildSizeRequested;
				vboxTeams.Add(w);
				// this shows Team widget if necessary
				w.UpdateTeamMembers();
			}
						
			// Widgets for Judges
			foreach(RoundDebater d in rd.AllJudges) {
				DebaterWidget w = new DebaterWidget(d); 
				
				w.SetupDragDropSource("Judge", d);
				w.SetDataTrigger += SetDataTrigger;
				
				w.Shown += delegate(object sender, EventArgs e) {
					NumJudges++;
				};
				w.Hidden += delegate(object sender, EventArgs e) {
					NumJudges--;
				};
				w.SizeRequested += VboxChildSizeRequested;
				
				vboxJudges.Add(w);
				// show checkbox for availability
				w.ShowJudgeState();
				// Judges are hidden by default
				if(d.IsShown) 
					w.ShowAll();				
			}
		}
		
		void VboxChildSizeRequested(object o, SizeRequestedArgs args) {
			// Teams or Judges?
			int i = o is DebaterWidget ? 1 : 0;
			widVboxes[i] = widVboxes[i]<args.Requisition.Width ? 
						args.Requisition.Width+25+i*30 : widVboxes[i];
			if(notebookDragDrop.CurrentPage==i)
				notebookDragDrop.WidthRequest = widVboxes[i]>200 ? widVboxes[i] : 200;				
		}
		
		void SetDataTrigger(Widget sender, object data) {
			// always hide sender
			(sender as IDragDropWidget).SetIsInPool(false, null);
			if(data != null)
				SetItem(data);
		}
			
		
		void SetItem(object data) {			
			if(data is RoundDebater) {
				RoundDebater d = (RoundDebater)data;
				if(d.Role.IsJudge) {
					FindJudge(d).SetIsInPool(true, roundName);
				}
				else if(d.Role.IsTeamMember) {
					Team t = FindTeam(d.Role.TeamName);
					t.FindTeamMember(d).SetIsInPool(true, roundName);
					t.UpdateTeamMembers();
				}
			}
			else if(data is TeamData) {
				FindTeam((TeamData)data).SetIsInPool(true, roundName);				
			}
			else 
				throw new NotImplementedException("Don't know how to handle data");		
		}
				
		public void ClearRound() {
			MiscHelpers.ClearContainer(vboxTeams);
			NumTeams = 0;
			MiscHelpers.ClearContainer(vboxJudges);
			NumJudges = 0;
		}
		
		DebaterWidget FindJudge(RoundDebater d) {
			foreach(DebaterWidget w in vboxJudges) 
				if(w.Equals(d))
					return w;
			return null;
		}
		
		Team FindTeam(string teamName) {
			foreach(Team w in vboxTeams) 
				if(w.Equals(teamName))
					return w;
			return null;	
		}
		
		Team FindTeam(TeamData td) {
			foreach(Team w in vboxTeams) 
				if(w.Equals(td))
					return w;
			return null;
		}
		
		
		protected virtual void OnNotebookDragDropSwitchPage (object o, Gtk.SwitchPageArgs args)
		{
			notebookDragDrop.WidthRequest = widVboxes[notebookDragDrop.CurrentPage];
		}
		
		public void SetFilter(string key) {
			vboxJudges.ShowAll();
			foreach(IMySearchable w in vboxJudges)
				if(!w.MatchesSearchString(key))
					((Widget)w).HideAll();
			vboxTeams.ShowAll();
			foreach(IMySearchable w in vboxTeams)
				if(!w.MatchesSearchString(key))
					((Widget)w).HideAll();
		}
		
		public int NumJudges {
			get {
				return this.numJudges;
			}
			set {
				labelJudgeList.Markup = "<b>Judges</b> ("+value+")";
				numJudges = value;
				if(numJudges==0) 
					notebookDragDrop.CurrentPage = 0;
				else if(numTeams==0) 
					notebookDragDrop.CurrentPage = 1;
			}
		}

		public int NumTeams {
			get {
				return this.numTeams;
			}
			set {
				labelTeamList.Markup = "<b>Teams</b> ("+value+")";
				numTeams = value;
				if(numTeams==0)
					notebookDragDrop.CurrentPage = 1;
				else if(numJudges==0)
					notebookDragDrop.CurrentPage = 0;	
			}
		}
}
}


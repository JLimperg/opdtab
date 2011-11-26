using System;
using Gtk;
using OPDtabData;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Team : Gtk.Bin, IDragDropWidget, IResultDataWidget, 
		IMySearchable
	{
		
		public event SetDataTriggerHandler SetDataTrigger;
		
		TeamData teamData;
		MyButton btnExpand;	
		MyConflictButton btnConflict;
		
		public static Team Small(TeamData td) {
			Team t = new Team(td, null);
			// generate abbreviated team name
			string abbrTeamName = "";
			foreach(string s in OPDtabData.MiscHelpers.StringToWords(td.TeamName)) {
				//Console.WriteLine(td.TeamName+" "+s);
				string tmp = s.Length>2 ? s.Substring(0,2)+". " : s;
				abbrTeamName = abbrTeamName + tmp;	
			}
			t.btnExpand.LabelText = abbrTeamName;
			t.lblFullTeamName.Markup = "<small>"+GLib.Markup.EscapeText(td.TeamName)+"</small>";
			MiscHelpers.SetIsShown(t.lblFullTeamName, true);
			return t;
		}
		
		public Team(TeamData td) : this(td, null, false) {
		}
		
		public Team(TeamData td, SetDataTriggerHandler dataHandler) : 
		this(td, dataHandler, false) {
		}
		
		public Team(TeamData td, SetDataTriggerHandler dataHandler, bool canFocus) { 
			this.Build();
			teamData = td;
			MiscHelpers.SetIsShown(vboxInfos, false);
			MiscHelpers.SetIsShown(lblFullTeamName, false);
			if(teamData == null) {
				Label lbl = new Label();
				lbl.Markup = "<i>No Team</i>";
				alBtn.Add(lbl);
				alBtn.ShowAll();
				return;
			}
			td.Sort();
			btnExpand = new MyButton(this, teamData.TeamName, "go-down", "go-next");
			btnExpand.CanFocus = canFocus;
			btnExpand.Clicked += OnBtnExpandClicked;
			alBtn.Add(btnExpand);
			alBtn.ShowAll();	
			// conflict...not shown by default
			btnConflict = new MyConflictButton();
			alConflictBtn.Add(btnConflict);
			// when members are draggable, team is also draggable.
			if(dataHandler != null)
				SetDataTrigger += dataHandler;
			SetupTeamMembers(dataHandler);
		}	
		
		
		public void SetupDragDropSource(string t, object data) {
			DragDropHelpers.SetupDragDropSourceButton(btnExpand, t, data);	
		}
			
		void SetupTeamMembers(SetDataTriggerHandler dataHandler) {
			foreach(RoundDebater d in teamData) {				
				DebaterWidget dw = new DebaterWidget(d);					
				if(dataHandler != null) {
					dw.SetupDragDropSource("TeamMember", d);
					// notify owner of child's data
					dw.SetDataTrigger += delegate(Widget sender, object data) {
						dataHandler(sender, data);
						UpdateTeamMembers();
					}; 	
				}
				vboxTeamMembers.PackStart(dw, false, false, 0);
			}
		}
		
		// method only used by DebaterPool
		public void UpdateTeamMembers() {
			int shownMembers = teamData.ShownMembers;	
			if(shownMembers == 0) {
				MiscHelpers.SetIsShown(this, false);
				return;
			}
			else if(shownMembers == vboxTeamMembers.Children.Length) {
				// complete Team
				btnExpand.ResetLabelColor();
				Drag.SourceSet(btnExpand,
			    			   Gdk.ModifierType.Button1Mask,
			        		   DragDropHelpers.TgtTeam,
			            	   Gdk.DragAction.Move);
			}
			else {
				// incomplete Team
				btnExpand.LabelColor = new Gdk.Color(160,0,0);
				Drag.SourceUnset(btnExpand);
			}
			MiscHelpers.SetIsShown(this, true);
		}	
		
			
		protected virtual void OnBtnExpandClicked (object sender, System.EventArgs e)
		{
			MiscHelpers.SetIsShown(vboxInfos, btnExpand.Toggled);
		}
		
		public DebaterWidget FindTeamMember(RoundDebater d) {
			foreach(DebaterWidget w in vboxTeamMembers) 
				if(w.RoundDebater.Equals(d))
					return w;
			return null;
		}
		
		public TeamData TeamData {
			get {
				return this.teamData;
			}
		}
		
		// Equals used by DebaterPool to find Widget by data
		public override bool Equals (object obj)
		{
			if(obj is Team)
				return ((Team)obj).TeamData.Equals(TeamData);
			else 
				return TeamData.Equals(obj);
		}
		
		public override int GetHashCode ()
		{
			return teamData.GetHashCode();
		}
		
		#region IDragDropWidget implementation
		public void SetData (object data)
		{
			if(SetDataTrigger != null)
				SetDataTrigger(this, data);
		}
		public void SetRoom(string n, RoomData rd)
		{
			foreach(IDragDropWidget w in vboxTeamMembers) 
				w.SetRoom(n, rd);
		}
		#endregion

		public override string ToString ()
		{
			return teamData.ToString();
		}
		
		public void SetIsInPool(bool flag, string roundName) {
			foreach(RoundDebater d in teamData)
				d.IsShown = flag;
			MiscHelpers.SetIsShown(this, flag);
			if(flag)
				SetRoom(roundName, RoomData.Dummy());
		}
		
		public void ShowNotifications() {
			RoomConflict sum = new RoomConflict(null);
			foreach(DebaterWidget w in vboxTeamMembers) {
				w.ShowNotifications();
				sum.Merge(w.RoundDebater.GetConflict());
			}
			btnConflict.SetRoomConflict(sum);
		}
				
		#region IMySearchable implementation
		public bool MatchesSearchString (string key)
		{
			bool flag = false;
			foreach(DebaterWidget w in vboxTeamMembers) {
				if(w.MatchesSearchString(key)) {
					flag = true;
					// iterate further so all teammembers get red...
				}
			}
			bool toggled = flag && key != "";
			MiscHelpers.SetIsShown(vboxInfos, toggled);
			btnExpand.Toggled = toggled;
			return flag;
		}
		#endregion

		#region IResultDataWidget implementation
		bool IResultDataWidget.HasResult {
			get {
				if(teamData == null)
					return false;
				foreach(IResultDataWidget w in vboxTeamMembers)
					if(!w.HasResult)
						return false;
				return true;
			}
		}
		
		void IResultDataWidget.UpdateInfo() {
			foreach(IResultDataWidget w in vboxTeamMembers)
				w.UpdateInfo();
		}
		#endregion		
		
		public VBox GetVboxTeamMembers() {
			return vboxTeamMembers;	
		}
		
}
}


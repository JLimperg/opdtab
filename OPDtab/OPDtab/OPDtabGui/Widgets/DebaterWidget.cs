using System;
using System.Collections.Generic;
using OPDtabData;
using Gtk;
namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DebaterWidget : Gtk.Bin, IDragDropWidget, IResultDataWidget, IMySearchable
	{
		MyButton btnExpand;
		MyConflictButton btnConflict;
		CheckButton cbJudgeAvail;
		
		RoundDebater roundDebater;
		Debater debater;
		bool debaterValid;
		Dictionary<string, Widget> infoWidgets;
		public event SetDataTriggerHandler SetDataTrigger;	
		
		// make an abbreviated and small version of this widget!
		public static DebaterWidget Small(RoundDebater d, bool canFocus) {
			DebaterWidget dw = new DebaterWidget(d, canFocus, true);
			dw.btnExpand.LabelText = d.Name.FirstName.Substring(0,1)+". "+
				d.Name.LastName.Substring(0,1)+". ";
			return dw;
		}
		
		public DebaterWidget(RoundDebater rd) : this(rd, false, false) {
			
		}
		
		public DebaterWidget(RoundDebater rd, bool canFocus, bool showDebaterName) 
		{
			this.Build ();
			roundDebater = rd; 
			debater = null;
			debaterValid = false;
			infoWidgets = new Dictionary<string, Widget>();
			
			if(roundDebater==null) {
				Label lbl = new Label();
				lbl.Markup = "<i>No Debater</i>";
				lbl.Xalign = 0f;
				alBtn.Add(lbl);
				alBtn.ShowAll();
				MiscHelpers.SetIsShown(hboxInfo, false);
				return;
			}
			
			// btnExpand Setup
			btnExpand = new MyButton(this, rd.Name.ToString(), 
			                         "weather-clear", "weather-clear-night");
			btnExpand.CanFocus = canFocus;
			btnExpand.Clicked += OnBtnExpandClicked;
			alBtn.Add(btnExpand);
			alBtn.ShowAll();
			
			// infoLabels Setup
			SetupInfo(showDebaterName);
			
			// conflict notifications
			btnConflict = new MyConflictButton();
			alConflictBtn.Add(btnConflict);
			
			// checkbox availJudges (default not shown, 
			// activated by ShowJudgeAvail from DebaterPool)
			cbJudgeAvail = new CheckButton();
			cbJudgeAvail.Active = roundDebater.JudgeAvail;
			btnExpand.SetStrikeThrough(!cbJudgeAvail.Active);
			cbJudgeAvail.Clicked += delegate(object sender, EventArgs e) {
				roundDebater.JudgeAvail = cbJudgeAvail.Active;
				btnExpand.SetStrikeThrough(!cbJudgeAvail.Active);
			};
			alJudgeAvail.Add(cbJudgeAvail);
			alJudgeAvail.NoShowAll = true;
			
			
			
			// SetIsShown in Realized so that everything is counted correctly by triggered events
			Realized += delegate(object sender, EventArgs e) {
				MiscHelpers.SetIsShown(this, roundDebater.IsShown);
				MiscHelpers.SetIsShown(hboxInfo, false);
			};	
		} 
		
		public void SetupDragDropSource(string t, object data) {
			DragDropHelpers.SetupDragDropSourceButton(btnExpand, t, data);	
		}
		
		void SetupInfo(bool showDebaterName) {			
			// Full Debater Name if for "small Mode"
			infoWidgets["fullName"] = MakeInfoLabel(roundDebater.Name.ToString());
			infoWidgets["fullName"].NoShowAll = !showDebaterName;
			// Role (is a bit more complex)
			HBox hboxRole = new HBox();
			HBox hboxStars = new HBox();
			hboxStars.Spacing = 0;
			//hboxRole.Spacing = 6;
			hboxRole.PackStart(hboxStars, false, false, 0);
			hboxRole.PackStart(MakeInfoLabel("<i>No Role</i>"), true, true, 0);
			infoWidgets["roleAndClub"] = hboxRole;
			// RoundResults
			infoWidgets["roundResults"] = MakeInfoLabel("<i>No results.</i>");
			foreach(Widget w in infoWidgets.Values)                              
				vboxInfo.Add(w);
			UpdateInfo();
			UpdateStaticInfo();
		}
		Widget MakeInfoLabel(string text) {
			Label lbl = new Label();
			lbl.Markup = SmallText(text);
			lbl.Xalign = 0;
			return lbl;
		}
		
		public void UpdateInfo() {
			if(!btnExpand.Toggled) 
				return;
			if(Debater != null) {
				SetMarkup(infoWidgets["roundResults"],SmallText(Debater.RoundResultsAsMarkup()));			
			}
			
		}		
		
		void UpdateStaticInfo() {
			if(Debater != null) {
				HBox c = infoWidgets["roleAndClub"] as HBox;
				if(Debater.Role.IsTeamMember)
					SetMarkup(c.Children[1], Debater.Role.TeamName+"\n"+Debater.Club.Name);
				else if(Debater.Role.IsJudge) {
					HBox hboxStars = c.Children[0] as HBox;
					foreach(Widget w in hboxStars)
						hboxStars.Remove(w);
					// Display stars for judgeQuality
					for(int i=0;i<Debater.Role.JudgeQuality;i++)
						hboxStars.Add(new Image(MiscHelpers.LoadIcon("face-smile")));
					if(Debater.Role.JudgeQuality>0)
						c.Spacing = 6;
					SetMarkup(c.Children[1], Debater.Club.Name);
				}
			}
		}
		
		void SetMarkup(Widget lbl, object text) {
			(lbl as Label).Markup = SmallText(text);
		}
		
		string SmallText(object text) {
			return "<small>"+text.ToString()+"</small>";
		}
		
		public void ShowNotifications() {
			if(roundDebater==null)
				return;
			// we only have conflicts as notifications
			btnConflict.SetRoomConflict(roundDebater.GetConflict());
		}	
				
		public void ShowJudgeAvail() {
			MiscHelpers.SetIsShown(alJudgeAvail, true);
		}
		
		public bool JudgeAvail {
			get {
				return cbJudgeAvail.Active;
			}
		}
		
		protected virtual void OnBtnExpandClicked (object sender, System.EventArgs e)
		{
			UpdateInfo();
			MiscHelpers.SetIsShown(hboxInfo, btnExpand.Toggled);	
		}
		
		public RoundDebater RoundDebater {
			get {
				return this.roundDebater;
			}
		}
		
		public Debater Debater {
			get {
				if(!debaterValid) {
					debaterValid = true;
					debater = Tournament.I.FindDebater(roundDebater);
				}
				return debater;
			}			
		}
		
		#region IDragDropWidget implementation
		void IDragDropWidget.SetData (object data)
		{
			if(SetDataTrigger != null) 
				SetDataTrigger(this, data);
						
		}
		public void SetRoom(string rN, RoomData rd)
		{
			if(Debater != null)
				Debater.SetRoom(rN, rd);
			else
				Console.WriteLine("WARNING: Debater NOT FOUND!");
		}
		#endregion
		
		public override string ToString ()
		{
			return string.Format ("[DebaterWidget: Debater={0}]", RoundDebater);
		}
		
		// Equals used by DebaterPool to find Widget by data
		public override bool Equals (object obj)
		{
			if(obj is DebaterWidget)
				return ((DebaterWidget)obj).RoundDebater.Equals(RoundDebater);
			else if(obj is RoundDebater)
				return ((RoundDebater)obj).Equals(RoundDebater);
			else
				throw new NotImplementedException();
		}
		
		public override int GetHashCode ()
		{
			return roundDebater.GetHashCode();
		}	

		#region IMySearchable implementation
		public bool MatchesSearchString (string key)
		{
			bool match = roundDebater.MatchesSearchString(key);
			if(match && key != "")
				btnExpand.LabelColor = new Gdk.Color(255,0,0);
			else
				btnExpand.ResetLabelColor();
			return match;
		}
		#endregion	
		
		public void SetIsInPool(bool flag, string roundName) {
			roundDebater.IsShown = flag;			
			MiscHelpers.SetIsShown(this, flag);
			
			if(flag)
				SetRoom(roundName, RoomData.Dummy()); 
		}
		
		#region IResultDataWidget implementation
		public bool HasResult {
			get {
				return Debater != null;	
			}
		}
		#endregion
}
}


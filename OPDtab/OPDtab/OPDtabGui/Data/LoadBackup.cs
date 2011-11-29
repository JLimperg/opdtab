using System;
using System.IO;
using OPDtabData;
using Gtk;
using System.Collections.Generic;
namespace OPDtabGui
{
	public partial class LoadBackup : Gtk.Window
	{
		ListStore store;
		Tournament tour;
		public LoadBackup () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			// determine available backups and append to store
			string dir = System.IO.Path.GetDirectoryName(AppSettings.I.TournamentFile);
			string backupPrefix = System.IO.Path.ChangeExtension(AppSettings.I.TournamentFile, "backup");
			DateTime now = DateTime.Now;
			store = new ListStore(typeof(string), typeof(TimeSpan));
			                                          
			foreach(string backupFile in Directory.GetFiles(dir, backupPrefix+"*")) {
				TimeSpan lastWriteAgo = now.Subtract(Directory.GetLastWriteTime(backupFile));
				store.AppendValues(backupFile, lastWriteAgo);
			}
			// setup combobox
			TreeModelSort sortedStore = new TreeModelSort(store);
			sortedStore.SetSortFunc(0, delegate(TreeModel model, TreeIter a, TreeIter b) {
				TimeSpan tsA = (TimeSpan)model.GetValue(a, 1);
				TimeSpan tsB = (TimeSpan)model.GetValue(b, 1);
				return tsA.CompareTo(tsB);
			});
			sortedStore.SetSortColumnId(0, SortType.Ascending);
			cbBackupFiles.Model = sortedStore;
			CellRendererText cellFile = new CellRendererText();
			cbBackupFiles.PackStart(cellFile, false);
			cbBackupFiles.SetCellDataFunc(cellFile, delegate(CellLayout layout,
		                          				  CellRenderer cell,
		                          				  TreeModel model,
		                          				  TreeIter iter) {
				object o = model.GetValue(iter, 0);
				if(o == null)
					return;
				(cell as CellRendererText).Text = System.IO.Path.GetFileName(o as string);
			});
			CellRendererText cellLastWrite = new CellRendererText();
			cbBackupFiles.PackStart(cellLastWrite, false);
			
			cbBackupFiles.SetCellDataFunc(cellLastWrite, delegate(CellLayout layout,
		                          				  CellRenderer cell,
		                          				  TreeModel model,
		                          				  TreeIter iter) {
			
				object o = model.GetValue(iter, 1);
				if(o == null)
					return;
				CellRendererText cellText = cell as CellRendererText;
				cellText.Xpad = 10;
				cellText.Markup = "<i><small>"+FormatTimeSpan((TimeSpan)o)+" ago</small></i>";
			});	
		}
		
		void ShowFileInfo() {
			if(tour==null)
				return;
			List<TeamData> teams = new List<TeamData>();
			int nJudges = 0;
			foreach(Debater d in tour.Debaters) {
				if(d.Role.IsTeamMember) 
					RoundData.AddMemberToTeamList(teams, new RoundDebater(d, true));
				else if(d.Role.IsJudge)
					nJudges++;
			}
			int nCompleteTeams = 0;
			foreach(TeamData td in teams)
				if(td.NTeamMembers==3)
					nCompleteTeams++;
			labelDebaters.Text = tour.Debaters.Count+" Debaters as\n"+
				teams.Count+" Teams ("+nCompleteTeams+" complete) and\n"+
					nJudges+" Judges";
			labelRounds.Text = "Rounds ("+tour.Rounds.Count+"):";
			foreach(Widget w in vboxRounds)
				vboxRounds.Remove(w);
			foreach(RoundData rd in tour.Rounds) {
				int nNonEmptyRooms = 0;
				foreach(RoomData room in rd.Rooms)
					if(!room.IsEmpty)
						nNonEmptyRooms++;
				Label label = new Label();
				label.Xalign = 0f;
				label.Markup = "<i>"+rd.RoundName+":</i> "
					+nNonEmptyRooms+" non-empty Rooms ("+rd.Rooms.Count+" total)";
				vboxRounds.PackStart(label, false, false, 0);  				
			}
			vboxRounds.PackStart(new Label(), true, true, 0);
			vboxRounds.ShowAll();
		}
		
				
		string FormatTimeSpan(TimeSpan ts) {
			string prefix = "";
			if(ts.Days>0)
				prefix = ts.Days+"d ";
			
			return 	prefix+String.Format(new System.Globalization.CultureInfo("en-US"),
				"{0:00}",ts.Hours)+":"+String.Format(new System.Globalization.CultureInfo("en-US"),
				"{0:00}",ts.Minutes);
		}
		
		protected virtual void OnCbBackupFilesChanged (object sender, System.EventArgs e)
		{
			TreeIter iter;
			if(cbBackupFiles.GetActiveIter(out iter)) {
				iter = (cbBackupFiles.Model as TreeModelSort).ConvertIterToChildIter(iter);
				string backupFile = (string)store.GetValue(iter, 0);
				try {
					tour = Tournament.DoLoad(backupFile);
					ShowFileInfo();
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this,
					                        "Unable to load "+backupFile+": "+ex.Message, 
					                        MessageType.Error);	
					return;
				}
			}
		}
		
		protected virtual void OnBtnLoadClicked (object sender, System.EventArgs e)
		{
			if(tour==null) {
				MiscHelpers.ShowMessage(this, "None selected.", MessageType.Error);
				return;
			}
			if(MiscHelpers.AskYesNo(this, "This will overwrite ALL data! Continue?") == ResponseType.Yes) {
				Tournament.I = tour;
				ShowRanking.I.UpdateAll();
				MiscHelpers.ShowMessage(this, "Backup loaded successfully.", MessageType.Info);	
			}
			
		}
		
		
		
	}
}


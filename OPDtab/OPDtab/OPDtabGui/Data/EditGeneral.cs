using System;
using OPDtabData;
using System.IO;
using System.Collections.Generic;
using Gtk;

namespace OPDtabGui
{
	public partial class EditGeneral : Gtk.Window
	{
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			// only close window if change of data file is successful
			args.RetVal = true;
			try {
				// tournament file changed?
				if(AppSettings.I.TournamentFile != entryTournamentFile.Text) {
					Tournament.I.Save(entryTournamentFile.Text);
					AppSettings.I.TournamentFile = entryTournamentFile.Text;
				}
				// save other stuff
				AppSettings.I.DeleteTexFile = cbDeleteTexFile.Active;
				Tournament.I.Title = entryTournamentTitle.Text;
				// can close window
				args.RetVal = false;
			}
			catch(Exception e) {
				MiscHelpers.ShowMessage(this, "Could not save settings. "+e.Message,
				                        MessageType.Error);
			}
			
		}
		
		
		public EditGeneral () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			entryTournamentTitle.Text = Tournament.I.Title;
			entryTournamentFile.Text = AppSettings.I.TournamentFile;
			cbDeleteTexFile.Active = AppSettings.I.DeleteTexFile;
		}
		
		// TODO finish implementation of export of all data...
		// the problem is, that it is 3 dimensional: 
		// speakers, judges, rounds are the "axes"
			
		
		/*protected virtual void OnBtnExportRoundDataClicked (object sender, System.EventArgs e)
		{
			
			
			FileChooserDialog dlg = new FileChooserDialog("Choose CSV file", this, FileChooserAction.Save, 
			                                              "Cancel",ResponseType.Cancel,
		                            					  "Save",ResponseType.Accept);
			if(dlg.Run() == (int)ResponseType.Accept) {
				// get avail judges
				List<Debater> judges = new List<Debater>();
				foreach(Debater d in Tournament.I.Debaters) {
					if(d.Role.IsJudge) 
						judges.Add(d);
				}
				TextWriter tw = new StreamWriter(dlg.Filename);
				
				foreach(RoundData rd in Tournament.I.Rounds) {
					Console.WriteLine(rd.RoundName);
					foreach(RoomData room in rd.Rooms) {
						//room.GetRoomMembers()	
					}
				}
				
				
				tw.Close();
			}
			dlg.Destroy();
		}*/
	}
}


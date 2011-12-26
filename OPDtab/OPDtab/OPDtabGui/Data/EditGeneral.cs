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
		
		protected void OnBtnExportRoundDataClicked (object sender, System.EventArgs e)
		{			
			
			// the Gtk Warning can be ignored, it's a bug in Gtk...
			FileChooserDialog dlg = new FileChooserDialog("Choose CSV file", this, FileChooserAction.Save, 
			                                              "Cancel",ResponseType.Cancel,
		                            					  "Save",ResponseType.Accept);
			dlg.SetFilename(System.IO.Path.Combine(Directory.GetCurrentDirectory, "RoundData.csv"));
			TextWriter tw;
			string fileName;
			if(dlg.Run() == (int)ResponseType.Accept) {
				// try open the file for writing
				try {
					fileName = dlg.Filename;
					dlg.Destroy();
					tw = new StreamWriter(fileName);
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Could not open file: "+ex.Message, MessageType.Error);
					return;
				}
			}
			else {
				dlg.Destroy();
				return;
			}
			
			
			// get judges  and speakers
			List<Debater> judges = new List<Debater>();
			List<Debater> speakers = new List<Debater>();
			foreach(Debater d in Tournament.I.Debaters) {
				if(d.Role.IsJudge) {
					judges.Add(d);
				}
				else if(d.Role.IsTeamMember) {
					speakers.Add(d);	
				}
			}
			judges.Sort();
			speakers.Sort();
			
			// write out the header
			List<string> judgesStr = new List<string>();
			foreach(Debater d in judges) {
				judgesStr.Add(d.Name+" ("+d.Club+")");
			}
			WriteCSVLine(tw, "Name", "Club", "Team", "Round", "Room", "Position", judgesStr, judgesStr);
				
			// for each round, write out all team members and it's results
			foreach(RoundData rd in Tournament.I.Rounds) {
				
				foreach(Debater d in speakers) {
					// init with null
					string roomStr = null;
					string posStr = null;
					List<string> points = new List<string>();
					for(int i=0;i<2*judges.Count;i++)
						points.Add(null);
					// check if debater was set in this round
					int roomIdx = d.GetRoomIndex(rd.RoundName);
					if(roomIdx>=0) {
						roomStr = (roomIdx+1).ToString();
						RoundResultData rr = d.RoundResults.Find(delegate(RoundResultData obj) {
							return obj.Equals(rd.RoundName);	
						});
						// check if debater has some results
						if(rr!=null) {
							posStr = rr.GetPosAsString();	
							// get the judges in the room, determine indices
							List<int> judgesIndex = new List<int>();
							foreach(RoundDebater j in rd.Rooms[roomIdx].Judges) {
								judgesIndex.Add(judges.FindIndex(delegate(Debater obj) {
									return obj.Equals(j);	
								}));
							}
							// check if data makes sense
							// rr.Role is for speaker only Gov, Opp, Free (Judge is impossible!)
							if(rr.SpeakerScores.Count!=judgesIndex.Count ||
							   rr.TeamScores.Count!=judgesIndex.Count) {
								Console.WriteLine("Data for "+d+" inconsistent, skipping.");
								continue;
							}
							// write it in points, doubled size since speaker and team points...
							for(int i=0;i<judgesIndex.Count;i++) {
								points[judgesIndex[i]] = rr.SpeakerScores[i].ToString();
								if(rr.Role != RoundResultData.RoleType.Free)
									points[judges.Count+judgesIndex[i]] = rr.TeamScores[i].ToString();
							}
						}
					}
					WriteCSVLine(tw, d.Name, d.Club, d.Role, rd.RoundName, roomStr, posStr, points);
				}
				
			}
			
			tw.Close();
			MiscHelpers.AskShowTemplate(this, "RoundData successfully exported.", fileName);
		}
		
		void WriteCSVLine(TextWriter tw, params object[] items) {
			List<string> strings = new List<string>();
			foreach(object o in items) {
				if(!(o is string) && o is System.Collections.IEnumerable) {
					foreach(object o_ in (o as System.Collections.IEnumerable))
						strings.Add("\""+o_+"\"");
				}
				else {
					strings.Add("\""+o+"\"");
				}
			}
			tw.WriteLine(String.Join(",",strings));
		}
		
		
	}
}


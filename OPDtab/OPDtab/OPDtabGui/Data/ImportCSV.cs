using System;
using System.IO;
using System.Collections.Generic;
using LumenWorks.Framework.IO.Csv;
using OPDtabData;
using Gtk;

namespace OPDtabGui
{
	public partial class ImportCSV : Gtk.Window
	{
		public ImportCSV () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			UpdateHeaderLabels();
		}		
		
		
		protected virtual void OnBtnChooseFileClicked (object sender, System.EventArgs e)
		{
			FileChooserDialog fc = new FileChooserDialog("Choose CSV",
			                                             this,
			                                             FileChooserAction.Open,
			                                             "Cancel",ResponseType.Cancel,
			                                             "Ok",ResponseType.Ok);
			if((ResponseType)fc.Run() == ResponseType.Ok) {
				labelFile.Text = fc.Filename;
				UpdateHeaderLabels();
			}
			fc.Destroy();
		}
		
		protected virtual void OnBtnImportCSVClicked (object sender, System.EventArgs e)
		{
			if(cbOverwrite.Active)
				if(MiscHelpers.AskYesNo(this, 
				                        "This clears all " +
				                         "entered Data for " +
				                          "overwritten Debaters. " +
				                           "Continue?") 
					== ResponseType.No)
						return;
			// make a backup before!
			Tournament.I.Save();
			// start import
			List<Debater> debaters = Tournament.I.Debaters;
			int line = 0;
			try {
				CsvReader csv = new CsvReader(new StreamReader(labelFile.Text),
				                              cbHasHeaders.Active,',','"','\\','#',
				                              ValueTrimmingOptions.All); 
        		
				int dups = 0;
				int ok = 0;
				while (csv.ReadNextRecord())
        		{
            		line++;
					EditableDebater d = new EditableDebater();
					
					d.Name = new Name(csv[sbLastName.ValueAsInt]+", "+csv[sbFirstName.ValueAsInt]);
					
					// Club 
					try {
						if(cbCity.Active)
							d.Club = new Club(csv[sbClub.ValueAsInt]+", "+csv[sbCity.ValueAsInt]);
						else {
							d.Club = new Club(csv[sbClub.ValueAsInt]);
						}
					}
					catch {
						d.Club = new Club("None");	
						d.BlackList.removePattern("None");
					}
					
					// Age or from Birthday
					try {
						if(cbAge.Active) 
							d.ParseAge(csv[sbAge.ValueAsInt]);
						
						if(cbBirthday.Active) { 
							DateTime bday = DateTime.ParseExact(csv[sbBirthday.ValueAsInt],
						    	                                entryBdayFormat.Text,
						        	                            null);
							DateTime now = DateTime.Today;
							int age = now.Year - bday.Year;
							if (bday > now.AddYears(-age)) age--;
							d.Age = (uint)age;
						}
						
					}
					catch {}
							
					// Role
					if(cbRole.Active) {
						if(rbTeamMember.Active) {
							d.ParseRole(csv[sbRole.ValueAsInt]);
						}
						else if(rbJudge.Active) {
							int judgeQuality = 0;
							try {
								judgeQuality = (int)uint.Parse(csv[sbRole.ValueAsInt]);
							}
							catch {}
							d.Role.JudgeQuality = judgeQuality;
						}
					}
					
					// ExtraInfo
					d.ExtraInfo = entryExtraInfoDefault.Text; 
					if(cbExtraInfo.Active)
						d.ExtraInfo = csv[sbExtraInfo.ValueAsInt] == "" ? 
							entryExtraInfoDefault.Text : csv[sbExtraInfo.ValueAsInt]; 
					
					// BlackList
					if(cbBlackList.Active)
						d.ParseBlackList(csv[sbBlackList.ValueAsInt]);
					
					
					// save it
					int i = debaters.FindLastIndex(delegate(Debater de) {
						return de.Equals(d);	
					});
					
					if(i<0) {
						debaters.Add(new Debater(d));
						ok++;
					}
					else {
						Console.WriteLine("Duplicate: "+d+", "+debaters[i]);
						dups++;
						if(cbOverwrite.Active) {
							debaters[i] = new Debater(d);
							ok++;
						}
					}
					
					
					
        		}
				
				string action = cbOverwrite.Active ? "overwritten" : "discarded";
				MiscHelpers.ShowMessage(this, "Imported "+ok+", "+action+" "+dups+" duplicates.",
				                        MessageType.Info);
			}
			catch(Exception ex) {
				MiscHelpers.ShowMessage(this, "Error encountered in line "+line.ToString()+": "+ex.Message,
				                        MessageType.Error);
			}
		}
		
		void UpdateHeaderLabels() {
			hdFirstName.Text = "";
			hdLastName.Text = "";
			hdClub.Text = "";
			hdCity.Text = "";
			hdAge.Text = "";
			hdBirthday.Text = "";
			hdRole.Text = "";
			labelHeader.Text = "";
			
			if(!cbHasHeaders.Active) 
				return;
			
			labelHeader.Text = "Header";
			
			try {
				CsvReader csv = new CsvReader(new StreamReader(labelFile.Text),
				                              true,',','"','\\','#',
				                              ValueTrimmingOptions.All);
				SetHeaderLabel(csv,sbFirstName,hdFirstName);
				SetHeaderLabel(csv,sbLastName,hdLastName);
				SetHeaderLabel(csv,sbClub,hdClub);
				SetHeaderLabel(csv,sbCity,hdCity);
				SetHeaderLabel(csv,sbAge,hdAge);
				SetHeaderLabel(csv,sbBirthday,hdBirthday);
				SetHeaderLabel(csv,sbRole,hdRole);
				SetHeaderLabel(csv,sbExtraInfo, hdExtraInfo);
				SetHeaderLabel(csv,sbBlackList, hdBlackList);
				
				
			}
			catch {		
				hdFirstName.Text = "?";
				hdLastName.Text = "?";
				hdClub.Text = "?";
				hdCity.Text = "?";
				hdAge.Text = "?";
				hdBirthday.Text = "?";
				hdRole.Text = "?";
				hdExtraInfo.Text = "?";
				hdBlackList.Text = "?";
			}
		}
		
		void SetHeaderLabel(CsvReader csv, SpinButton sb, Label hd) {
			int fieldCount = csv.FieldCount;
        	string[] headers = csv.GetFieldHeaders();
			if(sb.ValueAsInt<fieldCount)
				hd.Text = headers[sb.ValueAsInt];
			else
				hd.Text = "?";	
		}
		
		
		
		protected virtual void OnCbHasHeadersToggled (object sender, System.EventArgs e)
		{
			UpdateHeaderLabels();
		}
		
		protected virtual void OnSbChanged (object sender, System.EventArgs e)
		{
			UpdateHeaderLabels();
		}
		
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			
		}
		
		
		
		
		
		
	}
}


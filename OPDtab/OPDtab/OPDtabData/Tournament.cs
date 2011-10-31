using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Timers;
using Gtk;

namespace OPDtabData
{
	public class Tournament
	{
		static Tournament instance;
				
		List<Debater> debaters;
		List<RoundData> rounds;
		List<EqualPointsResolver> resolvers;
		string title;
		string roomDetails;	
		bool pleaseSave;
		Timer timer;
		
		
		public Tournament()
		{
			debaters = new List<Debater>();	
			rounds = new List<RoundData>();
			resolvers = new List<EqualPointsResolver>();
			timer = new Timer(10000);
			timer.AutoReset = true;
			timer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
				if(pleaseSave)
					Save(true);
				pleaseSave = false;
			};
			pleaseSave = false;
			timer.Start();
			
			title = "My Tournament";
			roomDetails = "Raum 1|Ort 1\n"+
				"Raum 2|Ort 2\n"+
				"Raum 3|Ort 3\n"+
				"Raum 4|Ort 4\n"+
				"Raum 5|Ort 5\n"+
				"Raum 6|Ort 6\n"+
				"Raum 7|Ort 7\n"+
				"Raum 8|Ort 8\n"+
				"Raum 9|Ort 9\n"+
				"Raum 10|Ort 10\n"+
				"Raum 11|Ort 11\n"+
				"Raum 12|Ort 12\n"+
				"Raum 13|Ort 13\n"+
				"Raum 14|Ort 14\n"+
				"Raum 15|Ort 15\n";
		}
		
		[XmlIgnore]
		public static Tournament I {
			get {
				if(instance == null)
					throw new Exception("Should call Load() before using I.");
				return instance;
			}
			set {
				instance = value;
			}
		}
		
		public void Save() {
			Save(false);	
		}
		
		public void Save(bool force) {
			try {
				if(force)
					Tournament.I.Save(AppSettings.I.TournamentFile);
				else 
					pleaseSave = true;
			}
			catch(Exception e) {
				// just print out errors
				Console.WriteLine("WARNING: Tournament NOT SAVED: "+e.Message+" "+e.InnerException);
			}
				
		}
		
		public void Save(string fileName) {
            XmlWriter writer = null;
            string tmpFile = Path.GetTempFileName();
			try {
    			XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				Console.Write("Trying to save to '"+tmpFile+"'...");
				writer = XmlWriter.Create(tmpFile, settings);
				XmlSerializer xml = new XmlSerializer(typeof(Tournament));
				xml.Serialize(writer, this);
	        	Console.WriteLine("Ok.");
			} finally {
				if (null != writer) {
					writer.Close();
				}                    
            }
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));
	        string backupFile = 
				Path.ChangeExtension(fileName, "backup-"+
				                     DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss'Z'"));
			if(File.Exists(backupFile)) {
				Console.WriteLine("WARNING: Backup '"+backupFile+"' needed to be removed.");
				File.Delete(backupFile);
			}			
			if(File.Exists(fileName))
				File.Move(fileName, backupFile);
			File.Move(tmpFile, fileName);
			File.Delete(tmpFile);
			Console.WriteLine("Tournament saved in '"+fileName+"', Backup kept in '"+backupFile+"'");
		}
    
    	public static void Load(string fileName) {
        	Tournament tour = null;
        	try {
               	tour = DoLoad(fileName);
				Console.WriteLine("Tournament loaded from '"+fileName+"'. Debaters: "+tour.Debaters.Count);
        	} catch(Exception e) {
				Console.WriteLine("Error loading Tournament in "+fileName+", empty loaded: "+e.Message+" "+e.InnerException);
            	tour = new Tournament();
        	} 
			instance = tour;
			//instance.Debaters.Sort();			
    	}

		public static Tournament DoLoad(string fileName) {
			XmlReader reader = null;
			Tournament tour = null;
			try {
				reader = XmlReader.Create(fileName); 
				XmlSerializer xml = new XmlSerializer(typeof(Tournament));
				tour = (Tournament)xml.Deserialize(reader);
			}
			finally {
            	if (null != reader)
                	reader.Close();
        	}	
			return tour;
		}
		
		
		public string Title {
			get {
				return this.title;
			}
			set {
				title = value;
			}
		}
		
		public string RoomDetails {
			get {
				return this.roomDetails;
			}
			set {
				roomDetails = value;
			}
		}

		public Debater FindDebater(RoundDebater rd) {
			// optimize by sort does not seem beneficial...
			if(rd==null || rd.IsEmpty)
				return null;
			return debaters.Find(delegate(Debater d) {
				return d.Equals(rd);	
			});
		}
		
		public RoundData FindRound(string roundName) {
			return rounds.Find(delegate(RoundData rd) {
				return rd.RoundName.Equals(roundName);	
			});	
		}
		
		public EqualPointsResolver FindResolver(string id) {
			return resolvers.Find(delegate(EqualPointsResolver res) {
				return res.Id.Equals(id);	
			});	
		}
		
		public List<Debater> Debaters {
			get {
				return debaters;	
			}
			set {
				debaters = value;	
				Save();
			}
		}
	
		public List<RoundData> Rounds {
			get {
				return this.rounds;
			}
			set {
				rounds = value;
				Save();
			}
		}

		public List<EqualPointsResolver> Resolvers {
			get {
				return this.resolvers;
			}
			set {
				resolvers = value;
			}
		}
}
}


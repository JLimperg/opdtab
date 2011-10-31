using System;
using Gtk;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace OPDtabData
{
	
	
	
	
	[Serializable]
	public class AppSettings
	{
		[Serializable]
		public class EditDebatersClass {
			public int lastSortCol = 0;
			public SortType lastSortOrder = SortType.Descending; 
			public int sortCol = 0;
			public SortType sortOrder = SortType.Descending; 
		}
		
		[Serializable]
		public class GenerateRoundClass {			
			/* hint:
				JudgeVsTeam=0, 
				JudgeVsFree=1,
				JudgeVsJudge=2,
				TeamVsTeam=3, 
				TeamVsFree=4, 			
				FreeVsFree=5,
				ChairVsAny=6 */
			public int conflictThresh = 2000;
			public bool includeOtherRounds = true;
			public string[] possibleIcons 
				= new string[] {"dialog-information", 
								"help-browser",
								"applications-other", 
								"software-update-available",	
								"software-update-urgent"};
			// one more than number of RoomConflict.Type, last one is "Other Round conflict"
			public int[] conflictIcons = new int[] {4, 4, 0, 2, 3, 2, 0, 1};	
			public int[] conflictPenalties = new int[] {1000, 800, 10, 600, 400, 200, 0, 100};
			public string[][] breakConfigPresets = new string[][] {
				new string[] {"RoundOf16", "1-16\n15-2\n3-14\n13-4\n5-12\n11-6\n7-10\n9-8"},
				new string[] {"QuarterFinal", "1-8\n7-2\n3-6\n5-4"},
				new string[] {"HalfFinal", "1-4\n3-2"},
				new string[] {"Final","1-2"},
				new string[] {"LukasHaffert27", "1-2\n3-4\n5-6\n7-8\n10-9\n11-12\n14-13\n16-15\n18-17\n\n"+
					"17-1\n2-18\n15-3\n4-16\n13-5\n6-14\n12-7\n8-10\n9-11"},
				new string[] {"LukasHaffert24", "1-2\n3-4\n5-6\n7-8\n9-10\n12-11\n14-13\n16-15\n\n"+
					"15-1\n2-16\n13-3\n4-14\n11-5\n6-12\n10-7\n8-9"}
			};
		}
		
		[NonSerialized] static int VERSION = 7;
		[NonSerialized] static AppSettings instance;
		string tournamentFile;
		EditDebatersClass editDebaters;
		GenerateRoundClass generateRound;
		//static public TemplatePool Tmpl;
		
		public AppSettings() {
			tournamentFile = Path.Combine(Directory.GetCurrentDirectory(),
			                              Path.Combine("data","tournament.dat"));
			editDebaters = new EditDebatersClass();
			generateRound = new GenerateRoundClass();
		}
		
		public static AppSettings I {
			get {
				if(instance == null)
					throw new Exception("Should call Load() before using I.");
				return instance;
			}
		}

		
		public string TournamentFile {
			get {
				return this.tournamentFile;
			}
			set {
				tournamentFile = value;
			}
		}
		
		public EditDebatersClass EditDebaters {
			get {
				return this.editDebaters;
			}
			set {
				editDebaters = value;
			}
		}

		public GenerateRoundClass GenerateRound {
			get {
				return this.generateRound;
			}
			set {
				generateRound = value;
			}
		}

		public void Save(string fileName) {
            Stream stream = null;
			try {
                IFormatter formatter = new BinaryFormatter();
				//fileName = Path.Combine(Directory.GetCurrentDirectory(),fileName);
				stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, VERSION);
                formatter.Serialize(stream, this);
            	Console.WriteLine("Defaults saved.");
			} catch(Exception e) {
                // do nothing, just ignore any possible errors
				Console.WriteLine("Defaults NOT saved: "+e.Message);
            } finally {
                if (null != stream)
                    stream.Close();
            }
			
    	}
    
    	public static void Load(string fileName) {
        	Stream stream = null;
        	AppSettings settings = null;
			try {
            	IFormatter formatter = new BinaryFormatter();
            	stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            	int version = (int)formatter.Deserialize(stream);
            	if(version != VERSION)
					throw new Exception("Version mismatch.");
            	settings = (AppSettings)formatter.Deserialize(stream);
				Console.WriteLine("Defaults loaded. Version: "+version);
        	} catch(Exception e) {
				Console.WriteLine("Error loading Defaults, standard loaded: "+e.Message);
            	settings = new AppSettings();
        	} finally {
            	if (null != stream)
                	stream.Close();
        	}
			instance = settings;
			// init template engine
			TemplatePool.Singleton(Path.Combine(Directory.GetCurrentDirectory(), 
			                                    "tmpl")+Path.DirectorySeparatorChar, 10);
    	}

	}
}


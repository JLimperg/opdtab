using System;
using Gtk;
using OPDtabData;

namespace OPDtab
{
	class MainClass
	{
		const string defaultsFile = "defaults.dat";

#pragma warning disable RECS0154 // Parameter is never used
		public static void Main (string[] args)
#pragma warning restore RECS0154 // Parameter is never used
		{
			Console.WriteLine("OPDtab version " + Constants.VersionString + " by Andreas Neiser");
			Console.WriteLine(Constants.Website);
			Console.WriteLine(Constants.LegalInfo);
							
			AppSettings.Load(defaultsFile);
			Tournament.Load(AppSettings.I.TournamentFile);
			Application.Init();
			(new MainWindow()).Show();
			Application.Run();
			Tournament.I.Save(true);
			AppSettings.I.Save(defaultsFile);
		}
	}
}

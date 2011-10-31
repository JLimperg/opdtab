using System;
using System.IO;
using Gtk;
using OPDtabData;

namespace OPDtab
{
	class MainClass
	{
		const string defaultsFile = "defaults.dat";
		
		public static void Main (string[] args)
		{
			Console.WriteLine("OPDtab version 3.14 by Andreas Neiser");
			Console.WriteLine("see http://code.google.com/p/opdtab/");
			Console.WriteLine("This work is licensed under GPLv3");
			Console.WriteLine("Attribution to LumenWorks.Framework.IO.CSV.CsvReader");
            Console.WriteLine("Copyright (c) 2005 Sébastien Lorion");		
							
			AppSettings.Load(defaultsFile);
			Tournament.Load(AppSettings.I.TournamentFile);			
			Application.Init();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run();
			Tournament.I.Save(true);
			AppSettings.I.Save(defaultsFile);
		}
	}
}


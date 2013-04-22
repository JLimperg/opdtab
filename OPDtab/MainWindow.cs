using System;
using System.IO;
using Gtk;
using OPDtabGui;
using OPDtabData;

public partial class MainWindow : Gtk.Window
{
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		// this initializes static reference to ranking window
		// so we can display the current ranking at any time we want!
		new ShowRanking();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
		
	protected virtual void OnBtnDataGeneralClicked (object sender, System.EventArgs e)
	{
		new EditGeneral();
	}
	
	protected virtual void OnBtnDataDebatersClicked (object sender, System.EventArgs e)
	{
		new EditDebaters();
		
	}
	
	protected virtual void OnBtnGenerateRoundClicked (object sender, System.EventArgs e)
	{
		new GenerateRound();
	}
	
	protected virtual void OnBtnDataImportJudgesClicked (object sender, System.EventArgs e)
	{
		new ImportCSV();
	}
	
	protected virtual void OnBtnEditRoundResultsClicked (object sender, System.EventArgs e)
	{
		new EditRoundResults();
	}
	
	protected virtual void OnBtnDataLoadBackupClicked (object sender, System.EventArgs e)
	{
		new LoadBackup();
	}
	
	protected virtual void OnBtnShowRankingClicked (object sender, System.EventArgs e)
	{
		ShowRanking.I.ShowAll();
	}	

	
}


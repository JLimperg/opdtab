using System;
namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class AlgoProgress : Gtk.Bin
	{
		public AlgoProgress ()
		{
			this.Build ();
		}
		
		public void Update(double percentage, string infoMarkup) {
			label.Markup = infoMarkup;
			progressbar.Fraction = percentage;
		}
		
		protected virtual void OnBtnHideClicked (object sender, System.EventArgs e)
		{
			MiscHelpers.SetIsShown(this.Parent,false);
		}
		
		
	}
}


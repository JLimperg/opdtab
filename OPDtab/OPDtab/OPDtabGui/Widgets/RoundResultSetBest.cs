using System;
using Gtk;

namespace OPDtabGui
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class RoundResultSetBest : Gtk.Bin
	{
		public event EventHandler Toggled;
		RadioButton radiobutton;
		int index;
		
		public RoundResultSetBest (RoundResultSetBest member, int i)
		{
			this.Build ();
			index = i;
			radiobutton = new RadioButton(member == null ? null : member.radiobutton, null);
			radiobutton.FocusOnClick = false;
			hbox.Add(radiobutton);
			radiobutton.Toggled += delegate(object sender, EventArgs e) {
				UpdateGui();
				if(Toggled != null)
					Toggled(this, EventArgs.Empty);
			};
			UpdateGui();
		}
		
		void UpdateGui() {
			MiscHelpers.SetIsShown(image, radiobutton.Active);
			MiscHelpers.SetIsShown(radiobutton, !radiobutton.Active);
		}
		
		public bool Active {
			get {
				return radiobutton.Active;	
			}
			set {
				radiobutton.Active = value;
				// fire event explicitly
				if(Toggled != null)
					Toggled(this, EventArgs.Empty);
			}
		}
		
		public int Index {
			get {
				return index;
			}
		}
	}
}


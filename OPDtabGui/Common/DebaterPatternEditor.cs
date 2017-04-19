using System;
using Gtk;
using OPDtabData;
using System.Collections.Generic;

namespace OPDtabGui
{
	public partial class DebaterPatternEditor : Gtk.Dialog
	{
		// this class is a small workaround since (non-simple typed) references 
		// in delegates don't really work... 
		class RemoveButton : Button {
			public RemoveButton(DebaterPattern p, object i) : base() {
				Image im = new Image();
				im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-remove", IconSize.Menu);
				Add(im);
				Clicked += delegate(object sender, EventArgs e) {
					if(i is string)
						p.RemoveClub((string)i);
					else if(i is Name)
						p.RemoveDebater((Name)i);
				};
			}			
		}
		
		DebaterPattern pattern;
		Alignment alAdd;
		
		public DebaterPatternEditor(string p)
		{
			this.Build ();
			this.ActionArea.Hide();
			this.DestroyWithParent = true;
			this.Modal = true;
			this.Decorated = true; 
			this.Title = "Press Esc to close";

			alAdd = new Alignment(0,0,0,0);
			Button btnAdd = new Button();
			Image im = new Image();
			im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-add", IconSize.Menu);
			btnAdd.Add(im);
			btnAdd.Clicked += OnBtnAddClicked;
			alAdd.Add (btnAdd);

			pattern = new DebaterPattern();
			try {
				pattern = DebaterPattern.Parse(p);
			}
			catch { }
			UpdateGui();
		}
		
		void UpdateGui() {
			foreach(Widget w in vbox)
				vbox.Remove(w);
			foreach(string c in pattern.ClubNames) {
				HBox hbox = new HBox();
				hbox.Spacing = 3;
				Button btn = new RemoveButton(pattern, c);
				btn.Clicked += delegate(object sender, EventArgs e) {
					UpdateGui();
				};				
				hbox.PackStart(btn, false, false, 0);
				hbox.PackStart(new Label("@"), false, false, 0);
				hbox.PackStart(new Label(c), false, false, 0);
				vbox.PackStart(hbox, false, false, 0);
			}

			foreach(Name n in pattern.Debaters) {
				HBox hbox = new HBox();
				hbox.Spacing = 3;
				Button btn = new RemoveButton(pattern, n);
				btn.Clicked += delegate(object sender, EventArgs e) {
					UpdateGui();
				};
				hbox.PackStart(btn, false, false, 0);
				hbox.PackStart(new Label(n.ToString()), false, false, 0);
				vbox.PackStart(hbox, false, false, 0);	
			}
			vbox.PackStart(alAdd, false, false, 0);
			
			vbox.ShowAll();	
			GtkScrolledWindow.Vadjustment.Value = GtkScrolledWindow.Vadjustment.Upper;
		}

		protected void OnBtnAddClicked (object sender, System.EventArgs e)
		{
			// create combobox selection by aggregating clubs
			List<string> clubs = new List<string>();			
			List<string> debaters = new List<string>();
			foreach(Debater d in Tournament.I.Debaters) {
				if(!clubs.Contains("@"+d.Club.Name))
					clubs.Add("@"+d.Club.Name);
				debaters.Add(d.Name.ToString());
			}
			clubs.Sort();
			debaters.Sort();

			ListStore store = new ListStore(typeof(string)); 
			foreach(string c in clubs)
				store.AppendValues(c);
			foreach(string d in debaters)
				store.AppendValues(d);
			
			ComboBoxEntry cb = new ComboBoxEntry(store, 0);
			cb.Entry.Completion = new EntryCompletion();
			cb.Entry.Completion.Model = store;
			cb.Entry.Completion.TextColumn = 0;
			cb.Entry.Completion.InlineCompletion = true;
			HBox hbox = new HBox();
			// btnOk
			Button btnOk = new Button();
			Image im = new Image();
			im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-apply", IconSize.Menu);
			btnOk.Add(im);
			btnOk.Clicked += delegate {
				try {
					pattern = DebaterPattern.Parse(pattern + "; " + cb.ActiveText);
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, 
					                        "Could not add pattern: "+ex.Message, 
					                        MessageType.Error);
					return;
				}
				UpdateGui();
			};
			hbox.PackStart(btnOk, false, false, 0);
			// btnCancel
			Button btnCancel = new Button();
			im = new Image();
			im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-cancel", IconSize.Menu);
			btnCancel.Add(im);
			btnCancel.Clicked += delegate {
				UpdateGui();	
			};
			hbox.PackStart(btnCancel, false, false, 0);
			// cb as last element
			hbox.PackStart(cb, false, false, 0);
			hbox.ShowAll();
			vbox.PackStart(hbox, false, false, 0);
			alAdd.Hide();
			GtkScrolledWindow.Vadjustment.Value = GtkScrolledWindow.Vadjustment.Upper;
		}
		
		void AppendCombobox(List<string> items) {
			ListStore store = new ListStore(typeof(string)); 
			foreach(string i in items)
				store.AppendValues(i);
			
			ComboBoxEntry cb = new ComboBoxEntry(store, 0);
			HBox hbox = new HBox();
			// btnOk
			Button btnOk = new Button();
			Image im = new Image();
			im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-apply", IconSize.Menu);
			btnOk.Add(im);
			btnOk.Clicked += delegate {
				try {
					pattern = DebaterPattern.Parse(pattern + "; " + cb.ActiveText);
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, 
					                        "Could not add pattern: "+ex.Message, 
					                        MessageType.Error);
					return;
				}
				UpdateGui();
			};
			hbox.PackStart(btnOk, false, false, 0);
			// btnCancel
			Button btnCancel = new Button();
			im = new Image();
			im.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-cancel", IconSize.Menu);
			btnCancel.Add(im);
			btnCancel.Clicked += delegate {
				UpdateGui();	
			};
			hbox.PackStart(btnCancel, false, false, 0);
			// cb as last element
			hbox.PackStart(cb, false, false, 0);
			hbox.ShowAll();
			vbox.PackStart(hbox, false, false, 0);
		}
		
     	public DebaterPattern Pattern {
			get {
				return this.pattern;
			}
		}
	}
	
	
}


// This file has been generated by the GUI designer. Do not modify.
namespace OPDtabGui
{
	public partial class LoadBackup
	{
		private global::Gtk.VBox vbox1;

		private global::Gtk.HBox hbox1;

		private global::Gtk.ComboBox cbBackupFiles;

		private global::Gtk.Button btnLoad;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment GtkAlignment;

		private global::Gtk.Table table1;

		private global::Gtk.ScrolledWindow GtkScrolledWindow2;

		private global::Gtk.VBox vboxRounds;

		private global::Gtk.Label label1;

		private global::Gtk.Label labelDebaters;

		private global::Gtk.Label labelRounds;

		private global::Gtk.Label GtkLabel1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget OPDtabGui.LoadBackup
			this.Name = "OPDtabGui.LoadBackup";
			this.Title = global::Mono.Unix.Catalog.GetString ("LoadBackup");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("OPDtabGui.AppIcon.png");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child OPDtabGui.LoadBackup.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.cbBackupFiles = new global::Gtk.ComboBox ();
			this.cbBackupFiles.Name = "cbBackupFiles";
			this.hbox1.Add (this.cbBackupFiles);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.cbBackupFiles]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnLoad = new global::Gtk.Button ();
			this.btnLoad.CanFocus = true;
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.UseUnderline = true;
			this.btnLoad.Label = global::Mono.Unix.Catalog.GetString ("Load");
			this.hbox1.Add (this.btnLoad);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.btnLoad]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.vbox1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			global::Gtk.Viewport w4 = new global::Gtk.Viewport ();
			w4.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport2.Gtk.Container+ContainerChild
			this.vboxRounds = new global::Gtk.VBox ();
			this.vboxRounds.Name = "vboxRounds";
			this.vboxRounds.Spacing = 6;
			w4.Add (this.vboxRounds);
			this.GtkScrolledWindow2.Add (w4);
			this.table1.Add (this.GtkScrolledWindow2);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow2]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Yalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Debaters:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelDebaters = new global::Gtk.Label ();
			this.labelDebaters.Name = "labelDebaters";
			this.labelDebaters.Xalign = 0F;
			this.labelDebaters.LabelProp = global::Mono.Unix.Catalog.GetString ("<i>None selected</i>");
			this.labelDebaters.UseMarkup = true;
			this.table1.Add (this.labelDebaters);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelDebaters]));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelRounds = new global::Gtk.Label ();
			this.labelRounds.Name = "labelRounds";
			this.labelRounds.Yalign = 0F;
			this.labelRounds.LabelProp = global::Mono.Unix.Catalog.GetString ("Rounds:");
			this.table1.Add (this.labelRounds);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelRounds]));
			w10.TopAttach = ((uint)(1));
			w10.BottomAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment.Add (this.table1);
			this.frame1.Add (this.GtkAlignment);
			this.GtkLabel1 = new global::Gtk.Label ();
			this.GtkLabel1.Name = "GtkLabel1";
			this.GtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("Info");
			this.GtkLabel1.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel1;
			this.vbox1.Add (this.frame1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frame1]));
			w13.Position = 1;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
			this.cbBackupFiles.Changed += new global::System.EventHandler (this.OnCbBackupFilesChanged);
			this.btnLoad.Clicked += new global::System.EventHandler (this.OnBtnLoadClicked);
		}
	}
}

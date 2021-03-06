
// This file has been generated by the GUI designer. Do not modify.
namespace OPDtabGui
{
	public partial class EditRoundResults
	{
		private global::Gtk.VBox vbox1;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment GtkAlignment;

		private global::Gtk.ComboBox cbRounds;

		private global::Gtk.Label GtkLabel;

		private global::Gtk.Frame frame2;

		private global::Gtk.Alignment GtkAlignment1;

		private global::Gtk.ComboBox cbRooms;

		private global::Gtk.Label GtkLabel1;

		private global::Gtk.HBox hbox2;

		private global::Gtk.Frame frameChair;

		private global::Gtk.Alignment cChair;

		private global::Gtk.Label GtkLabel2;

		private global::Gtk.Frame frame4;

		private global::Gtk.Alignment GtkAlignment3;

		private global::Gtk.Label lblMotion;

		private global::Gtk.Label GtkLabel3;

		private global::Gtk.ScrolledWindow scrolledwindow2;

		private global::Gtk.EventBox ebResultSheet;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget OPDtabGui.EditRoundResults
			this.Name = "OPDtabGui.EditRoundResults";
			this.Title = "EditRoundResults";
			this.Icon = global::Gdk.Pixbuf.LoadFromResource("OPDtabGui.AppIcon.png");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			// Container child OPDtabGui.EditRoundResults.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.Name = "frame1";
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.cbRounds = new global::Gtk.ComboBox();
			this.cbRounds.Name = "cbRounds";
			this.GtkAlignment.Add(this.cbRounds);
			this.frame1.Add(this.GtkAlignment);
			this.GtkLabel = new global::Gtk.Label();
			this.GtkLabel.Name = "GtkLabel";
			this.GtkLabel.LabelProp = "<b>Round</b>";
			this.GtkLabel.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel;
			this.hbox1.Add(this.frame1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.frame1]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame();
			this.frame2.Name = "frame2";
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.cbRooms = new global::Gtk.ComboBox();
			this.cbRooms.Name = "cbRooms";
			this.GtkAlignment1.Add(this.cbRooms);
			this.frame2.Add(this.GtkAlignment1);
			this.GtkLabel1 = new global::Gtk.Label();
			this.GtkLabel1.Name = "GtkLabel1";
			this.GtkLabel1.LabelProp = "<b>Room</b>";
			this.GtkLabel1.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel1;
			this.hbox1.Add(this.frame2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.frame2]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			this.vbox1.Add(this.hbox1);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.frameChair = new global::Gtk.Frame();
			this.frameChair.Name = "frameChair";
			// Container child frameChair.Gtk.Container+ContainerChild
			this.cChair = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.cChair.Name = "cChair";
			this.cChair.LeftPadding = ((uint)(12));
			this.cChair.RightPadding = ((uint)(6));
			this.frameChair.Add(this.cChair);
			this.GtkLabel2 = new global::Gtk.Label();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = "<b>Chair</b>";
			this.GtkLabel2.UseMarkup = true;
			this.frameChair.LabelWidget = this.GtkLabel2;
			this.hbox2.Add(this.frameChair);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.frameChair]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.frame4 = new global::Gtk.Frame();
			this.frame4.Name = "frame4";
			// Container child frame4.Gtk.Container+ContainerChild
			this.GtkAlignment3 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment3.Name = "GtkAlignment3";
			this.GtkAlignment3.LeftPadding = ((uint)(12));
			this.GtkAlignment3.RightPadding = ((uint)(6));
			// Container child GtkAlignment3.Gtk.Container+ContainerChild
			this.lblMotion = new global::Gtk.Label();
			this.lblMotion.Name = "lblMotion";
			this.lblMotion.Xalign = 0F;
			this.lblMotion.Ellipsize = ((global::Pango.EllipsizeMode)(3));
			this.lblMotion.WidthChars = 50;
			this.lblMotion.SingleLineMode = true;
			this.GtkAlignment3.Add(this.lblMotion);
			this.frame4.Add(this.GtkAlignment3);
			this.GtkLabel3 = new global::Gtk.Label();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = "<b>Motion</b>";
			this.GtkLabel3.UseMarkup = true;
			this.frame4.LabelWidget = this.GtkLabel3;
			this.hbox2.Add(this.frame4);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.frame4]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			this.vbox1.Add(this.hbox2);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox2]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.scrolledwindow2 = new global::Gtk.ScrolledWindow();
			this.scrolledwindow2.CanFocus = true;
			this.scrolledwindow2.Name = "scrolledwindow2";
			// Container child scrolledwindow2.Gtk.Container+ContainerChild
			global::Gtk.Viewport w14 = new global::Gtk.Viewport();
			w14.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport.Gtk.Container+ContainerChild
			this.ebResultSheet = new global::Gtk.EventBox();
			this.ebResultSheet.Name = "ebResultSheet";
			w14.Add(this.ebResultSheet);
			this.scrolledwindow2.Add(w14);
			this.vbox1.Add(this.scrolledwindow2);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.scrolledwindow2]));
			w17.Position = 2;
			this.Add(this.vbox1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 899;
			this.DefaultHeight = 731;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.cbRounds.Changed += new global::System.EventHandler(this.OnCbRoundsChanged);
			this.cbRooms.Changed += new global::System.EventHandler(this.OnCbRoomsChanged);
		}
	}
}

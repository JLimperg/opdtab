
// This file has been generated by the GUI designer. Do not modify.
namespace OPDtabGui
{
	public partial class DebaterPatternEditor
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.VBox vbox;

		private global::Gtk.Button button1;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget OPDtabGui.DebaterPatternEditor
			this.Name = "OPDtabGui.DebaterPatternEditor";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child OPDtabGui.DebaterPatternEditor.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			global::Gtk.Viewport w2 = new global::Gtk.Viewport();
			w2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport.Gtk.Container+ContainerChild
			this.vbox = new global::Gtk.VBox();
			this.vbox.Name = "vbox";
			this.vbox.Spacing = 3;
			w2.Add(this.vbox);
			this.GtkScrolledWindow.Add(w2);
			w1.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(w1 [this.GtkScrolledWindow]));
			w5.Position = 0;
			// Internal child OPDtabGui.DebaterPatternEditor.ActionArea
			global::Gtk.HButtonBox w6 = this.ActionArea;
			w6.Name = "dialog1_ActionArea";
			w6.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.button1 = new global::Gtk.Button();
			this.button1.CanDefault = true;
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseStock = true;
			this.button1.UseUnderline = true;
			this.button1.Label = "gtk-ok";
			this.AddActionWidget(this.button1, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w6 [this.button1]));
			w7.Expand = false;
			w7.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll();
			}
			this.DefaultWidth = 283;
			this.DefaultHeight = 162;
			this.Show();
		}
	}
}

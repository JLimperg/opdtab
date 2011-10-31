using System;
using Gtk;
using Gdk;
namespace OPDtabGui
{
	public partial class MyButton : Gtk.Button
	{
		bool toggled;
		Pixbuf icon1;
		Pixbuf icon2;
		IDragDropWidget owner;
		
		public MyButton (IDragDropWidget o, string strLabel, string strIcon1, string strIcon2)
		{
			this.Build ();
			Relief = ReliefStyle.None;
			FocusOnClick = false;
			toggled = false;
			owner = o;
			label.Text = strLabel;
			icon1 = MiscHelpers.LoadIcon(strIcon1);
			icon2 = MiscHelpers.LoadIcon(strIcon2);
					
			image.Pixbuf = icon2;
			
			DragBegin += delegate(object s, DragBeginArgs args) {
				Gdk.Pixbuf pb = Gdk.Pixbuf.FromDrawable(label.GdkWindow,
				                                        label.Colormap,
			                              	     		label.Allocation.X,
			                                       		label.Allocation.Y,
			                                       		0,0,
				                                        label.Allocation.Width,
			                                       		label.Allocation.Height);
				Gtk.Drag.SourceSetIconPixbuf((Widget)s, pb);	
			}; 
		}
		
		public IDragDropWidget Owner {
			get {
				return owner;
			}
		}
		
		public string LabelText {
			set {
				label.Text = value;	
			}
		}
		
		public void Toggle() {
			Toggled = !Toggled;
		}
		
		public Color LabelColor {
			set {
				label.ModifyFg(StateType.Normal, value);	
				label.ModifyFg(StateType.Prelight, value);
			}
		}
		
		public void ResetLabelColor() {
			label.ModifyFg(StateType.Normal);	
			label.ModifyFg(StateType.Prelight);
		}
		
		public void SetStrikeThrough(bool flag) {
			label.Markup = (flag?"<s>":"")+label.Text+(flag?"</s>":"");	
		}
		
		public bool Toggled {
			get {
				return toggled;
			}
			set {
				toggled = value;	
				if(toggled) 
					image.Pixbuf = icon1;	
				else 
					image.Pixbuf = icon2;
			}
		}
		
		protected override void OnClicked ()
		{
			Toggle();
			base.OnClicked ();
		}
		
		
	}
}


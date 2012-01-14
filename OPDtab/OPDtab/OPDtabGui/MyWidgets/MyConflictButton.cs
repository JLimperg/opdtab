using System;
using Gtk;
using OPDtabData;
using System.Collections.Generic;
using Gdk;
using System.Text.RegularExpressions;

namespace OPDtabGui
{
	public class MyConflictButton : Button
	{
		AppSettings.GenerateRoundClass settings;
		Menu m;
		
		public MyConflictButton () 
		{
			settings = AppSettings.I.GenerateRound;
			BorderWidth = 0;
			Relief = ReliefStyle.None;
			NoShowAll = true;
			FocusOnClick = false;
			ButtonPressEvent += OnBtnPressed;
		
		}
			                                               
		public void SetRoomConflict(RoomConflict rc) {
			if(rc==null || rc.IsEmpty) {
				HideAll();
				NoShowAll = true;	
				return;
			}
				
			HBox hbox = new HBox();
			hbox.Spacing = 1;
			m = new Menu();
			
			// this round/room
			foreach(RoomConflict.Type t in rc.Partners1.Keys) {
				if(rc.Partners1[t].Count==0)
					continue;
				int iconIndex = settings.conflictIcons[(int)t];
				if(iconIndex>=0) 
					hbox.Add(new Gtk.Image(MiscHelpers.LoadIcon(settings.possibleIcons[iconIndex])));
				MenuItem miType = new MenuItem(t.ToString());
				miType.Submenu = new Menu();
				foreach(RoundDebater d in rc.Partners1[t])
					(miType.Submenu as Menu).Add(new MenuItem(RoundDebaterToString(d)));	
				m.Add(miType);
			}
			
			// other Rounds...
			MenuItem miOther = new MenuItem();
			Label lbl = new Label();
			lbl.Markup = "<i>Other</i>";
			lbl.Xalign = 0;
			miOther.Add(lbl);
			Menu mOther = new Menu();
			miOther.Submenu = mOther;
			int validOthers = 0;
			
			// in other Conflicts, it should be okay to iterate using Tournament.I.Rounds
			foreach(RoundData rd in Tournament.I.Rounds) {
				RoomConflict.Complex cmplx;
				if(rc.Partners2.TryGetValue(rd.RoundName, out cmplx)) {
					Dictionary<string, List<RoundDebater>> store = cmplx.Store;
					List<string> keys = new List<string>(store.Keys);
					// sorting is a bit nasty, so that "Room 2" is before "Room 11"	
					// this is also inefficient, but performance doesn't matter here
					keys.Sort(delegate(string x, string y) {
						// try to extract numbers from string
						List<string> num_x = new List<string>(Regex.Split(x, @"\D+"));
						List<string> num_y = new List<string>(Regex.Split(y, @"\D+"));
						num_x.RemoveAll(item => string.IsNullOrWhiteSpace(item));
						num_y.RemoveAll(item => string.IsNullOrWhiteSpace(item));
						if(num_x.Count != 1 || num_y.Count != 1)
							return x.CompareTo(y);
						return int.Parse(num_x[0]).CompareTo(int.Parse(num_y[0]));
					});
					foreach(string room in keys) {
						MenuItem miRound = new MenuItem(rd.RoundName+", "+room);
						miRound.Submenu = new Menu();
						foreach(RoundDebater d in store[room])
							(miRound.Submenu as Menu).Add(new MenuItem(RoundDebaterToString(d)));		
						mOther.Add(miRound);
						validOthers++;
					}
				}
			}
			
			if(validOthers>0) {
				int numEnums = Enum.GetNames(typeof(RoomConflict.Type)).Length;
				int iconIndex = settings.conflictIcons[numEnums];
				if(iconIndex>=0) 
					hbox.Add(new Gtk.Image(MiscHelpers.LoadIcon(settings.possibleIcons[iconIndex])));
				m.Add(miOther);
			}
			
			m.ShowAll();			
			m.AttachToWidget(this, null);			
			
			// always show a (not attracting) icon...even if all are disabled..
			if(hbox.Children.Length==0) {
				// only loadable over pixbuf
				Pixbuf dummy = MiscHelpers.LoadIcon("weather-clear-night");
				//Stetic.IconLoader.LoadIcon(this,"stock_weather-night-clear",IconSize.Menu);
				hbox.Add(new Gtk.Image(dummy));
			}
			if(Children.Length>0)
				Remove(Child);
			Add(hbox);			
			NoShowAll = false;
			ShowAll();			
		}
		
		string RoundDebaterToString(RoundDebater rd) {
			return rd.Name+" ("+rd.Club+")";
		}
		
		[GLib.ConnectBefore]
		void OnBtnPressed(object s, ButtonPressEventArgs a) {
			if(a.Event.Button==1 && a.Event.Type == EventType.ButtonPress) {
				DoPopupMenu((Widget)s, a.Event);	
			}
		}
		
		void DoPopupMenu(Widget w, Gdk.EventButton evnt) {
			m.Popup(null, null, delegate(Menu me, out int x, out int y, out bool push_in) {
				push_in = true;
				Widget wi = me.AttachWidget;
				int x1, y1, x3, y3;
				wi.GetPointer(out x1, out y1);
				wi.Display.GetPointer(out x3, out y3);
				x=x3-x1;
				y=y3-y1+wi.Requisition.Height; // bottom of widget
			}, evnt.Button, evnt.Time);		
		}
	}
}


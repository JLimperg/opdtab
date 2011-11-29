using System;
using Gtk;
using OPDtabData;
using System.Collections.Generic;
using Gdk;


namespace OPDtabGui
{
	public class MyJudgeStateButton : Button
	{
		string[] stateToSymbol = new string[] {"J", "X", "C", "1"};
		string[] stateToDesc = new string[] 
			{"Use as Judge", "Not available", "Use as Chair", "Use as first Judge"};
		
		RoundDebater rd;
		Menu m;
		Label lbl;
		
		public event EventHandler Changed;
		
		public MyJudgeStateButton(RoundDebater rd_) {
			rd = rd_;
			BorderWidth = 0;
			Relief = ReliefStyle.None;
			CanFocus = false;
			lbl = new Label();
			Add(lbl);
			UpdateGui();
			SetupMenu();
			ButtonPressEvent += OnBtnPressed;
			Changed += delegate {
				Tournament.I.Save();
			};
		}
		
		void UpdateGui()  {
			lbl.Markup = "<b>"+stateToSymbol[(int)rd.JudgeState]+"</b>";
		}
		
		void SetupMenu() {
			m = new Menu();
			for(int i=0;i<stateToSymbol.Length;i++) {
				MenuItem mi = new MenuItem(stateToSymbol[i]+" "+
					stateToDesc[i]);
				mi.ButtonPressEvent += delegate(object o, ButtonPressEventArgs args) {
					// find index of mi, clumsy method...?!
					// using i in delegate does not work...
					int j=0;
					foreach(Widget w in m) {
						if(w.Equals(o)) 
							break;
						j++;
					}
					rd.JudgeState = (RoundDebater.JudgeStateType)j;
					UpdateGui();
					if(Changed != null)
						Changed(this, EventArgs.Empty);	
				};
				m.Add(mi);
			}
			
			m.ShowAll();			
			m.AttachToWidget(this, null);
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


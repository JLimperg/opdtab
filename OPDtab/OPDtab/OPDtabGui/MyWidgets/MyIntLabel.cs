using System;
using System.Collections.Generic;
using Gtk;

namespace OPDtabGui
{
	public interface IIntWidget {
		int GetInt();
		void ForceUpdateInt();
		event EventHandler IntChanged;
	}
	
	public class MyIntLabel : Label, IIntWidget
	{
		Dictionary<IIntWidget, int> parentWidgets;
		bool average;
		int val;
		
		public event EventHandler IntChanged;
		
		public MyIntLabel(bool avg) : base()
		{
			parentWidgets = new Dictionary<IIntWidget, int>();
			WidthRequest = 50;
			average = avg;
			UpdateInt();
		}
		
		public void AddParent(IIntWidget w) {
			parentWidgets.Add(w, w.GetInt());	
			UpdateInt();
			
			w.IntChanged += delegate(object sender, EventArgs e) {
				parentWidgets[(IIntWidget)sender] = ((IIntWidget)sender).GetInt();
				UpdateInt();
				if(IntChanged != null) 
					// maybe oneself is a parent, so tell the child
					IntChanged(this, EventArgs.Empty);			
			};
		}
		
		public void ClearParents() {
			parentWidgets.Clear();
			UpdateInt();
		}
		
		public void ForceUpdateInt() {
			// use temporary list, otherwise out of sync in Dictionary
			Dictionary<IIntWidget, int> dic = new Dictionary<IIntWidget, int>();
			foreach(IIntWidget w in parentWidgets.Keys) {
				w.ForceUpdateInt();
				dic.Add(w, w.GetInt());
			}
			parentWidgets = dic;			
			UpdateInt();
		}
		
		void UpdateInt() {
			if(average)
				val = OPDtabData.MiscHelpers.CalcAverage(parentWidgets.Values);
			else
				val = OPDtabData.MiscHelpers.CalcSum(parentWidgets.Values);
			
			if(val<0) {
				SetInvalid();
				return;
			}
			else if(val<20)
				ModifyFg(StateType.Normal, new Gdk.Color(255, 0, 0));
			else
				ModifyFg(StateType.Normal);
			
			Markup = "<big>"+val+"</big>";
		}
		
		void SetInvalid() {
			Markup = "<b>?</b>";
			ModifyFg(StateType.Normal);
			val = -1;
		}
						   
		public int GetInt() {
			return val;	
		}		
	}
}


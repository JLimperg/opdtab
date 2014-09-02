using System;
using System.Collections.Generic;
using Gtk;

namespace OPDtabGui
{
	public interface INumberWidget {
		double GetValue();
		void ForceUpdate();
		event EventHandler NumberChanged;
	}
	
	public class MyNumberLabel : Label, INumberWidget
	{
		Dictionary<INumberWidget, double> parentWidgets;
		bool average;
		double val;
		
		public event EventHandler NumberChanged;
		
		public MyNumberLabel(bool avg) : base()
		{
			parentWidgets = new Dictionary<INumberWidget, double>();
			WidthRequest = 70;
			average = avg;
			Update();
		}
		
		public void AddParent(INumberWidget w) {
			parentWidgets.Add(w, w.GetValue());	
			Update();
			
			w.NumberChanged += delegate(object sender, EventArgs e) {
				parentWidgets[(INumberWidget)sender] = ((INumberWidget)sender).GetValue();
				Update();
				if(NumberChanged != null) 
					// maybe oneself is a parent, so tell the child
					NumberChanged(this, EventArgs.Empty);			
			};
		}
		
		public void ClearParents() {
			parentWidgets.Clear();
			Update();
		}
		
		public void ForceUpdate() {
			// use temporary list, otherwise out of sync in Dictionary
			Dictionary<INumberWidget, double> dic = new Dictionary<INumberWidget, double>();
			foreach(INumberWidget w in parentWidgets.Keys) {
				w.ForceUpdate();
				dic.Add(w, w.GetValue());
			}
			parentWidgets = dic;			
			Update();
		}
		
		void Update() {
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
			
			Markup = "<big>" + OPDtabData.MiscHelpers.FmtDecimal(val) + "</big>";
		}
		
		void SetInvalid() {
			Markup = "<b>?</b>";
			ModifyFg(StateType.Normal);
			val = -1;
		}
						   
		public double GetValue() {
			return val;	
		}		
	}
}


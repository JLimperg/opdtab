using System;
using Gtk;
using OPDtabData;
namespace OPDtabGui
{
	public class MySpinButton : Gtk.SpinButton, IIntWidget
	{
		public event EventHandler IntChanged;
		
		int judgeIndex;
		int debaterIndex;
		
		public MySpinButton(double min, double max, double step, 
		                    int ji, int di) : base(min, max, step)
		{	
			judgeIndex = ji;
			debaterIndex = di;
			
			ValueChanged += delegate(object sender, EventArgs e) {
				Tournament.I.Save();
				if(IntChanged != null)
					IntChanged(this, EventArgs.Empty);
			};
			Alignment = 0.5f;
		}
		
		protected override int OnOutput ()
		{
			int ret = base.OnOutput ();
			if(Value<0) {
				Text = "?";
				return 1;
			}
			return ret;
		}
		
		protected override int OnInput (out double new_value)
		{
			int ret = base.OnInput(out new_value);
			if(Text=="?") {
				new_value = -1;
				return 1;
			}
			return ret;
		}
		
		
		public int GetInt() {
			return ValueAsInt;	
		}
		
		public void ForceUpdateInt() {
			// not listening to any "parents"
		}
	

		public int DebaterIndex {
			get {
				return this.debaterIndex;
			}
		}

		public int JudgeIndex {
			get {
				return this.judgeIndex;
			}
		}
}
}


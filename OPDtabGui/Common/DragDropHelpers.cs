using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gtk;
using OPDtabData;


namespace OPDtabGui
{
	
    public static class DragDropHelpers
	{
		static TargetEntry tgtTeam = new TargetEntry("TEAM",TargetFlags.App,0);
		static TargetEntry tgtFreeSpeaker = new TargetEntry("FREESPEAKER",TargetFlags.App,1);
		static TargetEntry tgtJudge = new TargetEntry("JUDGE",TargetFlags.App,2);
		static TargetEntry tgtOnlyOpp = new TargetEntry("OPP",TargetFlags.App,3);
		static TargetEntry tgtOnlyGov = new TargetEntry("GOV",TargetFlags.App,4);
		
		
		public static TargetEntry[] TgtTeam {
			get {
				return new TargetEntry[] {tgtTeam};
			}
		}
		
		public static TargetEntry[] TgtFreeSpeaker {
			get {
				return new TargetEntry[] {tgtFreeSpeaker};	
			}
		}
		
		public static TargetEntry[] TgtJudge {
			get {
				return new TargetEntry[] {tgtJudge};	
			}
		}
		
		public static TargetEntry[] TgtAll {
			get {
				return new TargetEntry[] {tgtTeam, tgtFreeSpeaker, tgtJudge};	
			}
		}
		
		public static TargetEntry[] TgtOnlyOpp {
			get {
				return new TargetEntry[] {tgtOnlyOpp};	
			}
		}
		
		public static TargetEntry[] TgtOnlyGov {
			get {
				return new TargetEntry[] {tgtOnlyGov};	
			}
		}
		
		public static TargetEntry[] TgtFromString(string s) {
			switch(s) {
			case "Gov":
			case "Opp":
			case "Team":
				return TgtTeam;
			case "FreeSpeakers":
			case "TeamMember":
				return TgtFreeSpeaker;
			case "Judges":
			case "Judge":
			case "Chair":
				return TgtJudge;
			case "OnlyGov":
				return TgtOnlyGov;
			case "OnlyOpp":
				return TgtOnlyOpp;
			default:
				throw new NotImplementedException("Don't which Target to select for "+s);			
			}
		}
		
		
		
		public static void Serialize(SelectionData data, object d) {
			IFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, d);
			stream.Close();			
			data.Set(data.Target,8,stream.ToArray());
		}
		
		public static object Deserialize(SelectionData data) {			
			IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data.Data);
			object d = formatter.Deserialize(stream);
			stream.Close();
			return d;
		}
		// FIRST  : SourceSet
		// SECOND : Add IDragDropWidget to container
		// THIRD  : DestSet and DragDataReceived
		public static void SetupDragDropSourceButton(MyButton btn,
		                             		   string t,
		                             		   object data) {
			if(btn==null)
				return;
			Drag.SourceSet(btn,
			               Gdk.ModifierType.Button1Mask,
			               TgtFromString(t),
			               Gdk.DragAction.Move);
			btn.DragDataGet += delegate(object o, DragDataGetArgs args_) {
				// save after drag & drop
				Tournament.I.Save();
				DragDropHelpers.Serialize(args_.SelectionData, data);
			};					                      
					
		}
			
		public static void DestSet(Widget w, DestDefaults d, TargetEntry[] t, Gdk.DragAction a) {
			Drag.DestSet(w, d, t, a);
			HandleDragScrolling(w);
		}
		
		public delegate void ApplyToWidgetHandler(Widget w);
		
		public static void ApplyToWidget(Widget parent, ApplyToWidgetHandler h ) {
			if(parent is Container) {
				h(parent);
				foreach(Widget child in ((Container)parent).AllChildren)
					ApplyToWidget(child,h);
			}
			else
				h(parent);				
		}	
				
		static void HandleDragScrolling(Widget w) {
			ScrolledWindow sw = (ScrolledWindow)w.GetAncestor(ScrolledWindow.GType); 
						
			if(sw != null) {
				Drag.DestSetTrackMotion(w,true);
				
				DragMotionHandler dm = delegate(object o, DragMotionArgs args) {
					int x, y;
					Gdk.ModifierType mt;
					sw.GdkWindow.GetPointer(out x,out y,out mt);
					args.Args[1] = x;
					args.Args[2] = y;				 
					OnSwDragMotion(sw, args);
				};
				w.DragMotion -= dm;
				w.DragMotion += dm;				
				Drag.DestSet(sw, DestDefaults.Motion, new TargetEntry[] {}, Gdk.DragAction.Move);
				Drag.DestSetTrackMotion(sw, true);
				sw.DragMotion -= OnSwDragMotion;					
				sw.DragMotion += OnSwDragMotion;					
			}
			else 
				Console.WriteLine("WARNING: No ScrolledWindow Container found: "+w.ToString());
		}
		
	
		static void OnSwDragMotion (object o, Gtk.DragMotionArgs args)
		{
			ScrolledWindow sw = (ScrolledWindow)o;
			
			int w = sw.Allocation.Width;
			int h = sw.Allocation.Height; 
						
			ScrollAdjustment(sw.Vadjustment,args.Y,h);
			ScrollAdjustment(sw.Hadjustment,args.X,w);
		}
		
		static void ScrollAdjustment(Adjustment adj, int pos, int max) {
			int delta = 30;
			if(pos<=delta) {
				double speed = 1.0-(double)pos/delta;
				int step = (int)(adj.StepIncrement*speed*speed);
				if(adj.Value>adj.Lower+step) 
					adj.Value -= step;
				else
					adj.Value = adj.Lower;
			}
			else if(max-pos <= delta) {
				double speed = 1.0-(double)(max-pos)/delta;
				int step = (int)(adj.StepIncrement*speed*speed);
				if(adj.Value<adj.Upper-adj.PageSize-step) 
					adj.Value += step;
				else
					adj.Value = adj.Upper-adj.PageSize;					
			}
			adj.ChangeValue();
		}
	}
}
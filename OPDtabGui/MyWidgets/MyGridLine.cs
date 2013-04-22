using System;
using Gtk;
namespace OPDtabGui
{
	public class MyGridLine : Gtk.DrawingArea
	{
		public static MyGridLine H(int h, Gdk.Color col)
		{
			MyGridLine g = new MyGridLine();
			g.ModifyBg(StateType.Normal, col);
			g.HeightRequest = h;
			return g;
		}
		
		public static MyGridLine V(int w, Gdk.Color col)
		{		
			MyGridLine g = new MyGridLine();
			g.ModifyBg(StateType.Normal, col);
			g.WidthRequest = w;
			return g;
		}
		
		public static MyGridLine H(int h) {
			MyGridLine g = new MyGridLine();
			g.HeightRequest = h;
			return g;	
		}
		
		public static void H(Table t, uint pos, uint maxcol, int h, Gdk.Color col) {
			t.Attach(H(h,col),0,maxcol,pos,pos+1,AttachOptions.Fill,AttachOptions.Fill,0,0);	
		}
		
		public static void V(Table t, uint pos, int w, Gdk.Color col) {
			t.Attach(V(w,col),pos,pos+1,0,t.NRows,AttachOptions.Fill,AttachOptions.Fill,0,0);	
		}
	}	
	
}


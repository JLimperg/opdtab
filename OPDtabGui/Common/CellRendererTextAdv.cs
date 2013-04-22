using System;
using System.Reflection;
using Gtk;
using System.Collections.Generic;
using OPDtabData;
namespace OPDtabGui
{
		
	public class CellRendererTextAdv : CellRendererText
	{
		public enum Type {Entry, EntryWithCompletion, DebaterPattern};
		
		public delegate void MyEditedHandler(CellRendererTextAdv sender, 
		                                     string path, string newText);		
		public event MyEditedHandler MyEdited;

		string tempEditString;
		int colNum;
		ListStore store;
		Type type;
		
		public CellRendererTextAdv(Type t, ListStore s, int c) : base() {			
			type = t;
			store = s;
			colNum = c;
			Editable = true;	
			if(type == Type.DebaterPattern) {
				this.Ellipsize = Pango.EllipsizeMode.End;
				this.Width = 250;
			}
			Edited += delegate(object o, EditedArgs args) {
				if(MyEdited!=null)
					MyEdited((CellRendererTextAdv)o, args.Path, args.NewText);
			};
		}
	
		public override CellEditable StartEditing (Gdk.Event evnt, Widget widget, string path, 
		                                           Gdk.Rectangle background_area, 
		                                           Gdk.Rectangle cell_area, CellRendererState flags)
		{
			TreeView tree = widget as TreeView;
					
			if(type == Type.DebaterPattern) {				
				DebaterPatternEditor editor = new DebaterPatternEditor(this.Text);
				int tree_x, tree_y, tree_w, tree_h, editor_w, editor_h;
				tree.BinWindow.GetOrigin(out tree_x, out tree_y);
				tree.BinWindow.GetSize(out tree_w, out tree_h);
				editor.GetSize(out editor_w, out editor_h);
		
				int x = tree_x + Math.Min(cell_area.X, tree_w - editor_w + tree.VisibleRect.X);
				int y = tree_y + Math.Min(cell_area.Y, tree_h - editor_h + tree.VisibleRect.Y);
				editor.Move(x,y);
				editor.Run();
				editor.GetSize(out editor_w, out editor_h);
				
				editor.Destroy();
				if(MyEdited!=null) 
					MyEdited(this, path, editor.Pattern.ToString());				
				return null;				
			}
			
			Entry entry = (Entry)base.StartEditing(evnt, widget, path, 
		                                       background_area, cell_area, flags);
			
			if(tempEditString!=null) {
				entry.Text = tempEditString;
				tempEditString = null;
			}
			if(type == Type.EntryWithCompletion) {
				entry.Completion = new EntryCompletion();		
				entry.Completion.Model = GetCompletionModel(tree);
				entry.Completion.TextColumn = 0;
				entry.Completion.InlineCompletion = true;		
			}
			return entry;
		}
		
		ListStore GetCompletionModel(TreeView t) {
			string prop = t.Columns[colNum].Title; 
			List<string> list = new List<string>();
			store.Foreach(
				delegate(TreeModel model, TreePath path, TreeIter iter) {
					object o = model.GetValue(iter, 0);
					object p = o.GetType().InvokeMember(prop+"Completion",
				                                    BindingFlags.GetProperty, null, o, null);
					
					if(p != null) 
						if(!list.Contains(p.ToString()))
							list.Add(p.ToString());
					return false;
				});
			ListStore s = new ListStore(typeof(string));
			foreach(string str in list) 
				s.AppendValues(str);	
			return s;	
		}
		
		public string TempEditString {
			get {
				return this.tempEditString;
			}
			set {
				tempEditString = value;
			}
		}		
		
	}
}


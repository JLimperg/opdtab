using System;
using System.Reflection;
using Gtk;
using System.Collections.Generic;
using OPDtabData;
namespace OPDtabGui
{
		
	public class CellRendererTextAdv : CellRendererText
	{
		public delegate ListStore CompletionModelHandler();		
		string tempEditString;
		CompletionModelHandler completionModel;
		
		
		public CellRendererTextAdv() : base() {			
			Editable = true;
		}
	
		public override CellEditable StartEditing (Gdk.Event evnt, Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
		{
			Entry entry = (Entry)base.StartEditing (evnt, widget, path, background_area, cell_area, flags);
			if(tempEditString!=null) {
				entry.Text = tempEditString;
				tempEditString = null;
			}
			if(CompletionModel!=null) {
				entry.Completion = new EntryCompletion();		
				entry.Completion.Model = CompletionModel();
				entry.Completion.TextColumn = 0;
				entry.Completion.InlineCompletion = true;
				
			}
			return entry;
		}
		
		
		public CompletionModelHandler CompletionModel {
			get {
				return this.completionModel;
			}
			set {
				completionModel = value;
			}
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


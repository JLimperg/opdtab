using System;
using System.IO;
using Gtk;
using OPDtabData;
namespace OPDtabGui
{
	public static class MiscHelpers
	{
		public static void ShowMessage(Window w, string msg, MessageType t) {
			MessageDialog md = new MessageDialog(w, DialogFlags.Modal,
					                             t,
				                                 ButtonsType.Ok,
			                                  	 msg);
				
			md.Run();
			md.Destroy();	
		}
		
		public static ResponseType AskYesNo(Window w, string msg) {
			MessageDialog md = new MessageDialog(w, DialogFlags.Modal,
					                             MessageType.Question,
				                                 ButtonsType.YesNo,
			                                  	 msg);
				
			ResponseType r = (ResponseType)md.Run();
			md.Destroy();	
			return r;
		}
		
		public static void ClearContainer(Container c) {
			foreach(Widget w in c.AllChildren)
				c.Remove(w);
		}
		
		public static void AddToContainer(Container c, Widget w) {			
			if(c is Bin) {
				if(c.Children.Length>0)
					c.Remove(c.Children[0]);
				c.Child = w;
			}
			else if(c is Box)
				((Box)c).PackStart(w, false, false, 0);
			else
				c.Add(w);
			c.ShowAll();
		}
		
		public static void SetIsShown(Widget w, bool flag) {
			if(flag) {
				w.NoShowAll = false;
				w.ShowAll();	
			}
			else {
				w.HideAll();
				w.NoShowAll = true;
			}
		}
		
		public static void EntryFilterFocusIn(object o, FocusInEventArgs a) {
			Entry e = (Entry)o;
			if(e.Text=="")
				e.Layout.SetText("");
		}
		
		public static void EntryFilterFocusOut(object o, FocusOutEventArgs a) {
			Entry e = (Entry)o;
			// show some hint
			if(e.Text=="") {
				e.Layout.SetMarkup("  <i><span foreground='#AAAAAA'>Type here to filter</span></i>");
			}
		}
		
		public static Entry MakeFilterEntry() {
			Entry e = new Entry();
			e.FocusInEvent += EntryFilterFocusIn;
			e.FocusOutEvent += EntryFilterFocusOut;
			
			e.ExposeEvent += delegate(object sender, ExposeEventArgs a) {
				if(!(sender as Widget).HasFocus)
					EntryFilterFocusOut(sender, null);	
			};
			return e;
		}
		
		public static void ClearTable(Table table) {
			// remove everything except headers
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				if(c.TopAttach>0)
					table.Remove(w);
			}
		}
		
		public static void SwapTableCols(Table table, uint col1, uint col2) {
			if(col1==col2)
				return; 
			table.NColumns++;
			MoveTableCol(table, col1, table.NColumns-1);
			MoveTableCol(table, col2, col1);
			MoveTableCol(table, table.NColumns-1, col2);
			table.NColumns--;
		}
		
		public static void SwapTableRows(Table table, uint row1, uint row2) {
			// like the tower of hanoi,
			// we need some temporary space
			if(row1==row2)
				return;
			table.NRows++;
			MoveTableRow(table, row1, table.NRows-1);
			MoveTableRow(table, row2, row1);
			MoveTableRow(table, table.NRows-1, row2);
			table.NRows--;
		}
		
		public static void MoveTableCol(Table table, uint source, uint dest) {
			if(source==dest)
				return;
			// assumes that only 1x1 cells are affected
			// and that dest is empty!
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				if(c.LeftAttach == source && c.RightAttach == source+1) {
					if(source<dest) {
						// first right...
						c.RightAttach = dest+1;
						// ...then left
						c.LeftAttach = dest;
					}
					else {
						// first left...
						c.LeftAttach = dest;
						// .. then right
						c.RightAttach = dest+1;
					}
				}
			}
		}
		
		public static void MoveTableRow(Table table, uint source, uint dest) {
			if(source==dest)
				return;
			// assumes that only 1x1 cells are affected
			// and that dest is empty!
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				if(c.TopAttach == source && c.BottomAttach == source+1) {
					if(source<dest) {
						// first bottom...
						c.BottomAttach = dest+1;
						// ...then top
						c.TopAttach = dest;
					}
					else {
						// first top...
						c.TopAttach = dest;
						// .. then bottom
						c.BottomAttach = dest+1;
					}
				}
			}
		}
		
		public static void InsertTableColumns(Table table, uint start, uint n) {
			// make some space for judges
			table.NColumns += n;
			// reattach widgets
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				// first right
				if(c.RightAttach>start)
					c.RightAttach += n;
				// then left!
				if(c.LeftAttach>=start) 
					c.LeftAttach += n;
			}
		}
		
		public static void RemoveTableColumns(Table table, uint start, uint n) {
			// delete affected widgets
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				if(c.LeftAttach>= start && c.RightAttach <= start+n)
					table.Remove(w);
			}			
			// reattach widgets 
			foreach(Widget w in table) {
				Table.TableChild c = table[w] as Table.TableChild;	
				// first left
				if(c.LeftAttach>=start+n) 
					c.LeftAttach -= n;
				// then right!
				if(c.RightAttach>start+n)
					c.RightAttach -= n;
			}
			table.NColumns -= n;
		}
		
		static string templateName = null;
		static ITemplate currTmpl = null;
		
		public static ITemplate GetTemplate(string name) {
			templateName = name;
			currTmpl = TemplatePool.Singleton().GetTemplate(templateName+"-tmpl.tex");
			return currTmpl;
		}
				
		public static void MakePDFfromTemplate(string prefix) {
			MakePDFfromTemplate(prefix, true);	
		}
		
		public static void MakePDFfromTemplate(string prefix, bool runtwice) {
			if(templateName == null || currTmpl == null)
				return;
			
			StreamWriter outfile = null;
			string oldDir = Directory.GetCurrentDirectory();
			try {
				ITmplBlock tmplDoc = currTmpl.ParseBlock();
				tmplDoc.Out();
				string fileName = prefix == null ? templateName : 
					prefix.Replace(" ","_")+"-"+templateName;
				string workingDir = Path.Combine(oldDir, "pdfs");
				Directory.SetCurrentDirectory(workingDir);
				string latexFile = Path.Combine(workingDir, fileName+".tex");
				outfile = new StreamWriter(latexFile);
				outfile.Write(tmplDoc.BlockString);	
				outfile.Close();
				// run latex
				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.EnableRaisingEvents=false; 
				proc.StartInfo.FileName = "pdflatex";
				proc.StartInfo.Arguments = "-interaction=nonstopmode \""+latexFile+"\"";
				// two times
				proc.Start();
				proc.WaitForExit();
				if(runtwice) {
					proc.Start();
					proc.WaitForExit();
				}
				// cleanup
				foreach(string ext in new string[] {"aux", "log", "out", "nav", "toc", "snm" }) {
					string fN = Path.Combine(workingDir, fileName+"."+ext); 
					if(File.Exists(fN))
						File.Delete(fN);
				}
			}
			finally {
				if(outfile != null)
					outfile.Close();
				Directory.SetCurrentDirectory(oldDir);
				templateName=null;
				currTmpl=null;
			}
				
		}
		
		public static Gdk.Pixbuf LoadIcon(string resource) {
			return Gdk.Pixbuf.LoadFromResource("OPDtabGui.Icons."+resource+".png");
		}
		
		
		public static EventBox MakeBackground(Widget w, Gdk.Color col) {
			EventBox eb = new EventBox();
			if(w != null)
				eb.Add(w);
			eb.ModifyBg(StateType.Normal, col);
			return eb;
		}
		
		public static EventBox MakeBackground(Widget w) {
			EventBox eb = new EventBox();
			if(w != null)
				eb.Add(w);
			return eb;
		}
	}
}


using System;
using System.Reflection;
using System.Collections.Generic;
using Gtk;
using OPDtabData;

namespace OPDtabGui
{
	public partial class EditDebaters : Gtk.Window
	{
		
		OPDtabData.AppSettings.EditDebatersClass editDebatersSettings;
		TreePath newDebaterPath;
		ListStore store;
		Entry entryDebatersFilter;
		
		public EditDebaters() : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			editDebatersSettings = AppSettings.I.EditDebaters;
			entryDebatersFilter = MiscHelpers.MakeFilterEntry();
			entryDebatersFilter.Changed += OnEntryDebatersFilterChanged;
			cEntryFilter.Add(entryDebatersFilter);
			cEntryFilter.ShowAll();
			InitTreeDebaters();
		}
		
		void InitTreeDebaters() {  
			// Init Store
			store = new ListStore(typeof(EditableDebater)); 
			foreach(Debater d in Tournament.I.Debaters) 
				store.AppendValues(new EditableDebater(d));
			
			
			// Setup Treeview
			TreeModelFilter modelDebatersFilter = new TreeModelFilter(store,null);
			modelDebatersFilter.VisibleFunc = FilterDebaters;
			
			TreeModelSort modelDebatersSort = new TreeModelSort(modelDebatersFilter);
						
			treeDebaters.Model = modelDebatersSort;
			
			SetupDebaterColumn("Name", false);
			SetupDebaterColumn("Club", true);
			SetupDebaterColumn("Age", false);
			SetupDebaterColumn("Role", true);
			SetupDebaterColumn("BlackList", false);
			SetupDebaterColumn("WhiteList", false);
			SetupDebaterColumn("ExtraInfo", false);
			treeDebaters.HeadersClickable = true;
			
			treeDebaters.Columns[editDebatersSettings.sortCol].SortIndicator = true;
			treeDebaters.Columns[editDebatersSettings.sortCol].SortOrder = editDebatersSettings.sortOrder;
			modelDebatersSort.SetSortColumnId(editDebatersSettings.sortCol, 
			                                  editDebatersSettings.sortOrder);
			
			UpdateDebatersInfo();
		}
		
		void UpdateDebatersInfo() {
			int num = treeDebaters.Model.IterNChildren();
			lblDebatersInfo.Text = num+" Row"+(num==1?"":"s");	
		}
		
		bool IsNotInStore(EditableDebater d) {
			bool notEqual = true; 
			foreach(object[] row in store)
				notEqual = notEqual && !d.Equals(row[0]);
			return notEqual;			
		}
		
		
		
		void SetupDebaterColumn(string prop, bool completion) {
			int colNum = treeDebaters.Columns.Length;
						
			CellRendererTextAdv renderer = new CellRendererTextAdv();
			TreeViewColumn col = treeDebaters.AppendColumn(prop,
			                          renderer,
			                          CellDataFuncDefault);
			
			
			renderer.Edited += delegate(object o, EditedArgs args) {
				CellRendererEdited(o, args, colNum);
			};
			
			if(completion) {
				renderer.CompletionModel = delegate() {
					List<string> list = new List<string>();
					store.Foreach(
						delegate(TreeModel model, TreePath path, TreeIter iter) {
							object o = model.GetValue(iter,0);
							object p = o.GetType().InvokeMember(col.Title+"Completion",BindingFlags.GetProperty,null,o,null);
							
							if(p != null) 
								if(!list.Contains(p.ToString()))
									list.Add(p.ToString());
							return false;
						});
					ListStore s = new ListStore(typeof(string));
					foreach(string str in list) 
						s.AppendValues(str);	
					return s;	
				};
			}			
			col.Resizable = true;			
			col.Clicked += delegate(object sender, EventArgs args) {
				SetSortColumn(colNum);
			};
			(treeDebaters.Model as TreeModelSort).SetSortFunc(colNum,
				delegate(TreeModel model, TreeIter a, TreeIter b) {
					return SortDebaters(model,a,b,colNum);
				});			
		}
		
		void CellRendererEdited(object sender, EditedArgs args, int colNum) {
			TreeModelSort model = (TreeModelSort)treeDebaters.Model;
			TreePath path = new TreePath(args.Path);
			TreeIter iter = TreeIter.Zero;
			model.GetIter(out iter, path);
			
			EditableDebater d = (EditableDebater)model.GetValue(iter, 0);
			// only save old data in rd if editing
			RoundDebater rd = null;
			if(newDebaterPath==null && colNum<4)
				rd = new RoundDebater(d);
			
			try {
				string prop = treeDebaters.Columns[colNum].Title;
				// This parses the given new string	
				d.GetType().InvokeMember("Parse"+prop,
			                             BindingFlags.InvokeMethod, null, d, 
				                         new object[] {args.NewText});
				
				// existing Debater: Update Data in (possibly) existing Rounds
				// tries to keep data consisting, but there's no guarantee
				// BlackList/WhiteList and ExtraInfo are not 
				// used in RoundDebater	
				if(newDebaterPath==null && colNum<4) {					
					object p = d.GetType().InvokeMember(prop, BindingFlags.GetProperty, null,
									                    d, new object[] {});
					// Only simple Renaming of Role is possible if Rounds exist
					if(colNum==3 
					   && Tournament.I.Rounds.Count>0 
					   && ((d.Role.IsTeamMember != rd.Role.IsTeamMember)
					       || (d.Role.IsJudge != rd.Role.IsJudge))) {
						MiscHelpers.ShowMessage(this, "Changing Role from Judge to TeamMember (or vice versa)" +
						 	" is not possible since Rounds are already set.", MessageType.Error);
						// reset to old role...
						d.Role = rd.Role;
						return;
					}
					
					// check if new TeamName is already present
					if(colNum==3 && d.Role.IsTeamMember) {
						int n=0;
						foreach(object[] row in store) {
							EditableDebater d_ = (EditableDebater)row[0];
							if(!d.Equals(d_) && d_.Role.TeamName==d.Role.TeamName) 
								n++;
						}	
						if(n==3) {
							MiscHelpers.ShowMessage(this, "New TeamName is already present in three other Debaters.", 
							                        MessageType.Error);
							// reset to old role...
							d.Role = rd.Role;
							return;	
						}
					}
					
					// check for duplicate
					if(colNum<3) {
						// need a temporary flag, throwing exceptions in delegates doesnt work...
						// the following flag stuff isn't elegant, but it should work
						bool flag = false;
						model.Foreach(delegate(TreeModel model_, TreePath path_, TreeIter iter_) {
							if(!iter.Equals(iter_)) {
								EditableDebater d_ = (EditableDebater)model_.GetValue(iter_,0);
								if(d_.Equals(d)) {
									// reset to old value...
									object old = rd.GetType().InvokeMember(prop,BindingFlags.GetProperty,
									                                       null, rd, new object[]{});
									d.GetType().InvokeMember(prop, BindingFlags.SetProperty, null, d, 
											                 new object[] {old});
									flag = true;
									return true;
								}
							}
							return false;
						});
						if(flag)
							throw new TargetInvocationException(new Exception("Debater exists."));
					}
					
					// keep data consistent in existing rounds									
					foreach(RoundData round in Tournament.I.Rounds) {
						foreach(RoomData room in round.Rooms) {
							foreach(RoundDebater rd_ in room.GetRoomMembers()) {
								if(rd_==null)
									continue;
								if(rd_.Equals(rd)) {
									rd_.GetType().InvokeMember(prop, BindingFlags.SetProperty, null, rd_, 
									                         new object[] {p});
								}
								if(rd_.Role.TeamName==rd.Role.TeamName) 
									rd_.Role.TeamName=d.Role.TeamName;
							}
						}
						if(rd.Role.IsTeamMember) {
							foreach(TeamData team in round.AllTeams) {
								foreach(RoundDebater rd_ in team) {
									if(rd_.Equals(rd)) {
										rd_.GetType().InvokeMember(prop, BindingFlags.SetProperty, null, rd_, 
									                             new object[] {p});
									}
									if(rd_.Role.TeamName==rd.Role.TeamName) 
										rd_.Role.TeamName=d.Role.TeamName;
								}
							}
						}
						else if(rd.Role.IsJudge) {
							foreach(RoundDebater rd_ in round.AllJudges) {
								if(rd_.Equals(rd)) {
									rd_.GetType().InvokeMember(prop, BindingFlags.SetProperty, null, rd_, 
										                     new object[] {p});
								}
							}
						}
					}
					
					// Renaming TeamName needs extra Handling
					if(colNum==3 && rd.Role.IsTeamMember && d.Role.IsTeamMember) {
						foreach(object[] row in store) {
							EditableDebater d_ = (EditableDebater)row[0];
							if(d_.Role.TeamName==rd.Role.TeamName)
								d_.Role.TeamName=d.Role.TeamName;
						}						
					}
				}
				// newDebater is entered...
				else if(colNum<3) {
					// continue with entering data
					// as idle so that cells are resized
					GLib.Idle.Add(delegate {
						treeDebaters.SetCursor(ConvertStorePathToModelPath(newDebaterPath),
						                       treeDebaters.Columns[colNum+1],true);
						return false;
					});
				}
				else {
					// new Debater entered completely
					iter = TreeIter.Zero;
					if(store.GetIter(out iter, newDebaterPath)) {
						// as idle to prevent gtk critical (no idea why this happens)
						GLib.Idle.Add(delegate {
							store.Remove(ref iter);
							newDebaterPath = null;
							if(IsNotInStore(d))
								store.AppendValues(d);
							else 
								MiscHelpers.ShowMessage(this, "Debater exists.", MessageType.Error);
							UpdateDebatersInfo();
							btnDebaterAdd.GrabFocus();							
							return false;
						});
					}
				}
				
				// Gui stuff
				treeDebaters.ColumnsAutosize();
				// ugly method of resorting the TreeSortModel...
				SortType st;
				int sortColumn;
				model.GetSortColumnId(out sortColumn,
				  	                  out st);
				if(st==SortType.Descending) {
					model.SetSortColumnId(sortColumn, SortType.Ascending);
					model.SetSortColumnId(sortColumn, SortType.Descending);
				}
				else {
					model.SetSortColumnId(sortColumn, SortType.Descending);
					model.SetSortColumnId(sortColumn, SortType.Ascending);
				}
				
				Tournament.I.Save();
			}
			catch(TargetInvocationException e) {
				MessageDialog md = new MessageDialog(this, DialogFlags.Modal,
					                                 MessageType.Error,
				                                     ButtonsType.OkCancel,
			                                    	 e.InnerException.Message+". Try again?");
				md.DefaultResponse = ResponseType.Ok;
				ResponseType r = (ResponseType)md.Run();
				md.Destroy();
				if(r == ResponseType.Ok) {
					// As Idle otherwise Editable isn't destroyed correctly
					GLib.Idle.Add(delegate {
						(sender as CellRendererTextAdv).TempEditString = args.NewText;
						treeDebaters.SetCursor(path,treeDebaters.Columns[colNum],true);				
						return false;
					});
				}
				else {				
					if(newDebaterPath == null)
						return;
					iter = TreeIter.Zero;
					if(store.GetIter(out iter, newDebaterPath)) {
						// remove not finished new debater,
						// with idle call, 
						// prevents Gtk critical filter model assertion.
						GLib.Idle.Add(delegate {
							newDebaterPath = null;
							store.Remove(ref iter);
							return false;
						});
					}
				}
			}				
		}
		
		void CellDataFuncDefault(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			object o = model.GetValue(iter,0);
			object prop = o.GetType().InvokeMember(column.Title,BindingFlags.GetProperty,null,o,null);
			(cell as CellRendererText).Text = prop.ToString();
		}
		
		void SetSortColumn(int colNum) {
			TreeViewColumn col = treeDebaters.Columns[colNum];
			col.SortIndicator = true;
			
			int sortCol;
			SortType sortOrder;
			TreeModelSort modelSort = treeDebaters.Model as TreeModelSort;
			modelSort.GetSortColumnId(out sortCol, out sortOrder);
			
			
			if(sortCol == colNum) {
				if(sortOrder==SortType.Ascending) 
					col.SortOrder = SortType.Descending;
				else 
					col.SortOrder = SortType.Ascending;
			}
			else {
				editDebatersSettings.lastSortCol = sortCol;
				editDebatersSettings.lastSortOrder = treeDebaters.Columns[sortCol].SortOrder;
				treeDebaters.Columns[sortCol].SortIndicator = false;
				col.SortOrder = SortType.Descending;
			}			
			
			editDebatersSettings.sortCol = colNum;
			editDebatersSettings.sortOrder = col.SortOrder;
			modelSort.SetSortColumnId(colNum,col.SortOrder);
		}
		
		
		
		int SortDebaters(TreeModel model, TreeIter a, TreeIter b, int col) {
			EditableDebater d_a = (EditableDebater)model.GetValue(a,0);
			EditableDebater d_b = (EditableDebater)model.GetValue(b,0);
			if(d_a!=null && d_b!=null) {
				string prop = treeDebaters.Columns[col].Title;	
				IComparable c_a = (IComparable)d_a.GetType().InvokeMember(prop,BindingFlags.GetProperty,null,d_a,null);
				IComparable c_b = (IComparable)d_b.GetType().InvokeMember(prop,BindingFlags.GetProperty,null,d_b,null);
				
				int compare = c_a.CompareTo(c_b);
				int lastSortCol = editDebatersSettings.lastSortCol;				
				
				if(lastSortCol != -1 && compare == 0) {
					string propLast = treeDebaters.Columns[lastSortCol].Title;	
					IComparable c_a_last = (IComparable)d_a.GetType().InvokeMember(propLast,BindingFlags.GetProperty,null,d_a,null);
					IComparable c_b_last = (IComparable)d_b.GetType().InvokeMember(propLast,BindingFlags.GetProperty,null,d_b,null);
					
					SortType lastSortOrder = editDebatersSettings.lastSortOrder;						
					if(treeDebaters.Columns[col].SortOrder == lastSortOrder)
						return c_a_last.CompareTo(c_b_last);
					else
						return -c_a_last.CompareTo(c_b_last);
				}
				else
					return compare;				
			}
			else 	
				return 0;
			
		}
		
		
		
		bool FilterDebaters(TreeModel model, TreeIter iter) {
			IMySearchable s = (IMySearchable)model.GetValue(iter, 0);
			if(s!=null) 
				return s.MatchesSearchString(entryDebatersFilter.Text);
			else
				return true;
		}
		
		
		
		protected virtual void OnBtnDebaterAddClicked (object sender, System.EventArgs e)
		{
			if(newDebaterPath != null)
				return;
			entryDebatersFilter.Text = "";
			
			EditableDebater d = new EditableDebater();			
			newDebaterPath = store.GetPath(store.AppendValues(d));
			treeDebaters.SetCursor(ConvertStorePathToModelPath(newDebaterPath),treeDebaters.Columns[0],true);
		}
		
		protected virtual void OnBtnDebaterRemoveClicked (object sender, System.EventArgs e)
		{
			TreePath path;
			TreeViewColumn col;
			treeDebaters.GetCursor(out path, out col);
			
			if(path != null) {
				if(Tournament.I.Rounds.Count>0 && 
				   MiscHelpers.AskYesNo(this, "Deleting with existing rounds can lead to inconsistent data. "+
				                        "Better try renaming. Continue?")==ResponseType.No)
					return;
				TreeIter iter = TreeIter.Zero;
				if(treeDebaters.Model.GetIter(out iter, path)) {
					iter = ConvertModelIterToStoreIter(iter);
					store.Remove(ref iter);
					UpdateDebatersInfo();
				}
			}
		}
		
		protected virtual void OnEntryDebatersFilterChanged (object sender, System.EventArgs e)
		{
			((treeDebaters.Model as TreeModelSort).Model as TreeModelFilter).Refilter();
			UpdateDebatersInfo();
		}
		
		TreeIter ConvertModelIterToStoreIter(TreeIter iter) {
			TreeModelSort sort = (TreeModelSort)treeDebaters.Model;
			TreeModelFilter filter = (TreeModelFilter)sort.Model;
			return filter.ConvertIterToChildIter(sort.ConvertIterToChildIter(iter));
		}
		
		TreePath ConvertStorePathToModelPath(TreePath path) {
			TreeModelSort sort = (TreeModelSort)treeDebaters.Model;
			TreeModelFilter filter = (TreeModelFilter)sort.Model;
			return sort.ConvertChildPathToPath(filter.ConvertChildPathToPath(path));
		}
		
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			List<Debater> list = new List<Debater>();
			foreach(object[] row in store) 
				list.Add(new Debater((EditableDebater)row[0]));
			Tournament.I.Debaters = list;
			ShowRanking.I.UpdateAll();
		}
		
		protected void OnBtnExportCSVClicked (object sender, System.EventArgs e)
		{
			DoTheExport(MiscHelpers.TemplateType.CSV);
		}
		
		protected void OnBtnExportPDFClicked (object sender, System.EventArgs e)
		{
			DoTheExport(MiscHelpers.TemplateType.PDF);			
		}
		
		void DoTheExport(MiscHelpers.TemplateType type) {
			TreeModel model = treeDebaters.Model;
			TreeIter iter;
			if(model.GetIterFirst(out iter)) {
				try {
					ITemplate tmpl = MiscHelpers.GetTemplate("debaters", type);
					ITmplBlock tmplDebaters = tmpl.ParseBlock("DEBATERS");
					int n=0;
					do {
						n++;
						TreeIter storeIter = ConvertModelIterToStoreIter(iter);
						EditableDebater d = (EditableDebater)store.GetValue(storeIter, 0);
						tmplDebaters.Assign("NUM",n.ToString());
						tmplDebaters.Assign("NAME",d.Name.ToString());
						tmplDebaters.Assign("CLUB",d.Club.ToString());
						tmplDebaters.Assign("AGE",d.Age.ToString());
						tmplDebaters.Assign("ROLE",EscapeString(d.Role.ToString(), type));
						tmplDebaters.Assign("BLACKLIST",EscapeString(d.BlackList.ToString(), type));
						tmplDebaters.Assign("WHITELIST",EscapeString(d.WhiteList.ToString(), type));
						tmplDebaters.Assign("EXTRAINFO",EscapeString(d.ExtraInfo.ToString(), type));
						tmplDebaters.Out();
					}
					while(model.IterNext(ref iter));
					MiscHelpers.AskShowTemplate(this,
						"Debaters successfully generated, see "+
						"pdfs/debaters.(pdf|csv)",
						MiscHelpers.MakeExportFromTemplate()
						);
				}
				catch(Exception ex) {
					MiscHelpers.ShowMessage(this, "Could not export Debaters: "+ex.Message, MessageType.Error);
				}
			}	
		}
		
		string EscapeString(string s, MiscHelpers.TemplateType type) {
			string[] escapedChars = type == MiscHelpers.TemplateType.CSV ? 
				new string[] {"\""} :
				new string[] {"#"};
			foreach(string c in escapedChars) {
				s = s.Replace(c, "\\"+c);
			}
			return s;
		}

		
	}
}

			                          
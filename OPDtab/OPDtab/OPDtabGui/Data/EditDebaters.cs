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
			
			SetupDebaterColumn("Name", CellRendererTextAdv.Type.Entry);
			SetupDebaterColumn("Club", CellRendererTextAdv.Type.EntryWithCompletion);
			SetupDebaterColumn("Age", CellRendererTextAdv.Type.Entry);
			SetupDebaterColumn("Role", CellRendererTextAdv.Type.EntryWithCompletion);
			SetupDebaterColumn("BlackList", CellRendererTextAdv.Type.DebaterPattern);
			SetupDebaterColumn("WhiteList", CellRendererTextAdv.Type.DebaterPattern);
			SetupDebaterColumn("ExtraInfo", CellRendererTextAdv.Type.Entry);
			treeDebaters.HeadersClickable = true;
			treeDebaters.Selection.Mode = SelectionMode.Multiple;
			
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
		
		
		
		void SetupDebaterColumn(string prop, CellRendererTextAdv.Type type) {
			int colNum = treeDebaters.Columns.Length;
						
			CellRendererTextAdv renderer = new CellRendererTextAdv(type, store, colNum);
			TreeViewColumn col = treeDebaters.AppendColumn(prop,
			                          renderer,
			                          CellDataFuncDefault);
			
			
			renderer.MyEdited += delegate(CellRendererTextAdv sender, string path, string newText) {
				CellRendererEdited(sender, path, newText, colNum);	
			};
				
			
			col.Resizable = true;			
			col.Clicked += delegate(object sender, EventArgs args) {
				SetSortColumn(colNum);
			};
			(treeDebaters.Model as TreeModelSort).SetSortFunc(colNum,
				delegate(TreeModel model, TreeIter a, TreeIter b) {
					return SortDebaters(model,a,b,colNum);
				});			
		}
		
		void CellRendererEdited(CellRendererTextAdv sender, 
		                        string pathStr, string newText, int colNum) {
			
			TreeModelSort model = (TreeModelSort)treeDebaters.Model;
			TreePath path = new TreePath(pathStr);
			TreeIter iter = TreeIter.Zero;
			model.GetIter(out iter, path);
			
			EditableDebater d = (EditableDebater)model.GetValue(iter, 0);
			// only save old data in rd if editing
			RoundDebater rd = null;
			if(newDebaterPath==null && colNum<4)
				rd = new RoundDebater(d);
			
			try {
				string prop = treeDebaters.Columns[colNum].Title;
				// This parses the given new string,
				// and updates the data in store
				d.GetType().InvokeMember("Parse"+prop,
			                             BindingFlags.InvokeMethod, null, d, 
				                         new object[] {newText});
				
				// existing Debater: Update Data in (possibly) existing Rounds
				// tries to keep data consisting, but there's no guarantee
				// BlackList/WhiteList and ExtraInfo are not 
				// used in RoundDebater, so skip this by condition colNum<4	
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
				else if(newDebaterPath != null && colNum<3) {
					// continue with entering data (goto next column)
					// as idle so that cells are resized
					GLib.Idle.Add(delegate {
						treeDebaters.SetCursor(ConvertStorePathToModelPath(newDebaterPath),
						                       treeDebaters.Columns[colNum+1],true);
						return false;
					});
				}
				else if(newDebaterPath != null) {
					// new Debater entered completely (at least all necessary data)
					iter = TreeIter.Zero;
					if(store.GetIter(out iter, newDebaterPath)) {
						// as idle to prevent gtk critical (no idea why this happens)
						GLib.Idle.Add(delegate {
							store.Remove(ref iter);
							newDebaterPath = null;
							if(IsNotInStore(d)) {
								store.AppendValues(d);
								SaveDebaters();
							}
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
				
				// save data from store if not adding new debater
				if(newDebaterPath==null)
					SaveDebaters();
				
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
						sender.TempEditString = newText;
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
			if(treeDebaters.Selection.CountSelectedRows() == 0)
				return;
			
			// display warning and hint
			if(MiscHelpers.AskYesNo(this, "Deleting with existing rounds can lead to inconsistent data. "+
		                        "Better try renaming of existing debaters or swapping their roles. Continue anyway?")
				== ResponseType.No) {
				return;
			}		
							
			// delete the selected rows
			List<TreeIter> storeIters = new List<TreeIter>();
			foreach(TreePath path in treeDebaters.Selection.GetSelectedRows()) {
				TreeIter iter;
				if(treeDebaters.Model.GetIter(out iter, path)) {
					TreeIter storeIter = ConvertModelIterToStoreIter(iter);			
					storeIters.Add(storeIter);
				}
			}
			foreach(TreeIter iter in storeIters) {
				RemoveDebaterFromRounds(iter);
				TreeIter refIter = iter;
				store.Remove(ref refIter);
			}				
			UpdateDebatersInfo();
			SaveDebaters();
		}
		
		void RemoveDebaterFromRounds(TreeIter storeIter) {
			EditableDebater d = (EditableDebater)store.GetValue(storeIter, 0);
			
			// removing a judge from existing rounds is easy
			// but everything else is not (consider non-complete teams...)
			// so, we rely on the issued warning and hope the user knows 
			// what to do
			if(!d.Role.IsJudge)
				return;
			
			foreach(RoundData round in Tournament.I.Rounds) {
				RoundDebater rd = round.AllJudges.Find(delegate(RoundDebater obj) {
					return obj.Equals(d);
					});	
				// if not found, continue!
				if(rd==null)
					continue;
				// d is Judge, so check Chair and Judges in rooms
				foreach(RoomData room in round.Rooms) {
					if(rd.Equals(room.Chair))
						
						room.Chair = null;
					else 
						// if judge isn't there, can't be removed...
						room.Judges.Remove(rd);
				}		
				// remove from all judges
				round.AllJudges.Remove(rd);	
			}
			
			
		}
		
		protected void OnBtnSwapRolesClicked (object sender, System.EventArgs e)
		{
			TreePath[] r = treeDebaters.Selection.GetSelectedRows();
			if(r.Length != 2) {
				MiscHelpers.ShowMessage(this, "Select exactly two Debaters to swap.",
					MessageType.Error);
				return;
			}
						
			TreeIter iter1;
			TreeIter iter2;
			if(!treeDebaters.Model.GetIter(out iter1, r[0]) 
				|| !treeDebaters.Model.GetIter(out iter2, r[1])) {
				// this should never happen
				return;	
			}
			// get the references in the store
			TreeIter storeIter1 = ConvertModelIterToStoreIter(iter1);
			TreeIter storeIter2 = ConvertModelIterToStoreIter(iter2);
			EditableDebater d1 = (EditableDebater)
				store.GetValue(storeIter1, 0);
			EditableDebater d2 = (EditableDebater)
				store.GetValue(storeIter2, 0);
			
			// swapping it in the store
			Role tmp = d2.Role;
			d2.Role = d1.Role;
			d1.Role = tmp;	
						
			// update in existing rounds (this is ugly due to our data model)
			// this resets the info about available judges...
			// any other approach wouldn't be future-proof
								
			foreach(RoundData round in Tournament.I.Rounds) {
				// always create new instance for each round
				RoundDebater rd1 = new RoundDebater(d1);
				RoundDebater rd2 = new RoundDebater(d2);
				
				// search for d1, replace by d2 and vice versa
				// this should work since d1 and d2 have already swapped roles
				foreach(RoomData room in round.Rooms) 
					room.ReplaceRoomMember(rd1, RoundDebater.Dummy());
				foreach(RoomData room in round.Rooms) 
					room.ReplaceRoomMember(rd2, rd1);
				foreach(RoomData room in round.Rooms) 
					room.ReplaceRoomMember(RoundDebater.Dummy(), rd2);
				
				// update also allArrays, the following UpdateAllArrays
				// relies on consistent arrays
				round.ReplaceInAllArrays(rd1, RoundDebater.Dummy());
				round.ReplaceInAllArrays(rd2, new RoundDebater(rd1));
				round.ReplaceInAllArrays(RoundDebater.Dummy(), new RoundDebater(rd2));
				
				// since complicated things can happen above
				// we make the arrays consistent by brute force
				round.UpdateAllArrays();
			}
			
			// tell the treeview to update, don't know why path and iter is necessary
			store.EmitRowChanged(store.GetPath(storeIter1), storeIter1);
			store.EmitRowChanged(store.GetPath(storeIter2), storeIter2);
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
			/*List<Debater> list = new List<Debater>();
			foreach(object[] row in store) 
				list.Add(new Debater((EditableDebater)row[0]));
			Tournament.I.Debaters = list;*/
			SaveDebaters();
			ShowRanking.I.UpdateAll();
		}
		
		void SaveDebaters() {
			List<Debater> list = new List<Debater>();
			foreach(object[] row in store) 
				list.Add(new Debater((EditableDebater)row[0]));
			Tournament.I.Debaters = list;
			Tournament.I.Save();
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

			                          
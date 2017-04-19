using System;
using System.Reflection;
using System.Collections.Generic;
using Gtk;
using OPDtabData;

namespace OPDtabGui
{
	public partial class EditDebaters : Window
	{

		/*
		 * The following interface, class and struct allow us to interact with the table model, which consists of one
		 * EditableDebater per table row. A column of the table represents one property of an EditableDebater and
		 * is described by a title and one ColumnInfo object. The latter specifies how the property is to be
		 * rendered, how it can be retrieved from an EditableDebater and parsed from a string.
		 *
		 * Retrieving a property gives us an IDebaterProperty which implements IComparable. Its Set method can be
		 * used to set the corresponding property of a Debater to the retrieved value. UnsafeSetRoundDebater does the same
		 * if RoundDebaters have the corresponding property.
		 *
		 * The DebaterProperty<T> class wraps a value of type T, implementing the IDebaterProperty interface by
		 * incorporating information on how to set an EditableDebater's corresponding property to the wrapped value.
		 */

		interface IDebaterProperty : IComparable {
			void Set(Debater debater);
			void UnsafeSetRoundDebater(RoundDebater debater);
		}

		class DebaterProperty<T> : IDebaterProperty where T : IComparable
		{
			public readonly T prop;
			public readonly Action<Debater, T> set;
			public readonly Action<RoundDebater, T> setRoundDebater;

			public DebaterProperty(
				T prop,
				Action<Debater, T> set,
				Action<RoundDebater, T> setRoundDebater)
			{
				this.prop = prop;
				this.set = set;
				this.setRoundDebater = setRoundDebater;
			}

			public int CompareTo(object obj)
			{
				return prop.CompareTo(obj);
			}

			public void Set(Debater debater)
			{
				set(debater, prop);
			}

			public void UnsafeSetRoundDebater(RoundDebater debater)
			{
				setRoundDebater(debater, prop);
			}

			public override string ToString() {
				return prop.ToString();
			}

			public override bool Equals(object obj)
			{
				return prop.Equals(obj);
			}

			public override int GetHashCode()
			{
				return prop.GetHashCode();
			}
		}

		struct ColumnInfo
		{
			public CellRendererTextAdv.Type cellRendererType;
			public Func<EditableDebater, IDebaterProperty> get;
			public Action<EditableDebater, string> parseAndSet;
		}


		AppSettings.EditDebatersClass editDebatersSettings;
		TreePath newDebaterPath;
		ListStore store;
		Entry entryDebatersFilter;

		// The following dictionary represents the list of columns in the tree view. It is morally constant, but
		// const dictionaries aren't yet a thing.
		Dictionary<string, ColumnInfo> columns;

		// TODO Currently, all the parsing functions simply swallow errors,
		// doing nothing if an error is encountered. It might me nicer to
		// give some feedback to the user.
		public EditDebaters() : base(WindowType.Toplevel)
		{
			columns = new Dictionary<string, ColumnInfo> {
				{"Name", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.Entry,
					get = e => new DebaterProperty<Name>(
							e.Name,
							(e1, name) => e1.Name = name,
							(e1, name) => e1.Name = name),
					parseAndSet = (e, name) => {
						OPDtabData.Name.Parse(name).Do(n => e.Name = n);
					}
				}},
				{"Club", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.EntryWithCompletion,
					get = e => new DebaterProperty<Club>(
							e.Club,
							(e1, club) => e1.Club = club,
							(e1, club) => e1.Club = club),
					parseAndSet = (e, club) => {
						Club.Parse(club).Do(c => e.Club = c);
					}
				}},
				{"Age", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.Entry,
					get = e => new DebaterProperty<uint>(
							e.Age,
							(e1, age) => e1.Age = age,
							(e1, age) => e1.Age = age),
						parseAndSet = (e, age) => {
							try {
								e.Age = uint.Parse(age);
							} catch (FormatException) {}
						}
				}},
				{"Role", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.EntryWithCompletion,
					get = e => new DebaterProperty<Role>(
							e.Role,
							(e1, role) => e1.Role = role,
							(e1, role) => e1.Role = role),
					parseAndSet = (e, role) => {
						e.Role = OPDtabData.Role.Parse(role);
					}
				}},
				{"BlackList", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.DebaterPattern,
					get = e => new DebaterProperty<DebaterPattern>(
							e.BlackList,
							(e1, bl) => e1.BlackList = bl,
							(_, __) => { throw new Exception(
								"Attempt to set BlackList property of RoundDebater."); }),
					parseAndSet = (e, bl) => e.BlackList = DebaterPattern.Parse(bl)}},
				{"WhiteList", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.DebaterPattern,
					get = e => new DebaterProperty<DebaterPattern>(
							e.WhiteList,
							(e1, wl) => e1.WhiteList = wl,
							(_, __) => { throw new Exception(
								"Attempt to set WhiteList property of RoundDebater."); }),
					parseAndSet = (e, wl) => e.WhiteList = DebaterPattern.Parse(wl)}},
				{"ExtraInfo", new ColumnInfo{
					cellRendererType = CellRendererTextAdv.Type.Entry,
					get = e => new DebaterProperty<string>(
							e.ExtraInfo,
							(e1, info) => e1.ExtraInfo = info,
							(_, __) => { throw new Exception(
								"Attempt to set ExtraInfo property of RoundDebater."); }),
					parseAndSet = (e, info) => e.ExtraInfo = info.Trim()}}
			};

			Build ();
			editDebatersSettings = AppSettings.I.EditDebaters;
			entryDebatersFilter = MiscHelpers.MakeFilterEntry ();
			entryDebatersFilter.Changed += OnEntryDebatersFilterChanged;
			cEntryFilter.Add (entryDebatersFilter);
			cEntryFilter.ShowAll ();
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

			foreach (var col in columns) {
				SetupDebaterColumn (col.Key, col.Value.cellRendererType);
			}
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
			col.Clicked += ((_, __) => SetSortColumn(colNum));
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

			
			try {
				ColumnInfo prop = columns[treeDebaters.Columns[colNum].Title];
				// This parses the given new string,
				// and updates the data in store
				prop.parseAndSet (d, newText);
				
				// existing Debater: Update Data in (possibly) existing Rounds
				// tries to keep data consisting, but there's no guarantee
				// BlackList/WhiteList and ExtraInfo are not 
				// used in RoundDebater, so skip this by condition colNum<4	
				if(newDebaterPath==null && colNum<4) {
					var rd = new EditableDebater (d);
					var p = prop.get(d);

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
						model.Foreach((model_, _, iter_) => {
							if(!iter.Equals(iter_)) {
								EditableDebater d_ = (EditableDebater)model_.GetValue(iter_,0);
								if(d_.Equals(d)) {
									// reset to old value...
									prop.get (rd).Set (d);
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
									p.UnsafeSetRoundDebater(rd_);
								}
								if(rd_.Role.TeamName==rd.Role.TeamName) 
									rd_.Role.TeamName=d.Role.TeamName;
							}
						}
						if(rd.Role.IsTeamMember) {
							foreach(TeamData team in round.AllTeams) {
								foreach(RoundDebater rd_ in team) {
									if(rd_.Equals(rd)) {
										p.UnsafeSetRoundDebater(rd_);
									}
									if(rd_.Role.TeamName==rd.Role.TeamName) 
										rd_.Role.TeamName=d.Role.TeamName;
								}
							}
						}
						else if(rd.Role.IsJudge) {
							foreach(RoundDebater rd_ in round.AllJudges) {
								if(rd_.Equals(rd)) {
									p.UnsafeSetRoundDebater(rd_);
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
			var debater = (EditableDebater)model.GetValue(iter,0);
			var cellText = (CellRendererText)cell;
			var val = columns[column.Title].get(debater);
			cellText.Text = val != null ? val.ToString() : "?";
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
				ColumnInfo prop = columns[treeDebaters.Columns[col].Title];
				IComparable c_a = prop.get (d_a);
				IComparable c_b = prop.get (d_b);
				
				int compare = c_a.CompareTo(c_b);
				int lastSortCol = editDebatersSettings.lastSortCol;				
				
				if(lastSortCol != -1 && compare == 0) {
					ColumnInfo propLast = columns[treeDebaters.Columns[lastSortCol].Title];
					IComparable c_a_last = propLast.get (d_a);
					IComparable c_b_last = propLast.get (d_b);
					
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
		
		
		
		protected virtual void OnBtnDebaterAddClicked (object sender, EventArgs e)
		{
			if(newDebaterPath != null)
				return;
			entryDebatersFilter.Text = "";
			
			EditableDebater d = new EditableDebater();			
			newDebaterPath = store.GetPath(store.AppendValues(d));
			treeDebaters.SetCursor(ConvertStorePathToModelPath(newDebaterPath),treeDebaters.Columns[0],true);
		}
		
		protected virtual void OnBtnDebaterRemoveClicked (object sender, EventArgs e)
		{
			if(treeDebaters.Selection.CountSelectedRows() == 0)
				return;
			
			// display warning and hint
			if(Tournament.I.Rounds.Count>0 &&
			   MiscHelpers.AskYesNo(this, "Deleting with existing rounds can lead to inconsistent data. "+
		                        "Better try renaming of existing debaters or swapping their roles. Continue anyway?")
				== ResponseType.No) {
				return;
			}
			else if(MiscHelpers.AskYesNo(this, "Do you really want to remove this entry?") ==
				        ResponseType.No) {
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
		
		protected void OnBtnSwapRolesClicked (object sender, EventArgs e)
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
				// clear all round results of the debaters in the store!
				d1.SetRoom(round.RoundName, null);
				d2.SetRoom(round.RoundName, null);


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
				// relies on at least consistent arrays
				round.ReplaceInAllArrays(rd1, RoundDebater.Dummy());
				round.ReplaceInAllArrays(rd2, new RoundDebater(rd1));
				round.ReplaceInAllArrays(RoundDebater.Dummy(), new RoundDebater(rd2));
				
				// since complicated things can happen above
				// we make the arrays consistent by brute force
				// this creates many warnings that round results are cleared,
				// but the debaters are backed up in store here...complicated, as said
				round.UpdateAllArrays();
			}

			// overwrite the changes with the changes from the store
			SaveDebaters();
			// and reflect the possible reset of RoundResults
			ShowRanking.I.UpdateAll();

			// tell the treeview to update, don't know why path and iter is necessary
			store.EmitRowChanged(store.GetPath(storeIter1), storeIter1);
			store.EmitRowChanged(store.GetPath(storeIter2), storeIter2);
		}
		
		
		protected virtual void OnEntryDebatersFilterChanged (object sender, EventArgs e)
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
		
		protected virtual void OnDeleteEvent (object o, DeleteEventArgs args)
		{
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
		
		protected void OnBtnExportCSVClicked (object sender, EventArgs e)
		{
			DoTheExport(MiscHelpers.TemplateType.CSV);
		}
		
		protected void OnBtnExportPDFClicked (object sender, EventArgs e)
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

			                          
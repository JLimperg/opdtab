using System;
using Gtk;
using OPDtabData;
namespace OPDtabGui
{
	public partial class EditRoundResults : Gtk.Window
	{
		ListStore rounds;
		ListStore rooms;
		RoundResultSheet resultsheet;
				
		public EditRoundResults () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();			
			RoundResultData.UpdateStats();
			// resultsheet
			resultsheet = new RoundResultSheet();
			ebResultSheet.Add(resultsheet);
			ebResultSheet.ShowAll();
			
			// cbRooms
			SetupCombobox(cbRooms);
			cbRooms.Sensitive = false;
			rooms = new ListStore(typeof(RoomData));
			cbRooms.Model = rooms;			
						
			// cbRounds			
			SetupCombobox(cbRounds);
			rounds = new ListStore(typeof(RoundData));
			cbRounds.Model = rounds;
			foreach(RoundData rd in Tournament.I.Rounds) {
			 	rounds.AppendValues(rd);	
			}
			SelectFirstIncomplete(cbRounds);
			
		}
		
		void SetupCombobox(ComboBox cb) {			
			CellRendererPixbuf cellPixbuf = new CellRendererPixbuf();
			cb.PackStart(cellPixbuf, false);
			cb.SetCellDataFunc(cellPixbuf, ComboboxCellDataFunc);
			
			CellRendererText cellText = new CellRendererText();
			cb.PackStart(cellText, false);
			cb.SetCellDataFunc(cellText, ComboboxCellDataFunc);				
		}
		
		void ComboboxCellDataFunc(CellLayout layout,
		                          CellRenderer cell,
		                          TreeModel model,
		                          TreeIter iter) {

			object o = model.GetValue(iter, 0);
			if(o == null)
				return;
			
			if(o is IRoundEditCbItem) {
				IRoundEditCbItem item = o as IRoundEditCbItem;
				if(cell is CellRendererText)
					(cell as CellRendererText).Text = item.CbText;
				else if(cell is CellRendererPixbuf) 
					(cell as CellRendererPixbuf).IconName = 
						item.ItemCompleted ? "gtk-yes" : "gtk-no";		
			}			
		}
		
		
		
		protected virtual void OnCbRoundsChanged (object sender, System.EventArgs e)
		{
			//Tournament.I.Save();
			TreeIter iter = TreeIter.Zero;
			if(cbRounds.GetActiveIter(out iter)) {
				RoundData rd = (RoundData)rounds.GetValue(iter, 0);
				if(rd.Motion == "")
					lblMotion.Markup = "<i>No Motion</i>";
				else
					lblMotion.Text = rd.Motion;
				
				rooms.Clear();
				foreach(RoomData room in rd.Rooms)
					rooms.AppendValues(room);
				cbRooms.Sensitive = true;
				SelectFirstIncomplete(cbRooms);
			}
			else
				cbRooms.Sensitive = false;
		}
		
		protected virtual void OnCbRoomsChanged (object sender, System.EventArgs e)
		{
			//Tournament.I.Save();
			TreeIter iter = TreeIter.Zero;
			if(cbRooms.GetActiveIter(out iter)) {
				RoomData room = (RoomData)rooms.GetValue(iter, 0);
				MiscHelpers.AddToContainer(cChair, new DebaterWidget(room.Chair));
				if(cbRounds.GetActiveIter(out iter)) {
					RoundData rd = (RoundData)rounds.GetValue(iter, 0);
					resultsheet.SetRoomData(rd.RoundName, room);
				}
				else 
					Console.WriteLine("This shouldn't be possible by GUI");
			}
		}
		
		void SelectFirstIncomplete(ComboBox cb) {
			cb.Model.Foreach(delegate(TreeModel m, TreePath p, TreeIter i) {
				IRoundEditCbItem item = (IRoundEditCbItem)m.GetValue(i, 0);
				if(!item.ItemCompleted) {
					cb.SetActiveIter(i);
					return true;
				}
				else 
					return false;
			});
		}
		
		public void UpdateComboboxes() {
			// enough to trigger redraws...
			cbRooms.ModifyBg(StateType.Normal);
			cbRounds.ModifyBg(StateType.Normal);
		}
		
		protected virtual void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
		{
			Tournament.I.Save();
		}
		
		
		
	}
}


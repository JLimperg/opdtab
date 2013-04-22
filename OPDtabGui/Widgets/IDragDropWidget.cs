
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Gtk;
using OPDtabData;
namespace OPDtabGui
{
	public delegate void SetupDragDropHandler (IDragDropWidget sender, MyButton btn);
	public delegate void SetDataTriggerHandler (Widget sender, object data);
	public interface IDragDropWidget {
		// currently only used by DragDropHelpers.SourceSet
		void SetupDragDropSource(string t, object data);
		// used to process the received data at the source...
		event SetDataTriggerHandler SetDataTrigger;
		// ...given by SetData from the dest
		void SetData(object data);
		// used for keeping track of visitedRooms in Debater Class
		void SetRoom(string roundName, RoomData roomData);
		// for DebaterPool
		void SetIsInPool(bool flag, string roundName);
		// for conflict indication...
		void ShowNotifications();
	};
}

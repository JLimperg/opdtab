using System;
using System.Xml.Serialization;
namespace OPDtabData
{
	public interface IRoomMember
	{
		void SetRoom(string roundName, RoomData rd);
		bool IsEmpty { get; }
		void SetEmpty();
	}
}

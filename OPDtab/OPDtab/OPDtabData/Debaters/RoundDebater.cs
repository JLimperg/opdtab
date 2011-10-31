using System;
using System.Collections.Generic;
namespace OPDtabData
{
	[Serializable]
	public class RoundDebater : AbstractDebater, IRoomMember
	{
		bool isShown;
		bool judgeAvail;
		[NonSerialized]
		RoomConflict conflict;
		
		public RoundDebater() : base() {
			isShown = false;
			judgeAvail = true;
		}
		
		public RoundDebater(AbstractDebater d) : this(d, true, true) 
		{
		}
		
		public RoundDebater (AbstractDebater d, bool shown) : this(d, shown, true)
		{
		}
		
		public RoundDebater (AbstractDebater d, bool shown, bool avail) : base(d)
		{
			isShown = shown;
			judgeAvail = avail;
		}
		
		
		public bool IsShown {
			get {
				return this.isShown;
			}
			set {
				isShown = value;
			}
		}	
		
		public bool JudgeAvail {
			get {
				return this.judgeAvail;
			}
			set {
				judgeAvail = value;
			}
		}

		public void SetConflict(RoomConflict rc) {
			conflict = rc;
		}
		
		public RoomConflict GetConflict() {
			// can be null...
			return conflict;	
		}
		
		public int GetConflictPenalty() {
			if(conflict == null)
				return -1;
			return conflict.GetConflictPenalty();						
		}
		
		#region IRoomMember implementation
		void IRoomMember.SetRoom (string roundName, RoomData rd)
		{
			// this is expensive...:(
			IRoomMember d = (IRoomMember)Tournament.I.FindDebater(this);
			if(d != null)
				d.SetRoom(roundName, rd);
		}
		#endregion
	
	}
}


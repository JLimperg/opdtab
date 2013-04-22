using System;
using System.Collections.Generic;
namespace OPDtabData
{
	[Serializable]
	public class RoundDebater : AbstractDebater, IRoomMember
	{
		public enum JudgeStateType {
			Judge=0, NotAvail=1, Chair=2, FirstJudge=3	
		};
		
		bool isShown;
		JudgeStateType judgeState;
		[NonSerialized]
		RoomConflict conflict;
		[NonSerialized]
		bool isDummy; // dummy debater used for swapping in existing round data
		
		public RoundDebater() : base() {
			isShown = false;
			judgeState = JudgeStateType.Judge;
			isDummy = false;
		}
		
		public static RoundDebater Dummy() {
			RoundDebater rd = new RoundDebater();
			rd.isDummy = true;
			return rd;
		}
				
		public RoundDebater(AbstractDebater d) : this(d, true, JudgeStateType.Judge) 
		{
		}
		
		public RoundDebater (AbstractDebater d, bool shown) : this(d, shown, JudgeStateType.Judge)
		{
		}
		
		public RoundDebater (AbstractDebater d, bool shown, JudgeStateType state) : base(d)
		{
			isShown = shown;
			judgeState = state;
		}
		
		
		public bool IsShown {
			get {
				return this.isShown;
			}
			set {
				isShown = value;
			}
		}	
		
		// this ensure backward compatibility
		[System.Xml.Serialization.XmlIgnore]
		public bool JudgeAvail {
			get {
				return judgeState != JudgeStateType.NotAvail;
			}
			set {
				judgeState = value ? JudgeStateType.Judge : JudgeStateType.NotAvail;
			}
		}
		
		public JudgeStateType JudgeState {
			get {
				return this.judgeState;
			}	
			set {
				judgeState = value;
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
		
		public override bool Equals (object obj)
		{
			// handle possible dummy debater
			if(obj is RoundDebater) { 
				RoundDebater rd = (RoundDebater)obj;
				if(rd.isDummy && isDummy)
					// two dummies equal
					return true;
				else if(rd.isDummy || isDummy)
					// dummy never equals non-dummy debater
					return false;
			}
			// by default, let base class decide
			return base.Equals(obj);
		}
		
		public override int GetHashCode ()
		{
			if(isDummy)
				return isDummy.GetHashCode();
			else 
				return base.GetHashCode();
		}
		
	}
}


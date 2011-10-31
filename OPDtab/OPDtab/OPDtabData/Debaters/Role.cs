using System;
namespace OPDtabData
{
	[Serializable]
	public class Role : IComparable
	{
		int judgeQuality;
		string teamName;
		
		
		
		public Role ()
		{
			JudgeQuality = -1;
			TeamName = null;
		}
		
		public Role(string str) {
			Parse(str);
		}
			
		public bool IsJudge {
			get {
				return TeamName==null && JudgeQuality>=0;
			}
		}
		
		public bool IsTeamMember {
			get {
				return TeamName!=null;		
			}
		}
		
		/*public bool IsNothing {
			get {
				return TeamName==null && JudgeQuality<0;	
			}
		}*/
		
		public int JudgeQuality {
			get {
				return this.judgeQuality;
			}
			set {
				judgeQuality = value;
			}
		}

		public string TeamName {
			get {
				return this.teamName;
			}
			set {
				teamName = value;
			}
		}

		
		
		public override string ToString ()
		{
			if(TeamName!=null)
				return TeamName;
			else if(JudgeQuality>=0) 
				return "#"+JudgeQuality.ToString();
			else
				return "";
		}
		
		void Parse(string s) {
			s = s.Trim();
			char[] whitespace = new char[] {' '};
			if(s.StartsWith("#")) {
				// Judge
				TeamName = null;
				JudgeQuality = (int)uint.Parse(s.Substring(1));
			}
			else if(s.Length>0) {
				// TeamMember
				JudgeQuality = -1;
				TeamName = String.Join(" ",s.Split(whitespace,StringSplitOptions.RemoveEmptyEntries));		
			}
			else {
				// No Role
				TeamName = null;
				JudgeQuality = -1;
			}
		}
		
		#region IComparable implementation
		int IComparable.CompareTo (object obj)
		{
			return obj.ToString().CompareTo(this.ToString());
		}
		#endregion		
	}
}


using System;
namespace OPDtabData
{
	[Serializable]
	public class Role : IComparable
	{
		int judgeQuality = -1;
		string teamName;

		public Role()
		{
		}

		public Role(int judgeQuality)
		{
			this.judgeQuality = judgeQuality;
		}

		public Role(string teamName)
		{
			this.teamName = teamName;
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

		public int JudgeQuality {
			get {
				return judgeQuality;
			}
			set {
				judgeQuality = value;
			}
		}

		public string TeamName {
			get {
				return teamName;
			}
			set {
				teamName = value;
			}
		}

		public override string ToString ()
		{
			if (TeamName!=null) {
				return TeamName;
			}

			if (JudgeQuality >= 0) {
				return "#" + JudgeQuality.ToString();
			}

			return "";
		}
		
		public static Role Parse(string s) {
			s = s.Trim();

			if (s.StartsWith("#", StringComparison.Ordinal)) {
				return new Role((int)uint.Parse(s.Substring(1)));
				// FIXME unsafe: uint.Parse can fail
			}

			if(s.Length>0) {
				// TeamMember
				return new Role(
					string.Join(" ", s.Split(new char [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
				);
			}

			return new Role();
		}
		
		#region IComparable implementation
		int IComparable.CompareTo (object obj)
		{
			return obj.ToString().CompareTo(this.ToString());
		}
		#endregion		
	}
}


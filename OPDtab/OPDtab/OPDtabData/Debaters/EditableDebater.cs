using System;
namespace OPDtabData
{
	public class EditableDebater : Debater
	{
		
		public EditableDebater () : base()
		{			
		}	
		
		public EditableDebater(Debater d) : base(d) 
		{
		}
		
		public void ParseBlackList(string s) {
			BlackList.ParsePattern(s);
		}
		
		public void ParseWhiteList(string s) {
			WhiteList.ParsePattern(s);	
		}
				
		public void ParseAge(string s) {
			Age = uint.Parse(s);
		}
		
		public void ParseName(string s) {
			Name = new Name(s);
		}
		
		public void ParseClub(string s) {
			Club = new Club(s);
		}	
		
		public void ParseRole(string s) {
			Role = new Role(s);	
		}
		
		public void ParseExtraInfo(string s) {
			ExtraInfo = s.Trim();	
		}
		
		public string RoleCompletion {
			get {
				return Role.ToString();
			}
		}
		
		public string ClubCompletion {
			get {
				return Club.ToString();		
			}
		}
		
		
		
		public override Club Club {
			get {
				return base.Club;
			}
			set {
				try {
					BlackList.removePattern(base.Club.Name);
				} 
				catch {
				}
				try {
					BlackList.addPattern(value.Name);
				}
				catch {
				}
				base.Club = value;
			}
		}
		
		
		
	}
}


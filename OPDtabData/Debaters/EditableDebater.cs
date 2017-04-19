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
				BlackList.RemoveClub(base.Club.Name);
				BlackList.AddClub(value.Name);
				base.Club = value;
			}
		}
		
		
		
	}
}


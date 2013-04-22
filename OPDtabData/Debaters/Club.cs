using System;
namespace OPDtabData
{
	[Serializable]
	public class Club : IComparable
	{
		string name;
		string city;
		
		public Club () {
		}
			
		public Club(string str) {
			string[] s = Parse(str);
			Name = s[0];
			City = s[1];
		}
		
		public string City {
			get {
				return this.city;
			}
			set {
				city = value;
			}
		}

		public string Name {
			get {
				return this.name;
			}
			set {
				name = value;
			}
		}
		
		public override bool Equals (object obj)
		{
			if(obj is Club) {
				Club c = (Club)obj;
				return c.Name.Equals(Name);
			}
			else if(obj is AbstractDebater) {
				AbstractDebater d = (AbstractDebater)obj;
				return d.Club.Equals(this);
			}
			else
				throw new NotImplementedException();
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode();
		}
		
		public override string ToString ()
		{
			if(Name!=null && City!=null)
				return string.Format ("{0}, {1}", Name, City);
			else if(Name != null)
				return Name;
			else
				return "";
		}
		
		public static string[] Parse(string s) {
			if(s == null)
				throw new ArgumentNullException();
			string[] parts = s.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length>2) {
				throw new FormatException("Club contains more than one comma");
			}
			else if(parts.Length==2) {
				string cN = MiscHelpers.SanitizeString(parts[0]); 
				string cC = MiscHelpers.SanitizeString(parts[1]); 
				if(cN=="" || cC=="")
					throw new FormatException("No ClubName or ClubCity found");
				return new string[] {cN, cC};
			}
			else if(parts.Length==1) {
				string cN = MiscHelpers.SanitizeString(parts[0]);
				if(cN=="")
					throw new FormatException("No ClubName found");
				return new string[] {cN, null};
			}
			else 
				throw new FormatException("Club not parsable");	
		}
		
		public bool IsEmpty {
			get {
				return Name == null;	
			}
		}
		
		public void	SetEmpty() {
			Name = null;
			City = null;
		}
	
		
		#region IComparable implementation
		public int CompareTo (object obj)
		{
			return this.ToString().CompareTo(obj.ToString());
		}
		#endregion
}
}


using System;
namespace OPDtabData
{
	[Serializable]
	public class Name : IComparable {
		string firstName;
		string lastName;
		
		public Name() {			
		}
		
		
		
		public Name(string str) {
			string[] s = Parse(str);
			FirstName = s[0];
			LastName = s[1];
		}
		
		public Name(string fN, string lN) {
			firstName = fN;
			lastName = lN;
		}
		
		public override bool Equals (object obj)
		{
			if(obj is Name) {
				Name n = (Name)obj;
				return n.FirstName.Equals(FirstName) &&
					n.LastName.Equals(LastName);
			}
			else if(obj is AbstractDebater) {
				AbstractDebater d = (AbstractDebater)obj;
				return d.Name.Equals(this);
			}
			else
				throw new NotImplementedException();
		}

		public override int GetHashCode ()
		{
			return FirstName.GetHashCode()+LastName.GetHashCode();
		}
		
		public override string ToString ()
		{
			if(FirstName!=null && LastName!=null)
				return string.Format ("{0}, {1}", LastName, FirstName);
			else
				return "";
		}
		
		public bool IsEmpty {
			get {
				return FirstName==null || LastName==null;
			}
		}
		
		public void SetEmpty() {
			FirstName = null;
			LastName = null;
		}
		
		
		public string FirstName {
			get {
				return this.firstName;
			}
			set {
				firstName = value;
			}
		}

		public string LastName {
			get {
				return this.lastName;
			}
			set {
				lastName = value;
			}
		}
		
		public static string[] Parse(string s) {
			// small parser for names
			if(s == null)
				throw new ArgumentNullException();
			string[] parts = s.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
			if(parts.Length>2) {
				throw new FormatException("Name contains more than one comma");
			}
			else if(parts.Length==2) {
				string lN = MiscHelpers.SanitizeString(parts[0]);
				string fN = MiscHelpers.SanitizeString(parts[1]);
				if(fN=="" || lN=="")
					throw new FormatException("No LastName or FirstName found");
				return new string[] {fN, lN};
			}
			else if(parts.Length==1) {
				string[] n = MiscHelpers.StringToWords(parts[0]); 
				if(n.Length!=2)
					throw new FormatException("No split in 'FirstName LastName' possible, too many whitespaces");
				return n;
			}
			else 
				throw new FormatException("Name not parsable");	
		}

		#region IComparable implementation
		public int CompareTo (object obj)
		{
			return this.ToString().CompareTo(obj.ToString());
		}
		#endregion
}
}


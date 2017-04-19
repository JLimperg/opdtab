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

		public Club(string name) {
			this.name = name;
		}

		public Club(string name, string city) {
			this.name = name;
			this.city = city;
		}
		
		public string City {
			get {
				return city;
			}
			set {
				city = value;
			}
		}

		public string Name {
			get {
				return name;
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

		public enum ParseError
		{
			Null,
			MoreThanOneComma,
			EmptyClubComponent,
			Empty,
			Unspecified
		}

		public static string ParseErrorDescription(ParseError e)
		{
			switch (e) {
				case ParseError.MoreThanOneComma:
					return "Club contains more than one comma";
				case ParseError.EmptyClubComponent:
					return "Club name or city is empty.";
				case ParseError.Empty:
					return "Club cannot be empty.";
				case ParseError.Unspecified:
					return "Unable to parse club.";
				default:
					throw new Exception("impossible");
			}
		}
		
		public static Either<ParseError, Club> Parse(string s) {
			if (s == null) {
				throw new ArgumentNullException();
			}

			var parts = s.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length>2) {
				return Either<ParseError, Club>.Left(ParseError.MoreThanOneComma);
			}

			if (parts.Length==2) {
				var cN = MiscHelpers.SanitizeString(parts[0]);
				var cC = MiscHelpers.SanitizeString(parts[1]);

				if(cN=="" || cC=="") {
					return Either<ParseError, Club>.Left(ParseError.EmptyClubComponent);
				}

				return Either<ParseError, Club>.Right(new Club(cN, cC));
			}

			if (parts.Length==1) {
				var cN = MiscHelpers.SanitizeString(parts[0]);

				if(cN=="") {
					return Either<ParseError, Club>.Left(ParseError.Empty);
				}

				return Either<ParseError, Club>.Right(new Club(cN));
			}

			return Either<ParseError, Club>.Left(ParseError.Unspecified);
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


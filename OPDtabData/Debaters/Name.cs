using System;
namespace OPDtabData
{
	[Serializable]
	public class Name : IComparable {
		string firstName;
		string lastName;
		
		public Name() {
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

		public enum ParseError
		{
			MoreThanOneComma,
			EmptyNameComponent,
			TooMuchWhitespace,
			Unspecified
		}

		public static string ParseErrorDescription(ParseError err)
		{
			switch (err) {
			case ParseError.MoreThanOneComma:
				return "Name contains more than one comma.";
			case ParseError.EmptyNameComponent:
				return "Either first name or last name is empty.";
			case ParseError.TooMuchWhitespace:
				return "Can't split into a pattern of the form 'FirstName LastName'.";
			case ParseError.Unspecified:
				return "Unable to parse name.";
			default:
				throw new Exception("impossible");
			}
		}
		
		public static Either<ParseError, Name> Parse(string s) {
			if (s == null) {
				throw new ArgumentNullException();
			}


			var parts = s.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length > 2) {
				return Either<ParseError, Name>.Left(ParseError.MoreThanOneComma);
			}

			if (parts.Length==2) {
				var lN = MiscHelpers.SanitizeString(parts[0]);
				var fN = MiscHelpers.SanitizeString(parts[1]);

				if (fN == "" || lN == "") {
					return Either<ParseError, Name>.Left(ParseError.EmptyNameComponent);
				}

				return Either<ParseError, Name>.Right(new Name(fN, lN));
			}

			if (parts.Length==1) {
				var n = MiscHelpers.StringToWords(parts[0]);

				if (n.Length != 2) {
					return Either<ParseError, Name>.Left(ParseError.TooMuchWhitespace);
				}

				return Either<ParseError, Name>.Right(new Name(n[0], n[1]));
			}

			return Either<ParseError, Name>.Left(ParseError.Unspecified);
		}

		#region IComparable implementation
		public int CompareTo (object obj)
		{
			return this.ToString().CompareTo(obj.ToString());
		}
		#endregion
}
}


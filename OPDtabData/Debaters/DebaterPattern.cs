using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace OPDtabData
{
	
	
	[Serializable]
	public class DebaterPattern : IComparable
	{
		List<Name> debaters = new List<Name>();
		List<string> clubNames = new List<string>();
		
		public DebaterPattern ()
		{
		}

		public DebaterPattern(List<Name> debaters, List<string> clubNames)
		{
			this.debaters = debaters;
			this.clubNames = clubNames;
		}
	
		public bool Matches(AbstractDebater debater) {
			foreach(string c in clubNames) {
				if(debater.Club.Name.Equals(c))
					return true;
			}
			foreach(Name n in debaters) {
				if(n.Equals(debater))
					return true;
			}
			return false;
		}
		
		public void AddClub(string cN) {
			if(!clubNames.Contains(cN)) {
				clubNames.Add(cN);
			}
		}
		
		public void RemoveClub(string cN) {
			if(clubNames.Contains(cN)) {
				clubNames.Remove(cN);
			}
		}

		public void AddDebater(Name n) {
			if (!debaters.Contains(n)) {
				debaters.Add(n);
			}
		}
		
		public void RemoveDebater(Name n) {
			if(debaters.Contains(n)) {
				debaters.Remove(n);
			}
		}
		
		public static DebaterPattern Parse(string s) {
			if (s==null) {
				throw new ArgumentNullException();
			}


			var parts = s.Split(';');
			var ret = new DebaterPattern();

			foreach (string p in parts) {
				var tmp = p.Trim();
				if (tmp.StartsWith("@", StringComparison.Ordinal)) {
					Club.Parse(tmp.Substring(1)).Do(
						club => ret.AddClub(club.Name));
				}
				else if (tmp != "") {
					Name.Parse(tmp).Do(ret.AddDebater);
				}
				// TODO The above swallows errors if Club.Parse() or
				// Name.Parse() fail.
			}

			return ret;
		}
		
		public List<string> ClubNames {
			get {
				return this.clubNames;
			}
			set {
				clubNames = value;
			}
		}

		public List<Name> Debaters {
			get {
				return this.debaters;
			}
			set {
				debaters = value;
			}
		}

		public override string ToString() 
		{
			var tmpItems = new List<string>();
			
			foreach(string c in clubNames) 
				tmpItems.Add("@"+c);
			foreach(Name n in debaters) 
				tmpItems.Add(n.ToString());			
			return string.Join("; ",tmpItems.ToArray());
		}
	

		#region IComparable implementation
		int IComparable.CompareTo (object obj)
		{
			return obj.ToString().CompareTo(this.ToString());
		}
		#endregion
}
}


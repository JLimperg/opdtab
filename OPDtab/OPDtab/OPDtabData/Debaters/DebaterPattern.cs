using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace OPDtabData
{
	
	
	[Serializable]
	public class DebaterPattern : IComparable
	{
		List<Name> debaters;
		List<string> clubNames;
		
		public DebaterPattern ()
		{
			debaters = new List<Name>();
			clubNames = new List<string>();
		}
	
		public bool matches(AbstractDebater debater) {
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
		
		public void addPattern(string cN) {
			if(!clubNames.Contains(cN))
				clubNames.Add(cN);
			else
				throw new Exception("Pattern already contains ClubName");
		}
		
		public void removePattern(string cN) {
			if(clubNames.Contains(cN)) 
				clubNames.Remove(cN);
			else
				throw new Exception("Pattern does not contain ClubName");
		}
		
		public void ParsePattern(string s) {
			if(s==null)
				throw new ArgumentNullException();
			string[] parts = s.Split(';');
			List<string> tmpClubNames = new List<string>();
			List<Name> tmpDebaters = new List<Name>();
			
			foreach(string p in parts) {
				string tmp = p.Trim();
				if(tmp.StartsWith("@")) {
					string club = tmp.Substring(1); //); //.Trim();
					// club parser sanitizes...
					string[] c = Club.Parse(club);
					tmpClubNames.Add(c[0]);
				}
				else if(tmp != "") {
					string[] n = Name.Parse(tmp);
					tmpDebaters.Add(new Name(n[0],n[1]));
				}
			}
			
			clubNames = tmpClubNames;
			debaters = tmpDebaters;
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
			List<string> tmpItems = new List<string>();
			
			foreach(string c in clubNames) 
				tmpItems.Add("@"+c);
			foreach(Name n in debaters) 
				tmpItems.Add(n.ToString());			
			return String.Join("; ",tmpItems.ToArray());
		}
	

		#region IComparable implementation
		int IComparable.CompareTo (object obj)
		{
			return obj.ToString().CompareTo(this.ToString());
		}
		#endregion
}
}


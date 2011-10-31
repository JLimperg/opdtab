using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
namespace OPDtabData
{
		
	[Serializable]
	public abstract class AbstractDebater : IComparable, IMySearchable 
	{			
		Name name;
		uint age;
		Club club;
		Role role;
		
		public AbstractDebater() {
			name = new Name();
			club = new Club();
			role = new Role();
		}
		
		public AbstractDebater(AbstractDebater d) {
			// make a nice copy, but be lazy and use Parse methods...
			name = new Name(d.Name.ToString());
			age = d.Age;
			club = new Club(d.Club.ToString());
			role = new Role(d.Role.ToString());
		}
		
		public bool IsEmpty {
			get {
				return Name.IsEmpty && Club.IsEmpty;
			}
		}
		
		public void SetEmpty() {
			Name.SetEmpty();
			Club.SetEmpty();	
		}
		
		public uint Age {
			get {
					return this.age;
			}
			set {
				age = value;
			}
		}
				
		public virtual Club Club {
			get {
				return club;
			}			
			set {
				club = value;
			}
		}
			
		public Name Name {
			get {
				return name;
			}
			set {
				name = value;		
			}
		}
		
		public Role Role {
			get {
				return this.role;
			}
			set {
				role = value;
			}
		}			
		
		public override string ToString ()
		{
			// ToString is used for unique ID in EqualPointsResolver
			// and also in RankingData and ShowRanking
			return string.Format ("DEBATER: {1}, {0} ({2}) {3}", Name.FirstName,  Name.LastName, Club, Role);
		}
		
		public override bool Equals (object obj)
		{
			if(obj is AbstractDebater) {
				AbstractDebater d = (AbstractDebater)obj;
				return d.Name.Equals(Name) &&
						d.Club.Equals(Club);
			}
			else if(obj == null)
				return false;
			else 
				throw new NotImplementedException("Don't know how to equal "+obj);
			
		}
		
		public override int GetHashCode ()
		{
			return Name.GetHashCode()+
				Club.GetHashCode();	
		}


		#region IComparable implementation
		public int CompareTo (object obj)
		{
			if(obj is AbstractDebater) {
				AbstractDebater d = (AbstractDebater)obj;
				int res = Name.CompareTo(d.Name);
				if(res == 0)
					return Club.CompareTo(d.Club);
				else
					return res;
			}
			else
				return 0;
		}
		#endregion

		#region IMySearchable implementation
		public bool MatchesSearchString (string key)
		{
			return Name.ToString().ToLower().Contains(key.ToLower()) 
				|| Club.ToString().ToLower().Contains(key.ToLower())
				|| Role.ToString().ToLower().Contains(key.ToLower());
		}
		#endregion
		
		
}
}


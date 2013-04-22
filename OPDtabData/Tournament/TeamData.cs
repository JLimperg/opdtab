using System;
using System.Collections.Generic;
//using System.Xml.Serialization;

namespace OPDtabData
{
	[Serializable]
	public class TeamData : IRoomMember, IList<RoundDebater>, 
		IComparable, ICollection<RoundDebater>
	{
		List<RoundDebater> teamMembers;
		
		public TeamData() {
			teamMembers = new List<RoundDebater>();	
		}
		
		public TeamData(RoundDebater d) : this() {
			AddMember(d);
		}
		
		public TeamData(TeamData td, bool isShown) : this() {
			foreach(RoundDebater rd in td) {				
				AddMember(new RoundDebater(rd, isShown));
			}
		}
		
		
		
		public string TeamName {
			get {
				if(!IsEmpty)
					return this.teamMembers[0].Role.TeamName;
				else
					return "Empty Team";
			}
		}
		
		public void Sort() {
			teamMembers.Sort();
		}
			
		public bool IsEmpty {
			get {
				return teamMembers.Count==0;
			}
		}
		
		public void SetEmpty() {
			teamMembers.Clear();
		}
	
		
		public int NTeamMembers {
			get {
				return teamMembers.Count;	
			}
		}
		
		public void AddMember(RoundDebater d) {
			if(IsEmpty || 
			   (d.Role.IsTeamMember && d.Role.TeamName == TeamName)) {
				teamMembers.Add(d);
				if(NTeamMembers>3)
					Console.WriteLine("WARNING: Team "+this+" has more than 3 Members.");	
			}
			else
				throw new ArgumentException("Could not add '"+
				                            d.Name+"' to Team '"+
				                            this+"' with "+NTeamMembers+" Members.");		
		}
		
		public void Add(RoundDebater o) {
			AddMember((RoundDebater)o);
		}
		
		public int ShownMembers {
			get {
				int n = 0;
				foreach(RoundDebater d in teamMembers)
					if(d.IsShown)
						n++;
				return n;
			}
		}
		
		public override bool Equals (object obj)
		{
			if(obj is TeamData) 
				return ((TeamData)obj).TeamName.Equals(TeamName);	
			else if(obj is string)
				return ((string)obj).Equals(TeamName);
			/*else if(obj is RoundDebater) {
				foreach(RoundDebater d in teamMembers)
					if(d.Equals(obj))
						return true;
				return false;	
			}*/
			else
				throw new NotImplementedException();
		}
		
		public override int GetHashCode ()
		{
			int sum = 0;
			foreach(RoundDebater d in teamMembers)
				sum += d.GetHashCode();
			return sum;
			//return TeamName.GetHashCode();
		}
		
		public override string ToString ()
		{			
			// ToString is used for unique ID in EqualPointsResolver
			// and also in RankingData and ShowRanking
			return "TEAM: "+TeamName;
		}
		
		public RoundDebater this[int index] {
			get {
				if(index<teamMembers.Count)
					return teamMembers[index];
				else
					return null;
			}
			set {
				teamMembers[index] = value;
			}
		}
		
		#region IComparable implementation
		int IComparable.CompareTo (object obj)
		{
			if(obj is TeamData) {
				TeamData td = (TeamData)obj;
				return TeamName.ToString().CompareTo(td.TeamName.ToString());
			}
			else
				return 0;
		}
		#endregion		
		
		#region IRoomMember implementation
		void IRoomMember.SetRoom (string roundName, RoomData rd)
		{
			foreach(IRoomMember d in teamMembers)
				d.SetRoom(roundName, rd);
		}		
		#endregion
		
		#region IEnumerable[T] implementation
		IEnumerator<RoundDebater> IEnumerable<RoundDebater>.GetEnumerator ()
		{
			return teamMembers.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return teamMembers.GetEnumerator();
		}	
		#endregion
		
		#region IList[RoundDebater] implementation
		public int IndexOf (RoundDebater item)
		{
			throw new NotImplementedException ();
		}

		public void Insert (int index, RoundDebater item)
		{
			throw new NotImplementedException ();
		}

		public void RemoveAt (int index)
		{
			throw new NotImplementedException ();
		}

		
		#endregion	
		
		#region ICollection[RoundDebater] implementation
		public void Clear ()
		{
			throw new NotImplementedException ();
		}

		public bool Contains (RoundDebater item)
		{
			return teamMembers.Contains(item);
		}

		public void CopyTo (RoundDebater[] array, int arrayIndex)
		{
			teamMembers.CopyTo(array, arrayIndex);
		}

		public  bool Remove (RoundDebater item)
		{
			throw new NotImplementedException ();
		}

		public int Count {
			get {
				return NTeamMembers;	
			}
		}

		public bool IsReadOnly {
			get {
				throw new NotImplementedException ();
			}
		}
		#endregion		
	}
}


using System;
using System.Linq;
using System.Collections.Generic;

namespace OPDtabData
{
	public class RoomConflict
	{
		public enum Type {
			JudgeVsTeam=0, 
			JudgeVsFree=1,
			JudgeVsJudge=2,
			TeamVsTeam=3, 
			TeamVsFree=4, 			
			FreeVsFree=5,
			ChairVsAny=6
		}
		
		
		RoundDebater owner;
		Dictionary<Type, List<RoundDebater>> partners1;
		
		Dictionary<string, Complex> partners2;
		
				
		public RoomConflict (RoundDebater o)
		{
			owner = o;
			partners1 = new Dictionary<Type, List<RoundDebater>>(); 
			foreach(Type t in Enum.GetValues(typeof(Type))) 
				partners1[t] = new List<RoundDebater>();				
			partners2 = new Dictionary<string, Complex>();
			// Don't init partners2, we reflect that by access methods..
			// we don't know the current number of rooms, 
			// Tournament.I.Rounds might be invalid (but I.Debaters is valid!)
			
		}
		
		// a sloppy & expensive method for the Team Widget displaying all TeamMembers conflicts...
		public void Merge(RoomConflict rc) {
			if(rc==null || rc.IsEmpty)
				return;
			foreach(Type t in Enum.GetValues(typeof(Type))) 
				Partners1[t] = new List<RoundDebater>
					(Partners1[t].Union(rc.Partners1[t]));	
			
			// Partners2 is a bit more difficult
			// we can't rely on nothing yo man!					
			foreach(string key in rc.Partners2.Keys) {
				if(!Partners2.ContainsKey(key)) {
					// we don't have it...
					// just add it to ours...
					Partners2[key] = rc.Partners2[key];
				}
				else if(rc.Partners2.ContainsKey(key)) {
					// we have it and they have it, so
					// union the Complex
					Partners2[key].Merge(rc.Partners2[key]);						                                    
				}
			}
		}
		
		
		public int GetConflictPenalty() {
			int[] penalties = AppSettings.I.GenerateRound.conflictPenalties;
			int sum = 0;
			// partners1
			for(int i=0;i<partners1.Count;i++)
				sum += penalties[i]*partners1[(Type)i].Count;
			// partners2
			foreach(string key in partners2.Keys) {
				sum += partners2[key].GetConflictPenalty();	
			}
			return sum;
		}
		
		public RoundDebater Owner {
			get {
				return this.owner;
			}
			set {
				owner = value;
			}
		}

		public Dictionary<Type, List<RoundDebater>> Partners1 {
			get {
				return this.partners1;
			}
			set {
				partners1 = value;
			}
		}
		
		public Dictionary<string, Complex> Partners2 {
			get {
				return this.partners2;
			}
			set {
				partners2 = value;
			}
		}
		
		public Complex GetPartners2(string key) {
			Complex c = null;
			Partners2.TryGetValue(key, out c);
			if(c==null) {
				c = new Complex();
				Partners2[key] = c;
			}
			return c;
		}
		
		public bool IsEmpty {
			get {
				foreach(Type t in partners1.Keys) 	
					if(partners1[t].Count != 0)
						return false;
				foreach(Complex c in partners2.Values)
					if(!c.IsEmpty)
						return false;
				return true;
			}
		}
		
		public class Complex {
			// uh yeah, generics at its best >>>>> :)
			Dictionary<string, List<RoundDebater>> store;
			public Complex() {
				store = new Dictionary<string, List<RoundDebater>>();	
			}
			
			public List<RoundDebater> GetList(string room) {
				List<RoundDebater> list;
				store.TryGetValue(room, out list);
				if(list == null) {
					list = new List<RoundDebater>();
					store[room] = list;
				}
				return list;
					
			}
			
			public int GetConflictPenalty() {
				int[] penalties = AppSettings.I.GenerateRound.conflictPenalties;
				int sum = 0;
				foreach(string key in store.Keys)
					sum += store[key].Count*penalties[penalties.Length-1];
				return sum;
			}
			
			public void Merge(Complex c) {
				// the roomIndex of this and c may not be the same
				foreach(string key in c.Store.Keys) {
					if(!store.ContainsKey(key)) {
						// we don't have it...
						// just add the new list to our store
						store[key] = c.Store[key];
					}
					else if(c.Store.ContainsKey(key)) {
						// we have it and they have it, so
						// union the both RoundDebaters lists
						store[key] = new List<RoundDebater>
							(c.store[key].Union(store[key]));						                                    
					}
				}
			}
			
			public bool IsEmpty {
				get {
					foreach(List<RoundDebater> l in store.Values)
						if(l.Count!=0)
							return false;
					return true;
				}
			}
			
			public Dictionary<string, List<RoundDebater>> Store {
				get {
					return store;
				}
			}
			
			
		}
		

	}
}


using System;
using System.Collections.Generic;

namespace OPDtabData
{
	public class EqualPointsResolver
	{
		// unique id
		string id;
		// is the position in the ranking, 
		// to be used in the sorting of the ranking (if points are equal)
		int resolution;
		
		public EqualPointsResolver ()
		{
			id = null;
			resolution = -1;
		}
		
		public EqualPointsResolver(string id, int resolution) {
			this.id = id;
			this.resolution = resolution;
		}
		
		// these methods are used for generating 
		// unique Ids to identify the EqualPoints "conflicts"
		// and to resolve them...
		// since the ranking and points are always recalculated, 
		// we need this rather clumsy rationale...
		
		public static string IdRanking(SortedDictionary<string, bool> availRounds, 
			RankingDataItem item) {
			string str = "";
			foreach(string key in availRounds.Keys) {
				if(availRounds[key]) 
					str += key+"|";
			}
			return str+"|"+item.Data;
		}		
		
		public static string IdRanking(List<KeyValuePair<string, bool>> availRounds,
			RankingDataItem item) {
			return IdRanking(RankingData.MakeDic(availRounds), item);	
		}
				
		// getters and setters
		public string Id {
			get {
				return this.id;
			}
			set {
				id = value;
			}
		}

		public int Resolution {
			get {
				return this.resolution;
			}
			set {
				resolution = value;
			}
		}
		
		// standard stuff
		public override bool Equals (object obj)
		{
			if(obj is EqualPointsResolver) {
				return Id.Equals((obj as EqualPointsResolver).Id);	
			}
			else if (obj is string) {
				return Id.Equals(obj as string);	
			}
			else
				throw new NotImplementedException();
		}
		
		public override int GetHashCode ()
		{
			return Id.GetHashCode();
		}
	}
}


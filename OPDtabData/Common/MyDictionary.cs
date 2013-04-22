using System;
using System.Collections.Generic;
namespace OPDtabData
{
	
	public class KVP<K, V> {
		K key;
		V val;
		public KVP() {}
	
		public KVP(K k, V v) {
			key = k;
			val = v;
		}
		
		public K Key {
			get {
				return this.key;
			}
			set {
				key = value;
			}
		}
		public V Val {
			get {
				return this.val;
			}
			set {
				val = value;
			}
		}
	}
	
	
	public class MyDictionary<K, V>
	{
			
		
		List<KVP<K,V>> store;
		
		public MyDictionary ()
		{
			store = new List<KVP<K,V>>();
		}
		
		public MyDictionary(MyDictionary<K, V> mydic) {
			store = new List<KVP<K, V>>(mydic.Store);	
		}
		
		public V this[K key] {
			get {
				 KVP<K, V> item = store.Find(delegate(KVP<K, V> kvp) {
					return kvp.Key.Equals(key);
				});
				if(item != null)
					return item.Val;
				else
					return default(V);
			}
			set {
				int i = store.FindIndex(delegate(KVP<K, V> kvp) {
					return kvp.Key.Equals(key);
				});
				if(i<0)
					store.Add(new KVP<K, V>(key, value));
				else
					store[i] = new KVP<K, V>(key, value);					
			}
		}
		
		public bool Remove(K key) {
			int i = GetStoreIndex(key);
			if(i<0)
				return false;
			else {
				store.RemoveAt(i);
				return true;
			}
		}
		
		int GetStoreIndex(K key) {
			return store.FindIndex(delegate(KVP<K, V> kvp) {
				return kvp.Key.Equals(key);
			});
		}
			
		
		public bool GetKVP(K key, out KVP<K, V> kvp) {
			int i = GetStoreIndex(key);
			if(i<0) {
				kvp = default(KVP<K, V>);
				return false;
			}
			else {
				kvp = store[i];
				return true;
			}
		}
		
		public List<KVP<K, V>> Store {
			get {
				return this.store;
			}
			set {
				store = value;
			}
		}		
		
	}
}


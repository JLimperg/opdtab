/**
 * @file	DoubleLinkedList.cs
 * 
 * @author	Agemo Cui
 * @date	2004-08-15
 * @email	elcaro@citiz.net
 * @msn		elcaro@citiz.net
 */

using System;
using System.Collections;

namespace OPDtabData
{
	public interface IIterator
	{
		IIterator IterNext {get;}
		IIterator IterPrev {get;}
		object Value {get;}
	}
	/// <summary>
	/// Summary description for DoubleLinkedList.
	/// </summary>
	public sealed class DoubleLinkedList
	{
		public DoubleLinkedList()
		{
			head = new Node();
		}

		public IIterator IterBegin
		{
			get
			{
				return (IIterator)head.Next;
			}
		}

		public IIterator IterEnd
		{
			get
			{
				return (IIterator)head;
			}
		}

		public bool AddFirst(IIterator iter)
		{
			if(iter is Node)
			{
				Insert(head, (Node)iter);
				return true;
			}

			return false;
		}

		public IIterator AddFirst(object value)
		{
			return Insert(head, value);
		}

		public IIterator AddLast(object value)
		{
			return Insert(head.Prev, value);
		}

		public void RemoveFirst()
		{
			if(IsEmpty)
				return;
			Remove(head.Next);
		}

		public void RemoveLast()
		{
			if(IsEmpty)
				return;
			Remove(head.Prev);
		}

		public IIterator Insert(IIterator prev, object value)
		{
			if(prev is Node)
			{
				Node node = new Node(value);
				Insert((Node)prev, node);
				return (IIterator)node;
			}

			return null;
		}

		private void Insert(Node prev, Node newNode)
		{
			newNode.Next = prev.Next;
			newNode.Prev = prev;
			prev.Next = newNode;
			newNode.Next.Prev = newNode;
			count++;
		}

		public bool Remove(IIterator iter)
		{
			if(IsEmpty)
				return false;

			if(iter is Node)
			{
				Node node = (Node)iter;
				node.Prev.Next = node.Next;
				node.Next.Prev = node.Prev;
				node.Prev = node.Next = null;
				count--;
				return true;
			}

			return false;
		}

		public bool IsEmpty
		{
			get
			{
				return count == 0;
			}
		}

		public void Clear()
		{
			head.Init();
			count = 0;
		}

		public int Count
		{
			get
			{
				return count;
			}
		}

		private sealed class Node : IIterator
		{
			public Node()
			{
				Init();
			}

			public Node(object val)
			{
				this.val = val;
				prev = next = this;
			}

			public Node(object val, Node prev, Node next)
			{
				this.val = val;
				this.prev = prev;
				this.next = next;
			}

			public void Init()
			{
				val = null;
				prev = next = this;
			}

			public IIterator IterPrev
			{
				get
				{
					return (IIterator)prev;
				}
			}

			public IIterator IterNext
			{
				get
				{
					return (IIterator)next;
				}
			}

			public object Value
			{
				get
				{
					return val;
				}
				set
				{
					val = value;
				}
			}

			public Node Prev
			{
				get
				{
					return prev;
				}
				set
				{
					prev = value;
				}
			}

			public Node Next
			{
				get
				{
					return next;
				}
				set
				{
					next = value;
				}
			}

			private Node prev;
			private Node next;
			private object val;
		}

		private Node head = null;
		private int count = 0;
	}
}

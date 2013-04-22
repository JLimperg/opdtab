/**
 * @file	TemplatePool.cs
 * 
 * @author	Agemo Cui
 * @date	2004-08-10
 * @email	elcaro@citiz.net
 * @msn		elcaro@citiz.net
 */

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace OPDtabData
{
	/// <summary>
	/// Summary description for TemplatePool.
	/// </summary>
	public sealed class TemplatePool : ITmplLoader
	{
		private TemplatePool(string tmplDir, int capacity)
		{
			list = new DoubleLinkedList();
			TemplatePool.tmplDir = tmplDir;
			TemplatePool.capacity = capacity;
		}

		private TemplatePool()
		{
			list = new DoubleLinkedList();
		}

		public static TemplatePool Singleton()
		{
			if(pool == null)
			{
				lock(objLock)
				{
					if(pool == null)
						pool = new TemplatePool();
				}
			}

			return pool;
		}

		public static TemplatePool Singleton(string tmplDir, int capacity)
		{
			if(pool == null)
			{
				lock(objLock)
				{
					if(pool == null)
						pool = new TemplatePool(tmplDir, capacity);
				}
			}

			return pool;
		}

		public StringBuilder LoadTmpl(string tmplFileName)
		{
			StreamReader reader = null;
			StringBuilder tmplBufBuilder = null;

			try
			{
				reader = new StreamReader(tmplDir + tmplFileName);

				int len = 4096;
				int n = 0;
				char[] buf = new char[len];

				tmplBufBuilder = new StringBuilder();

				while((n = reader.Read(buf, 0, len)) > 0)
				{
					tmplBufBuilder.Append(new string(buf, 0, n));
				}
			}
			finally
			{
				if(reader != null)
					reader.Close();
			}

			return tmplBufBuilder;
		}

		public ITemplate GetTemplate(string tmplFileName)
		{
			TmplParser tmpl = null;
			// do not use cached templates
			//if((tmpl = SearchInPool(tmplFileName, true)) != null)
			//	return (ITemplate)tmpl.Clone();

			tmpl = new TmplParser(tmplFileName);
			if(tmpl.Initialize(this) == false)
				return null;

			Put2Pool(tmpl);

			return (ITemplate)tmpl.Clone();
		}

		public void Release()
		{
			lock(this)
			{
				list.Clear();
			}
		}

		private TmplParser SearchInPool(string tmplName, bool adjust)
		{
			TmplParser tmpl = null;

			IIterator iter = list.IterBegin;
			for(; iter != list.IterEnd; iter = iter.IterNext)
			{
				tmpl = (TmplParser)iter.Value;
				if(tmpl.IsName(tmplName))
				{
					if(adjust)
					{
						lock(this)
						{
							list.Remove(iter);
							list.AddFirst(iter);
						}
					}
					return tmpl;
				}
			}

			return null;
		}

		private void Put2Pool(TmplParser tmpl)
		{
			lock(this)
			{
				if(SearchInPool(tmpl.TmplName, false) == null)
				{
					if(list.Count == capacity)
						list.RemoveLast();
					list.AddFirst(tmpl);
				}
			}
		}

		private DoubleLinkedList list = null;
		private static int capacity = 50;
		private static string tmplDir = "";
		private static TemplatePool pool = null;
		private static Object objLock = new Object();
	}
}

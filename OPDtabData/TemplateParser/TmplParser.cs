using System;
using System.Collections;
using System.IO;
using System.Text;

namespace OPDtabData
{
	public interface ITmplLoader
	{
		StringBuilder LoadTmpl(string tmplFileName);
	}

	public interface ITmplBlock
	{
		string Name {get;}
		void Assign(string label, string value);
		void Out();
		string BlockString {get;}
	}

	public interface ITemplate
	{
		ITmplBlock ParseBlock();
		ITmplBlock ParseBlock(string blkName);
	}

	public sealed class TmplParser : ITemplate
	{
		public TmplParser()
		{
		}

		public TmplParser(string targetString, string startPrefix, string startSuffix, string endPrefix, string endSuffix)
		{
			TargetString = targetString;
			this.startPrefix = startPrefix;
			this.startSuffix = startSuffix;
			this.endPrefix = endPrefix;
			this.endSuffix = endSuffix;
		}

		public TmplParser(string tmplFileName)
		{
			this.tmplFileName = tmplFileName;
		}

		public bool Initialize(ITmplLoader loader)
		{
			if(loader != null)
				tmplBuf = loader.LoadTmpl(tmplFileName);
			else
				tmplBuf = null;

			return Build();
		}

		public bool Build()
		{
			if(tmplBuf == null)
				return false;
			
			//depth first
			Stack stack = new Stack();

			int index = 0;
			string name = "";
			BlockParser block = new BlockParser();

			block.Name = name;
			
			if(blockVec == null)
				blockVec = new ArrayList();
			else
				blockVec.Clear();

			index = blockVec.Add(block);
			BlockBound blkBnd = new BlockBound(name, 0, index);
			
			stack.Push(blkBnd);

			int fromIndex = 0;
			string tag = null;
			TagBound tagBegin = null;
			TagBound tagEnd = null;
			
			while(stack.Count > 0)
			{
				while( (tagEnd = HasSubBlock(blkBnd)) != null )
				{
					tagBegin = StartIndexOf(blkBnd.Begin);

					if( tagBegin.IsGreat(tagEnd)
					|| (name = GetStartName(tagBegin)).Length == 0)
					{
						blockVec.Clear();
						return false;
					}

					fromIndex = TrimIndexRightFrom(tagBegin.PrefixIndex);
					Trim(fromIndex, tagBegin.SuffixIndex + startSuffix.Length);
					block = new BlockParser();
					block.Name = name;
					index = blockVec.Add(block);
					blkBnd = new BlockBound(name, fromIndex, index);
					stack.Push(blkBnd); 
				}
			
				stack.Pop();

				block = (BlockParser)blockVec[blkBnd.Index];
				
				if(stack.Count == 0)
				{
					block.TmplBlock = tmplBuf.ToString();
					block.Parent = null;
				}
				else
				{
					block.TmplBlock = tmplBuf.ToString().Substring(blkBnd.Begin, blkBnd.End - blkBnd.Begin);
					tag = new StringBuilder().Append(tagPrefix).Append(blkBnd.Name).Append(tagSuffix).ToString();
					tmplBuf.Remove(blkBnd.Begin, blkBnd.End - blkBnd.Begin);
					tmplBuf.Insert(blkBnd.Begin, tag);
					fromIndex = blkBnd.Begin + tag.Length;
					blkBnd = (BlockBound)stack.Peek();
					block.Parent = (BlockParser)blockVec[blkBnd.Index];
					blkBnd.End = fromIndex;
				}
			}
		
			tmplBuf = null;
			return true;
		}

		public bool IsName(string tmplName)
		{
			return tmplFileName.Equals(tmplName);
		}

		public ITmplBlock BlockAt(int index)
		{
			return index >= blockVec.Count || index < 0 ? null : (ITmplBlock)blockVec[index];
		}

		private BlockParser ItemAt(int index)
		{
			return index >= blockVec.Count || index < 0 ? null : (BlockParser)blockVec[index];
		}

		private int GetIndex(Object elem)
		{
			for(int index = 0; index < blockVec.Count; index++)
			{
				if(blockVec[index] == elem)
					return index;
			}

			return -1;
		}

		public int Count
		{
			get
			{
				return blockVec.Count;
			}
		}
		
		public ITmplBlock ParseBlock(string blkName)
		{
			if(blockVec == null || blockVec.Count == 0 || blkName == null || blkName.Length == 0)
				return null;
			
			//Console.WriteLine("not easily null");
			
			BlockParser blk = null;
			for(int index = 0; index < blockVec.Count; index++)
			{
				//Console.WriteLine(index+": "+((BlockParser)blockVec[index]).Name);
				if(((BlockParser)blockVec[index]).IsName(blkName))
				{
					blk = (BlockParser)blockVec[index];
					blk.Init();
					break;
				}
			}

			return (ITmplBlock)blk;
		}
		
		public TmplParser Clone()
		{
			TmplParser tmplParser = new TmplParser();
			tmplParser.EndPrefix = endPrefix;
			tmplParser.EndSuffix = endSuffix;
			tmplParser.StartPrefix = startPrefix;
			tmplParser.StartSuffix = startSuffix;
			tmplParser.tmplBuf = tmplBuf;
			tmplParser.tmplFileName = tmplFileName;

			if(blockVec != null && blockVec.Count > 0)
			{
				tmplParser.blockVec = new ArrayList(blockVec.Count);
				for(int index = 0; index < blockVec.Count; index++)
				{
					tmplParser.blockVec.Add(((BlockParser)blockVec[index]).Clone());
					((BlockParser)tmplParser.blockVec[index]).Parent = 
						tmplParser.ItemAt(GetIndex(((BlockParser)blockVec[index]).Parent));
				}
			}
            
			return tmplParser;
		}
		
		public ITmplBlock ParseBlock()
		{
			if(blockVec == null || blockVec.Count == 0)
				return null;

			((BlockParser)blockVec[0]).Init();

			return (ITmplBlock)blockVec[0];		
		}

		public string TargetString
		{
			set
			{
				tmplBuf = new StringBuilder(value);
			}
		}

		public string TmplName
		{
			get
			{
				return tmplFileName;
			}
			set
			{
				tmplFileName = value;
			}
		}

		public string StartPrefix
		{
			get
			{
				return startPrefix;
			}
			set
			{
				startPrefix = value;
			}
		}

		public string StartSuffix
		{
			get
			{
				return startSuffix;
			}
			set
			{
				startSuffix = value;
			}
		}

		public string EndPrefix
		{
			get
			{
				return endPrefix;
			}
			set
			{
				endPrefix = value;
			}
		}

		public string EndSuffix
		{
			get
			{
				return endSuffix;
			}
			set
			{
				endSuffix = value;
			}
		}

		private int TrimIndexRightFrom(int right)
		{
			for(; right > 0 && Char.IsWhiteSpace(tmplBuf.ToString(), right - 1); right--);
			return right;
		}
		
		private void Trim(int start, int end)
		{
			if(start < end)
				tmplBuf.Remove(start, end - start);
		}
		
		private TagBound HasSubBlock(BlockBound blkBnd)
		{
			TagBound tagEnd = EndIndexOf(blkBnd.End);
			
			if(tagEnd == null)
			{
				blkBnd.End = tmplBuf.Length;
				return null;
			}
			
			if(GetEndName(tagEnd).Equals(blkBnd.Name))
			{
				int start = TrimIndexRightFrom(tagEnd.PrefixIndex);			
				blkBnd.End = start;
				Trim(start, tagEnd.SuffixIndex + endSuffix.Length);			
				return null;
			}

			return tagEnd;
		}
		
		private string GetStartName(TagBound startTag)
		{
			return GetTagName(startTag, startPrefix);
		}
		
		private string GetEndName(TagBound endTag)
		{
			return GetTagName(endTag, endPrefix);
		}
		
		private string GetTagName(TagBound tagBnd, string prefix)
		{
			return tmplBuf.ToString().Substring(tagBnd.PrefixIndex + prefix.Length,
				tagBnd.SuffixIndex - tagBnd.PrefixIndex - prefix.Length).Trim();
		}
		
		private TagBound StartIndexOf(int fromIndex)
		{
			return IndexOf(fromIndex, startPrefix, startSuffix);
		}
		
		private TagBound EndIndexOf(int fromIndex)
		{
			return IndexOf(fromIndex, endPrefix, endSuffix);
		}
		
		private TagBound IndexOf(int fromIndex, string prefix, string suffix)
		{
			string tpb = tmplBuf.ToString();
			int index = tpb.IndexOf(prefix, fromIndex);
			if(index < 0)
				return null;
			
			TagBound tagBnd = new TagBound(index, 0);
			
			if((index = tpb.IndexOf(suffix, tagBnd.PrefixIndex)) < 0)
				return null;

			tagBnd.SuffixIndex = index;
			
			return tagBnd;
		}

		private sealed class BlockParser : ITmplBlock
		{
			public BlockParser()
			{
			}
			
			public BlockParser(string name)
			{
				this.name = name;
			}
	
			public BlockParser(string name, string tmplBlock, BlockParser parent)
			{
				this.name = name;
				this.tmplBlock = tmplBlock;
				this.parent = parent;
			}

			public BlockParser(string name, string tmplBlock)
			{
				this.name = name;
				this.tmplBlock = tmplBlock;
			}

			public BlockParser Clone()
			{
				return new BlockParser(name, tmplBlock);
			}

			public void Init()
			{
				if(block == null)
					block = new StringBuilder(tmplBlock);
			}

			public void Assign(string label, string value)
			{
				if(label == null || value == null || block == null)
					return;

				string elemLabel = new StringBuilder(labelPrefix).Append(label).Append(labelSuffix).ToString();

				block.Replace(elemLabel, value);
			}
			
			public void Out()
			{
				RemoveAllTag();

				if(parent == null)
					return;

				string tag = new StringBuilder(tagPrefix).Append(name).Append(tagSuffix).ToString();

				parent.InsertBeforeTag(tag, BlockString);
				block = new StringBuilder(tmplBlock);
			}
			
			public void RemoveTag(string tagName)
			{
				if(tagName == null || block == null)
					return;
				
				string tag = new StringBuilder(tagPrefix).Append(tagName).Append(tagSuffix).ToString();
				int start = block.ToString().IndexOf(tag);
				if(start >= 0)
					block.Remove(start, tag.Length);
			}
			
			public void RemoveAllTag()
			{
				RemoveElement(tagPrefix, tagSuffix);
			}

			public void RemoveAllLabel()
			{
				RemoveElement(labelPrefix, labelSuffix);
			}
			
			public void TrimRight()
			{
				int end = block.Length;
				int start = end;
				for(; start > 0 && Char.IsWhiteSpace(block.ToString(), start - 1); start--);
				if(start < end)
					block.Remove(start, end - start);	
			}
			
			public bool IsName(string blkName)
			{
				return name == blkName;
			}

			public string LabelPrefix
			{
				get
				{
					return labelPrefix;
				}
			}

			public string LabelSuffix
			{
				get
				{
					return labelSuffix;
				}
			}

			public static string TagPrefix
			{
				get
				{
					return tagPrefix;
				}
			}

			public static string TagSuffix
			{
				get
				{
					return tagSuffix;
				}
			}

			public string BlockString
			{
				get
				{
					return block == null ? null : block.ToString();
				}
			}

			public string TmplBlock
			{
				set
				{
					tmplBlock = value;
				}
			}

			public BlockParser Parent
			{
				get
				{
					return parent;
				}
				set
				{
					parent = value;
				}
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			private void RemoveElement(string prefix, string suffix)
			{
				if(block == null)
					return;

				int start = 0;
				int end = 0;
				string blk = block.ToString();
				
				while( (start = blk.IndexOf(prefix, start)) >= 0
					&& (end = blk.IndexOf(suffix, start + prefix.Length)) >= 0 )
				{
					block.Remove(start, end + suffix.Length - start);
					blk = block.ToString();
				}
			}

			private void InsertBeforeTag(string tag, string value)
			{
				if(tag == null || value == null)
					return;

				if(tmplBlock == null || tmplBlock.Length == 0)
				{
					block = new StringBuilder(value);
					return;
				}

				if(block == null)
					block = new StringBuilder(tmplBlock);

				int offset = block.ToString().IndexOf(tag);
				if(offset >= 0)
					block.Insert(offset, value);		
			}

			private static readonly string labelPrefix = "[";
			private static readonly string labelSuffix = "]";
			private static readonly string tagPrefix = "<tag:";
			private static readonly string tagSuffix = "/>";

			private string name = null;
			private StringBuilder block = null;
			private string tmplBlock = null;
			private BlockParser parent = null;
		}

		private sealed class TagBound
		{
			public TagBound()
			{
			}

			public TagBound(int prefixIndex, int suffixIndex)
			{
				this.prefixIndex = prefixIndex;
				this.suffixIndex = suffixIndex;
			}

			public int PrefixIndex
			{
				get
				{
					return prefixIndex;
				}
				set
				{
					prefixIndex = value;
				}
			}

			public int SuffixIndex
			{
				get
				{
					return suffixIndex;
				}
				set
				{
					suffixIndex = value;
				}
			}

			public bool IsGreat(TagBound tagBnd)
			{
				return prefixIndex > tagBnd.SuffixIndex;
			}

			private int prefixIndex = 0;
			private int suffixIndex = 0;
		}

		private sealed class BlockBound
		{
			public BlockBound()
			{
			}

			public BlockBound(string name, int begin, int index)
			{
				this.name = name;
				this.begin = begin;
				this.end = this.begin;
				this.index = index;
			}

			public BlockBound(string name, int begin, int end, int index)
			{
				this.name = name;
				this.begin = begin;
				this.end = end;
				this.index = index;
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					if(value != null)
						name = value;
				}
			}

			public int Begin
			{
				get
				{
					return begin;
				}
				set
				{
					if(value <= end && value >= 0)
						begin = value;
				}
			}

			public int End
			{
				get
				{
					return end;
				}
				set
				{
					if(value >= begin)
						end = value;
				}
			}

			public int Index
			{
				get
				{
					return index;
				}
				set
				{
					index = value;
				}
			}

			private string name = null;
			private int begin = 0;
			private int end = 0;
			private int index = 0;
		}

		private string tmplFileName = null;
		private StringBuilder tmplBuf = null;
		private ArrayList blockVec = null;
		private static readonly string tagPrefix = "<tag:";
		private static readonly string tagSuffix = "/>";

		private string startPrefix = "<!-BEGIN:";
		private string startSuffix = "->";
		private string endPrefix = "<!-END:";
		private string endSuffix = "->";
	}
}
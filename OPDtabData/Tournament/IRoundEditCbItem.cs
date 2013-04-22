using System;
namespace OPDtabData
{
	public interface IRoundEditCbItem
	{
		bool ItemCompleted {
			get;
		}
		string CbText {
			get;	
		}
	}
}


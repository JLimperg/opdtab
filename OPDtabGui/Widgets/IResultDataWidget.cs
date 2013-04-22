using System;
using OPDtabData;
namespace OPDtabGui
{
	public interface IResultDataWidget
	{
		// used in EditRoundResults!
		bool HasResult { get; }
		void UpdateInfo();
	}
}


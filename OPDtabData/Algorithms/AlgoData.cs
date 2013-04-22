using System;
namespace OPDtabData
{
	public delegate void AlgoDataUpdateHandler(double percentage, string infoMarkup);
	public delegate void AlgoDataFinishedHandler(object data);
		
	public class AlgoData
	{
		
		public event AlgoDataUpdateHandler OnUpdate;
		public event AlgoDataFinishedHandler OnFinished;
				
		public void Update(double percentage, string infoMarkup) {          
			if(OnUpdate!=null)
				Gtk.Application.Invoke(delegate {
             		OnUpdate(percentage, infoMarkup);
                });
		}
		
		public void Finished(object data) {
			if(OnFinished!=null)
				Gtk.Application.Invoke(delegate {
					OnFinished(data);
				});
		}		
	}
}


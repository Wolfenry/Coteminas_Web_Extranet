using System;
namespace Coteminas_Web_Extranet.Models
{

	[Serializable]
	public class LogData
	{


		public int Id
		 { get; set; }
		public string Item
		 { get; set; }
		public string Descr
		 { get; set; }
		public DateTime CR
		 { get; set; }
		public string CR_User
		 { get; set; }
		public DateTime TS
		 { get; set; }
		public string TS_User
		 { get; set; }

	}
}
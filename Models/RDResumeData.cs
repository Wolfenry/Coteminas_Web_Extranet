using System;
namespace Coteminas_Web_Extranet.Models
{

	[Serializable]
	public class RDResumeData
	{

		public int Id
		 { get; set; }
		public string RECEIPTKEY
		 { get; set; }
		public string EXTERNRECEIPTKEY
		 { get; set; }
		public string SKU
		 { get; set; }
		public long QTYEXPECTED
		 { get; set; }
		public long QTYRECEIVED
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
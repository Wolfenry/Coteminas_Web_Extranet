using System;
namespace Coteminas_Web_Extranet.Models
{

	[Serializable]
	public class SkuStockData
	{

		public bool _det { get; set; } //not mapped
		public string SKU
		 { get; set; }
		public string DESCR
		 { get; set; }
		public int STOCK
		 { get; set; }
		public int RESERVADO
		 { get; set; }
		public int DISPONIBLE
		 { get; set; }

	}
}
using System;
namespace Coteminas_Web_Extranet.Models
{

	[Serializable]
	public class ProfilesData
	{


		public int Id
		 { get; set; }
		public string IdLog
		 { get; set; }
		public string Nombre
		 { get; set; }
		public string Clave
		 { get; set; }
		public string Representante
		 { get; set; }
		public string LibCom
		 { get; set; }
		public string LibLog
		 { get; set; }
		public DateTime CR
		 { get; set; }
		public string CR_Usuario
		 { get; set; }
		public DateTime TS
		 { get; set; }
		public string TS_Usuario
		 { get; set; }

	}
}
using System;

namespace Coteminas_Web_Extranet.Models
{
    [Serializable]
    public class OrderDetailData
    {
        public string SKU { get; set; }
        public string DESCR { get; set; }
        public int BLT { get; set; }
        public int UNI { get; set; }
        public decimal VOL { get; set; }
        public int QTY { get; set; }
        public string _qty { get; set; }
    }
}
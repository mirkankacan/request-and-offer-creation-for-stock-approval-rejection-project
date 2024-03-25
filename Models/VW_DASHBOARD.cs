using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace U_Project.Models
{
    public class VW_DASHBOARD
    {
        public int BEKLEMEDE_TALEP { get; set; }
        public int BEKLEMEDE_TEKLIF { get; set; }
        public int BEKLEMEDE_SIPARIS { get; set; }
        public int ONAYLANAN_TALEP { get; set; }
        public int ONAYLANAN_TEKLIF { get; set; }
        public int TAMAMLANAN_SIPARIS { get; set; }
    }
}
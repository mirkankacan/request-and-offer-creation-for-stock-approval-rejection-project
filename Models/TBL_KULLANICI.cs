//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace U_Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_KULLANICI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_KULLANICI()
        {
            this.TBL_STOK_HAREKET_LOG = new HashSet<TBL_STOK_HAREKET_LOG>();
            this.TBL_STOK_HAREKET2 = new HashSet<TBL_STOK_HAREKET2>();
            this.TBL_TALEP = new HashSet<TBL_TALEP>();
            this.TBL_TALEP1 = new HashSet<TBL_TALEP>();
            this.TBL_TALEP2 = new HashSet<TBL_TALEP>();
            this.TBL_TEKLIF = new HashSet<TBL_TEKLIF>();
            this.TBL_TEKLIF1 = new HashSet<TBL_TEKLIF>();
            this.TBL_TEKLIF2 = new HashSet<TBL_TEKLIF>();
        }
    
        public int ID { get; set; }
        public string KULLANICI_ADI { get; set; }
        public string SIFRE { get; set; }
        public string ISIM_SOYISIM { get; set; }
        public int ROL_ID { get; set; }
        public int SUBE_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_STOK_HAREKET_LOG> TBL_STOK_HAREKET_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_STOK_HAREKET2> TBL_STOK_HAREKET2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TALEP> TBL_TALEP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TALEP> TBL_TALEP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TALEP> TBL_TALEP2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TEKLIF> TBL_TEKLIF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TEKLIF> TBL_TEKLIF1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_TEKLIF> TBL_TEKLIF2 { get; set; }
        public virtual TBL_SUBE TBL_SUBE { get; set; }
    }
}

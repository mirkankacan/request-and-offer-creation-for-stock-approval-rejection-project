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
    
    public partial class TBL_TALEP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_TALEP()
        {
            this.TBL_SIPARIS = new HashSet<TBL_SIPARIS>();
        }
    
        public int ID { get; set; }
        public string TALEP_NO { get; set; }
        public int TALEP_MIKTAR { get; set; }
        public int TALEP_DEPARTMAN_ID { get; set; }
        public string TALEP_NOT { get; set; }
        public string PLAKA_BIRIM { get; set; }
        public System.DateTime OLUSTURULMA_TARIH { get; set; }
        public int OLUSTURAN_ID { get; set; }
        public string STOK_KODU { get; set; }
        public int SUBE_ID { get; set; }
        public string TALEP_ISLEM_ACIKLAMA { get; set; }
        public Nullable<bool> ONAY1 { get; set; }
        public Nullable<bool> ONAY2 { get; set; }
        public Nullable<bool> ONAY3 { get; set; }
        public Nullable<System.DateTime> ONAY1_TARIH { get; set; }
        public Nullable<System.DateTime> ONAY2_TARIH { get; set; }
        public Nullable<System.DateTime> ONAY3_TARIH { get; set; }
        public Nullable<int> ONAY1_KULLANICI { get; set; }
        public Nullable<int> ONAY2_KULLANICI { get; set; }
        public Nullable<int> ONAY3_KULLANICI { get; set; }
        public Nullable<int> TALEP_DURUM_ID { get; set; }
    
        public virtual TBL_DEPARTMAN TBL_DEPARTMAN { get; set; }
        public virtual TBL_DURUM TBL_DURUM { get; set; }
        public virtual TBL_KULLANICI TBL_KULLANICI { get; set; }
        public virtual TBL_KULLANICI TBL_KULLANICI1 { get; set; }
        public virtual TBL_KULLANICI TBL_KULLANICI2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_SIPARIS> TBL_SIPARIS { get; set; }
        public virtual TBL_STOK TBL_STOK { get; set; }
        public virtual TBL_SUBE TBL_SUBE { get; set; }
    }
}

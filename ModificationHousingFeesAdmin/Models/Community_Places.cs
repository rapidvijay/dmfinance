//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModificationHousingFeesAdmin.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Community_Places
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Community_Places()
        {
            this.Area_CommunitiesPlaces = new HashSet<Area_CommunitiesPlaces>();
            this.Request_Modify_DM_Housing_Fees = new HashSet<Request_Modify_DM_Housing_Fees>();
        }
    
        public int Id { get; set; }
        public string Arabic_Name { get; set; }
        public string English_Name { get; set; }
        public string Created_By { get; set; }
        public System.DateTime Creation_Date { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
        public string Updated_By { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Area_CommunitiesPlaces> Area_CommunitiesPlaces { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request_Modify_DM_Housing_Fees> Request_Modify_DM_Housing_Fees { get; set; }
    }
}

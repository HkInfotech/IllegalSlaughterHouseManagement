//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SSCWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CityLows
    {
        public long Id { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> LowId { get; set; }
    
        public virtual Citys Citys { get; set; }
        public virtual Lows Lows { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VideoQuiz.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QZQuestion
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int SectionID { get; set; }
        public Nullable<int> SIO_ID { get; set; }
        public string Question { get; set; }
        public string Objective { get; set; }
        public string MediaLink { get; set; }
        public string Link { get; set; }
        public bool ChoiceRandomDisabled { get; set; }
        public string Feedback { get; set; }
        public int Sequence { get; set; }
        public double Score { get; set; }
        public bool IsRemark { get; set; }
        public bool OptRationale { get; set; }
        public int ImgRationale { get; set; }
    
        public virtual QZSection QZSection { get; set; }
    }
}

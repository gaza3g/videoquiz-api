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
    
    public partial class QZResult
    {
        public int ID { get; set; }
        public int QuizID { get; set; }
        public System.Guid PUID { get; set; }
        public int Attempt { get; set; }
        public double TotalScore { get; set; }
        public Nullable<System.DateTime> Begintime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<double> MaxScore { get; set; }
    
        public virtual QZQuiz QZQuiz { get; set; }
    }
}

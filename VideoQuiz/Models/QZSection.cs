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
    
    public partial class QZSection
    {
        public QZSection()
        {
            this.QZQuestion = new HashSet<QZQuestion>();
        }
    
        public int ID { get; set; }
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Medialink { get; set; }
        public int Sequence { get; set; }
    
        public virtual ICollection<QZQuestion> QZQuestion { get; set; }
        public virtual QZQuiz QZQuiz { get; set; }
    }
}

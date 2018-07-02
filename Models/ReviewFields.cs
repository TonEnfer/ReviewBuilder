using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class ReviewFields
    {
        public int Id { get; set; }
        public string Discipline { get; set; }
        public string Theme { get; set; }
        public string StudentName { get; set; }
        public string StudentGroup { get; set; }
        public string ChiefName { get; set; }
       
        public List<Evaluation> EvaluatonsSet { get; set; }
    }

     public class Evaluation
        {
            public int Id { get; set; }
            public bool Low, Medium, High;
        }
}
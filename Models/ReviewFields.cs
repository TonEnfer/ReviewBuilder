using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class ReviewFields
    {
        //public int Id { get; set; }
        public string Discipline { get; set; }
        public string Theme { get; set; }
        public string StudentName { get; set; }
        public string StudentGroup { get; set; }
        public string ChiefName { get; set; }
        public uint Evaluation { get; set; }
        public List<Evaluations> EvaluatonsSet { get; set; }
        public static readonly int EvaluationsCount = 11;
    }

    public class Evaluations
    {
        public bool Low, Medium, High;
    }
}
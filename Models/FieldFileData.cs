using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class FieldFileData : FileData
    {
        public List<ReviewFields> reviewFields { get; set; } = new List<ReviewFields>();
        
    }


}
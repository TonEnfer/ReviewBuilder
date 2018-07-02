using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class FieldFileModel : FileModel
    {
        public List<ReviewFields> reviewFields { get; set; } = new List<ReviewFields>();
        
    }


}
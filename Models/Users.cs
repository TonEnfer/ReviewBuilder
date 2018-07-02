using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class User
    {
        public int Id { get; set; }
        public List<FieldFileData> fieldFiles { get; set; } = new List<FieldFileData>();
        public List<FileData> buildFiles { get; set; } = new List<FileData>();
        public bool builded { get; set; }
        public DateTime dt { get; set; }

    }
}
using System;
using System.Collections.Generic;

namespace ReviewBuilder.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public List<FieldFileModel> fieldFiles { get; set; } = new List<FieldFileModel>();
        public List<FileModel> buildFiles { get; set; } = new List<FileModel>();
        public bool builded { get; set; }
        public DateTime dt { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ReviewBuilder.Models
{
    public class UserData
    {
        //[Key]

        public List<ReviewFields> reviewFields { get; set; }
        public MemoryStream inputFile { get; set; }
        public MemoryStream outputFile { get; set; }
        public bool isReady { get; set; }
        public bool isFailed { get; set; }
        public DateTime uploadFile { get; set; }
        public DateTime downloadedTime { get; set; }

        public UserData()
        {
            isReady = false;
            isFailed = false;
            inputFile = new MemoryStream();
            outputFile = new MemoryStream();
            // downloadedTime = new DateTime();
        }


    }
}
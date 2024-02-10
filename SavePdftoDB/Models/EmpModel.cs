using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SavePdftoDB.Models
{
    public class EmpModel
    {
        [Required]
        [DataType(DataType.Upload)]
        [Display(Name = ".")]
        public HttpPostedFileBase files { get; set; }
    }

    public class FileDetailsModel
    {
        public int Id { get; set; }
        [Display(Name = "Uploaded Files")]
        public String FileName { get; set; }
        public byte[] FileContent { get; set; }
        public String ContentType { get; set; }
        public int ContentLength { get; set; }
    }
}
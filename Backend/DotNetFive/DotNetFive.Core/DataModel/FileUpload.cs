using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetFive.Core.DataModel
{
    public class FileUpload
    {
        [Key]
        public Guid FileId { get; set; }
        [MaxLength(50)]
        public string MasterIdf { get; set; }
        [MaxLength(50)]
        public string Module { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public int Size { get; set; }
        [MaxLength(10)]
        public string Type { get; set; }
        [MaxLength(100)]
        public string OriginalName { get; set; }
        [MaxLength(300)]
        public string OtherDetails { get; set; }
        [MaxLength(20)]
        public string CreatedBy { get; set; }
        [MaxLength(20)]
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

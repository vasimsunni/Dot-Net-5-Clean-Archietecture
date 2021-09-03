using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetFive.Core.DataModel
{
    public class Administrator
    {
        [Key]
        public Guid AdminId { get; set; }
        [MaxLength(50)]
        public string IdentityUserIdf { get; set; }
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(20)]
        public string Mobile { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(20)]
        public string CreatedBy { get; set; }
        [MaxLength(20)]
        public string UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}

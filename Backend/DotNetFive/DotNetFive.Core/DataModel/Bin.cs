using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetFive.Core.DataModel
{
    public class Bin
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string EntityId { get; set; }
        [MaxLength(50)]
        public string Entity { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string DeletedByIdentityId { get; set; }
        public DateTime DeletedOn { get; set; }
    }
}

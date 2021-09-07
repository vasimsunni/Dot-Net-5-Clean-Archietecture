using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotNetFive.Infrastructure.DTO.Request
{
    public class Administrator
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public string Password { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(20)]
        public string Mobile { get; set; }
        public string[] Roles { get; set; }
        public string UploadedFileId { get; set; }
    }
}

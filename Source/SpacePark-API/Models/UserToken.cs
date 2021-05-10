using System;
using System.ComponentModel.DataAnnotations;

namespace SpacePark_API.Models
{
    public record UserToken
    {
        [Key]
        public string Token { get; set; }
        public Account Account { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
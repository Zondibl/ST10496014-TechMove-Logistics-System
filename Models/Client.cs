using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace EAPD7111wPOE_Part1.Models
{
    public class Client
    {
        [Key]
        public int ClientID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string ContactDetails { get; set; }

        [Required]
        [StringLength(50)]
        public string Region { get; set; }

        // Navigation Property
        public ICollection<Contract> Contracts { get; set; }
    }

}

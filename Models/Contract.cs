using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAPD7111wPOE_Part1.Models
{

    public enum ContractStatus
    {
        Draft,
        Active,
        UnderReview,
        OnHold,
        Expired,
        Cancelled,
        Rejected
    }

    [Table("Contracts")]
    public class Contract
    {
        [Key]
        public int ContractID { get; set; }

        [Required]
        public int ClientID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string ServiceLevel { get; set; }

        // =====================================
        // FILE STORAGE
        // =====================================

        public string? ContractDocumentPath { get; set; }

        // =====================================
        // NOT MAPPED FILE
        // =====================================

        [NotMapped]
        public IFormFile? ContractDocument { get; set; }
    }
}

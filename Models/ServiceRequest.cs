using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAPD7111wPOE_Part1.Models
{

    public enum RequestStatus
    {
        Draft,
        Active,
        UnderReview,
        OnHold,
        Expired,
        Cancelled,
        Rejected
    }

    public class ServiceRequest
    {

        [Key]
        public int ServRequestID { get; set; }

        [Required]
        public int ContractID { get; set; }

        [Required(ErrorMessage =
            "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage =
            "Cost is required.")]
        [Range(0.01, double.MaxValue,
            ErrorMessage =
            "Cost must be greater than zero.")]
        public decimal Cost { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";

        [NotMapped]
        public virtual Contract Contract { get; set; }

        [NotMapped]
        public string Currency { get; set; }

    }
}

using Microsoft.AspNetCore.Mvc;

namespace EAPD7111wPOE_Part1.Models
{
    public static class WorkflowStatuses
    {
        // Contract Statuses
        public const string Active = "Active";
        public const string OnHold = "On hold";
        public const string Expired = "Expired";

        // Service Request Statuses
        public const string Draft = "Draft";
        public const string UnderReview = "Under Review";
        public const string Cancelled = "Cancelled";
        public const string Rejected = "Rejected";
    }
}

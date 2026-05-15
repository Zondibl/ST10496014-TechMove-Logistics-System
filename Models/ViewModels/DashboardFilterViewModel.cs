using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using EAPD7111wPOE_Part1.Models;

namespace EAPD7111wPOE_Part1.Models.ViewModels
{

    public class DashboardFilterViewModel
    {
        // Dashboard Stats
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int OnHoldContracts { get; set; }
        public int ExpiredContracts { get; set; }
        public int DraftContracts { get; set; }
        public int CancelledContracts { get; set; }

        // Service request stats
        public int TotalRequests { get; set; }
        public int DraftRequests { get; set; }
        public int UnderReviewRequests { get; set; }
        public int ActiveRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CancelledRequests { get; set; }

        // Filter Inputs
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }

        public string Status { get; set; }

        // Results
        public List<Contract> FilteredContracts { get; set; }
            = new List<Contract>();
    }
}

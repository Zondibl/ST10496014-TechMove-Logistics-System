using EAPD7111wPOE_Part1.Data;
using EAPD7111wPOE_Part1.Models;
using EAPD7111wPOE_Part1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Http;

namespace EAPD7111wPOE_Part1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /*public IActionResult ServiceRequest()
        {
            return View();
        } */

        public IActionResult CurrencyConversion()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }



        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*public async Task<IActionResult> Dashboard()
        {
            var contracts = await _context.Contracts.ToListAsync();
            var requests = await _context.ServiceRequests.ToListAsync();

            var model = new DashboardViewModel
            {
                TotalContracts = contracts.Count,
                ActiveContracts = contracts.Count(c => c.Status == "Active"),
                OnHoldContracts = contracts.Count(c => c.Status == "On Hold"),
                ExpiredContracts = contracts.Count(c => c.Status == "Expired"),
                DraftContracts = contracts.Count(c => c.Status == "Draft"),
                CancelledContracts = contracts.Count(c => c.Status == "Cancelled"),

                TotalRequests = requests.Count,
                DraftRequests = requests.Count(r => r.Status == "Assigned"),
                UnderReviewRequests = requests.Count(r => r.Status == "Booked"),
                ActiveRequests = requests.Count(r => r.Status == "In Transit"),
                RejectedRequests = requests.Count(r => r.Status == "Delivered"),
                CancelledRequests = requests.Count(r => r.Status == "Cancelled")
            };

            return View("~/Views/Home/Dashboard.cshtml", model);
        } */

        public async Task<IActionResult> Dashboard(DateTime? startDateFrom,
                                                   DateTime? startDateTo,
                                                   string status)
        {
            // ==========================================
            // LOAD CONTRACTS + REQUESTS
            // ==========================================
            var contractsQuery = _context.Contracts.AsQueryable();

            var requests = await _context.ServiceRequests.ToListAsync();

            // ==========================================
            // LINQ FILTERING
            // ==========================================

            if (startDateFrom.HasValue)
            {
                contractsQuery = contractsQuery
                    .Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                contractsQuery = contractsQuery
                    .Where(c => c.EndDate <= startDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                contractsQuery = contractsQuery
                    .Where(c => c.Status == status);
            }

            // ==========================================
            // FILTERED RESULTS
            // ==========================================
            var filteredContracts = await contractsQuery
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            // ==========================================
            // DASHBOARD STATS
            // ==========================================
            var allContracts = await _context.Contracts.ToListAsync();

            var model = new DashboardFilterViewModel
            {
                // Dashboard statistics
                TotalContracts = allContracts.Count,
                ActiveContracts = allContracts.Count(c => c.Status == "Active"),
                OnHoldContracts = allContracts.Count(c => c.Status == "On Hold"),
                ExpiredContracts = allContracts.Count(c => c.Status == "Expired"),
                DraftContracts = allContracts.Count(c => c.Status == "Draft"),
                CancelledContracts = allContracts.Count(c => c.Status == "Cancelled"),

                TotalRequests = requests.Count,
                DraftRequests = requests.Count(r => r.Status == "Assigned"),
                UnderReviewRequests = requests.Count(r => r.Status == "Booked"),
                ActiveRequests = requests.Count(r => r.Status == "In Transit"),
                RejectedRequests = requests.Count(r => r.Status == "Delivered"),
                CancelledRequests = requests.Count(r => r.Status == "Cancelled"),

                // Filter values
                StartDateFrom = startDateFrom,
                StartDateTo = startDateTo,
                Status = status,

                // Results
                FilteredContracts = filteredContracts
            };

            return View("~/Views/Home/Dashboard.cshtml", model);
        }

        public async Task<IActionResult> ContractManagement()
        {
            var contracts = await _context.Contracts
                .OrderByDescending(c => c.ContractID)
                .ToListAsync();

            return View(
                "~/Views/Home/ContractManagement.cshtml",
                contracts
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContract(
                                                        EAPD7111wPOE_Part1.Models.Contract contract)
        {
            try
            {
                // =====================================
                // VALIDATION
                // =====================================

                if (contract.ClientID <= 0)
                {
                    ModelState.AddModelError("",
                        "Client ID is required.");
                }

                if (contract.StartDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("",
                        "Start Date is required.");
                }

                if (contract.EndDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("",
                        "End Date is required.");
                }

                if (string.IsNullOrEmpty(contract.Status))
                {
                    ModelState.AddModelError("",
                        "Status is required.");
                }

                if (string.IsNullOrEmpty(contract.ServiceLevel))
                {
                    ModelState.AddModelError("",
                        "Service Level is required.");
                }

                // =====================================
                // FILE UPLOAD
                // =====================================

                if (contract.ContractDocument != null)
                {
                    // =====================================
                    // VALIDATE PDF ONLY
                    // =====================================

                    var extension = Path.GetExtension(
                        contract.ContractDocument.FileName);

                    if (extension.ToLower() != ".pdf")
                    {
                        ModelState.AddModelError("",
                            "Only PDF documents are allowed.");
                    }
                    else
                    {
                        string uploadsFolder =
                            Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot/contracts"
                            );

                        // CREATE FOLDER

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // UNIQUE FILE NAME

                        string uniqueFileName =
                            Guid.NewGuid().ToString() +
                            "_" +
                            contract.ContractDocument.FileName;

                        string filePath =
                            Path.Combine(
                                uploadsFolder,
                                uniqueFileName
                            );

                        // SAVE FILE

                        using (var fileStream =
                               new FileStream(
                                   filePath,
                                   FileMode.Create))
                        {
                            await contract.ContractDocument
                                .CopyToAsync(fileStream);
                        }

                        contract.ContractDocumentPath =
                            "/contracts/" + uniqueFileName;
                    }
                }

                // =====================================
                // SAVE TO DATABASE
                // =====================================

                if (ModelState.IsValid)
                {
                    _context.Contracts.Add(contract);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Contract created successfully.";

                    return RedirectToAction(
                        nameof(ContractManagement));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            var contracts = await _context.Contracts
                .ToListAsync();

            return View(
                "~/Views/Home/ContractManagement.cshtml",
                contracts
            );
        }
    }


}

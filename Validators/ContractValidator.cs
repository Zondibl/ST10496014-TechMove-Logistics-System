using EAPD7111wPOE_Part1.Models;

namespace EAPD7111wPOE_Part1.Validators
{
    public class ContractValidator
    {
        public ValidationResultModel
            Validate(Contract contract)
        {
            var result =
                new ValidationResultModel();

            // =====================================
            // CLIENT ID
            // =====================================

            if (contract.ClientID <= 0)
            {
                result.Errors.Add(
                    "Client ID is required.");
            }

            // =====================================
            // START DATE
            // =====================================

            if (contract.StartDate ==
                DateTime.MinValue)
            {
                result.Errors.Add(
                    "Start Date is required.");
            }

            // =====================================
            // END DATE
            // =====================================

            if (contract.EndDate ==
                DateTime.MinValue)
            {
                result.Errors.Add(
                    "End Date is required.");
            }

            // =====================================
            // DATE RANGE
            // =====================================

            if (contract.EndDate <
                contract.StartDate)
            {
                result.Errors.Add(
                    "End Date cannot be before Start Date.");
            }

            // =====================================
            // STATUS
            // =====================================

            if (string.IsNullOrWhiteSpace(
                contract.Status))
            {
                result.Errors.Add(
                    "Status is required.");
            }

            // =====================================
            // SERVICE LEVEL
            // =====================================

            if (string.IsNullOrWhiteSpace(
                contract.ServiceLevel))
            {
                result.Errors.Add(
                    "Service Level is required.");
            }

            result.IsValid =
                !result.Errors.Any();

            return result;
        }
    }
}
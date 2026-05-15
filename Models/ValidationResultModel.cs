namespace EAPD7111wPOE_Part1.Models
{
    public class ValidationResultModel
    {
        public bool IsValid { get; set; }

        public List<string> Errors { get; set; }
            = new List<string>();
    }
}
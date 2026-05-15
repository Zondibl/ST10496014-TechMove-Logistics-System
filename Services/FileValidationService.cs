using System.IO;

namespace EAPD7111wPOE_Part1.Services
{
    public class FileValidationService
    {
        public bool IsPdfFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var extension =
                Path.GetExtension(fileName);

            return extension.Equals(
                ".pdf",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}

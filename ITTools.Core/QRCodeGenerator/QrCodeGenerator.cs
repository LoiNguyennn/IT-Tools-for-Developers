using ITTools.Core.Models;
using QRCoder;

namespace ImageVideoTools
{
    public class QrCodeGenerator : ITool
    {
        public string Name => "Qr Code Generator";
        public string Description => "Generates a Qr code from text (e.g., as Base64)";
        public string Category => "Image/Video Tools";

        public string Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input text cannot be empty.");
            }

            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q))
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to generate QR code: {ex.Message}", ex);
            }
        }
    }
}
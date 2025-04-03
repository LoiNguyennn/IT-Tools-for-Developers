using ITTools.Core.Models;
using QRCoder;

namespace ImageVideoTools
{
    public class QrCodeGenerator : ITool
    {
        public string Name => "QR Code Generator";
        public string Description => "Generates a QR code from text (e.g., as Base64)";
        public string Category => "Image/Video Tools";

        public string Execute(string input)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeBytes = qrCode.GetGraphic(20); // Get the PNG as a byte array
                return Convert.ToBase64String(qrCodeBytes); // Convert to Base64 string
            }
        }
    }
}
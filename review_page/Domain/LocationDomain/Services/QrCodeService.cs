using QRCoder;
using review_page.Domain.LocationDomain.Core.Interfaces;

namespace review_page.Domain.LocationDomain.Services
{
    public class QrCodeService : IQrCodeService
    {


        public byte[] GenerateQrCode(string url, int pixelsPerModule = 10)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var code = new PngByteQRCode(data);
            return code.GetGraphic(pixelsPerModule);
        }

        public string GenerateBase64(string url) =>
            Convert.ToBase64String(GenerateQrCode(url));
    }
}

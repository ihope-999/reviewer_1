namespace review_page.Domain.LocationDomain.Core.Interfaces
{
    public interface IQrCodeService
    {
        public byte[] GenerateQrCode(string url, int pixelsPerModule = 10);
    }
}

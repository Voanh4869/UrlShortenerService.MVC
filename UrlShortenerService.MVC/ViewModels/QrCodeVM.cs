namespace UrlShortenerService.MVC.ViewModels
{
    public class QrCodeVM
    {
        public string Url { get; set; } = null;
        public string? QrImageBase64 { get; set; }  // Ảnh Base64 để hiển thị
        public string? ErrorMessage { get; set; }   // Thông báo lỗi (nếu có)
        public string? SavedFilePath { get; set; }  // Đường dẫn file đã lưu (vd: /qrcodes/qr_20251106090000.png)
    }
}

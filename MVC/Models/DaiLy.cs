using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class DaiLy
{
    [Key]
    public string? MaDaiLy { get; set; }
    public string? TenDaiLy { get; set; }
    public string? DiaChi { get; set; }
    public string? NguoiDaiDien { get; set; }
    public string? DienThoai { get; set; }
    public string? MaHTPP { get; set; }

    // Liên kết navigation property
    public HeThongPhanPhoi? HeThongPhanPhoi { get; set; }
}

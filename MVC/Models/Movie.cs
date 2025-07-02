using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MvcMovie.Model;

public class Movie
{
    public int Id { get; set; }

    public string? Title { get; set; }
    [DataType(DataType.Date)]

    public DateTime ReleaseDate { get; set; }
    public string? Genre { get; set; }
    public decimal Price { get; set; }
}
public class Person
{
    public string? PersonId { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
}
public class Employee : Person
{
    public string? EmployeeId { get; set; }
    public int Age { get; set; }
}
public class HeThongPhanPhoi
{
    public string? MaHTPP { get; set; }
    public string? TenHTPP { get; set; }
}
public class DaiLy
{
    public string? MaDaiLy { get; set; }
    public string? TenDaiLy { get; set; }
    public string? DiaChi { get; set; }
    public string? NguoiDaiDien { get; set; }
    public string? DienThoai { get; set; }

    // Khóa ngoại
    public string? MaHTPP { get; set; }

    // Liên kết đến class HeThongPhanPhoi
    public HeThongPhanPhoi? HeThongPhanPhoi { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MvcMovie.Models;

[Table("Person")]
public class Person
{
    [Key]
    public string? PersonId { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]    
    public string? FullName { get; set; }
    [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]    
    public string? Address { get; set; }
}

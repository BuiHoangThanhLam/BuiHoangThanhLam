using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models.Entities
{
    public class Person
    {
        [Key]
        public string PersonId { get; set; } = default!;
        [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
        public string FullName { get; set; } = default!;
         [Required(ErrorMessage = "Vui lòng nhập đầy đủ thông tin")]
        public string? Address { get; set; } = default!;
    }
}
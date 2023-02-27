using System.ComponentModel.DataAnnotations;

namespace PTMK
{
    public class Human
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }
        
        [Required]
        public DateTime BirthDate { get; set; }
        
        [Required]
        public bool Sex { get; set; }
    }
}

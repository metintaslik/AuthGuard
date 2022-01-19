using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthGuard.API.Entities
{
    public class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(25, MinimumLength = 3)]
        public string FirstName { get; set; } = null!;

        [Required]
        [DataType(DataType.Text)]
        [StringLength(25, MinimumLength = 2)]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(0, 2)]
        public int Gender { get; set; }

        [Required]
        [Range(8, 80)]
        public int Age { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;

namespace Unece.Data.Models
{
    public class USR_Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDeleted { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Unece.Data.Models
{
    public class FieldTypeMaster
    {
        [Key]
        public int Id { get; set; }
        public string FieldTypeName { get; set; }

    }
}

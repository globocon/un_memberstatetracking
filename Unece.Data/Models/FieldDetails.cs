using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unece.Data.Models
{


    public class FieldDetails
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FieldTypeMasterId")]
        public int FieldTypeMasterId { get; set; }
        public string FieldName { get; set; }
        

    }
}

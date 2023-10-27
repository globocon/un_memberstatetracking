using System.ComponentModel.DataAnnotations;

namespace Unece.Data.Models
{
    public class ModuleTwo
    {
        [Key]
        public int Id { get; set; }
        public string FieldOne { get; set; }
        public string FieldTwo { get; set; }
    }
}

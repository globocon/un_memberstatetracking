using System.ComponentModel.DataAnnotations;

namespace Unece.Data.Models
{
    public class SiteMaster
    {
        [Key]
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
    }
}

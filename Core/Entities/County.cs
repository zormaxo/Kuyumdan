using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class County : BaseEntity
    {
        [Required]
        public string CountyName { get; set; }
        public City City { get; set; }
        public int CityId { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
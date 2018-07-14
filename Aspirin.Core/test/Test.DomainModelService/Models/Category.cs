using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using URF.Core.EF.Trackable;

namespace Test.DomainModelService.Models
{
    public partial class Category : Entity
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
    }
}

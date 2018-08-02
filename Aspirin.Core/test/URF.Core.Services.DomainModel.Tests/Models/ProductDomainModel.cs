using URF.Core.EF.Trackable;

namespace URF.Core.Services.DomainModel.Tests.Models
{
    public class ProductDomainModel : Entity
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public bool Discontinued { get; set; }
        public int CategoryId { get; set; }
        public CategoryDomainModel Category { get; set; }
    }
}
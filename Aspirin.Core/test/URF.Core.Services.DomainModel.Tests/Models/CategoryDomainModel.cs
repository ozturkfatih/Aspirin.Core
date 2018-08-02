using System.Collections.Generic;
using URF.Core.EF.Trackable;

namespace URF.Core.Services.DomainModel.Tests.Models
{
    public class CategoryDomainModel : Entity
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<ProductDomainModel> Products { get; set; }
    }
}
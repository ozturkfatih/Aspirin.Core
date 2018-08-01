using AutoMapper;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services.DomainModel.Tests.Models;

namespace URF.Core.Services.DomainModel.Tests.Services
{
    public class ProductDomainService : DomainService<ProductDomainModel,Product>, IProductDomainService
    {
        public ProductDomainService(ITrackableRepository<Product> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URF.Core.Abstractions.Trackable;
using URF.Core.Services.DomainModel.Tests.Models;

namespace URF.Core.Services.DomainModel.Tests.Services
{
    public class CustomerDomainService: DomainService<CustomerDomainModel, Customer>, ICustomerDomainService
    {
        public CustomerDomainService(ITrackableRepository<Customer> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        public async Task<IEnumerable<CustomerDomainModel>> CustomersByCompany(string companyName)
        {
           var data = await Repository.Queryable()
                .Where(x => x.CompanyName.Contains(companyName))
                .ToListAsync();
            return Mapper.Map<List<Customer>, List<CustomerDomainModel>>(data);
        }
    }
}
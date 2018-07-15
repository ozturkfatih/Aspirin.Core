using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.Abstractions.Services.DomainModel;
using URF.Core.Services.DomainModel.Tests.Models;

namespace URF.Core.Services.DomainModel.Tests.Services
{
    public interface ICustomerDomainService : IDomainService<CustomerDomainModel>
    {
        Task<IEnumerable<CustomerDomainModel>> CustomersByCompany(string companyName);
    }
}
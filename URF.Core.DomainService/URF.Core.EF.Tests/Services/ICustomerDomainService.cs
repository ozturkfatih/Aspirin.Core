using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.EF.Tests.Models;

namespace URF.Core.EF.Tests.Services
{
    public interface ICustomerDomainService
    {
        Task<IEnumerable<CustomerDomainModel>> CustomersByCompany(string companyName);
    }
}
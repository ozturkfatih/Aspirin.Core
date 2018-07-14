using System.Collections.Generic;
using System.Threading.Tasks;
using Test.DomainModelService.Models;

namespace Test.DomainModelService.Services
{
    public interface ICustomerDomainService
    {
        Task<IEnumerable<CustomerDomainModel>> CustomersByCompany(string companyName);
    }
}
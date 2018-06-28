using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Tests.Contexts;
using URF.Core.EF.Tests.Models;
using URF.Core.EF.Tests.Services;
using URF.Core.EF.Trackable;
using Xunit;
namespace URF.Core.EF.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class DomainServiceTest
    {
        private readonly List<Customer> _customers;


        private readonly NorthwindDbContextFixture _fixture;
        public DomainServiceTest(NorthwindDbContextFixture fixture)
        {
            _customers = Factory.Customers();

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Customers.AddRange(_customers);
                _fixture.Context.SaveChanges();
            });
        }

        [Fact]
        public async Task CustomersByCompany_Should_Return_Customer()
        {
            // Arrange
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository);
            const string company = "Alfreds Futterkiste";

            // Act
            var customers = await customerDomainService.CustomersByCompany(company);

            // Assert
            Assert.Collection(customers, customer
                => Assert.Equal("ALFKI", customer.CustomerId));
        }

        [Fact]
        public async Task Service_Insert_Should_Insert_Into_Database()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(customerRepository);
            const string customerId = "COMP1";
            const string companyName = "Company 1";

            var customer = new CustomerDomainModel
            {
                CustomerId = customerId,
                CompanyName = companyName
            };

            // Act
            customerService.Insert(customer);

            // Assert

            // Act
            var savedChanges = await unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(1, savedChanges);

            // Act
            var newCustomer = await customerRepository.FindAsync(customerId);

            // Assert
            Assert.Equal(newCustomer.CustomerId, customerId);
            Assert.Equal(newCustomer.CompanyName, companyName);
        }
    }
}
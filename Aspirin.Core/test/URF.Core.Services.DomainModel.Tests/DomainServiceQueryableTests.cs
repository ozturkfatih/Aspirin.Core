using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URF.Core.EF.Trackable;
using URF.Core.Services.DomainModel.Tests.Contexts;
using URF.Core.Services.DomainModel.Tests.Models;
using URF.Core.Services.DomainModel.Tests.Services;
using Xunit;

namespace URF.Core.Services.DomainModel.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class DomainServiceQueryableTests
    {
        private readonly IMapper _mapper;
        private readonly NorthwindDbContextFixture _fixture;

        public DomainServiceQueryableTests(NorthwindDbContextFixture fixture)
        {
            var customers = Factory.Customers();

            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Customers.AddRange(customers);
                _fixture.Context.SaveChanges();
            });

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CustomerDomainModel, Customer>();
                cfg.CreateMap<Customer, CustomerDomainModel>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Queryable_Should_Return_Customer_Query()
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository, _mapper);
            const string companyName = "Alfreds Futterkiste";

            // Act
            var query = customerDomainService.Queryable();
            var customers = await query
                .Where(x => x.CompanyName.Contains(companyName))
                .ToListAsync();

            // Assert
            Assert.Collection(customers, customer
                => Assert.Equal("ALFKI", customer.CustomerId));
        }

        [Fact]
        public async Task QueryableSql_Should_Return_Customer_Query()
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository, _mapper);
            const string companyName = "Alfreds Futterkiste";

            // Act
            var query = customerDomainService.QueryableSql("SELECT * FROM Customers");
            var customers = await query
                .Where(x => x.CompanyName.Contains(companyName))
                .ToListAsync();

            // Assert
            Assert.Collection(customers, customer
                => Assert.Equal("ALFKI", customer.CustomerId));
        }
    }
}
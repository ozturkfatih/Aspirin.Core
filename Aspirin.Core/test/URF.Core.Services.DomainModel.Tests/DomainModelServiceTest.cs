using System.Threading.Tasks;
using AutoMapper;
using URF.Core.Abstractions;
using URF.Core.EF;
using URF.Core.EF.Trackable;
using URF.Core.Services.DomainModel.Tests.Contexts;
using URF.Core.Services.DomainModel.Tests.Models;
using URF.Core.Services.DomainModel.Tests.Services;
using Xunit;

namespace URF.Core.Services.DomainModel.Tests
{
    [Collection(nameof(NorthwindDbContext))]
    public class DomainModelServiceTest
    {
        private readonly IMapper _mapper;
        private readonly NorthwindDbContextFixture _fixture;

        public DomainModelServiceTest(NorthwindDbContextFixture fixture)
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
        public async Task CustomersByCompany_Should_Return_Customer()
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository, _mapper);
            const string company = "Alfreds Futterkiste";

            // Act
            var customers = await customerDomainService.CustomersByCompany(company);

            // Assert
            Assert.Collection(customers, customer
                => Assert.Equal("ALFKI", customer.CustomerId));
        }

        [Fact]
        public async Task Insert_Should_Create_Customer()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository, _mapper);
            const string customerId = "COMP1";
            const string companyName = "Company 1";

            var customer = new CustomerDomainModel
            {
                CustomerId = customerId,
                CompanyName = companyName
            };

            // Act
            customerDomainService.Insert(customer);
            var savedChanges = await unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(1, savedChanges);
            var newCustomer = await customerRepository.FindAsync(customerId);
            Assert.Equal(newCustomer.CustomerId, customerId);
            Assert.Equal(newCustomer.CompanyName, companyName);
        }

        [Fact]
        public async Task Update_Should_Update_Customer()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerDomainService = new CustomerDomainService(customerRepository, _mapper);
            const string customerId = "BERGS";
            const string companyName = "Eastern Connection 1";

            var data = await customerRepository.FindAsync(customerId);
            customerRepository.Detach(data);
            var customerDomainModel = new CustomerDomainModel
            {
                CustomerId = data.CustomerId,
                CompanyName = companyName,
                ContactName = data.ContactName,
                ContactTitle = data.ContactTitle,
                Address =data.Address,
                City = data.City,
                Region = data.Region,
                PostalCode = data.PostalCode,
                Country = data.Country,
                Phone = data.Phone,
                Fax = data.Fax
            };

            // Act
            customerDomainService.Update(customerDomainModel);
            var savedChanges = await unitOfWork.SaveChangesAsync();

            // Assert
            Assert.Equal(1, savedChanges);
            Assert.Equal(customerDomainModel.CompanyName, companyName);
        }
    }
}
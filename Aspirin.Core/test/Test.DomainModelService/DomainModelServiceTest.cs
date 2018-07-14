using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.DomainModelService.Contexts;
using Test.DomainModelService.Models;
using Test.DomainModelService.Services;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF;
using URF.Core.EF.Trackable;
using Xunit;

namespace Test.DomainModelService
{
    [Collection(nameof(NorthwindDbContext))]
    public class DomainModelServiceTest
    {
        private readonly List<Customer> _customers;


        private readonly NorthwindDbContextFixture _fixture;
        public DomainModelServiceTest(NorthwindDbContextFixture fixture)
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
        public async Task Insert()
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

        [Fact]
        public async Task Update()
        {
            // Arrange
            IUnitOfWork unitOfWork = new UnitOfWork(_fixture.Context);
            ITrackableRepository<Customer> customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(customerRepository);
            const string customerId = "BERGS";
            const string companyName = "Eastern Connection 1";

            var data =await customerRepository.FindAsync(customerId);
            var customerDomainModel=new CustomerDomainModel
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
            customerService.Update(customerDomainModel,data);
            // Act
            var savedChanges = await unitOfWork.SaveChangesAsync();
            // Assert
            Assert.Equal(1, savedChanges);
            // Assert
            Assert.Equal(customerDomainModel.CompanyName, companyName);
        }
    }
}
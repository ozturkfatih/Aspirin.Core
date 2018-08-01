using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly List<Customer> _customers;
        private readonly List<Category> _categories;
        private readonly List<Product> _products;
        public DomainModelServiceTest(NorthwindDbContextFixture fixture)
        {
            _customers = Factory.Customers();
            _categories = Factory.Categories();
            _products = Factory.Products();
            _fixture = fixture;
            _fixture.Initialize(true, () =>
            {
                _fixture.Context.Customers.AddRange(_customers);
                _fixture.Context.Categories.AddRange(_categories);
                _fixture.Context.Products.AddRange(_products);
                _fixture.Context.SaveChanges();
            });

            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDomainModel>()
                    .ForMember(dto => dto.CategoryDomainModel, conf => conf.MapFrom(ol => ol.Category));
            });
            _mapper = mapperConfiguration.CreateMapper();
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
                Address = data.Address,
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
            var updatedCustomer = await customerRepository.FindAsync(customerId);
            Assert.Equal(data.Address, updatedCustomer.Address);
            Assert.Equal(customerDomainModel.CompanyName, companyName);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindAsync_Should_Return_DomainModel(bool useKey)
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(customerRepository, _mapper);

            var customerDomainModel = new CustomerDomainModel();
            // Act
            if (useKey)
                customerDomainModel = await customerService.FindAsync("ALFKI");
            else
                customerDomainModel = await customerService.FindAsync(new object[] { "ALFKI" });
            // Assert
            Assert.Equal("ALFKI", customerDomainModel.CustomerId);
            Assert.NotNull(customerDomainModel);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ExistsAsync_Should_Return_True(bool useKey)
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(customerRepository, _mapper);

            // Act
            bool result;
            if (useKey)
                result = await customerService.ExistsAsync("ALFKI");
            else
                result = await customerService.ExistsAsync(new object[] { "ALFKI" });

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ExistsAsync_Should_Return_False(bool useKey)
        {
            // Arrange
            var customerRepository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(customerRepository, _mapper);

            // Act
            bool result;
            if (useKey)
                result = await customerService.ExistsAsync("TEST");
            else
                result = await customerService.ExistsAsync(new object[] { "TEST" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoadPropertyAsync_Should_Load_Property()
        {
            // Arrange
            var repository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(repository, _mapper);

            var customerDomainModel = new CustomerDomainModel
            {
                CustomerId = "WOLZA",
                CompanyName = "Wolski  Zajazd",
                ContactName = "Zbyszek Piestrzeniewicz",
                ContactTitle = "Owner",
                Address = "ul. Filtrowa 68",
                City = "Warszawa",
                Region = "",
                PostalCode = "01-012",
                Country = "Poland",
                Phone = "(26) 642-7012",
                Fax = "(26) 642-7012"
            };

            customerService.Attach(customerDomainModel);

            // Act
            await customerService.LoadPropertyAsync(customerDomainModel, p => p.City);

            // Assert
            Assert.Same(customerDomainModel.City, _customers.Where(c => c.CustomerId == "WOLZA").Select(c => c.City));
        }
        [Fact]
        public async Task SelectAsync_Should_Return_DomainModels()
        {
            // Arrange
            var repository = new TrackableRepository<Customer>(_fixture.Context);
            var customerService = new CustomerDomainService(repository, _mapper);

            // Act
            var customers = await customerService.SelectAsyncCustomers();
            var enumerable = customers as CustomerDomainModel[] ?? customers.ToArray();

            // Assert
            Assert.Equal(90, enumerable.Length);
        }

        [Fact]
        public async Task Queryable_Should_Allow_Composition()
        {
            // Arrange
            var comparer = new MyProductComparer();
            var expected1 = new MyProduct { Id = 1, Name = "Chai", Price = 18.00m, Category = "Beverages" };
            var expected2 = new MyProduct { Id = 2, Name = "Chang", Price = 19.00m, Category = "Beverages" };

            var repository = new TrackableRepository<Product>(_fixture.Context);

            var productDomainService = new ProductDomainService(repository, _mapper);
            // Act

            var query = productDomainService.Queryable();

            var products = await query
                .Include(p => p.CategoryDomainModel)
                .Take(2)
                .Where(p => p.UnitPrice > 15)
                .Select(p => new MyProduct
                {
                    Id = p.ProductId,
                    Name = p.ProductName,
                    Price = p.UnitPrice,
                    Category = p.CategoryDomainModel.CategoryName
                })
                .ToListAsync();

            // Assert
            Assert.Collection(products,
                p => Assert.Equal(expected1, p, comparer),
                p => Assert.Equal(expected2, p, comparer));
        }
    }
}
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Services.DomainModel;
using URF.Core.Abstractions.Trackable;

namespace URF.Core.Services.DomainModel
{
    public abstract class DomainService<TDomainModel, TEntity> : IDomainService<TDomainModel> 
        where TEntity: class, ITrackable 
        where TDomainModel : class, ITrackable
    {
        protected IMapper Mapper { get; }
        protected ITrackableRepository<TEntity> Repository { get; }

        protected DomainService(ITrackableRepository<TEntity> repository, IMapper mapper)
        {
            Mapper = mapper;
            Repository = repository;
        }

        public virtual async Task<TDomainModel> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.FindAsync(keyValues, cancellationToken);
            return Mapper.Map<TEntity, TDomainModel>(entity);
        }

        public virtual async Task<TDomainModel> FindAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.FindAsync(keyValue, cancellationToken);
            return Mapper.Map<TEntity, TDomainModel>(entity);
        }

        public virtual void Insert(TDomainModel item)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            Repository.Insert(entity);
        }

        public virtual void Delete(TDomainModel item)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            Repository.Delete(entity);
        }

        public virtual void Attach(TDomainModel item)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            Repository.Attach(entity);
        }

        public virtual void Detach(TDomainModel item)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            Repository.Detach(entity);
        }

        public virtual void ApplyChanges(params TDomainModel[] items)
        {
            var entities = Mapper.Map<TDomainModel[], TEntity[]>(items);
            Repository.ApplyChanges(entities);
        }

        public virtual void AcceptChanges(params TDomainModel[] items)
        {
            var entities = Mapper.Map<TDomainModel[], TEntity[]>(items);
            Repository.AcceptChanges(entities);
        }

        public virtual void DetachEntities(params TDomainModel[] items)
        {
            var entities = Mapper.Map<TDomainModel[], TEntity[]>(items);
            Repository.DetachEntities(entities);
        }

        public virtual async Task LoadRelatedEntities(params TDomainModel[] items)
        {
            var entities = Mapper.Map<TDomainModel[], TEntity[]>(items);
            await Repository.LoadRelatedEntities(entities);
        }

        public virtual async Task<bool> ExistsAsync(object[] keyValues, CancellationToken cancellationToken = default) 
            => await Repository.ExistsAsync(keyValues, cancellationToken);

        public virtual async Task<bool> ExistsAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await Repository.ExistsAsync(keyValue, cancellationToken);

        public virtual async Task LoadPropertyAsync(TDomainModel item, Expression<Func<TDomainModel, object>> property, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            var entityProperty = ConvertPropertyExpression(property);
            await Repository.LoadPropertyAsync(entity, entityProperty, cancellationToken);
        }

        public virtual void Update(TDomainModel item)
        {
            var entity = Mapper.Map<TDomainModel, TEntity>(item);
            Repository.Update(entity);
        }

        public virtual async Task<bool> DeleteAsync(object[] keyValues, CancellationToken cancellationToken = default)
            => await Repository.DeleteAsync(keyValues, cancellationToken);

        public virtual async Task<bool> DeleteAsync<TKey>(TKey keyValue, CancellationToken cancellationToken = default)
            => await Repository.DeleteAsync(keyValue, cancellationToken);

        public virtual IQueryable<TDomainModel> Queryable()
            => Repository.Queryable().ProjectTo<TDomainModel>(Mapper.ConfigurationProvider);

        public IQueryable<TDomainModel> QueryableSql(string sql, params object[] parameters)
            => Repository.QueryableSql(sql, parameters).ProjectTo<TDomainModel>(Mapper.ConfigurationProvider);

        public virtual IQuery<TDomainModel> Query()
            => throw new NotSupportedException();

        private Expression<Func<TEntity, object>> ConvertPropertyExpression(Expression<Func<TDomainModel, object>> property)
        {
            var memberName = ((MemberExpression)property.Body).Member.Name;
            var param = Expression.Parameter(typeof(TDomainModel));
            var field = Expression.Property(param, memberName);
            return Expression.Lambda<Func<TEntity, object>>(field, param);
        }
    }
}
using TrackableEntities.Common.Core;

namespace URF.Core.Abstractions.Services.DomainModel
{
    public interface IDomainService<TDomainModel> : IService<TDomainModel> where TDomainModel : class, ITrackable
    {
    }
}
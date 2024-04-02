using DevElectronic_Store.Core.DomainObjects;
using System;

namespace DevElectronic_Store.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; } 
    }
}

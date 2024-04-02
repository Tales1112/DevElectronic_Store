using System.Threading.Tasks;

namespace DevElectronic_Store.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}

using DevElectronic_Store.Core.Messages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevElectronic_Store.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task<ValidationResult> EnviarComando<T>(T comando) where T : Command;
        Task PublicarEvento<T>(T evento) where T : Command;
    }
}
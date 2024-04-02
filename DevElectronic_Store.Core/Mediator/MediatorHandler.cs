using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevElectronic_Store.Core.Mediator
{
    public class MediatorHandler
    {
        private readonly IMediator _mediator;
        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;   
        }
        public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
    }
}

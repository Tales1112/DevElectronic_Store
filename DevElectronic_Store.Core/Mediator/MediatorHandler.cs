﻿using DevElectronic_Store.Core.Messages;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DevElectronic_Store.Core.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;
        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
        {
            return await _mediator.Send(comando);
        }
        public async Task PublicarEvento<T>(T evento) where T : Command
        {
            await _mediator.Publish(evento);
        }
    }
}

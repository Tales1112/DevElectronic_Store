using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace DevElectronic_Store.Core.Messages
{
    public abstract class Command : Message, IRequest<ValidationResult>
    {
        public DateTime Timestamp { get; set; }
        public ValidationResult ValidationResult { get; set; }  
        protected Command()
        {
            Timestamp = DateTime.Now;
        }
        public virtual bool EhValido()
        {
            throw new NotImplementedException();    
        }
    }
}

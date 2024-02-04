using DevElectronic_Store.Core.Communication;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace DevElectronic_Store.WebApi.Core.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        public ICollection<string> Erros = new List<string>();
        protected IActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(result);
            }
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }
        protected IActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }
            return CustomResponse();
        }
        protected IActionResult CustomResponse(ResponseResult resposta)
        {
            ResponsePossuiErros(resposta);
            return CustomResponse();
        }
        protected IActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }
        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta == null || !resposta.Erros.Mensagens.Any()) return false;
            foreach (var mensagem in resposta.Erros.Mensagens)
            {
                AdicionarErroProcessamento(mensagem);
            }
            return true;
        }
        protected bool OperacaoValida()
        {
            return !Erros.Any();
        }
        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }
        protected void LimparErrosProcessamento()
        {
            Erros.Clear();
        }
    }
}

using System.Collections.Generic;

namespace DevElectronic_Store.Core.Communication
{
    public class ResponseResult
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public ResponseErrorMessages Erros { get; set; }
        public ResponseResult()
        {
            Erros = new ResponseErrorMessages();
        }
    }
    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; }
        public ResponseErrorMessages()
        {
            Mensagens = new List<string>(); 
        }
    }
}

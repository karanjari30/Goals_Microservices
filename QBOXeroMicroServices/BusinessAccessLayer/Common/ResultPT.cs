using static BusinessAccessLayer.Common.Enums;

namespace BusinessAccessLayer.Common
{
    public class ResultPT
    {
        public ResponseStatus ResponseStatus { get; set; }
        public string Message { get; set; }
        public dynamic Result { get; set; }
    }
}
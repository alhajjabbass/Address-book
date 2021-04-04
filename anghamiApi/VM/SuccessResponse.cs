using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anghamiApi.VM
{
    public class SuccessResponse<T>
    {
        public int Code { set; get; }
        public T Data { set; get; }
    }
}

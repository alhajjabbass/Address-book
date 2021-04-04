using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace anghamiApi.VM
{
    public class FailureResponse
    {
        public int Code { set; get; }
        public List<string> Error { set; get; }
    }
}

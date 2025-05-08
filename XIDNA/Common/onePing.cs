using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Common
{
    public class onePing
    {
        //public async override Task<onePingResponse> Handle(onePingRequest request)
        //{
        //    return new onePingResponse
        //    {
        //        sOutbound = API.GetString("api/QSAPI/CreateQS?QSD=23386EA5-4E05-4CBA-8FAB-083E6B0005A1&sUniqueRef=9032781899")
        //    };
        //}



    }
    public class onePingRequest
    {
        public string sInbound { get; set; }
    }
    public class onePingResponse
    {
        public string sOutbound { get; set; }
    }
}

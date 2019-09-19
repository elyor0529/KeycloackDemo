using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceApi.Models
{
    public class ClientCredsModel
    {
        public dynamic client_id { get; set; }
        public dynamic refresh_token { get; set; }
    }
}

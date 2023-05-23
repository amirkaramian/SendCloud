
using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.SendCloud
{
    public class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Widgets.SendCloud",
                 "Plugins/WidgetsSendCloudInfo/GetSendCloudInfo",
                 new { controller = "WidgetsSendCloudInfo", action = "GetSendCloudInfo", area = "" }
            );
        }


        public int Priority => 0;

    }
}

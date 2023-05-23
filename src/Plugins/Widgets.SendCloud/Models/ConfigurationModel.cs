using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Widgets.SendCloud.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.ClientId")]
        public string ClientId { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.ClientSecret")]
        public string ClientSecret { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.SendCloudUrl")]
        public string SendCloudUrl { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.ServiceName")]
        public string ServiceName { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.EnablePickup")]
        public bool EnablePickup { get; set; }

        [GrandResourceDisplayName("Widgets.SendCloud.Carrier")]
        public string Carrier { get; set; }
        [GrandResourceDisplayName("Widgets.SendCloud.CarrierList")]
        public IList<SelectListItem> CarrierList { get; set; }




    }
}
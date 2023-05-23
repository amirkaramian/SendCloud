﻿using Grand.Domain.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Widgets.SendCloud
{
    public class SendCloudSettings : ISettings
    {
        public SendCloudSettings() {
            CarrierList = new List<SelectListItem>();
        }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SendCloudUrl { get; set; }
        public string Carrier { get; set; }
        public bool EnablePickup { get; set; }
        public bool AllowToDisable { get; set; }
        public string ServiceName { get; set; }
        public int DisplayOrder { get;  set; }
        public IList<SelectListItem> CarrierList { get; set; }
    }
}
using Grand.Domain.Orders;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Stores;
using Amazon.Runtime.Internal.Transform;

namespace Widgets.SendCloud.Controllers
{
    public class WidgetsSendCloudInfoController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        public WidgetsSendCloudInfoController(ISettingService settingService, IStoreService storeService)
        {
            _settingService = settingService;
            _storeService = storeService;
        }
        public async Task<(bool success, Dictionary<string, string> values)> GetSendCloudInfo()
        {
            var stores = await _storeService.GetAllStores();
            var settings = _settingService.LoadSetting<SendCloudSettings>(stores.FirstOrDefault()!.Id);
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "Carrier", settings.Carrier },
                { "ClientId", settings.ClientId },
                { "ClientSecret", settings.ClientSecret },
                { "EnablePickup", settings.EnablePickup.ToString() },
                { "SendCloudUrl", settings.SendCloudUrl },
                { "ServiceName", settings.ServiceName },
            };
            return (true, values);
        }
    }
}

using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Cms;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using Widgets.SendCloud.Models;

namespace Widgets.SendCloud.Components
{
    [ViewComponent(Name = "WidgetsSendCloud")]
    public class WidgetsSendCloudViewComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly ICookiePreference _cookiePreference;
        private readonly SendCloudSettings _settings;

        public WidgetsSendCloudViewComponent(
            IWorkContext workContext,
            IOrderService orderService,
            ICookiePreference cookiePreference,
            SendCloudSettings settings
            )
        {
            _workContext = workContext;
            _orderService = orderService;
            _cookiePreference = cookiePreference;
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {

            if (_settings.EnablePickup)
            {
                var enabled = await _cookiePreference.IsEnable(_workContext.CurrentCustomer, _workContext.CurrentStore, SendCloudDefaults.ConsentCookieSystemName);
                if ((enabled.HasValue && !enabled.Value) || (!enabled.HasValue && !_settings.EnablePickup))
                    return Content("");
            }
            //page
            if (widgetZone == SendCloudDefaults.Page)
            {
                return View("Default", GetTrackingScript());
            }
            //add to cart
            //if (widgetZone == SendCloudDefaults.AddToCart)
            //{
            //    var model = JsonConvert.DeserializeObject<SendCloudAddToCartModelModel>(JsonConvert.SerializeObject(additionalData));
            //    if (model != null)
            //    {
            //        return View("Default", GetAddToCartScript(model));
            //    }
            //}
            //order details 
            //if (widgetZone == SendCloudDefaults.OrderDetails)
            //{
            //    var orderId = additionalData as string;
            //    if (!string.IsNullOrEmpty(orderId))
            //    {
            //        return View("Default", await GetOrderScript(orderId));
            //    }

            //}
            return Content("");
        }

        private string GetTrackingScript()
        {
            var trackingScript = _settings.ClientId + "\n";
            trackingScript = trackingScript.Replace("{ClientId}", _settings.ClientId);
            return trackingScript;
        }

        //private string GetAddToCartScript(SendCloudAddToCartModelModel model)
        //{
        //    var trackingScript = _settings.AddToCartScript + "\n";
        //    trackingScript = trackingScript.Replace("{PRODUCTID}", model.ProductId);
        //    trackingScript = trackingScript.Replace("{PRODUCTNAME}", model.ProductName);
        //    trackingScript = trackingScript.Replace("{QTY}", model.Quantity.ToString("N0"));
        //    trackingScript = trackingScript.Replace("{AMOUNT}", model.DecimalPrice.ToString("F2", CultureInfo.InvariantCulture));
        //    trackingScript = trackingScript.Replace("{CURRENCY}", _workContext.WorkingCurrency.CurrencyCode);
        //    return trackingScript;
        //}

        //private async Task<string> GetOrderScript(string orderId)
        //{
        //    var trackingScript = _settings.DetailsOrderScript + "\n";
        //    var order = await _orderService.GetOrderById(orderId);
        //    if (order != null)
        //    {
        //        trackingScript = trackingScript.Replace("{AMOUNT}", order.OrderTotal.ToString("F2", CultureInfo.InvariantCulture));
        //        trackingScript = trackingScript.Replace("{CURRENCY}", order.CustomerCurrencyCode);
        //    }
        //    return trackingScript;
        //}
    }
}
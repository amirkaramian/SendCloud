namespace Widgets.SendCloud
{
    public static class SendCloudDefaults
    {
        public static string ConsentCookieSystemName = "SendCloud";
        public const string ProviderSystemName = "Widgets.SendCloud";
        public const string FriendlyName = "Widgets.SendCloud.FriendlyName";
        public const string ConfigurationUrl = "../WidgetsSendCloud/Configure";

        public static string Page => "head_html_tag";
        public static string AddToCart => "popup_add_to_cart_content_before";
        public static string OrderDetails => "checkout_completed_top";
    }
}

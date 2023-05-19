using Grand.Domain.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Grand.Domain.Shipping
{
    public class SendCloudShippingMethodsList
    {
        [JsonPropertyName("Shipping_Methods")]
        public List<Shipping_Methods> Shipping_Methods { get; set; }
    }
    public class Shipping_Methods
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Carrier { get; set; }
        public string Min_Weight { get; set; }
        public string Max_Weight { get; set; }
        public string Service_Point_Input { get; set; }
        public int Price { get; set; }
        public List<Country> Countries { get; set; }
    }

}



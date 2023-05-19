using Grand.Domain;
using Grand.Domain.Data;
using Grand.Domain.Shipping;
using Grand.Infrastructure.Caching;
using Newtonsoft.Json;
using Shipping.SendCloud.Domain;
using Shipping.SendCloud.Models;
using System.Net.Http;
using static QRCoder.PayloadGenerator;

namespace Shipping.SendCloud.Services
{
    public class ShippingSendCloudService : IShippingSendCloudService
    {
        #region Constants
        private const string SHIPPINGSendCloud_ALL_KEY = "Grand.shippingSendCloud.all-{0}-{1}";
        private const string SHIPPINGSendCloud_PATTERN_KEY = "Grand.shippingSendCloud.";
        #endregion

        #region Fields

        private readonly IRepository<ShippingSendCloudRecord> _sbwRepository;
        private readonly ICacheBase _cacheBase;
        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public ShippingSendCloudService(ICacheBase cacheBase,
            IRepository<ShippingSendCloudRecord> sbwRepository, IHttpClientFactory httpClientFactory)
        {
            _cacheBase = cacheBase;
            _sbwRepository = sbwRepository;
            _httpClient = httpClientFactory.CreateClient("SendCloudUrl");
        }

        #endregion

        #region Methods

        public virtual async Task DeleteShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord)
        {
            if (shippingSendCloudRecord == null)
                throw new ArgumentNullException(nameof(shippingSendCloudRecord));

            await _sbwRepository.DeleteAsync(shippingSendCloudRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGSendCloud_PATTERN_KEY);
        }

        public virtual async Task<IPagedList<ShippingSendCloudRecord>> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(SHIPPINGSendCloud_ALL_KEY, pageIndex, pageSize);
            return await _cacheBase.GetAsync(key, () =>
            {
                var query = from sbw in _sbwRepository.Table
                            select sbw;

                return Task.FromResult(new PagedList<ShippingSendCloudRecord>(query, pageIndex, pageSize));
            });
        }

        public virtual async Task<ShippingSendCloudRecord> FindRecord(string shippingMethodId,
            string storeId, string warehouseId,
            string countryId, string stateProvinceId, string zip, double weight)
        {
            zip ??= string.Empty;
            zip = zip.Trim();

            var existingRates = (await GetAll())
                .Where(sbw => sbw.ShippingMethodId == shippingMethodId && weight >= sbw.From && weight <= sbw.To)
                .ToList();

            if (!string.IsNullOrEmpty(warehouseId))
                existingRates = existingRates.Any(x => x.WarehouseId == warehouseId)
                    ? existingRates.Where(x => x.WarehouseId == warehouseId).ToList()
                    : existingRates.Where(x => string.IsNullOrEmpty(x.WarehouseId)).ToList();

            if (!string.IsNullOrEmpty(storeId))
                existingRates = existingRates.Any(x => x.StoreId == storeId)
                    ? existingRates.Where(x => x.StoreId == storeId).ToList()
                    : existingRates.Where(x => string.IsNullOrEmpty(x.StoreId)).ToList();

            if (!string.IsNullOrEmpty(countryId))
                existingRates = existingRates.Any(x => x.CountryId == countryId)
                    ? existingRates.Where(x => x.CountryId == countryId).ToList()
                    : existingRates.Where(x => string.IsNullOrEmpty(x.CountryId)).ToList();

            if (!string.IsNullOrEmpty(stateProvinceId))
                existingRates = existingRates.Any(x => x.StateProvinceId == stateProvinceId)
                    ? existingRates.Where(x => x.StateProvinceId == stateProvinceId).ToList()
                    : existingRates.Where(x => string.IsNullOrEmpty(x.StateProvinceId)).ToList();

            if (!string.IsNullOrEmpty(zip))
                existingRates = existingRates.Any(x => x.Zip == zip)
                    ? existingRates.Where(x => x.Zip == zip).ToList()
                   : existingRates.Where(x => string.IsNullOrEmpty(x.Zip)).ToList();

            return existingRates.FirstOrDefault();

        }

        public virtual Task<ShippingSendCloudRecord> GetById(string shippingSendCloudRecordId)
        {
            return _sbwRepository.GetByIdAsync(shippingSendCloudRecordId);
        }

        public virtual async Task InsertShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord)
        {
            if (shippingSendCloudRecord == null)
                throw new ArgumentNullException(nameof(shippingSendCloudRecord));

            await _sbwRepository.InsertAsync(shippingSendCloudRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGSendCloud_PATTERN_KEY);
        }

        public virtual async Task UpdateShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord)
        {
            if (shippingSendCloudRecord == null)
                throw new ArgumentNullException(nameof(shippingSendCloudRecord));

            await _sbwRepository.UpdateAsync(shippingSendCloudRecord);

            await _cacheBase.RemoveByPrefix(SHIPPINGSendCloud_PATTERN_KEY);
        }

        public virtual async Task<ShoppingPriceModel> GetRate(ShoppingRateRecord shoppingRate)
        {
            var response = await _httpClient.GetAsync($"shipping-price?shipping_method_id={shoppingRate.ShoppingMethodId}&from_country={shoppingRate.FromCountry}&to_country={shoppingRate.ToCountry}&weight={shoppingRate.Weight}&weight_unit={shoppingRate.Weightunit}");

            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ShoppingPriceModel>>(json);
            return list?.FirstOrDefault();

        }
        public virtual async Task<SenderAddressModel> GetSenderAddress()
        {
            var response = await _httpClient.GetAsync("user/addresses/sender");
            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SenderAddressModel>(json);

        }
        public virtual async Task<ShippingMethodList> GetShippingMethods(ShippingMethodModel model)
        {
            var response = await _httpClient.GetAsync($"shipping_methods?sender_address={model.SenderAddress}&to_country={model.ToCountry}&from_postal_code={model.FromPostal_code}&to_postal_code={model.ToPostal_code}");
            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ShippingMethodList>(json);
        }
        public virtual async Task<ParcelModelRoot> CreateParcel(ParcelRecord model)
        {
            string request = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("parcels", content);
            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ParcelModelRoot>(json);

        }
        public virtual async Task<ParcelModelRoot> CreateLable(LableRecord model)
        {
            string request = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("parcels", content);
            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ParcelModelRoot>(json);

        }
        public virtual async Task<PickupRecord> CreatePickUpRequest(PickupRequestModel model)
        {
            string request = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("pickups", content);
            if (!response.IsSuccessStatusCode)
            {
                var jsonError = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendCloudError>(jsonError);
                throw new Exception(result.error.message);
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PickupRecord>(json);

        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using AdvancedPlanningSystem.Models;

namespace AdvancedPlanningSystem.MES
{
    public class MesHttpClient : IMesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JavaScriptSerializer _serializer;

        public MesHttpClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10); // Batch requests might take longer
            _serializer = new JavaScriptSerializer();
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            string url = $"{_baseUrl}{endpoint}";
            try
            {
                LogHelper.MES.Info($"[MES GET] {url}");
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                string responseJson = await response.Content.ReadAsStringAsync();
                
                LogHelper.MES.Info($"[MES Response] {response.StatusCode}: {responseJson}");

                if (response.IsSuccessStatusCode)
                {
                    // [Smart Unwrapping]
                    // 嘗試先以 Wrapper 格式解序列化
                    try
                    {
                        var wrapper = _serializer.Deserialize<MesApiResponse<T>>(responseJson);
                        // 若成功解析且 Data 不為 null，則回傳 Data
                        // 注意：JavaScriptSerializer 若格式不符通常不會拋例外，而是屬性為 null/default
                        if (wrapper != null && wrapper.Data != null)
                        {
                            return wrapper.Data;
                        }
                    }
                    catch { /* Ignore and try raw */ }

                    // 若 Wrapper 解析失敗或 Data 為空，則嘗試直接解析為 T (Raw Format)
                    return _serializer.Deserialize<T>(responseJson);
                }
                return default(T);
            }
            catch (Exception ex) 
            {
                LogHelper.MES.Error($"[MES Error] GET {url}: {ex.Message}", ex);
                return default(T); 
            }
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string url = $"{_baseUrl}{endpoint}";
            try
            {
                string json = _serializer.Serialize(data);
                LogHelper.MES.Info($"[MES POST] {url} Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                
                string responseJson = await response.Content.ReadAsStringAsync();
                LogHelper.MES.Info($"[MES Response] {response.StatusCode}: {responseJson}");

                if (response.IsSuccessStatusCode)
                {
                    // [Smart Unwrapping]
                    try
                    {
                        var wrapper = _serializer.Deserialize<MesApiResponse<T>>(responseJson);
                        if (wrapper != null && wrapper.Data != null)
                        {
                            return wrapper.Data;
                        }
                    }
                    catch { }

                    return _serializer.Deserialize<T>(responseJson);
                }
                return default(T);
            }
            catch (Exception ex)
            {
                LogHelper.MES.Error($"[MES Error] POST {url}: {ex.Message}", ex);
                return default(T);
            }
        }

        // --- Batch Implementations ---

        public async Task<List<OrderInfoResponse>> GetOrderInfoBatchAsync(List<string> workNos)
        {
            // Endpoint: /order/batch
            return await PostAsync<List<OrderInfoResponse>>("/order/batch", workNos) ?? new List<OrderInfoResponse>();
        }

        public async Task<List<StepTimeResponse>> GetAllStepTimesAsync()
        {
            // Endpoint: /steptime/all (GET)
            return await GetAsync<List<StepTimeResponse>>("/steptime/all") ?? new List<StepTimeResponse>();
        }

        public async Task<List<QTimeLimitResponse>> GetAllQTimeLimitsAsync()
        {
            // Endpoint: /qtime/all (GET) - 舊有
            // 這裡需使用 MES namespace 下的 Model
            return await GetAsync<List<QTimeLimitResponse>>("/qtime/all") ?? new List<QTimeLimitResponse>();
        }

        public async Task<List<WipInfoResponse>> GetWipBatchAsync(List<string> eqpIds)
        {
            // Endpoint: /wip/batch
            return await PostAsync<List<WipInfoResponse>>("/wip/batch", eqpIds) ?? new List<WipInfoResponse>();
        }

        public async Task<List<EqStatusResponse>> GetEquipmentStatusBatchAsync(List<string> eqpIds)
        {
            // Endpoint: /equipment/batch
            return await PostAsync<List<EqStatusResponse>>("/equipment/batch", eqpIds) ?? new List<EqStatusResponse>();
        }

        // --- Legacy ---
        public async Task<StandardResponse> ValidateMoveAsync(string cassetteId, string source, string destination)
        {
             return await PostAsync<StandardResponse>("/validate_move", new { });
        }
        public async Task<StandardResponse> ReportStatusAsync(string portId, string cassetteId, string status)
        {
            return await PostAsync<StandardResponse>("/report_status", new { });
        }
    }
}

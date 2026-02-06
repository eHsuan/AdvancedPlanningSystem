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
            _httpClient.Timeout = TimeSpan.FromSeconds(10); 
            _serializer = new JavaScriptSerializer();
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            string url = $"{_baseUrl}{endpoint}";
            LogHelper.MES.Info($"[MES GET] {url}");
            
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseJson = await response.Content.ReadAsStringAsync();
            
            LogHelper.MES.Info($"[MES Response] {response.StatusCode}: {responseJson}");

            if (response.IsSuccessStatusCode)
            {
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
            
            // Throw exception if not success
            throw new HttpRequestException($"MES GET Failed: {response.StatusCode} - {responseJson}");
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string url = $"{_baseUrl}{endpoint}";
            string json = _serializer.Serialize(data);
            LogHelper.MES.Info($"[MES POST] {url} Body: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            
            string responseJson = await response.Content.ReadAsStringAsync();
            LogHelper.MES.Info($"[MES Response] {response.StatusCode}: {responseJson}");

            if (response.IsSuccessStatusCode)
            {
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
            
            // Throw exception if not success
            throw new HttpRequestException($"MES POST Failed: {response.StatusCode} - {responseJson}");
        }

        // --- Batch Implementations ---

        public async Task<List<OrderInfoResponse>> GetOrderInfoBatchAsync(List<string> workNos)
        {
            return await PostAsync<List<OrderInfoResponse>>("/order/batch", workNos) ?? new List<OrderInfoResponse>();
        }

        public async Task<List<StepTimeResponse>> GetAllStepTimesAsync()
        {
            return await GetAsync<List<StepTimeResponse>>("/steptime/all") ?? new List<StepTimeResponse>();
        }

        public async Task<List<QTimeLimitResponse>> GetAllQTimeLimitsAsync()
        {
            return await GetAsync<List<QTimeLimitResponse>>("/qtime/all") ?? new List<QTimeLimitResponse>();
        }

        public async Task<List<WipInfoResponse>> GetWipBatchAsync(List<string> eqpIds)
        {
            return await PostAsync<List<WipInfoResponse>>("/wip/batch", eqpIds) ?? new List<WipInfoResponse>();
        }

        public async Task<List<EqStatusResponse>> GetEquipmentStatusBatchAsync(List<string> eqpIds)
        {
            return await PostAsync<List<EqStatusResponse>>("/equipment/batch", eqpIds) ?? new List<EqStatusResponse>();
        }

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
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

            HttpResponseMessage response;
            if (endpoint.Equals("/EqpTransaction", StringComparison.OrdinalIgnoreCase) || endpoint.Equals("/WOQRY", StringComparison.OrdinalIgnoreCase))
            {
                var dict = new Dictionary<string, string> { { "pParameter", json } };
                var content = new FormUrlEncodedContent(dict);
                response = await _httpClient.PostAsync(url, content);
            }
            else
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, content);
            }
            
            string responseContent = await response.Content.ReadAsStringAsync();
            LogHelper.MES.Info($"[MES Response] {response.StatusCode}: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                string jsonToDecode = responseContent;
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    try
                    {
                        var doc = System.Xml.Linq.XDocument.Parse(responseContent);
                        jsonToDecode = doc.Root.Value;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.MES.Error($"Failed to parse XML response from {url}: {ex.Message}. Using raw response.");
                    }
                }

                try
                {
                    var wrapper = _serializer.Deserialize<MesApiResponse<T>>(jsonToDecode);
                    if (wrapper != null && wrapper.Data != null)
                    {
                        return wrapper.Data;
                    }
                }
                catch { }

                return _serializer.Deserialize<T>(jsonToDecode);
            }
            
            // Throw exception if not success
            throw new HttpRequestException($"MES POST Failed: {response.StatusCode} - {responseContent}");
        }

        // --- Batch Implementations ---

        private string ParseToDbTimeStr(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr)) return null;
            DateTime dt;
            if (DateTime.TryParse(timeStr, out dt))
            {
                return dt.ToString("yyyyMMddHHmmss");
            }
            return null;
        }

        private async Task<List<ApsEqpInfo>> GetApsEqpInfoListAsync(List<string> eqpIds)
        {
            if (eqpIds == null || eqpIds.Count == 0) return new List<ApsEqpInfo>();

            var ask = new ApsEqpAsk
            {
                TransactionName = "WOQRY",
                EqpNo = eqpIds[0],
                WONO = "0000000000",
                UserID = "000000",
                GetAPSInfo_ByEqp = string.Join(",", eqpIds)
            };

            var reply = await PostAsync<ApsEqpReply>("/EqpTransaction", ask);
            if (reply == null || reply.Result != "success")
            {
                throw new Exception($"MES GetApsEqpInfo Failed: {(reply != null ? reply.Message : "Null Response")}");
            }

            if (string.IsNullOrEmpty(reply.APSInfo_ByEqp_Result)) return new List<ApsEqpInfo>();
            return _serializer.Deserialize<List<ApsEqpInfo>>(reply.APSInfo_ByEqp_Result) ?? new List<ApsEqpInfo>();
        }

        public async Task<List<OrderInfoResponse>> GetOrderInfoBatchAsync(List<string> workNos)
        {
            if (workNos == null || workNos.Count == 0) return new List<OrderInfoResponse>();

            var ask = new ApsLotAsk
            {
                TransactionName = "WOQRY",
                EqpNo = AppConfig.MesDefaultEqpNo,
                WONO = "0000000000",
                UserID = "000000",
                GetAPSInfo_ByLot = string.Join(",", workNos)
            };

            var reply = await PostAsync<ApsLotReply>("/EqpTransaction", ask);
            if (reply == null || reply.Result != "success")
            {
                throw new Exception($"MES GetOrderInfoBatch Failed: {(reply != null ? reply.Message : "Null Response")}");
            }

            if (string.IsNullOrEmpty(reply.APSInfo_ByLot_Result)) return new List<OrderInfoResponse>();
            var lotInfos = _serializer.Deserialize<List<ApsLotInfo>>(reply.APSInfo_ByLot_Result) ?? new List<ApsLotInfo>();

            var list = new List<OrderInfoResponse>();
            foreach (var l in lotInfos)
            {
                list.Add(new OrderInfoResponse
                {
                    WorkNo = l.WONo,
                    carrier_id = "CASS-MOCK",
                    step_id = l.WorkCenterNo,
                    next_step_id = l.WCNext1,
                    prev_out_time = ParseToDbTimeStr(l.PreStepOutTime),
                    priority_type = (l.Urgent == "Y") ? 1 : 0,
                    due_date = ParseToDbTimeStr(l.EstimateProcessEndDate)
                });
            }
            return list;
        }

        public async Task<List<StepTimeResponse>> GetAllStepTimesAsync()
        {
            // 正式 MES API spec 中已無 StepTime，直接回傳空 List 降級為預設 5 分鐘
            return await Task.FromResult(new List<StepTimeResponse>());
        }

        public async Task<List<QTimeLimitResponse>> GetAllQTimeLimitsAsync()
        {
            var ask = new ApsQTimeAsk
            {
                TransactionName = "WOQRY",
                EqpNo = AppConfig.MesDefaultEqpNo,
                WONO = "0000000000",
                UserID = "000000",
                GetAPSInfo_QTime = "UP"
            };

            var reply = await PostAsync<ApsQTimeReply>("/EqpTransaction", ask);
            if (reply == null || reply.Result != "success")
            {
                throw new Exception($"MES GetAllQTimeLimits Failed: {(reply != null ? reply.Message : "Null Response")}");
            }

            if (string.IsNullOrEmpty(reply.APSInfo_QTime_Result)) return new List<QTimeLimitResponse>();
            var rawQTimes = _serializer.Deserialize<List<ApsQTimeInfo>>(reply.APSInfo_QTime_Result) ?? new List<ApsQTimeInfo>();

            var list = new List<QTimeLimitResponse>();
            foreach (var q in rawQTimes)
            {
                if (q.Enable != "Y") continue;
                list.Add(new QTimeLimitResponse
                {
                    step_id = q.StartWorkcenterNo,
                    next_step_id = q.EndWorkcenterNo,
                    qtime_limit_min = (int)q.QuotaTimes
                });
            }
            return list;
        }

        public async Task<List<WipInfoResponse>> GetWipBatchAsync(List<string> eqpIds)
        {
            var eqpInfos = await GetApsEqpInfoListAsync(eqpIds);
            var list = new List<WipInfoResponse>();
            foreach (var e in eqpInfos)
            {
                list.Add(new WipInfoResponse
                {
                    eq_id = e.MachNo,
                    current_wip_qty = e.By_Eqp_Now_Used_Lot_Count,
                    max_wip_qty = e.MaxLot
                });
            }
            return list;
        }

        public async Task<List<EqStatusResponse>> GetEquipmentStatusBatchAsync(List<string> eqpIds)
        {
            var eqpInfos = await GetApsEqpInfoListAsync(eqpIds);
            var list = new List<EqStatusResponse>();
            foreach (var e in eqpInfos)
            {
                string curWorkNo = "";
                if (!string.IsNullOrEmpty(e.WONo_List))
                {
                    var parts = e.WONo_List.Split(',');
                    if (parts.Length > 0) curWorkNo = parts[0].Trim();
                }

                list.Add(new EqStatusResponse
                {
                    eqp_id = e.MachNo,
                    status = e.StatusCode ?? "IDLE",
                    duration = e.StatusDurationSec.ToString(),
                    current_WorkNo = curWorkNo
                });
            }
            return list;
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
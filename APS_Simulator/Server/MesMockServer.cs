using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APSSimulator.DB;
using APSSimulator.Models;
using Newtonsoft.Json;
using log4net;
using System.Linq;

namespace APSSimulator.Server
{
    public class MesMockServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MesMockServer));
        private HttpListener _listener;
        private bool _isRunning;
        private string _baseUrl;

        public MesMockServer()
        {
            // Default initialization if needed
        }

        public void Start(int port)
        {
            if (_isRunning) return;
            try
            {
                _baseUrl = $"http://localhost:{port}/";
                _listener = new HttpListener();
                _listener.Prefixes.Add(_baseUrl);
                _listener.Start();
                _isRunning = true;
                log.Info($"MES Mock Server started at {_baseUrl}");
                Task.Run(() => ListenLoop());
            }
            catch (Exception ex)
            {
                log.Error($"Error starting server: {ex.Message}");
                _isRunning = false; // Ensure flag is reset on failure
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            log.Info("MES Mock Server stopped.");
        }

        private void ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
                catch (HttpListenerException)
                {
                    // Listener stopped
                    break;
                }
                catch (Exception ex)
                {
                    log.Error($"Listen Error: {ex.Message}");
                }
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath.ToLower();
            string method = context.Request.HttpMethod;
            string responseString = "";
            int statusCode = 200;

            string requestBody = "";
            if (context.Request.HasEntityBody)
            {
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
            }

            bool isFormParam = false;
            if (method == "POST" && requestBody.StartsWith("pParameter=", StringComparison.OrdinalIgnoreCase))
            {
                isFormParam = true;
                string encodedVal = requestBody.Substring("pParameter=".Length);
                requestBody = Uri.UnescapeDataString(encodedVal);
            }

            log.Info($"--- Incoming Request ---");
            log.Info($"{method} {path}");
            if (!string.IsNullOrEmpty(requestBody))
            {
                try {
                    var parsedJson = JsonConvert.DeserializeObject(requestBody);
                    log.Info($"[Request Body]:\n{JsonConvert.SerializeObject(parsedJson, Formatting.Indented)}");
                } catch {
                    log.Info($"[Request Body (Raw)]: {requestBody}");
                }
            }

            try
            {
                using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    // API 路徑加上 /api 前綴
                    if (path == "/api/woqry" && method == "POST")
                    {
                        var reqDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody) ?? new Dictionary<string, object>();
                        if (reqDict.ContainsKey("GetAPSInfo_ByEqp"))
                        {
                            string eqpStr = reqDict["GetAPSInfo_ByEqp"]?.ToString() ?? "";
                            var eqpIds = eqpStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => id.Trim()).ToList();
                            var list = new List<object>();
                            string sql = "SELECT eqp_id, current_wip_qty, max_wip_qty, status, status_duration, current_work_no FROM mock_mes_equipments";
                            using (var cmd = new SQLiteCommand(sql, conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string id = reader["eqp_id"].ToString();
                                    if (eqpIds.Count == 0 || eqpIds.Contains(id))
                                    {
                                        list.Add(new {
                                            MachNo = id,
                                            MaxLot = Convert.ToInt32(reader["max_wip_qty"]),
                                            By_Eqp_Now_Used_Lot_Count = Convert.ToInt32(reader["current_wip_qty"]),
                                            StatusCode = reader["status"].ToString(),
                                            StatusDurationSec = Convert.ToInt64(reader["status_duration"]),
                                            WONo_List = reader["current_work_no"] == DBNull.Value ? "" : reader["current_work_no"].ToString(),
                                            MachName = "Mock Machine",
                                            MachAlias = "Mock Mach Alias"
                                        });
                                    }
                                }
                            }
                            var reply = new { Result = "success", APSInfo_ByEqp_Result = JsonConvert.SerializeObject(list) };
                            responseString = JsonConvert.SerializeObject(reply);
                        }
                        else if (reqDict.ContainsKey("GetAPSInfo_ByLot"))
                        {
                            string lotStr = reqDict["GetAPSInfo_ByLot"]?.ToString() ?? "";
                            var lotIds = lotStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(lot => lot.Trim()).ToList();
                            var repo = new MockMesRepository();
                            var orders = repo.GetOrders(conn, lotIds);
                            var list = new List<object>();
                            foreach (var o in orders)
                            {
                                list.Add(new {
                                    WONo = o.WorkOrderNumber,
                                    WorkCenterNo = o.StepId,
                                    WorkCenterName = "Mock Center",
                                    WCNext1 = o.NextStepId,
                                    PreStepOutTime = DateTime.TryParse(o.PrevOutTime, out var t) ? t.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                                    Urgent = o.PriorityType > 0 ? "Y" : "N",
                                    EstimateProcessEndDate = DateTime.TryParse(o.DueDate, out var d) ? d.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.AddDays(2).ToString("yyyy-MM-dd HH:mm:ss")
                                });
                            }
                            var reply = new { Result = "success", APSInfo_ByLot_Result = JsonConvert.SerializeObject(list) };
                            responseString = JsonConvert.SerializeObject(reply);
                        }
                        else if (reqDict.ContainsKey("GetAPSInfo_QTime"))
                        {
                            var list = new List<object>();
                            using (var cmd = new SQLiteCommand("SELECT * FROM mock_mes_qtime_rule", conn))
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    list.Add(new {
                                        MatGroupNo = "UP",
                                        RouteNo = "UP-01",
                                        StartWorkcenterNo = reader["step_id"].ToString(),
                                        EndWorkcenterNo = reader["next_step_id"].ToString(),
                                        QuotaTimes = Convert.ToDouble(reader["qtime_limit_min"]),
                                        Enable = "Y"
                                    });
                                }
                            }
                            var reply = new { Result = "success", APSInfo_QTime_Result = JsonConvert.SerializeObject(list) };
                            responseString = JsonConvert.SerializeObject(reply);
                        }
                    }
                    else if (path == "/api/steptime/all" && method == "GET")
                    {
                        var list = new List<StepTimeResponse>();
                        using (var cmd = new SQLiteCommand("SELECT * FROM mock_mes_step_time", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new StepTimeResponse
                                {
                                    StepId = reader["step_id"].ToString(),
                                    EqpGroup = "UNK",
                                    ProcessTimeSec = Convert.ToInt32(reader["std_time_sec"])
                                });
                            }
                        }
                        responseString = JsonConvert.SerializeObject(list);
                    }
                    else if (path == "/api/qtime/all" && method == "GET")
                    {
                         var list = new List<QTimeLimitResponse>();
                        using (var cmd = new SQLiteCommand("SELECT * FROM mock_mes_qtime_rule", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new QTimeLimitResponse
                                {
                                    FromStepId = reader["step_id"].ToString(),
                                    ToStepId = reader["next_step_id"].ToString(),
                                    QTimeLimitMin = Convert.ToInt32(reader["qtime_limit_min"])
                                });
                            }
                        }
                        responseString = JsonConvert.SerializeObject(list);
                    }
                    else if (path == "/api/equipment/batch" && method == "POST")
                    {
                        var eqpIds = JsonConvert.DeserializeObject<List<string>>(requestBody);
                        var list = new List<EqStatusResponse>();
                        string sql = "SELECT eqp_id, status, status_duration, current_work_no FROM mock_mes_equipments";
                        using (var cmd = new SQLiteCommand(sql, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string id = reader["eqp_id"].ToString();
                                if (eqpIds == null || eqpIds.Count == 0 || eqpIds.Contains(id))
                                {
                                    list.Add(new EqStatusResponse
                                    {
                                        EqpId = id,
                                        Status = reader["status"].ToString(),
                                        Duration = reader["status_duration"].ToString(), // DB: status_duration -> JSON: duration
                                        CurrentWorkNo = reader["current_work_no"] == DBNull.Value ? "" : reader["current_work_no"].ToString() // DB: current_work_no -> JSON: current_WorkNo
                                    });
                                }
                            }
                        }
                        responseString = JsonConvert.SerializeObject(list);
                    }
                    else if (path == "/api/wip/batch" && method == "POST")
                    {
                        var eqpIds = JsonConvert.DeserializeObject<List<string>>(requestBody);
                        var list = new List<WipInfoResponse>();
                        string sql = "SELECT eqp_id, current_wip_qty, max_wip_qty FROM mock_mes_equipments";
                         using (var cmd = new SQLiteCommand(sql, conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string id = reader["eqp_id"].ToString();
                                if (eqpIds == null || eqpIds.Count == 0 || eqpIds.Contains(id))
                                {
                                    list.Add(new WipInfoResponse
                                    {
                                        EqpId = id,
                                        CurrentWipQty = Convert.ToInt32(reader["current_wip_qty"]),
                                        MaxWipQty = Convert.ToInt32(reader["max_wip_qty"])
                                    });
                                }
                            }
                        }
                        responseString = JsonConvert.SerializeObject(list);
                    }
                    else if (path == "/api/order/batch" && method == "POST")
                    {
                        var workNos = JsonConvert.DeserializeObject<List<string>>(requestBody);
                        var repo = new MockMesRepository();
                        var orders = repo.GetOrders(conn, workNos);
                        responseString = JsonConvert.SerializeObject(orders);
                    }
                    else
                    {
                        statusCode = 404;
                        responseString = "Not Found";
                    }
                }
            }
            catch (Exception ex)
            {
                statusCode = 500;
                responseString = JsonConvert.SerializeObject(new { error = ex.Message });
                log.Error($"Error processing request: {ex.Message}");
            }

            log.Info($"[Response Status]: {statusCode}");
            if (statusCode == 200)
            {
                try {
                    var parsedJson = JsonConvert.DeserializeObject(responseString);
                    log.Info($"[Response Body]:\n{JsonConvert.SerializeObject(parsedJson, Formatting.Indented)}");
                } catch {
                    log.Info($"[Response Body]: {responseString}");
                }
            }
            log.Info($"------------------------");

            byte[] buffer;
            if (isFormParam && path.EndsWith("/woqry"))
            {
                string responseXml = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<string xmlns=\"CyntecMES\">{responseString}</string>";
                buffer = Encoding.UTF8.GetBytes(responseXml);
                context.Response.ContentType = "text/xml; charset=utf-8";
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentType = "application/json";
            }
            context.Response.StatusCode = statusCode;
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}
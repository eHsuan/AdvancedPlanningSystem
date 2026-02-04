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
                    if (path == "/api/steptime/all" && method == "GET")
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

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}
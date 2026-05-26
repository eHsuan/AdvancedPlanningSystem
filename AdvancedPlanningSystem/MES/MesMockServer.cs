using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using AdvancedPlanningSystem.Models; // Use unified models

namespace AdvancedPlanningSystem.MES
{
    public class MesMockServer
    {
        private HttpListener _listener;
        private bool _isRunning = false;
        private Thread _serverThread;
        private JavaScriptSerializer _serializer;

        public MesMockServer()
        {
            _serializer = new JavaScriptSerializer();
        }

        public void Start(int port = 9000)
        {
            if (_isRunning) return;
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{port}/api/");
                _listener.Start();
                _isRunning = true;
                _serverThread = new Thread(ListenLoop) { IsBackground = true };
                _serverThread.Start();
                Console.WriteLine($"[Mock MES] Server started at http://localhost:{port}/api/");
            }
            catch (Exception ex) { Console.WriteLine($"[Mock MES] Failed: {ex.Message}"); }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
        }

        private void ListenLoop()
        {
            while (_isRunning && _listener.IsListening)
            {
                try { ProcessRequest(_listener.GetContext()); }
                catch (HttpListenerException) { break; }
                catch { }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath.ToLower();
            string method = context.Request.HttpMethod;
            string requestBody = "";
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestBody = reader.ReadToEnd();
            }

            object responseObj = null;

            if (method == "POST")
            {
                if (path.EndsWith("/woqry"))
                {
                    var reqDict = _serializer.Deserialize<Dictionary<string, object>>(requestBody) ?? new Dictionary<string, object>();
                    if (reqDict.ContainsKey("GetAPSInfo_ByEqp"))
                    {
                        string eqpStr = reqDict["GetAPSInfo_ByEqp"]?.ToString() ?? "";
                        var eqpIds = eqpStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var list = new List<ApsEqpInfo>();
                        foreach (var eq in eqpIds)
                        {
                            list.Add(new ApsEqpInfo
                            {
                                MachNo = eq.Trim(),
                                MaxLot = 20,
                                By_Eqp_Now_Used_Lot_Count = 5,
                                StatusCode = "RUN",
                                StatusDurationSec = 120,
                                WONo_List = "LOT-001",
                                MachName = "Mock Machine",
                                MachAlias = "Mock Mach Alias"
                            });
                        }
                        responseObj = new ApsEqpReply
                        {
                            Result = "success",
                            APSInfo_ByEqp_Result = _serializer.Serialize(list)
                        };
                    }
                    else if (reqDict.ContainsKey("GetAPSInfo_ByLot"))
                    {
                        string lotStr = reqDict["GetAPSInfo_ByLot"]?.ToString() ?? "";
                        var lotIds = lotStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        var list = new List<ApsLotInfo>();
                        foreach (var lot in lotIds)
                        {
                            list.Add(new ApsLotInfo
                            {
                                WONo = lot.Trim(),
                                WorkCenterNo = "STEP_A",
                                WCNext1 = "STEP_B",
                                PreStepOutTime = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ss"),
                                Urgent = "N",
                                EstimateProcessEndDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-ddTHH:mm:ss")
                            });
                        }
                        responseObj = new ApsLotReply
                        {
                            Result = "success",
                            APSInfo_ByLot_Result = _serializer.Serialize(list)
                        };
                    }
                    else if (reqDict.ContainsKey("GetAPSInfo_QTime"))
                    {
                        var list = new List<ApsQTimeInfo>
                        {
                            new ApsQTimeInfo
                            {
                                StartWorkcenterNo = "STEP_A",
                                EndWorkcenterNo = "STEP_B",
                                QuotaTimes = 120,
                                Enable = "Y"
                            }
                        };
                        responseObj = new ApsQTimeReply
                        {
                            Result = "success",
                            APSInfo_QTime_Result = _serializer.Serialize(list)
                        };
                    }
                }
                else if (path.EndsWith("/order/batch"))
                {
                    // Mock: Return info for requested WorkNos
                    var workNos = _serializer.Deserialize<List<string>>(requestBody) ?? new List<string>();
                    var list = new List<OrderInfoResponse>();
                    foreach (var wn in workNos)
                    {
                        list.Add(new OrderInfoResponse
                        {
                            WorkNo = wn,
                            carrier_id = "CASS-MOCK",
                            step_id = "STEP_A",
                            next_step_id = "STEP_B",
                            prev_out_time = DateTime.Now.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ss"),
                            priority_type = 2,
                            due_date = DateTime.Now.AddDays(2).ToString("yyyy-MM-ddTHH:mm:ss")
                        });
                    }
                    responseObj = list;
                }
                else if (path.EndsWith("/wip/batch"))
                {
                    var eqps = _serializer.Deserialize<List<string>>(requestBody) ?? new List<string>();
                    var list = new List<WipInfoResponse>();
                    foreach (var eq in eqps)
                    {
                        list.Add(new WipInfoResponse { eq_id = eq, current_wip_qty = 5, max_wip_qty = 20 });
                    }
                    responseObj = list;
                }
                else if (path.EndsWith("/equipment/batch"))
                {
                    var eqps = _serializer.Deserialize<List<string>>(requestBody) ?? new List<string>();
                    var list = new List<EqStatusResponse>();
                    foreach (var eq in eqps)
                    {
                        list.Add(new EqStatusResponse { eqp_id = eq, status = "RUN", duration = "120", current_WorkNo = "LOT-001" });
                    }
                    responseObj = list;
                }
            }
            else if (method == "GET")
            {
                if (path.EndsWith("/steptime/all"))
                {
                    responseObj = new List<StepTimeResponse>
                    {
                        new StepTimeResponse { step_id = "STEP_A", std_time_sec = 3600 },
                        new StepTimeResponse { step_id = "STEP_B", std_time_sec = 1800 }
                    };
                }
                else if (path.EndsWith("/qtime/all"))
                {
                    responseObj = new List<QTimeLimitResponse>
                    {
                        new QTimeLimitResponse { step_id = "STEP_A", next_step_id = "STEP_B", qtime_limit_min = 120 }
                    };
                }
            }

            string responseJson = responseObj != null ? _serializer.Serialize(responseObj) : "{}";
            byte[] buffer = Encoding.UTF8.GetBytes(responseJson);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }
    }
}

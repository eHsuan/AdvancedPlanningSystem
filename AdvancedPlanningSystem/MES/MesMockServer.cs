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
                if (path.EndsWith("/order/batch"))
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

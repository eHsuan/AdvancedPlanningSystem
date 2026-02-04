using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdvancedPlanningSystem.MES;
using System.Web.Script.Serialization;

namespace AdvancedPlanningSystem
{
    public partial class TestForm : Form
    {
        private IMesService _mesService;
        private JavaScriptSerializer _jsonSerializer;

        public TestForm(IMesService mesService)
        {
            InitializeComponent();
            _mesService = mesService;
            _jsonSerializer = new JavaScriptSerializer();
        }

        private void Log(string msg)
        {
            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
        }

        private string FormatJson(object obj)
        {
            try
            {
                // Basic JSON serialization
                string json = _jsonSerializer.Serialize(obj);
                
                // Manual pretty-printing for better readability
                var sb = new StringBuilder();
                int indent = 0;
                bool inString = false;
                
                foreach (char c in json)
                {
                    if (c == '"') inString = !inString;
                    
                    if (!inString)
                    {
                        if (c == '{' || c == '[')
                        {
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            indent++;
                            sb.Append(new string(' ', indent * 2));
                        }
                        else if (c == '}' || c == ']')
                        {
                            sb.Append(Environment.NewLine);
                            indent--;
                            sb.Append(new string(' ', indent * 2));
                            sb.Append(c);
                        }
                        else if (c == ',')
                        {
                            sb.Append(c);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', indent * 2));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"JSON Format Error: {ex.Message}";
            }
        }

        private async void btnTestGetWip_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetWipBatchAsync...");
                string input = txtInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    input = "EQP01,EQP02,EQP03"; // Default test IDs
                    Log($"Using default IDs: {input}");
                }
                
                var ids = input.Split(',').Select(s => s.Trim()).ToList();
                var result = await _mesService.GetWipBatchAsync(ids);
                
                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestGetEqStatus_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetEquipmentStatusBatchAsync...");
                string input = txtInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    input = "EQP01,EQP02,EQP03"; 
                    Log($"Using default IDs: {input}");
                }

                var ids = input.Split(',').Select(s => s.Trim()).ToList();
                var result = await _mesService.GetEquipmentStatusBatchAsync(ids);

                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestGetOrder_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetOrderInfoBatchAsync...");
                string input = txtInput.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    input = "LOT001,LOT002"; 
                    Log($"Using default IDs: {input}");
                }

                var ids = input.Split(',').Select(s => s.Trim()).ToList();
                var result = await _mesService.GetOrderInfoBatchAsync(ids);

                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestQTime_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetAllQTimeLimitsAsync...");
                var result = await _mesService.GetAllQTimeLimitsAsync();
                
                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void btnTestStepTime_Click(object sender, EventArgs e)
        {
            try
            {
                Log("Executing GetAllStepTimesAsync...");
                var result = await _mesService.GetAllStepTimesAsync();

                Log($"Result Count: {result.Count}");
                txtOutput.AppendText(FormatJson(result) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }
    }
}

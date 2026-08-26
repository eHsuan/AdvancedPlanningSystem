using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AdvancedPlanningSystem.Services;
using AdvancedPlanningSystem.Models;
using AdvancedPlanningSystem.Repositories;
using AdvancedPlanningSystem.MES;

namespace AdvancedPlanningSystem.Tests.Services
{
    public class DispatchServiceTests
    {
        private Mock<IApsLocalDbRepository> _mockRepo;
        private Mock<IApsCloudDbRepository> _mockCloud;
        private Mock<ITcpServerModule> _mockTcp;
        private Mock<IMesService> _mockMes;
        private DispatchService _dispatchService;
        private DataSyncService _dataSyncService;

        public DispatchServiceTests()
        {
            _mockRepo = new Mock<IApsLocalDbRepository>();
            _mockCloud = new Mock<IApsCloudDbRepository>();
            _mockTcp = new Mock<ITcpServerModule>();
            _mockMes = new Mock<IMesService>();

            _dispatchService = new DispatchService(_mockRepo.Object, _mockCloud.Object, _mockTcp.Object, null);
            
            // 建立 DataSyncService 並初始化內部狀態
            _dataSyncService = new DataSyncService(_mockMes.Object, _mockRepo.Object, _mockCloud.Object, _dispatchService, null);
            _dispatchService.SetDataSyncService(_dataSyncService);

            // 預設 Mock 設定
            _mockRepo.Setup(r => r.GetAllBindings()).Returns(new List<StateBinding>());
            _mockRepo.Setup(r => r.GetAllTransits()).Returns(new List<StateTransit>());
            _mockRepo.Setup(r => r.GetQTimeConfigs()).Returns(new List<ConfigQTime>());

            AppConfig.BypassedPorts = "";
        }

        [Fact]
        public async Task ExecuteDispatch_FullBatch_ShouldTriggerOpenCommand()
        {
            // --- Arrange ---
            string stepId = "STEP_TEST";
            string eqpId = "EQP_TEST";
            int batchSize = 4;

            var candidates = new List<StateBinding>();
            for (int i = 1; i <= batchSize; i++) {
                candidates.Add(new StateBinding { 
                    CarrierId = $"CST{i:D2}", LotId = $"LOT{i:D2}", 
                    NextStepId = stepId, MachNosNext1 = eqpId, PortId = $"P{i:D2}", IsHold = 0 
                });
            }
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);

            _mockRepo.Setup(r => r.GetEqpConfig(eqpId)).Returns(new ConfigEqp { 
                EqpId = eqpId, BatchSize = batchSize, MaxWipQty = 10 
            });

            // 直接設定 internal 快取
            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse> {
                { eqpId, new WipInfoResponse { eq_id = eqpId, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse> {
                { eqpId, new EqStatusResponse { eqp_id = eqpId, status = "IDLE", duration = "100" } }
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert ---
            _mockTcp.Verify(t => t.SendCommand(It.Is<string>(s => s.StartsWith("OPEN,"))), Times.Exactly(batchSize));
            _mockRepo.Verify(r => r.InsertBinding(It.Is<StateBinding>(b => !string.IsNullOrEmpty(b.DispatchTime))), Times.Exactly(batchSize));
        }

        [Fact]
        public async Task ExecuteDispatch_UnderBatchSize_ShouldNotDispatch()
        {
            // --- Arrange ---
            string stepId = "STEP_TEST";
            string eqpId = "EQP_TEST";
            int batchSize = 4;

            var candidates = new List<StateBinding>();
            for (int i = 1; i <= 3; i++) {
                candidates.Add(new StateBinding { 
                    CarrierId = $"CST{i:D2}", NextStepId = stepId, MachNosNext1 = eqpId, PortId = $"P{i:D2}", IsHold = 0 
                });
            }
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);
            _mockRepo.Setup(r => r.GetEqpConfig(eqpId)).Returns(new ConfigEqp { 
                EqpId = eqpId, BatchSize = batchSize, MaxWipQty = 10, ForceIdleSec = 9999 
            });

            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse> {
                { eqpId, new WipInfoResponse { eq_id = eqpId, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse> {
                { eqpId, new EqStatusResponse { eqp_id = eqpId, status = "IDLE", duration = "10" } }
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert ---
            _mockTcp.Verify(t => t.SendCommand(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task ExecuteDispatch_BypassedPort_ShouldNotDispatch()
        {
            // --- Arrange ---
            AppConfig.BypassedPorts = "4,6";
            Assert.True(AppConfig.IsPortBypassed("P04"));
            Assert.True(AppConfig.IsPortBypassed("4"));
            Assert.False(AppConfig.IsPortBypassed("P01"));

            string stepId = "STEP_TEST";
            string eqpId = "EQP_TEST";
            int batchSize = 4;

            var candidates = new List<StateBinding>();
            for (int i = 1; i <= batchSize; i++) {
                candidates.Add(new StateBinding { 
                    CarrierId = $"CST{i:D2}", LotId = $"LOT{i:D2}", 
                    NextStepId = stepId, MachNosNext1 = eqpId, PortId = $"P{i:D2}", IsHold = 0 
                });
            }
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);

            _mockRepo.Setup(r => r.GetEqpConfig(eqpId)).Returns(new ConfigEqp { 
                EqpId = eqpId, BatchSize = batchSize, MaxWipQty = 10 
            });

            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse> {
                { eqpId, new WipInfoResponse { eq_id = eqpId, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse> {
                { eqpId, new EqStatusResponse { eqp_id = eqpId, status = "IDLE", duration = "100" } }
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert --- P04 被 Bypass 攔截，因此只有 3 個成功下達 OPEN 指令
            _mockTcp.Verify(t => t.SendCommand(It.Is<string>(s => s.StartsWith("OPEN,"))), Times.Exactly(3));

            // 清除靜態狀態避免影響其他測試
            AppConfig.BypassedPorts = "";
        }

        [Fact]
        public async Task ExecuteDispatch_N2Enabled_AllN2Down_ShouldBlockDispatchAndSetWaitReason()
        {
            // --- Arrange ---
            AppConfig.EnableLookAheadN2 = true;
            string stepId = "STEP_TEST";
            string eqpN1 = "EQP_N1";
            string eqpN2A = "EQP_N2A";
            string eqpN2B = "EQP_N2B";
            int batchSize = 1;

            var candidates = new List<StateBinding>
            {
                new StateBinding
                {
                    CarrierId = "CST_N2_TEST",
                    LotId = "LOT_N2_TEST",
                    NextStepId = stepId,
                    MachNosNext1 = eqpN1,
                    MachNosNext2 = $"{eqpN2A},{eqpN2B}",
                    PortId = "P01",
                    IsHold = 0
                }
            };
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);
            _mockRepo.Setup(r => r.GetEqpConfig(eqpN1)).Returns(new ConfigEqp
            {
                EqpId = eqpN1,
                BatchSize = batchSize,
                MaxWipQty = 10
            });

            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse>
            {
                { eqpN1, new WipInfoResponse { eq_id = eqpN1, current_wip_qty = 0, max_wip_qty = 10 } },
                { eqpN2A, new WipInfoResponse { eq_id = eqpN2A, current_wip_qty = 0, max_wip_qty = 10 } },
                { eqpN2B, new WipInfoResponse { eq_id = eqpN2B, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse>
            {
                { eqpN1, new EqStatusResponse { eqp_id = eqpN1, status = "IDLE", duration = "100" } },
                { eqpN2A, new EqStatusResponse { eqp_id = eqpN2A, status = "DOWN", duration = "500" } },
                { eqpN2B, new EqStatusResponse { eqp_id = eqpN2B, status = "ALARM", duration = "300" } }
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            StateBinding capturedBinding = null;
            _mockRepo.Setup(r => r.InsertBinding(It.IsAny<StateBinding>()))
                     .Callback<StateBinding>(b => capturedBinding = b);

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert ---
            // 1. 絕對不能發送 OPEN 指令
            _mockTcp.Verify(t => t.SendCommand(It.IsAny<string>()), Times.Never());

            // 2. 必須更新等待原因為 N+2 All Eqp DOWN
            Assert.NotNull(capturedBinding);
            Assert.Equal("N+2 All Eqp DOWN (Q-Time Guard)", capturedBinding.WaitReason);
            Assert.Null(capturedBinding.DispatchTime);

            // 清除狀態
            AppConfig.EnableLookAheadN2 = false;
        }

        [Fact]
        public async Task ExecuteDispatch_N2Enabled_AtLeastOneN2Active_ShouldDispatch()
        {
            // --- Arrange ---
            AppConfig.EnableLookAheadN2 = true;
            string stepId = "STEP_TEST";
            string eqpN1 = "EQP_N1";
            string eqpN2A = "EQP_N2A";
            string eqpN2B = "EQP_N2B";
            int batchSize = 1;

            var candidates = new List<StateBinding>
            {
                new StateBinding
                {
                    CarrierId = "CST_N2_OK",
                    LotId = "LOT_N2_OK",
                    NextStepId = stepId,
                    MachNosNext1 = eqpN1,
                    MachNosNext2 = $"{eqpN2A},{eqpN2B}",
                    PortId = "P01",
                    IsHold = 0
                }
            };
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);
            _mockRepo.Setup(r => r.GetEqpConfig(eqpN1)).Returns(new ConfigEqp
            {
                EqpId = eqpN1,
                BatchSize = batchSize,
                MaxWipQty = 10
            });

            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse>
            {
                { eqpN1, new WipInfoResponse { eq_id = eqpN1, current_wip_qty = 0, max_wip_qty = 10 } },
                { eqpN2A, new WipInfoResponse { eq_id = eqpN2A, current_wip_qty = 0, max_wip_qty = 10 } },
                { eqpN2B, new WipInfoResponse { eq_id = eqpN2B, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse>
            {
                { eqpN1, new EqStatusResponse { eqp_id = eqpN1, status = "IDLE", duration = "100" } },
                { eqpN2A, new EqStatusResponse { eqp_id = eqpN2A, status = "DOWN", duration = "500" } },
                { eqpN2B, new EqStatusResponse { eqp_id = eqpN2B, status = "RUN", duration = "300" } } // 至少一台 RUN
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert ---
            _mockTcp.Verify(t => t.SendCommand(It.Is<string>(s => s.StartsWith("OPEN,P01,EQP_N1"))), Times.Once());

            // 清除狀態
            AppConfig.EnableLookAheadN2 = false;
        }

        [Fact]
        public async Task ExecuteDispatch_N2Disabled_AllN2Down_ShouldStillDispatch()
        {
            // --- Arrange ---
            AppConfig.EnableLookAheadN2 = false; // 關閉功能 (保持原始行為)
            string stepId = "STEP_TEST";
            string eqpN1 = "EQP_N1";
            string eqpN2A = "EQP_N2A";
            int batchSize = 1;

            var candidates = new List<StateBinding>
            {
                new StateBinding
                {
                    CarrierId = "CST_N2_OFF",
                    LotId = "LOT_N2_OFF",
                    NextStepId = stepId,
                    MachNosNext1 = eqpN1,
                    MachNosNext2 = eqpN2A,
                    PortId = "P01",
                    IsHold = 0
                }
            };
            _mockRepo.Setup(r => r.GetSortedWaitBindings()).Returns(candidates);
            _mockRepo.Setup(r => r.GetEqpConfig(eqpN1)).Returns(new ConfigEqp
            {
                EqpId = eqpN1,
                BatchSize = batchSize,
                MaxWipQty = 10
            });

            _dataSyncService._cachedWip = new Dictionary<string, WipInfoResponse>
            {
                { eqpN1, new WipInfoResponse { eq_id = eqpN1, current_wip_qty = 0, max_wip_qty = 10 } },
                { eqpN2A, new WipInfoResponse { eq_id = eqpN2A, current_wip_qty = 0, max_wip_qty = 10 } }
            };
            _dataSyncService._cachedEqpStatus = new Dictionary<string, EqStatusResponse>
            {
                { eqpN1, new EqStatusResponse { eqp_id = eqpN1, status = "IDLE", duration = "100" } },
                { eqpN2A, new EqStatusResponse { eqp_id = eqpN2A, status = "DOWN", duration = "500" } }
            };
            _dataSyncService._lastMesSyncTime = DateTime.Now;

            // --- Act ---
            await _dispatchService.ExecuteDispatchAsync();

            // --- Assert ---
            _mockTcp.Verify(t => t.SendCommand(It.Is<string>(s => s.StartsWith("OPEN,P01,EQP_N1"))), Times.Once());
        }
    }
}
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

            _dispatchService = new DispatchService(_mockRepo.Object, _mockCloud.Object, _mockTcp.Object);
            
            // 建立 DataSyncService 並初始化內部狀態
            _dataSyncService = new DataSyncService(_mockMes.Object, _mockRepo.Object, _mockCloud.Object, _dispatchService, null);
            _dispatchService.SetDataSyncService(_dataSyncService);

            // 預設 Mock 設定
            _mockRepo.Setup(r => r.GetAllBindings()).Returns(new List<StateBinding>());
            _mockRepo.Setup(r => r.GetAllTransits()).Returns(new List<StateTransit>());
            _mockRepo.Setup(r => r.GetQTimeConfigs()).Returns(new List<ConfigQTime>());
        }

        [Fact]
        public async Task ExecuteDispatch_FullBatch_ShouldTriggerOpenCommand()
        {
            // --- Arrange ---
            string stepId = "STEP_TEST";
            string eqpId = "EQP_TEST";
            int batchSize = 4;

            _mockRepo.Setup(r => r.GetStepEqpMappings()).Returns(new List<ConfigStepEqp> {
                new ConfigStepEqp { StepId = stepId, EqpId = eqpId }
            });

            var candidates = new List<StateBinding>();
            for (int i = 1; i <= batchSize; i++) {
                candidates.Add(new StateBinding { 
                    CarrierId = $"CST{i:D2}", LotId = $"LOT{i:D2}", 
                    NextStepId = stepId, PortId = $"P{i:D2}", IsHold = 0 
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

            _mockRepo.Setup(r => r.GetStepEqpMappings()).Returns(new List<ConfigStepEqp> {
                new ConfigStepEqp { StepId = stepId, EqpId = eqpId }
            });

            var candidates = new List<StateBinding>();
            for (int i = 1; i <= 3; i++) {
                candidates.Add(new StateBinding { 
                    CarrierId = $"CST{i:D2}", NextStepId = stepId, PortId = $"P{i:D2}", IsHold = 0 
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
    }
}
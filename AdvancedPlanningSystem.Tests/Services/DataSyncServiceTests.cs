using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AdvancedPlanningSystem.Services;
using AdvancedPlanningSystem.Models;
using AdvancedPlanningSystem.MES;
using AdvancedPlanningSystem.Repositories;

namespace AdvancedPlanningSystem.Tests.Services
{
    public class DataSyncServiceTests
    {
        private Mock<IMesService> _mockMes;
        private Mock<IApsLocalDbRepository> _mockRepo;
        private Mock<IApsCloudDbRepository> _mockCloud;
        private Mock<DispatchService> _mockDispatch;
        private DataSyncService _service;

        public DataSyncServiceTests()
        {
            _mockMes = new Mock<IMesService>();
            _mockRepo = new Mock<IApsLocalDbRepository>();
            _mockCloud = new Mock<IApsCloudDbRepository>();
            
            var tcpMock = new Mock<TcpServerModule>();
            _mockDispatch = new Mock<DispatchService>(_mockRepo.Object, _mockCloud.Object, tcpMock.Object, null);

            _service = new DataSyncService(
                _mockMes.Object, 
                _mockRepo.Object, 
                _mockCloud.Object, 
                _mockDispatch.Object, 
                null);

            // 全域 Mock 設定：避免 NullReference
            _mockRepo.Setup(r => r.GetBinding(It.IsAny<string>()))
                     .Returns((string cid) => new StateBinding { CarrierId = cid });
            _mockRepo.Setup(r => r.GetStepEqpMappings()).Returns(new List<ConfigStepEqp>());
            _mockRepo.Setup(r => r.GetAllTransits()).Returns(new List<StateTransit>());
            _mockRepo.Setup(r => r.GetQTimeConfigs()).Returns(new List<ConfigQTime>());
        }

        [Fact]
        public void CalculateScore_GreenZone_ShouldHaveNormalScore()
        {
            // Arrange
            var port = new StatePort { CarrierId = "CST01", LotId = "LOT01", PortId = "P01" };
            var order = new OrderInfoResponse 
            { 
                WorkNo = "LOT01", 
                step_id = "STEP1", 
                next_step_id = "STEP2",
                prev_out_time = DateTime.Now.AddMinutes(-30).ToString("yyyyMMddHHmmss"), 
                priority_type = 0
            };
            
            StateBinding capturedBinding = null;
            _mockRepo.Setup(r => r.InsertBinding(It.IsAny<StateBinding>()))
                     .Callback<StateBinding>(b => capturedBinding = b);

            // Act
            _service.ProcessBindingSyncAndScore(port, order);

            // Assert
            Assert.NotNull(capturedBinding);
            Assert.Equal(0, capturedBinding.IsHold);
            Assert.True(capturedBinding.DispatchScore > 0);
        }

        [Fact]
        public void CalculateScore_DeadZone_ShouldBeHold()
        {
            // Arrange
            var port = new StatePort { CarrierId = "CST_DEAD", LotId = "LOT_DEAD", PortId = "P02" };
            var order = new OrderInfoResponse 
            { 
                WorkNo = "LOT_DEAD", 
                step_id = "STEP1", 
                next_step_id = "STEP2",
                prev_out_time = DateTime.Now.AddMinutes(-1000).ToString("yyyyMMddHHmmss"), 
                priority_type = 0
            };
            
            StateBinding capturedBinding = null;
            _mockRepo.Setup(r => r.InsertBinding(It.IsAny<StateBinding>()))
                     .Callback<StateBinding>(b => capturedBinding = b);

            // Act
            _service.ProcessBindingSyncAndScore(port, order);

            // Assert
            Assert.NotNull(capturedBinding);
            Assert.Equal(1, capturedBinding.IsHold); 
            Assert.Equal(0, capturedBinding.DispatchScore);
        }

        [Fact]
        public void CalculateScore_UrgentLot_ShouldAddBonusScore()
        {
            // Arrange
            var port = new StatePort { CarrierId = "CST_URGENT", LotId = "LOT_URGENT", PortId = "P03" };
            var order = new OrderInfoResponse 
            { 
                WorkNo = "LOT_URGENT", 
                step_id = "STEP1", 
                next_step_id = "STEP2",
                prev_out_time = DateTime.Now.AddMinutes(-10).ToString("yyyyMMddHHmmss"),
                priority_type = 2 
            };
            
            StateBinding capturedBinding = null;
            _mockRepo.Setup(r => r.InsertBinding(It.IsAny<StateBinding>()))
                     .Callback<StateBinding>(b => capturedBinding = b);

            // Act
            _service.ProcessBindingSyncAndScore(port, order);

            // Assert
            Assert.NotNull(capturedBinding);
            Assert.Equal(100000.0, capturedBinding.ScoreUrgent); 
        }

        [Fact]
        public async Task UpdateMesCache_ShouldCallGetEqpBatchInfoAndPopulateCache()
        {
            // Arrange
            var eqpIds = new List<string> { "EQP01" };
            _mockRepo.Setup(r => r.GetStepEqpMappings()).Returns(new List<ConfigStepEqp> {
                new ConfigStepEqp { StepId = "STEP1", EqpId = "EQP01" }
            });

            var batchInfo = new EqpBatchInfoResponse();
            batchInfo.Wips.Add(new WipInfoResponse { eq_id = "EQP01", current_wip_qty = 2, max_wip_qty = 8 });
            batchInfo.Statuses.Add(new EqStatusResponse { eqp_id = "EQP01", status = "RUN", duration = "120" });

            _mockMes.Setup(m => m.GetEqpBatchInfoAsync(It.IsAny<List<string>>()))
                    .ReturnsAsync(batchInfo);

            // Act
            await _service.ForceUpdateCacheAsync();

            // Assert
            _mockMes.Verify(m => m.GetEqpBatchInfoAsync(It.Is<List<string>>(list => list.Contains("EQP01"))), Times.Once);
            
            var cachedWip = _service.GetCachedWip();
            var cachedStatus = _service.GetCachedEqStatus();

            Assert.True(cachedWip.ContainsKey("EQP01"));
            Assert.Equal(2, cachedWip["EQP01"].current_wip_qty);
            Assert.Equal(8, cachedWip["EQP01"].max_wip_qty);

            Assert.True(cachedStatus.ContainsKey("EQP01"));
            Assert.Equal("RUN", cachedStatus["EQP01"].status);
            Assert.Equal("120", cachedStatus["EQP01"].duration);
        }
    }
}
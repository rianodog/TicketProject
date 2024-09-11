using Moq;
using TicketProject.DAL.Implement;
using TicketProject.Models.Entity;
using TicketProject.Services.Interfaces;
using TicketProject.DAL;
using Moq.EntityFrameworkCore;

namespace TicketProject.Tests.DAL.Implement
{
    /// <summary>
    /// DAL層與Redis的簡單測試，用於確認功能區塊正常，不會每個都寫測試。
    /// CampaignReadDao 的單元測試類別。
    /// </summary>
    public class CampaignReadDaoTests
    {
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly Mock<ReadTicketDbContext> _mockDbContext;
        private readonly Mock<IErrorHandler<CampaignReadDao>> _mockErrorHandler;
        private readonly CampaignReadDao _campaignReadDao;

        /// <summary>
        /// 初始化 CampaignReadDaoTests 類別的新實例。
        /// </summary>
        public CampaignReadDaoTests()
        {
            _mockRedisService = new Mock<IRedisService>();
            _mockErrorHandler = new Mock<IErrorHandler<CampaignReadDao>>();
            _mockDbContext = new Mock<ReadTicketDbContext>();

            _campaignReadDao = new CampaignReadDao(_mockDbContext.Object, _mockErrorHandler.Object, _mockRedisService.Object);
        }

        /// <summary>
        /// 測試 GetCampaignAsync 方法是否能從快取中返回資料。
        /// </summary>
        [Fact]
        public async Task GetCampaignAsync_ShouldReturnFromCache_IfCacheExists()
        {
            // Arrange
            var cacheKey = "SampleCity";
            var cachedData = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };

            _mockRedisService.Setup(r => r.GetCacheAsync<List<Campaign>>(It.IsAny<string>()))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _campaignReadDao.GetCampaignAsync(c => c.CampaignName == "Test Campaign", cacheKey);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
            _mockRedisService.Verify(r => r.GetCacheAsync<List<Campaign>>($"Campaigns:City:{cacheKey}"), Times.Once);
            _mockDbContext.Verify(c => c.Campaigns, Times.Never);
        }

        /// <summary>
        /// 測試 GetCampaignAsync 方法在快取未命中時是否能從資料庫中獲取資料並設置快取。
        /// </summary>
        [Fact]
        public async Task GetCampaignAsync_ShouldFetchFromDbAndSetCache_IfCacheMiss()
        {
            // Arrange
            var cacheKey = "SampleCity";
            var dbData = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };

            _mockRedisService.Setup(r => r.GetCacheAsync<List<Campaign>>(It.IsAny<string>()))
                .ReturnsAsync((List<Campaign>?)null);

            _mockDbContext.Setup(c => c.Campaigns).ReturnsDbSet(dbData);

            // Act
            var result = await _campaignReadDao.GetCampaignAsync(c => c.CampaignName == "Test Campaign", cacheKey);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
            _mockRedisService.Verify(r => r.GetCacheAsync<List<Campaign>>($"Campaigns:City:{cacheKey}"), Times.Once);
            _mockDbContext.Verify(c => c.Campaigns, Times.Once);
            _mockRedisService.Verify(r => r.SetCacheAsync($"Campaigns:City:{cacheKey}", dbData, null), Times.Once);
        }

        /// <summary>
        /// 測試 GetCampaignAsync 方法在未提供快取鍵時是否能從資料庫中返回資料。
        /// </summary>
        [Fact]
        public async Task GetCampaignAsync_ShouldReturnFromDbIfNoCacheKeyProvided()
        {
            // Arrange
            var dbData = new List<Campaign> { new Campaign { CampaignId = 1, CampaignName = "Test Campaign" } };

            _mockDbContext.Setup(c => c.Campaigns).ReturnsDbSet(dbData);

            // Act
            var result = await _campaignReadDao.GetCampaignAsync(c => c.CampaignName == "Test Campaign", string.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Campaign", result.First().CampaignName);
            _mockRedisService.Verify(r => r.GetCacheAsync<List<Campaign>>(It.IsAny<string>()), Times.Never);
            _mockDbContext.Verify(c => c.Campaigns, Times.Once);
        }

        /// <summary>
        /// 測試 GetCampaignAsync 方法在發生異常時是否能正確處理。
        /// </summary>
        [Fact]
        public async Task GetCampaignAsync_ShouldThrowException_AndHandleError()
        {
            // Arrange
            var cacheKey = "SampleCity";
            var exception = new Exception("Something went wrong");

            _mockRedisService.Setup(r => r.GetCacheAsync<List<Campaign>>(It.IsAny<string>()))
                .ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _campaignReadDao.GetCampaignAsync(c => c.CampaignName == "Test Campaign", cacheKey));
            _mockErrorHandler.Verify(e => e.HandleError(exception), Times.Once);
        }
    }
}
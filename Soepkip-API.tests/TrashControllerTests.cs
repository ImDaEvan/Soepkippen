using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SoepkipAPI.Controllers;
using SoepkipAPI.Data.Interfaces;
using SoepkipAPI.Models;
using SoepkipAPI.Services;

namespace SoepkipAPI.tests
{
    [TestClass]
    public sealed class TrashControllerTests
    {
        public TestContext? TestContext { get; set; }
        private const string ISO = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
        private Mock<ITrashRepository> _repo = null!;
        private Mock<ILogger<TrashController>> _log = null!;
        private Mock<IWeatherService> _weatherService = null!;
        private TrashController _sut = null!;

        [TestInitialize]
        public void SetUp()
        {
            _repo = new Mock<ITrashRepository>(MockBehavior.Strict);
            _log = new Mock<ILogger<TrashController>>();
            _sut = new TrashController(_repo.Object, _log.Object, weatherService: null!);
            _weatherService = new Mock<IWeatherService>();
        }

        [TestMethod]
        public void ValidRequest_ReturnsOk_WithPayload()
        {
            // Arrange
            var from = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc);
            var sample = new List<TrashItem> {
                new() { id = "1", timestamp = from, type = "cup",  confidence = .9f },
                new() { id = "2", timestamp = to,   type = "bottle",confidence = .8f }
            };

            _repo.Setup(r => r.TryParseIsoUtc(from.ToString(ISO), out from)).Returns(true);
            _repo.Setup(r => r.TryParseIsoUtc(to.ToString(ISO), out to)).Returns(true);
            _repo.Setup(r => r.ReadRange(from, to)).Returns(sample);

            // Act
            var result = _sut.GetTrash(from.ToString(ISO), to.ToString(ISO));

            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            CollectionAssert.AreEqual(sample, ((IEnumerable<TrashItem>)ok.Value!).ToList());
        }

        [DataTestMethod]
        [DataRow("wrong", "2025-06-30T23:59:59.999Z")]
        [DataRow("2025-06-01T00:00:00.000Z", "wrong")]
        public void InvalidDateFormat_ReturnsBadRequest(string left, string right)
        {
            // Arrange
            _repo.Setup(r => r.TryParseIsoUtc(left, out It.Ref<DateTime>.IsAny)).Returns(false);
            _repo.Setup(r => r.TryParseIsoUtc(right, out It.Ref<DateTime>.IsAny)).Returns(false);
            // Act
            var result = _sut.GetTrash(left, right);
            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void LeftIsBiggerThanRight_ReturnsBadRequest()
        {
            // Arrange
            var left = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc);
            var right = new DateTime(2025, 6, 1, 23, 59, 59, DateTimeKind.Utc);
            _repo.Setup(r => r.TryParseIsoUtc(left.ToString(ISO), out left)).Returns(true);
            _repo.Setup(r => r.TryParseIsoUtc(right.ToString(ISO), out right)).Returns(true);
            // Act
            var result = _sut.GetTrash(left.ToString(ISO), right.ToString(ISO));
            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void NoTrashFound_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var from = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc);
            _repo.Setup(r => r.TryParseIsoUtc(from.ToString(ISO), out from)).Returns(true);
            _repo.Setup(r => r.TryParseIsoUtc(to.ToString(ISO), out to)).Returns(true);
            _repo.Setup(r => r.ReadRange(from, to)).Returns(new List<TrashItem>());
            // Act
            var result = _sut.GetTrash(from.ToString(ISO), to.ToString(ISO));
            // Assert
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok, "Controller should return 200 OK.");

            var payload = ok.Value as IEnumerable<TrashItem>;
            Assert.IsNotNull(payload, "Payload should be a list.");
            Assert.AreEqual(0, payload.Count(), "List should be empty.");
        }

        [TestMethod]
        public async Task ValidPost_ReturnsData_WithWeatherAsync()
        {
            //Arrange
            var trash = new TrashItem
            {
                id = "1",
                timestamp = DateTime.UtcNow,
                type = "cardboard",
                confidence = 0.84f,
                longitude = 51.58656f,
                latitude = 4.77596f,
            };

            var responseWeather = new WeatherData
            {
                Plaats = "Breda",
                Temp = 21.1f,
                GTemp = 22.1f,
                WindrGr = 28.5f,
                WindMs = 8.3f,
                WindBft = 4.0f,
                Time = "12-06-2025 16:03",
                Timestamp = "1749736980"
            };

            _weatherService.Setup(w => w.GetWeatherAsync((float)trash.longitude, (float)trash.latitude)).ReturnsAsync(responseWeather);

            _repo.Setup(r => r.Write(It.IsAny<TrashItem>()));
            _repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            _sut = new TrashController(_repo.Object, _log.Object, _weatherService.Object);

            // Act
            var result = await _sut.Write(new List<TrashItem> { trash });

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(1, okResult.Value, "Expected 1 row affected");

            _weatherService.Verify(w =>
                w.GetWeatherAsync((float)trash.longitude, (float)trash.latitude),
                Times.Once);

            _repo.Verify(r => r.Write(It.Is<TrashItem>(t =>
                t.actual_temp_celsius == responseWeather.Temp &&
                t.feels_like_temp_celsius == responseWeather.GTemp &&
                t.wind_force_bft == responseWeather.WindBft &&
                t.wind_direction == responseWeather.WindrGr &&
                t.weather_timestamp == responseWeather.ParsedTime
            )), Times.Once);

            _repo.Verify(r => r.SaveChangesAsync(), Times.Once);

        }
    }
}

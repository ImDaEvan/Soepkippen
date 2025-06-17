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
        private TrashController _sut = null!;

        [TestInitialize]
        public void SetUp()
        {
            _repo = new Mock<ITrashRepository>(MockBehavior.Strict);
            _log = new Mock<ILogger<TrashController>>();
            _sut = new TrashController(_repo.Object, _log.Object, weatherService: null!);
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
    }
}

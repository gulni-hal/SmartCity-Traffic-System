using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dispatcher.Application.Services;
using Xunit;
using FluentAssertions;
using Dispatcher.Application;

namespace Dispatcher.UnitTests
{
    public class SpeedControlServiceTests
    {
        [Fact]
        public void IsSpeedLimitExceeded_ShouldReturnTrue_WhenSpeedAboveLimit()
        {
            // Arrange
            var service = new SpeedControlService();

            // Act
            var result = service.IsSpeedLimitExceeded(120);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsSpeedLimitExceeded_ShouldReturnFalse_WhenSpeedBelowLimit()
        {
            // Arrange
            var service = new SpeedControlService();

            // Act
            var result = service.IsSpeedLimitExceeded(70);

            // Assert
            result.Should().BeFalse();
        }
    }
}

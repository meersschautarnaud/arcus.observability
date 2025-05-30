﻿using System;
using System.Collections.Generic;
using Arcus.Observability.Telemetry.Core;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Arcus.Observability.Tests.Unit.Telemetry.Logging
{
    [Trait("Category", "Unit")]
    public class SecurityEventLoggingTests
    {
        [Fact]
        public void LogSecurityEvent_ValidArguments_Succeeds()
        {
            // Arrange
            var logger = new TestLogger();
            const string message = "something was invalidated wrong";

            // Act
            logger.LogSecurityEvent(message);

            // Assert
            string logMessage = logger.WrittenMessage;
            Assert.Contains(TelemetryType.Events.ToString(), logMessage);
            Assert.Contains(message, logMessage);
            Assert.Contains("[EventType, Security]", logMessage);
        }

        [Fact]
        public void LogSecurityEvent_ValidArgumentsWithContext_Succeeds()
        {
            // Arrange
            var logger = new TestLogger();
            const string message = "something was invalidated wrong";
            var telemetryContext = new Dictionary<string, object>
            {
                ["Property"] = "something was wrong with this Property"
            };

            // Act
            logger.LogSecurityEvent(message, telemetryContext);

            // Assert
            string logMessage = logger.WrittenMessage;
            Assert.Contains(TelemetryType.Events.ToString(), logMessage);
            Assert.Contains(message, logMessage);
            Assert.Contains("[EventType, Security]", logMessage);
            Assert.Contains("[Property, something was wrong with this Property]", logMessage);
        }

        [Fact]
        public void LogSecurityEvent_WithNoEventName_ThrowsException()
        {
            // Arrange
            var logger = new TestLogger();
            string eventName = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => logger.LogSecurityEvent(eventName));
        }
    }
}

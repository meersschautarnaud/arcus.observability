﻿using System;
using System.Collections.Generic;
using Arcus.Observability.Telemetry.Core;
using Arcus.Observability.Telemetry.Core.Logging;
using Arcus.Observability.Telemetry.Serilog.Sinks.ApplicationInsights.Configuration;
using GuardNet;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Events;

namespace Arcus.Observability.Telemetry.Serilog.Sinks.ApplicationInsights.Converters
{
    /// <summary>
    /// Represents a conversion from a Serilog <see cref="LogEvent"/> to an <see cref="DependencyTelemetry"/> instance.
    /// </summary>
    public class DependencyTelemetryConverter : CustomTelemetryConverter<DependencyTelemetry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyTelemetryConverter" /> class.
        /// </summary>
        [Obsolete("Use the constructor overload with the Application Insights options instead")]
        public DependencyTelemetryConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyTelemetryConverter" /> class.
        /// </summary>
        /// <param name="options">The user-defined configuration options to influence the behavior of the Application Insights Serilog sink.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="options"/> is <c>null</c>.</exception>
        public DependencyTelemetryConverter(ApplicationInsightsSinkOptions options) : base(options)
        {
        }

        /// <summary>
        ///     Creates a telemetry entry for a given log event
        /// </summary>
        /// <param name="logEvent">Event that was logged and written to this sink</param>
        /// <param name="formatProvider">Provider to format event</param>
        /// <returns>Telemetry entry to emit to Azure Application Insights</returns>
        protected override DependencyTelemetry CreateTelemetryEntry(LogEvent logEvent, IFormatProvider formatProvider)
        {
            Guard.NotNull(logEvent, nameof(logEvent), "Requires a Serilog log event to create an Azure Application Insights Dependency telemetry instance");
            Guard.NotNull(logEvent.Properties, nameof(logEvent), "Requires a Serilog event with a set of properties to create an Azure Application Insights Dependency telemetry instance");

            StructureValue logEntry = logEvent.Properties.GetAsStructureValue(ContextProperties.DependencyTracking.DependencyLogEntry);
            string dependencyId = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.DependencyId));
            string dependencyType = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.DependencyType));
            string dependencyName = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.DependencyName));
            string target = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.TargetName));
            string data = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.DependencyData));
            DateTimeOffset startTime = logEntry.Properties.GetAsDateTimeOffset(nameof(DependencyLogEntry.StartTime));
            TimeSpan duration = logEntry.Properties.GetAsTimeSpan(nameof(DependencyLogEntry.Duration));
            string resultCode = logEntry.Properties.GetAsRawString(nameof(DependencyLogEntry.ResultCode));
            bool outcome = logEntry.Properties.GetAsBool(nameof(DependencyLogEntry.IsSuccessful));
            IDictionary<string, string> context = logEntry.Properties.GetAsDictionary(nameof(DependencyLogEntry.Context));

            var dependencyTelemetry = new DependencyTelemetry(dependencyType, target, dependencyName, data, startTime, duration, resultCode, success: outcome)
            {
                Id = dependencyId
            };

            dependencyTelemetry.Properties.AddRange(context);
            return dependencyTelemetry;
        }
    }
}

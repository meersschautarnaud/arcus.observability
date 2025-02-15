﻿using System;
using System.Collections.Generic;
using Arcus.Observability.Telemetry.Core;
using Arcus.Observability.Telemetry.Core.Logging;
using GuardNet;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Telemetry extensions on the <see cref="ILogger"/> instance to write Application Insights compatible log messages.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class ILoggerExtensions
    {
        /// <summary>
        /// Logs an event related to an security activity (i.e. input validation, authentication, authorization...).
        /// </summary>
        /// <param name="logger">The logger to track the security event.</param>
        /// <param name="name">The name of the security event written.</param>
        /// <param name="context">The context that provides more insights on the event that occurred.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> is blank.</exception>
        public static void LogSecurityEvent(this ILogger logger, string name, Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNullOrWhitespace(name, nameof(name), "Requires a non-blank name of the event to track an security event");

            context = context ?? new Dictionary<string, object>();
            context["EventType"] = "Security";

            LogCustomEvent(logger, name, context);
        }

        /// <summary>
        /// Logs a custom event
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="name">Name of the event</param>
        /// <param name="context">Context that provides more insights on the event that occurred</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> is blank.</exception>
        public static void LogCustomEvent(this ILogger logger, string name, Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNullOrWhitespace(name, nameof(name), "Requires a non-blank event name to track an custom event");

            context = context ?? new Dictionary<string, object>();

            logger.LogWarning(MessageFormats.EventFormat, new EventLogEntry(name, context));
        }

        /// <summary>
        /// Logs a custom event
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="name">Name of the event</param>
        /// <param name="context">Context that provides more insights on the event that occurred</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="name"/> is blank.</exception>
        [Obsolete("Use " + nameof(LogCustomEvent) + " instead")]
        public static void LogEvent(this ILogger logger, string name, Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNullOrWhitespace(name, nameof(name), "Requires a non-blank event name to track an custom event");

            context = context ?? new Dictionary<string, object>();

            logger.LogWarning(MessageFormats.EventFormat, new EventLogEntry(name, context));
        }
    }
}

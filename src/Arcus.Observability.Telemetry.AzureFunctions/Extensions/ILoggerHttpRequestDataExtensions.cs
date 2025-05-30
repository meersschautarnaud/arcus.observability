﻿using System;
using System.Collections.Generic;
using System.Net;
using Arcus.Observability.Telemetry.Core;
using Arcus.Observability.Telemetry.Core.Logging;
using GuardNet;
#if NET6_0
using Microsoft.Azure.Functions.Worker.Http; 
#endif

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging
{
#if NET6_0
    /// <summary>
    /// Telemetry extensions on the <see cref="ILogger"/> instance to write Application Insights compatible log messages.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class ILoggerRequestExtensions
    {
        /// <summary>
        /// Logs an HTTP request.
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="request">The incoming HTTP request that was processed.</param>
        /// <param name="responseStatusCode">The HTTP status code returned by the service.</param>
        /// <param name="measurement">The instance to measure the duration of the HTTP request.</param>
        /// <param name="context">The context that provides more insights on the tracked HTTP request.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the <paramref name="logger"/>, <paramref name="request"/>, or the <paramref name="measurement"/>  is <c>null</c>.
        /// </exception>
        public static void LogRequest(
            this ILogger logger,
            HttpRequestData request,
            HttpStatusCode responseStatusCode,
            DurationMeasurement measurement,
            Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNull(request, nameof(request), "Requires a HTTP request instance to track a HTTP request");
            Guard.NotNull(measurement, nameof(measurement), "Requires an measurement instance to time the duration of the HTTP request");

            LogRequest(logger, request, responseStatusCode, measurement.StartTime, measurement.Elapsed, context);
        }

        /// <summary>
        /// Logs an HTTP request.
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="request">The incoming HTTP request that was processed.</param>
        /// <param name="responseStatusCode">The HTTP status code returned by the service.</param>
        /// <param name="startTime">The time when the HTTP request was received.</param>
        /// <param name="duration">The duration of the HTTP request processing operation.</param>
        /// <param name="context">The context that provides more insights on the tracked HTTP request.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger"/> or the <paramref name="request"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="duration"/> is a negative time range.</exception>
        public static void LogRequest(
            this ILogger logger,
            HttpRequestData request,
            HttpStatusCode responseStatusCode,
            DateTimeOffset startTime,
            TimeSpan duration,
            Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNull(request, nameof(request), "Requires a HTTP request instance to track a HTTP request");
            Guard.NotLessThan(duration, TimeSpan.Zero, nameof(duration), "Requires a positive time duration of the request operation");

            LogRequest(logger, request, responseStatusCode, operationName: null, startTime, duration, context);
        }

        /// <summary>
        /// Logs an HTTP request.
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="request">The incoming HTTP request that was processed.</param>
        /// <param name="responseStatusCode">The HTTP status code returned by the service.</param>
        /// <param name="operationName">The name of the operation of the request.</param>
        /// <param name="measurement">The instance to measure the duration of the HTTP request.</param>
        /// <param name="context">The context that provides more insights on the tracked HTTP request.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when the <paramref name="logger"/>, <paramref name="request"/>, or the <paramref name="measurement"/>  is <c>null</c>.
        /// </exception>
        public static void LogRequest(
            this ILogger logger,
            HttpRequestData request,
            HttpStatusCode responseStatusCode,
            string operationName,
            DurationMeasurement measurement,
            Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNull(request, nameof(request), "Requires a HTTP request instance to track a HTTP request");
            Guard.NotNull(measurement, nameof(measurement), "Requires an measurement instance to time the duration of the HTTP request");

            LogRequest(logger, request, responseStatusCode, operationName, measurement.StartTime, measurement.Elapsed, context);
        }

        /// <summary>
        /// Logs an HTTP request.
        /// </summary>
        /// <param name="logger">The logger to track the telemetry.</param>
        /// <param name="request">The incoming HTTP request that was processed.</param>
        /// <param name="responseStatusCode">The HTTP status code returned by the service.</param>
        /// <param name="operationName">The name of the operation of the request.</param>
        /// <param name="startTime">The time when the HTTP request was received.</param>
        /// <param name="duration">The duration of the HTTP request processing operation.</param>
        /// <param name="context">The context that provides more insights on the tracked HTTP request.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="logger"/> or the <paramref name="request"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="duration"/> is a negative time range.</exception>
        public static void LogRequest(
            this ILogger logger,
            HttpRequestData request,
            HttpStatusCode responseStatusCode,
            string operationName,
            DateTimeOffset startTime,
            TimeSpan duration,
            Dictionary<string, object> context = null)
        {
            Guard.NotNull(logger, nameof(logger), "Requires a logger instance to track telemetry");
            Guard.NotNull(request, nameof(request), "Requires a HTTP request instance to track a HTTP request");
            Guard.NotLessThan(duration, TimeSpan.Zero, nameof(duration), "Requires a positive time duration of the request operation");

            context = context ?? new Dictionary<string, object>();

            logger.LogWarning(MessageFormats.RequestFormat,
                RequestLogEntry.CreateForHttpRequest(request.Method,
                    request.Url.Scheme,
                    request.Url.Host,
                    request.Url.AbsolutePath,
                    operationName,
                    (int) responseStatusCode,
                    startTime,
                    duration,
                    context));
        }
    }
#endif
}

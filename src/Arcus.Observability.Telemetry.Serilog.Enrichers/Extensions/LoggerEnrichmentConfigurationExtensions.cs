﻿using System;
using Arcus.Observability.Correlation;
using Arcus.Observability.Telemetry.Core;
using Arcus.Observability.Telemetry.Serilog.Enrichers;
using Arcus.Observability.Telemetry.Serilog.Enrichers.Configuration;
using GuardNet;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;

// ReSharper disable once CheckNamespace
namespace Serilog
{
    /// <summary>
    /// Adds user-friendly extensions to append additional Serilog enrichers to the <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerEnrichmentConfigurationExtensions
    {
        /// <summary>
        /// Adds the <see cref="VersionEnricher"/> to the logger enrichment configuration which adds the current runtime version (i.e. 'version' = '1.0.0') of the assembly.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="propertyName">The name of the property to enrich the log event with the current runtime version.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="propertyName"/> is blank.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the process executable in the default application domain cannot be retrieved.</exception>
        public static LoggerConfiguration WithVersion(this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName = VersionEnricher.DefaultPropertyName)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the version enricher");
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName), "Requires a non-blank property name to enrich the log event with the current runtime version");

            return enrichmentConfiguration.With(new VersionEnricher(propertyName));
        }

        /// <summary>
        /// Adds the <see cref="VersionEnricher"/> to the logger enrichment configuration which adds the current application version retrieved via the given <paramref name="appVersion"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="appVersion">The instance to retrieve the current application version.</param>
        /// <param name="propertyName">The name of the property to enrich the log event with the current application version.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="appVersion"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="propertyName"/> is blank.</exception>
        public static LoggerConfiguration WithVersion(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            IAppVersion appVersion,
            string propertyName = VersionEnricher.DefaultPropertyName)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the version enricher");
            Guard.NotNull(appVersion, nameof(appVersion), "Requires an application version implementation to enrich the log event with the application version");
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName), "Requires a non-blank property name to enrich the log event with the current runtime version");

            return enrichmentConfiguration.With(new VersionEnricher(appVersion, propertyName));
        }

        /// <summary>
        /// Adds the <see cref="VersionEnricher"/> to the logger enrichment configuration which adds the current application version retrieved via the given <paramref name="serviceProvider"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The instance to retrieve the current application version.</param>
        /// <param name="propertyName">The name of the property to enrich the log event with the current application version.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="propertyName"/> is blank.</exception>
        public static LoggerConfiguration WithVersion(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            IServiceProvider serviceProvider,
            string propertyName = VersionEnricher.DefaultPropertyName)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the version enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), $"Requires a services provider collection to look for registered '{nameof(IAppVersion)}' implementations");
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName), "Requires a non-blank property name to enrich the log event with the current runtime version");

            IAppVersion appVersion = serviceProvider.GetService<IAppVersion>() ?? new AssemblyAppVersion();
            return enrichmentConfiguration.With(new VersionEnricher(appVersion, propertyName));
        }

        /// <summary>
        /// Adds the <see cref="ApplicationEnricher"/> to the logger enrichment configuration which adds the given application's <paramref name="componentName"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="componentName">The name of the application component.</param>
        /// <param name="propertyName">The name of the property to enrich the log event with the <paramref name="componentName"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="componentName"/> or <paramref name="propertyName"/> is blank.</exception>
        public static LoggerConfiguration WithComponentName(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            string componentName, 
            string propertyName = ApplicationEnricher.ComponentName)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Require an enrichment configuration to add the application component enricher");
            Guard.NotNullOrWhitespace(componentName, nameof(componentName), "Requires a non-blank application component name");
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName), "Requires a non-blank property name to enrich the log event with the component name");

            return enrichmentConfiguration.With(new ApplicationEnricher(componentName, propertyName));
        }

        /// <summary>
        /// Adds the <see cref="ApplicationEnricher"/> to the logger enrichment configuration which adds the given application's name,
        /// where the name is retrieved from a registered <see cref="IAppName"/> in the <paramref name="serviceProvider"/>.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The service provider instance that has a registered <see cref="IAppName"/>.</param>
        /// <param name="propertyName">The name of the property to enrich the log event with the application's name.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="serviceProvider"/> or <paramref name="propertyName"/> is blank.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="IAppName"/> is registered in the <paramref name="serviceProvider"/>.</exception>
        public static LoggerConfiguration WithComponentName(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider,
            string propertyName = ApplicationEnricher.ComponentName)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the application enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), $"Requires a services provider collection to look for registered '{nameof(IAppName)}' implementations");
            Guard.NotNullOrWhitespace(propertyName, nameof(propertyName), "Requires a non-blank property name to enrich the log event with the current application's name");

            var appName = serviceProvider.GetService<IAppName>();
            if (appName is null)
            {
                throw new InvalidOperationException(
                    $"Cannot retrieve component name because no {nameof(IAppName)} is registered in the application's services");
            }

            string componentName = appName.GetApplicationName();
            return enrichmentConfiguration.With(new ApplicationEnricher(componentName, propertyName));
        }

        /// <summary>
        /// Adds the <see cref="KubernetesEnricher"/> to the logger enrichment configuration which adds Kubernetes information from the environment.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="nodeNamePropertyName">The name of the property to enrich the log event with the Kubernetes node name.</param>
        /// <param name="podNamePropertyName">The name of the property to enrich the log event with the Kubernetes pod name.</param>
        /// <param name="namespacePropertyName">The name of the property to enrich the log event with the Kubernetes namespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="nodeNamePropertyName"/>, <paramref name="podNamePropertyName"/>, or <paramref name="namespacePropertyName"/> is blank.</exception>
        public static LoggerConfiguration WithKubernetesInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string nodeNamePropertyName = ContextProperties.Kubernetes.NodeName,
            string podNamePropertyName = ContextProperties.Kubernetes.PodName,
            string namespacePropertyName = ContextProperties.Kubernetes.Namespace)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the Kubernetes enricher");
            Guard.NotNullOrWhitespace(nodeNamePropertyName, nameof(nodeNamePropertyName), "Requires a non-blank property name to enrich the log event with the Kubernetes node name");
            Guard.NotNullOrWhitespace(podNamePropertyName, nameof(podNamePropertyName), "Requires a non-blank property name to enrich the log event with the Kubernetes pod name");
            Guard.NotNullOrWhitespace(namespacePropertyName, nameof(namespacePropertyName), "Requires a non-blank property name to enrich the log event with the Kubernetes namespace name");

            return enrichmentConfiguration.With(new KubernetesEnricher(nodeNamePropertyName, podNamePropertyName, namespacePropertyName));
        }

        /// <summary>
        /// Adds the <see cref="DefaultCorrelationInfoAccessor"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        [Obsolete("Use the " + nameof(WithCorrelationInfo) + " overload with providing your own default correlation accessor")]
        public static LoggerConfiguration WithCorrelationInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            return WithCorrelationInfo(enrichmentConfiguration, DefaultCorrelationInfoAccessor.Instance, operationIdPropertyName, transactionIdPropertyName);
        }

        /// <summary>
        /// Adds the previously registered <see cref="ICorrelationInfoAccessor"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <remarks>
        ///     Use the <see cref="WithCorrelationInfo(LoggerEnrichmentConfiguration,IServiceProvider,Action{CorrelationInfoEnricherOptions})"/> overload to configure the operation parent ID.
        /// </remarks>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The instance to provide the <see cref="ICorrelationInfoAccessor"/> service while enriching the log events with the correlation information.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        public static LoggerConfiguration WithCorrelationInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), "Requires a provider to retrieve the correlation information accessor instance");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            var accessor = serviceProvider.GetRequiredService<ICorrelationInfoAccessor>();
            return WithCorrelationInfo(enrichmentConfiguration, accessor, operationIdPropertyName, transactionIdPropertyName);
        }
        
        /// <summary>
        /// Adds the previously registered <see cref="ICorrelationInfoAccessor"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The instance to provide the <see cref="ICorrelationInfoAccessor"/> service while enriching the log events with the correlation information.</param>
        /// <param name="configureOptions">The function to configure the options to change the behavior of the enricher.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        public static LoggerConfiguration WithCorrelationInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider,
            Action<CorrelationInfoEnricherOptions> configureOptions)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), "Requires a provider to retrieve the correlation information accessor instance");

            var accessor = serviceProvider.GetRequiredService<ICorrelationInfoAccessor>();
            return WithCorrelationInfo(enrichmentConfiguration, accessor, configureOptions);
        }

        /// <summary>
        /// Adds the <see cref="DefaultCorrelationInfoAccessor{TCorrelationInfo}"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        [Obsolete("Use the " + nameof(WithCorrelationInfo) + " overload with providing your own default correlation accessor")]
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId)
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            return WithCorrelationInfo(enrichmentConfiguration, DefaultCorrelationInfoAccessor<TCorrelationInfo>.Instance, operationIdPropertyName, transactionIdPropertyName);
        }

        /// <summary>
        /// Adds the <see cref="DefaultCorrelationInfoAccessor{TCorrelationInfo}"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <remarks>
        ///     Use the <see cref="WithCorrelationInfo{TCorrelationInfo}(LoggerEnrichmentConfiguration,IServiceProvider,Action{CorrelationInfoEnricherOptions})"/> overload to configure the operation parent ID.
        /// </remarks>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The instance to provide the <see cref="ICorrelationInfoAccessor"/> service while enriching the log events with the correlation information.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId)
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), "Requires a provider to retrieve the correlation information accessor instance");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            var accessor = serviceProvider.GetRequiredService<ICorrelationInfoAccessor<TCorrelationInfo>>();
            return WithCorrelationInfo(enrichmentConfiguration, accessor, operationIdPropertyName, transactionIdPropertyName);
        }
        
        /// <summary>
        /// Adds the <see cref="DefaultCorrelationInfoAccessor{TCorrelationInfo}"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="serviceProvider">The instance to provide the <see cref="ICorrelationInfoAccessor"/> service while enriching the log events with the correlation information.</param>
        /// <param name="configureOptions">The function to configure the options to change the behavior of the enricher.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IServiceProvider serviceProvider,
            Action<CorrelationInfoEnricherOptions> configureOptions)
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(serviceProvider, nameof(serviceProvider), "Requires a provider to retrieve the correlation information accessor instance");

            var accessor = serviceProvider.GetRequiredService<ICorrelationInfoAccessor<TCorrelationInfo>>();
            return WithCorrelationInfo(enrichmentConfiguration, accessor, configureOptions);
        }

        /// <summary>
        /// Adds the <see cref="CorrelationInfoEnricher{TCorrelationInfo}"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <remarks>
        ///     Use the <see cref="WithCorrelationInfo(LoggerEnrichmentConfiguration,ICorrelationInfoAccessor,Action{CorrelationInfoEnricherOptions})"/> to configure the operation parent ID.
        /// </remarks>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="correlationInfoAccessor">The accessor implementation for the <see cref="CorrelationInfo"/> model.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="correlationInfoAccessor"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        public static LoggerConfiguration WithCorrelationInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            ICorrelationInfoAccessor correlationInfoAccessor,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(correlationInfoAccessor, nameof(correlationInfoAccessor), "Requires an correlation accessor to retrieve the correlation information during the enrichment of the log events");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            return WithCorrelationInfo<CorrelationInfo>(enrichmentConfiguration, correlationInfoAccessor, operationIdPropertyName, transactionIdPropertyName);
        }
        
        /// <summary>
        /// Adds the <see cref="CorrelationInfoEnricher{TCorrelationInfo}"/> to the logger enrichment configuration which adds the <see cref="CorrelationInfo"/> information from the current context.
        /// </summary>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="correlationInfoAccessor">The accessor implementation for the <see cref="CorrelationInfo"/> model.</param>
        /// <param name="configureOptions">The function to configure the options to change the behavior of the enricher.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="correlationInfoAccessor"/> is <c>null</c>.</exception>
        public static LoggerConfiguration WithCorrelationInfo(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            ICorrelationInfoAccessor correlationInfoAccessor,
            Action<CorrelationInfoEnricherOptions> configureOptions)
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(correlationInfoAccessor, nameof(correlationInfoAccessor), "Requires an correlation accessor to retrieve the correlation information during the enrichment of the log events");

            return WithCorrelationInfo<CorrelationInfo>(enrichmentConfiguration, correlationInfoAccessor, configureOptions);
        }

        /// <summary>
        /// Adds the <see cref="CorrelationInfoEnricher{TCorrelationInfo}"/> to the logger enrichment configuration which adds the custom <typeparamref name="TCorrelationInfo"/> information from the current context.
        /// </summary>
        /// <typeparam name="TCorrelationInfo">The type of the custom <see cref="CorrelationInfo"/> model.</typeparam>
        /// <remarks>
        ///     Use the <see cref="WithCorrelationInfo{TCorrelationInfo}(LoggerEnrichmentConfiguration,ICorrelationInfoAccessor{TCorrelationInfo},Action{CorrelationInfoEnricherOptions})"/>
        ///     overload to configure the the operation parent ID.
        /// </remarks>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="correlationInfoAccessor">The accessor implementation for the <typeparamref name="TCorrelationInfo"/> model.</param>
        /// <param name="operationIdPropertyName">The name of the property to enrich the log event with the correlation operation ID.</param>
        /// <param name="transactionIdPropertyName">The name of the property to enrich the log event with the correlation transaction ID.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="correlationInfoAccessor"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="operationIdPropertyName"/> or <paramref name="transactionIdPropertyName"/> is blank.</exception>
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            ICorrelationInfoAccessor<TCorrelationInfo> correlationInfoAccessor,
            string operationIdPropertyName = ContextProperties.Correlation.OperationId,
            string transactionIdPropertyName = ContextProperties.Correlation.TransactionId) 
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(correlationInfoAccessor, nameof(correlationInfoAccessor), "Requires an correlation accessor to retrieve the correlation information during the enrichment of the log events");
            Guard.NotNullOrWhitespace(operationIdPropertyName, nameof(operationIdPropertyName), "Requires a property name to enrich the log event with the correlation operation ID");
            Guard.NotNullOrWhitespace(transactionIdPropertyName, nameof(transactionIdPropertyName), "Requires a property name to enrich the log event with the correlation transaction ID");

            return enrichmentConfiguration.With(new CorrelationInfoEnricher<TCorrelationInfo>(correlationInfoAccessor, operationIdPropertyName, transactionIdPropertyName));
        }

        /// <summary>
        /// Adds the <see cref="CorrelationInfoEnricher{TCorrelationInfo}"/> to the logger enrichment configuration which adds the custom <typeparamref name="TCorrelationInfo"/> information from the current context.
        /// </summary>
        /// <typeparam name="TCorrelationInfo">The type of the custom <see cref="CorrelationInfo"/> model.</typeparam>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="correlationInfoAccessor">The accessor implementation for the <typeparamref name="TCorrelationInfo"/> model.</param>
        /// <param name="configureOptions">The function to configure the options to change the behavior of the enricher.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or <paramref name="correlationInfoAccessor"/> is <c>null</c>.</exception>
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            ICorrelationInfoAccessor<TCorrelationInfo> correlationInfoAccessor,
            Action<CorrelationInfoEnricherOptions> configureOptions) 
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(correlationInfoAccessor, nameof(correlationInfoAccessor), "Requires an correlation accessor to retrieve the correlation information during the enrichment of the log events");

            var options = new CorrelationInfoEnricherOptions();
            configureOptions?.Invoke(options);
            
            return enrichmentConfiguration.With(new CorrelationInfoEnricher<TCorrelationInfo>(correlationInfoAccessor, options));
        }
        
        /// <summary>
        /// Adds the <see cref="CorrelationInfoEnricher{TCorrelationInfo}"/> to the logger enrichment configuration which adds the custom <typeparamref name="TCorrelationInfo"/> information from the current context.
        /// </summary>
        /// <typeparam name="TCorrelationInfo">The type of the custom <see cref="CorrelationInfo"/> model.</typeparam>
        /// <param name="enrichmentConfiguration">The configuration to add the enricher.</param>
        /// <param name="correlationInfoEnricher">The custom correlation enricher implementation for the <typeparamref name="TCorrelationInfo"/> model.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="enrichmentConfiguration"/> or the <paramref name="correlationInfoEnricher"/> is <c>null</c>.</exception>
        public static LoggerConfiguration WithCorrelationInfo<TCorrelationInfo>(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, 
            CorrelationInfoEnricher<TCorrelationInfo> correlationInfoEnricher) 
            where TCorrelationInfo : CorrelationInfo
        {
            Guard.NotNull(enrichmentConfiguration, nameof(enrichmentConfiguration), "Requires an enrichment configuration to add the correlation information enricher");
            Guard.NotNull(correlationInfoEnricher, nameof(correlationInfoEnricher), "Requires an correlation enricher to enrich the log events with correlation information");

            return enrichmentConfiguration.With(correlationInfoEnricher);
        }
    }
}

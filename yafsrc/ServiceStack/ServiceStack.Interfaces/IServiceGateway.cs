// ***********************************************************************
// <copyright file="IServiceGateway.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack
{
    /// <summary>
    /// The minimal API Surface to capture the most common SYNC requests.
    /// Convenience extensions over these core API's available in ServiceGatewayExtensions
    /// </summary>
    public interface IServiceGateway
    {
        /// <summary>
        /// Normal Request/Reply Services
        /// </summary>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="requestDto">The request dto.</param>
        /// <returns>TResponse.</returns>
        TResponse Send<TResponse>(object requestDto);

        /// <summary>
        /// Auto Batched Request/Reply Requests
        /// </summary>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="requestDtos">The request dtos.</param>
        /// <returns>List&lt;TResponse&gt;.</returns>
        List<TResponse> SendAll<TResponse>(IEnumerable<object> requestDtos);

        /// <summary>
        /// OneWay Service
        /// </summary>
        /// <param name="requestDto">The request dto.</param>
        void Publish(object requestDto);

        /// <summary>
        /// Auto Batched OneWay Requests
        /// </summary>
        /// <param name="requestDtos">The request dtos.</param>
        void PublishAll(IEnumerable<object> requestDtos);
    }

    /// <summary>
    /// The minimal API Surface to capture the most common ASYNC requests.
    /// Convenience extensions over these core API's available in ServiceGatewayExtensions
    /// </summary>
    public interface IServiceGatewayAsync
    {
        /// <summary>
        /// Normal Request/Reply Services
        /// </summary>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="requestDto">The request dto.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;TResponse&gt;.</returns>
        Task<TResponse> SendAsync<TResponse>(object requestDto, CancellationToken token = default);

        /// <summary>
        /// Auto Batched Request/Reply Requests
        /// </summary>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="requestDtos">The request dtos.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task&lt;List&lt;TResponse&gt;&gt;.</returns>
        Task<List<TResponse>> SendAllAsync<TResponse>(IEnumerable<object> requestDtos, CancellationToken token = default);

        /// <summary>
        /// OneWay Service
        /// </summary>
        /// <param name="requestDto">The request dto.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task PublishAsync(object requestDto, CancellationToken token = default);

        /// <summary>
        /// Auto Batched OneWay Requests
        /// </summary>
        /// <param name="requestDtos">The request dtos.</param>
        /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Task.</returns>
        Task PublishAllAsync(IEnumerable<object> requestDtos, CancellationToken token = default);
    }
}


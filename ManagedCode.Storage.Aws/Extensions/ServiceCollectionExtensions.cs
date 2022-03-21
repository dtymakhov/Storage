﻿using System;
using ManagedCode.Storage.Aws.Options;
using ManagedCode.Storage.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ManagedCode.Storage.Aws.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAWSStorage(this IServiceCollection serviceCollection, Action<AWSStorageOptions> action)
    {
        var awsStorageOptions = new AWSStorageOptions();
        action.Invoke(awsStorageOptions);
        return serviceCollection.AddAWSStorage(awsStorageOptions);
    }

    public static IServiceCollection AddAWSStorageAsDefault(this IServiceCollection serviceCollection, Action<AWSStorageOptions> action)
    {
        var awsStorageOptions = new AWSStorageOptions();
        action.Invoke(awsStorageOptions);
        return serviceCollection.AddAWSStorageAsDefault(awsStorageOptions);
    }
    
    public static IServiceCollection AddAWSStorage(this IServiceCollection serviceCollection, AWSStorageOptions options)
    {
        return serviceCollection.AddScoped<IAWSStorage>(_ => new AWSStorage(options));
    }

    public static IServiceCollection AddAWSStorageAsDefault(this IServiceCollection serviceCollection, AWSStorageOptions options)
    {
        return serviceCollection.AddScoped<IStorage>(_ => new AWSStorage(options));
    }
}
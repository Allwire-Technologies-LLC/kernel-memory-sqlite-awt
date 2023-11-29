// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.MemoryStorage;
using AWT.KernelMemory.SQLite;

namespace AWT.KernelMemory.SQLite;

/// <summary>
/// Extensions for KernelMemoryBuilder
/// </summary>
public static partial class KernelMemoryBuilderExtensions
{
    /// <summary>
    /// Kernel Memory Builder extension method to add SQLite memory connector.
    /// </summary>
    /// <param name="builder">KM builder instance</param>
    /// <param name="config">SQLite configuration</param>
    public static IKernelMemoryBuilder WithSQLite(this IKernelMemoryBuilder builder, SQLiteConfig config)
    {
        builder.Services.AddSQLiteAsVectorDb(config);
        return builder;
    }

    /// <summary>
    /// Kernel Memory Builder extension method to add SQLite memory connector.
    /// </summary>
    /// <param name="builder">KM builder instance</param>
    /// <param name="connString">SQLite connection string</param>
    public static IKernelMemoryBuilder WithSQLite(this IKernelMemoryBuilder builder, string connString)
    {
        builder.Services.AddSQLiteAsVectorDb(connString);
        return builder;
    }
}

/// <summary>
/// Extensions for KernelMemoryBuilder and generic DI
/// </summary>
public static partial class DependencyInjection
{
    /// <summary>
    /// Inject SQLite as the default implementation of IVectorDb
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="config">SQLite configuration</param>
    public static IServiceCollection AddSQLiteAsVectorDb(this IServiceCollection services, SQLiteConfig config)
    {
        return services
            .AddSingleton<SQLiteConfig>(config)
            .AddSingleton<IVectorDb, SQLiteMemory>();
    }

    /// <summary>
    /// Inject SQLite as the default implementation of IVectorDb
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connString">SQLite connection string</param>
    public static IServiceCollection AddSQLiteAsVectorDb(this IServiceCollection services, string connString)
    {
        var config = new SQLiteConfig { ConnString = connString };
        return services.AddSQLiteAsVectorDb(config);
    }
}

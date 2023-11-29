// Copyright (c) Microsoft. All rights reserved.

namespace AWT.KernelMemory.SQLite;

/// <summary>
/// Postgres configuration
/// </summary>
public class SQLiteConfig
{
    /// <summary>
    /// Connection string required to connect to Postgres
    /// </summary>
    public string ConnString { get; set; } = string.Empty;
}

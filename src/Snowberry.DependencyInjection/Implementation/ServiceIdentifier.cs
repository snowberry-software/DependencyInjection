﻿using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Implementation;

/// <see cref="IServiceIdentifier"/>
public readonly struct ServiceIdentifier : IServiceIdentifier
{
    public ServiceIdentifier(Type serviceType)
    {
        ServiceType = serviceType;
    }

    public ServiceIdentifier(Type serviceType, object? serviceKey)
    {
        ServiceType = serviceType;
        ServiceKey = serviceKey;
    }

    /// <inheritdoc/>
    public bool Equals(IServiceIdentifier other)
    {
        if (ServiceKey == null && other.ServiceKey == null)
            return ServiceType == other.ServiceType;

        if (ServiceKey != null && other.ServiceKey != null)
            return ServiceType == other.ServiceType && ServiceKey.Equals(other.ServiceKey);

        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ServiceIdentifier identifier && Equals(identifier);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (ServiceKey == null)
            return ServiceType.GetHashCode();

        unchecked
        {
            return (ServiceType.GetHashCode() * 0x1337) ^ ServiceKey.GetHashCode();
        }
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        if (ServiceKey == null)
            return ServiceType.ToString();

        return $"({ServiceKey}, {ServiceType})";
    }

    /// <inheritdoc/>
    public Type ServiceType { get; }

    /// <inheritdoc/>
    public object? ServiceKey { get; }
}

﻿using System.Reflection;
using Snowberry.DependencyInjection.Attributes;
using Snowberry.DependencyInjection.Exceptions;
using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Lookup;

public partial class DefaultServiceFactory
{
    /// <inheritdoc/>
    public object CreateInstance(Type type)
    {
        _ = type ?? throw new ArgumentNullException(nameof(type));

        return CreateInstance(type, null);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>()
    {
        return CreateInstance<T>(null);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>(IScope? scope)
    {
        object service = CreateInstance(typeof(T), scope)!;

        if (service is T type)
            return type;

        ThrowHelper.ThrowInvalidServiceImplementationCast(typeof(T), service.GetType());
        return default!;
    }

    /// <inheritdoc/>
    public ConstructorInfo? GetConstructor(Type instanceType)
    {
        var constructors = instanceType.GetConstructors();

        if (constructors.Length == 1)
            return constructors[0];

        // Check for preferred constructor.
        for (int i = 0; i < constructors.Length; i++)
        {
            var constructor = constructors[i];

            if (constructor.GetCustomAttribute<PreferredConstructorAttribute>() != null)
                return constructor;
        }

        // Otherwise get the constructor with the largest number of parameters.
        return constructors
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type, IScope? scope)
    {
        _ = type ?? throw new ArgumentNullException(nameof(type));

        if (type.IsInterface || type.IsAbstract)
            throw new InvalidServiceImplementationType(type, $"Cannot instantiate abstract classes or interfaces! ({type.FullName})!");

        var constructor = GetConstructor(type);

        if (constructor == null)
        {
            if (type.IsValueType)
                return CreateBuiltInType(type);
        }
        else
        {
            object[] args = constructor.GetParameters()
                .Select(param =>
                {
                    object? serviceKey = null;
                    var keyedServiceAttribute = param.GetCustomAttribute<FromKeyedServicesAttribute>();

                    if (keyedServiceAttribute != null)
                        serviceKey = keyedServiceAttribute.ServiceKey;

                    return GetInstanceFromServiceType(param.ParameterType, scope, serviceKey);
                })
                .ToArray();

            object? instance = constructor.Invoke(args);

            var properties = type.GetProperties()
                .Where(x => x.SetMethod != null);

            foreach (var property in properties)
            {
                var injectAttribute = property.GetCustomAttribute<InjectAttribute>();

                if (injectAttribute == null)
                    continue;

                var keyedServiceAttribute = property.GetCustomAttribute<FromKeyedServicesAttribute>();

                object? propertyValue = null;
                if (keyedServiceAttribute != null)
                    propertyValue = GetOptionalKeyedService(property.PropertyType, keyedServiceAttribute.ServiceKey, scope);
                else
                    propertyValue = GetOptionalService(property.PropertyType, scope);

                if (injectAttribute.IsRequired && propertyValue is null)
                    throw new ServiceTypeNotRegistered(property.PropertyType, $"The required service for the property `{property.Name}` is not registered!");

                property.SetValue(instance, propertyValue);
            }

            return instance;
        }

        ThrowHelper.ThrowInvalidConstructor(type);
        return null!;
    }
}

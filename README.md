A lightweight easy to use IoC container for .NET.

# Usage

The ``ServiceContainer`` should be disposed when no longer in use.

```cs
var serviceContainer = new ServiceContainer();

// Provided instances won't be disposed by the container.
var providedSingletonInstance = new SomeType();
serviceContainer.RegisterSingleton<ISomeType>(providedSingletonInstance);

// The instance created by the container will be disposed by the container.
serviceContainer.RegisterSingleton<ISomeOtherType, SomeOtherType>();
serviceContainer.RegisterTransient<ITransientType, TransientType>();

// Dispose container
serviceContainer.Dispose();
```

Registered services can be overwritten

```cs
...

serviceContainer.RegisterTransient<ITransientType, TransientType>();

// This makes the ITransientType service a singleton service with another implementation type.
// The container/scope will still dispose the previously created transient service instances.
serviceContainer.RegisterSingleton<ITransientType, NewTransientType>();

...
```

This behavior can be disabled by using the `ServiceContainerOptions` and creating the `ServiceContainer` like this
```cs
var serviceContainer = new ServiceContainer(ServiceContainerOptions.ReadOnly);
```

## Scopes

```cs
var serviceContainer = new ServiceContainer();

serviceContainer.RegisterScoped<IScopedType, ScopedType>();
serviceContainer.RegisterTransient<ITransientType, TransientType>();

// The current scope in this case would be the container itself, means it will be disposed by the container.
_ = serviceContainer.GetService<IScopedType>();
_ = serviceContainer.GetService<ITransientType>();

using(var scope = serviceContainer.CreateScope())
{
    // The instance was created for current scope, the instance will be disposed by the scope.
    _ = scope.ServiceFactory.GetService<IScopedType>();
    _ = scope.ServiceFactory.GetService<ITransientType>();
}

// Dispose container
serviceContainer.Dispose();
```

## Service lifetime

| Name      | Description                                                                              |
| --------- | ---------------------------------------------------------------------------------------- |
| Singleton | Specifies that a single instance of the service will be created.                         |
| Transient | Specifies that a new instance of the service will be created every time it is requested. |
| Scoped    | Specifies that a new instance of the service will be created for each scope.             |
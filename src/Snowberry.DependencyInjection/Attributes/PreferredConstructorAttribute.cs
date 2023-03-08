using System;
using System.Collections.Generic;
using System.Text;

namespace Snowberry.DependencyInjection.Attributes;

/// <summary>
/// Specifies which constructor should be used to instantiate an instance.
/// </summary>
/// <remarks>This attribute is not needed, if there is only one constructor in a type.</remarks>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public class PreferredConstructorAttribute : Attribute
{
}

#nullable enable

namespace Nexd.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Pumped
    {
        public static Return? GetFieldValue<Return, Class>(string field) where Class : class
            => GetFieldValue<Return, Class>(null, field);

        public static Return? GetPropertyValue<Return, Class>(string property) where Class : class
            => GetPropertyValue<Return, Class>(null, property);

        public static void SetFieldValue<Class, Value>(string field, Value value) where Class : class
            => SetFieldValue<Class, Value>(null, field, value);

        public static void SetPropertyValue<Class, Value>(string property, Value value) where Class : class
            => SetPropertyValue<Class, Value>(null, property, value);

        public static Return? Invoke<Return, Class>(string method, params object[] args) where Class : class
            => Invoke<Return, Class>(null, method, args);

        public static void InvokeVoid<Class>(string method, params object[] args) where Class : class
            => InvokeVoid<Class>(null, method, args);

        public static Return? GetFieldValue<Return, Class>(Class? pThis, string field)
            where Class : class
        {
            FieldInfo? info = typeof(Class).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            return (Return)info?.GetValue(pThis)!;
        }

        public static Return? GetPropertyValue<Return, Class>(Class? pThis, string property)
            where Class : class
        {
            PropertyInfo? info = typeof(Class).GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            return (Return)info?.GetValue(pThis)!;
        }

        public static void SetFieldValue<Class, Value>(Class? pThis, string field, Value value)
            where Class : class
        {
            FieldInfo? info = typeof(Class).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            info?.SetValue(pThis, value);
        }

        public static void SetPropertyValue<Class, Value>(Class? pThis, string property, Value value)
            where Class : class
        {
            PropertyInfo? info = typeof(Class).GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            info?.SetValue(pThis, value);
        }

        public static Return? Invoke<Return, Class>(Class? pThis, string method, params object[] args)
            where Class : class
        {
            MethodInfo? targetMethod = typeof(Class).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return (Return)targetMethod?.Invoke(pThis, args)!;
        }

        public static void InvokeVoid<Class>(Class? pThis, string method, params object[] args) where Class : class
        {
            MethodInfo? targetMethod = typeof(Class).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            targetMethod?.Invoke(pThis, args);
        }

        /// <summary>
        /// Invoke a <see langword="static"/> method from a hidden class that is unavailable due to its protection level.
        /// </summary>
        /// <param name="assembly">Assembly where the class (and method..) is located.</param>
        /// <param name="class">Name of the hidden class</param>
        /// <param name="method">Name of the method within the hidden class.</param>
        /// <param name="args">Optional parameters, if any.</param>
        /// <returns><typeparamref name="Return"/> returned from the method.</returns>
        public static Return? InvokeInternal<Return>(Assembly assembly, string @class, string method, params object[] args)
        {
            Type? versionType = assembly.GetType(@class);
            MethodInfo? methodInfo = versionType?.GetMethod(method);
            return (Return)methodInfo?.Invoke(null, args)!;
        }

        /// <summary>
        /// Invoke a <see langword="static"/> method from a hidden class that is unavailable due to its protection level.
        /// </summary>
        /// <param name="assembly">Assembly where the class (and method..) is located.</param>
        /// <param name="class">Name of the hidden class</param>
        /// <param name="method">Name of the method within the hidden class.</param>
        /// <param name="args">Optional parameters, if any.</param>
        public static void InvokeInternalVoid(Assembly assembly, string @class, string method, params object[] args)
        {
            Type? versionType = assembly.GetType(@class);
            MethodInfo? methodInfo = versionType?.GetMethod(method);
            methodInfo?.Invoke(null, args);
        }

        /// <summary>
        /// Gets <see cref="Assembly"/> by name.
        /// </summary>
        /// <param name="name"><see cref="Assembly"/> name that you want to get.</param>
        /// <returns><see cref="Assembly"/> if found, otherwise <see langword="null"/></returns>
        public static Assembly? GetAssembly(string name)
            => AppDomain.CurrentDomain.GetAssemblies().ToList().Find(x => x.GetName().Name == name);

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
            => assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));

        public static Type FindDerivedType(Assembly assembly, Type baseType)
            => FindDerivedTypes(assembly, baseType).First();
    }
}

#nullable disable
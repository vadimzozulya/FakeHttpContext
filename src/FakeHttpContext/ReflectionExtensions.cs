using System;
using System.Reflection;

namespace FakeHttpContext
{
    internal static class ReflectionExtensions
    {
        public static object GetPrivateStaticFieldValue(this Type type, string fieldName)
        {
            var fieldInfo = GetField(type, fieldName, BindingFlags.NonPublic | BindingFlags.Static);
            return fieldInfo.GetValue(null);
        }

        public static object GetPrivateFieldValue(this object target, string fieldName)
        {
            var fieldInfo = GetField(target.GetType(), fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfo.GetValue(target);
        }

        public static T GetPrivateFieldValue<T>(this object target, string fieldName)
        {
            return (T)target.GetPrivateFieldValue(fieldName);
        }

        public static void SetPrivateStaticFieldValue(this Type type, string fieldName, object value)
        {
            var fieldInfo = GetField(type, fieldName, BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(null, value);
        }


        public static void SetPrivateFieldValue(this object target, string fieldName, object value)
        {
            var fieldInfo = GetField(target.GetType(), fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(target, value);
        }

        private static FieldInfo GetField(Type type, string fieldName, BindingFlags bindingFlags)
        {
            var fieldInfo = type.GetField(fieldName, bindingFlags);
            if (fieldInfo == null)
            {
                throw new InvalidOperationException(
                    string.Format("Can't find field '{0}' in the type '{1}'", fieldName, type.FullName));
            }

            return fieldInfo;
        }
    }
}
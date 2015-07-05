namespace FakeHttpContext
{
  using System;
  using System.Diagnostics;
  using System.Reflection;

  internal static class ReflectionExtensions
  {
    public static object GetPrivateStaticFieldValue(this Type type, string fieldName)
    {
      var fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);

      Debug.Assert(fieldInfo != null, "fieldInfo != null");
      return fieldInfo.GetValue(null);
    }

    public static object GetPrivateFieldValue(this object target, string fieldName)
    {
      var fieldInfo = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

      Debug.Assert(fieldInfo != null, "fieldInfo != null");
      return fieldInfo.GetValue(target);
    }

    public static void SetPrivateFieldValue(this object target, string fieldName, object value)
    {
      var fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
      Debug.Assert(fieldInfo != null, "fieldInfo != null");
      fieldInfo.SetValue(target, value);
    }
  }
}
namespace FakeHttpContext.Switchers
{
  using System;

  internal class PrivateFieldSwitcher : IDisposable
  {
    private readonly object backupValue;

    public PrivateFieldSwitcher(object tartget, string fieldName, object value)
    {
      this.Tartget = tartget;
      this.FieldName = fieldName;
      this.NewValue = value;

      this.backupValue = this.Tartget.GetPrivateFieldValue(this.FieldName);
      this.Tartget.SetPrivateFieldValue(this.FieldName, value);
    }

    internal object Tartget { get; private set; }

    internal string FieldName { get; private set; }

    internal object NewValue { get; private set; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Tartget.SetPrivateFieldValue(this.FieldName, this.backupValue);
    }
  }
}
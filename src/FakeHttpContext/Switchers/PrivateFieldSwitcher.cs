using System;

namespace FakeHttpContext.Switchers
{
    internal class PrivateFieldSwitcher : IDisposable
    {
        private readonly object _backupValue;

        public PrivateFieldSwitcher(object tartget, string fieldName, object value)
        {
            Tartget = tartget;
            FieldName = fieldName;
            NewValue = value;

            _backupValue = Tartget.GetPrivateFieldValue(FieldName);
            Tartget.SetPrivateFieldValue(FieldName, value);
        }

        internal object Tartget { get; private set; }

        internal string FieldName { get; private set; }

        internal object NewValue { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Tartget.SetPrivateFieldValue(FieldName, _backupValue);
        }
    }
}
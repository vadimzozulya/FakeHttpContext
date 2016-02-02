namespace FakeHttpContext.Switchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class AppDomainDataSwitcher : SwitcherContainer, IEnumerable
    {
        internal Dictionary<string, object[]> FakeLocalStore { get; private set; }

        public AppDomainDataSwitcher()
        {
            this.FakeLocalStore =
                new Dictionary<string, object[]>(
                    AppDomain.CurrentDomain.GetPrivateFieldValue<Dictionary<string, object[]>>("_LocalStore")
                    ?? new Dictionary<string, object[]>());

            this.Switchers.Add(new PrivateFieldSwitcher(AppDomain.CurrentDomain, "_LocalStore", this.FakeLocalStore));
        }

        public IEnumerator GetEnumerator()
        {
            return this.FakeLocalStore.GetEnumerator();
        }

        public void Add(string name, object value)
        {
            AppDomain.CurrentDomain.SetData(name, value);
        }
    }
}
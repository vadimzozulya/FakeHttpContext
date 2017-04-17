using System;
using System.Collections;
using System.Collections.Generic;

namespace FakeHttpContext.Switchers
{
    internal class AppDomainDataSwitcher : SwitcherContainer, IEnumerable
    {
        internal Dictionary<string, object[]> FakeLocalStore { get; private set; }

        public AppDomainDataSwitcher()
        {
            FakeLocalStore =
                new Dictionary<string, object[]>(
                    AppDomain.CurrentDomain.GetPrivateFieldValue<Dictionary<string, object[]>>("_LocalStore")
                    ?? new Dictionary<string, object[]>());

            Switchers.Add(new PrivateFieldSwitcher(AppDomain.CurrentDomain, "_LocalStore", FakeLocalStore));
        }

        public IEnumerator GetEnumerator()
        {
            return FakeLocalStore.GetEnumerator();
        }

        public void Add(string name, object value)
        {
            AppDomain.CurrentDomain.SetData(name, value);
        }
    }
}
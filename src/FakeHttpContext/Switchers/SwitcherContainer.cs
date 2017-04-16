namespace FakeHttpContext.Switchers
{
    using System;
    using System.Collections.Generic;

    public abstract class SwitcherContainer : IDisposable
    {
        protected SwitcherContainer()
        {
            Switchers = new List<IDisposable>();
        }

        protected internal List<IDisposable> Switchers { get; private set; }

        public virtual void Dispose()
        {
            Switchers.ForEach(x => x.Dispose());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace FakeHttpContext.Switchers
{
    public abstract class SwitcherContainer : IDisposable
    {
        protected SwitcherContainer()
        {
            Switchers = new List<IDisposable>();
        }

        protected internal List<IDisposable> Switchers { get; private set; }

        public virtual void Dispose()
        {
            var exceptions = new List<Exception>();
            Switchers.ForEach(x =>
            {
                try
                {
                    x.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            });
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
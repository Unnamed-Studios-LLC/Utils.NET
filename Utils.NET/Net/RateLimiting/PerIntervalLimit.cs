using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Utils.NET.Collections;

namespace Utils.NET.Net.RateLimiting
{
    /// <summary>
    /// A rate limiting class used to limit requests to a certain amount per a given interval.
    /// </summary>
    public class PerIntervalLimit : IDisposable
    {
        private class LimitedInstance
        {
            public IPAddress address;

            public int count;

            public LimitedInstance(IPAddress address)
            {
                this.address = address;
                count = 0;
            }
        }

        private ConcurrentExpirationQueue<LimitedInstance> expirationQueue;

        private ConcurrentDictionary<IPAddress, LimitedInstance> instances = new ConcurrentDictionary<IPAddress, LimitedInstance>();

        private readonly int rateLimit;

        private Timer expirationTimer;

        public PerIntervalLimit(int rateLimit, double interval)
        {
            this.rateLimit = rateLimit;
            expirationQueue = new ConcurrentExpirationQueue<LimitedInstance>(interval);

            int ms = (int)(interval * 1000);
            expirationTimer = new Timer(FlushExpired, null, ms, ms);
        }

        private void FlushExpired(object state)
        {
            foreach (var expired in expirationQueue.GetExpired())
            {
                instances.TryRemove(expired.address, out var dummy);
            }
        }

        public bool CanRequest(IPAddress address)
        {
            if (!instances.TryGetValue(address, out var instance))
            {
                instance = new LimitedInstance(address);
                if (!instances.TryAdd(address, instance))
                {
                    if (!instances.TryGetValue(address, out instance))
                        return false;
                }
                else
                {
                    expirationQueue.Enqueue(instance);
                }
            }

            lock (instance)
            {
                instance.count++;
                return instance.count < rateLimit;
            }
        }

        public void Dispose()
        {
            expirationTimer.Dispose();
        }
    }
}

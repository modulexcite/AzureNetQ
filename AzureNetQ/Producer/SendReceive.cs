﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AzureNetQ.Consumer;
using AzureNetQ.Topology;

namespace AzureNetQ.Producer
{
    public class SendReceive : ISendReceive
    {
        private readonly ConcurrentDictionary<string, IQueue> declaredQueues = new ConcurrentDictionary<string, IQueue>(); 

        public SendReceive()
        {
        }

        public void Send<T>(string queue, T message)
            where T : class
        {
            Preconditions.CheckNotNull(queue, "queue");
            Preconditions.CheckNotNull(message, "message");

            throw new NotImplementedException();
        }

        public IDisposable Receive<T>(string queue, Action<T> onMessage)
            where T : class
        {
            Preconditions.CheckNotNull(queue, "queue");
            Preconditions.CheckNotNull(onMessage, "onMessage");

            return Receive<T>(queue, message => TaskHelpers.ExecuteSynchronously(() => onMessage(message)));
        }

        public IDisposable Receive<T>(string queue, Func<T, Task> onMessage)
            where T : class
        {
            Preconditions.CheckNotNull(queue, "queue");
            Preconditions.CheckNotNull(onMessage, "onMessage");

            throw new NotImplementedException();
        }

        public IDisposable Receive(string queue, Action<IReceiveRegistration> addHandlers)
        {
            throw new NotImplementedException();
        }
    }
}
﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AzureNetQ.Consumer
{
    public class ConsumerDispatcher : IConsumerDispatcher
    {
        private readonly Thread dispatchThread;
        private readonly BlockingCollection<Action> queue;
        private bool disposed;

        public ConsumerDispatcher(IAzureNetQLogger logger)
        {
            Preconditions.CheckNotNull(logger, "logger");

            queue = new BlockingCollection<Action>();

            dispatchThread = new Thread(_ =>
                {
                    try
                    {
                        while (true)
                        {
                            if (disposed) break;

                            queue.Take()();
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // InvalidOperationException is thrown when Take is called after 
                        // queue.CompleteAdding(), this is signals that this class is being
                        // disposed, so we allow the thread to complete.
                    }
                    catch (Exception exception)
                    {
                        logger.ErrorWrite(exception);
                    }
                }) { Name = "AzureNetQ consumer dispatch thread" };
            dispatchThread.Start();
        }

        public void QueueAction(Action action)
        {
            Preconditions.CheckNotNull(action, "action");
            queue.Add(action);
        }

        public void OnDisconnected()
        {
            // throw away any queued actions. RabbitMQ will redeliver any in-flight
            // messages that have not been acked when the connection is lost.
            Action result;
            while(queue.TryTake(out result)) {}
        }

        public void Dispose()
        {
            queue.CompleteAdding();
            disposed = true;
        }

        public bool IsDisposed
        {
            get { return disposed; }
        }
    }
}
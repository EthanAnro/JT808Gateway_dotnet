﻿using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Enums;
using JT808.Gateway.InMemoryMQ.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JT808.Gateway.InMemoryMQ
{
    public class JT808MsgConsumer : IJT808MsgConsumer
    {
        private readonly JT808MsgService JT808MsgService;
        private readonly Func<JT808ConsumerType, JT808MsgServiceBase> func;
        public CancellationTokenSource Cts => new CancellationTokenSource();
        private readonly ILogger logger;
        public string TopicName => JT808GatewayConstants.MsgTopic;
        public JT808MsgConsumer(
            Func<JT808ConsumerType, JT808MsgServiceBase> func,
            JT808MsgService jT808MsgService,
            ILoggerFactory loggerFactory)
        {
            JT808MsgService = jT808MsgService;
            this.func = func;
            logger = loggerFactory.CreateLogger("JT808MsgConsumer");
        }

        public void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
            Task.Run(async() =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try
                    {
                        var item = await JT808MsgService.ReadAsync(Cts.Token);
                        foreach(var type in JT808ServerInMemoryMQExtensions.ConsumerTypes)
                        {
                            var method = func(type);
                            if (method != null)
                            {
                                await method.WriteAsync(item.TerminalNo, item.Data);
                            }
                        }
                        //callback(item);
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "");
                    }
                }
            }, Cts.Token);
        }

        public void Subscribe()
        {

        }

        public void Unsubscribe()
        {
            Cts.Cancel();
        }

        public void Dispose()
        {
            Cts.Dispose();
        }
    }
}

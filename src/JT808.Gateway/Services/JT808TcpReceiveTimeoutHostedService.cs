﻿using JT808.Gateway.Abstractions.Configurations;
using JT808.Gateway.Session;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JT808.Gateway.Services
{
    internal class JT808TcpReceiveTimeoutHostedService : BackgroundService
    {
        private readonly ILogger Logger;

        private readonly JT808SessionManager SessionManager;

        private readonly JT808Configuration Configuration;
        public JT808TcpReceiveTimeoutHostedService(
                IOptions<JT808Configuration> jT808ConfigurationAccessor,
                ILoggerFactory loggerFactory,
                JT808SessionManager jT808SessionManager
            )
        {
            SessionManager = jT808SessionManager;
            Logger = loggerFactory.CreateLogger<JT808TcpReceiveTimeoutHostedService>();
            Configuration = jT808ConfigurationAccessor.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var item in SessionManager.GetTcpAll())
                    {
                        try
                        {
                            if (item.ActiveTime.AddSeconds(Configuration.TcpReaderIdleTimeSeconds) < DateTime.Now)
                            {
                                item.ReceiveTimeout.Cancel();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e, "Check whether the session with id {id} and sim number {terminal} has timed out and failed.", item.SessionID, item.TerminalPhoneNo);
                            continue;
                        }
                    }
                    Logger.LogInformation($"[Check Receive Timeout]");
                    Logger.LogInformation($"[Session Online Count]:{SessionManager.TcpSessionCount}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"[Receive Timeout]");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(Configuration.TcpReceiveTimeoutCheckTimeSeconds), stoppingToken);
                }
            }
        }
    }
}

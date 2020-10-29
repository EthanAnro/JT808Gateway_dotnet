﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions.Configurations
{
    public class JT808Configuration
    {
        public int TcpPort { get; set; } = 808;
        public int UdpPort { get; set; } = 808;
        public int WebApiPort { get; set; } = 828;
        /// <summary>
        /// WebApi 默认token 123456 
        /// </summary>
        public string WebApiToken { get; set; } = "123456";
        public int SoBacklog { get; set; } = 8192;
        public int MiniNumBufferSize { get; set; } = 8096;
        /// <summary>
        /// Tcp读超时 
        /// 默认10分钟检查一次
        /// </summary>
        public int TcpReaderIdleTimeSeconds { get; set; } = 60 * 10;
        /// <summary>
        /// Tcp 60s检查一次
        /// </summary>
        public int TcpReceiveTimeoutCheckTimeSeconds { get; set; } = 60;
        /// <summary>
        /// Udp读超时
        /// </summary>
        public int UdpReaderIdleTimeSeconds { get; set; } = 60;
        /// <summary>
        /// Udp 60s检查一次
        /// </summary>
        public int UdpReceiveTimeoutCheckTimeSeconds { get; set; } = 60;
        /// <summary>
        /// 网关不做消息业务处理，往队列发送
        /// </summary>
        public List<uint> FilterMsgIdHandlerForQueue { get; set; } = new List<uint>();
    }
}

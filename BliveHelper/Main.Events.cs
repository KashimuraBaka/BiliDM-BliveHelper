﻿using BilibiliDM_PluginFramework;
using BliveHelper.Utils;
using System.IO;

namespace BliveHelper
{
    public partial class Main
    {
        public bool IsConnected { get; set; }

        public override async void Inited()
        {
            Log("加载配置中...");
            if (!Directory.Exists(ENV.ConfigDirectory))
            {
                Log("未发现配置文件夹，尝试创建中");
                Directory.CreateDirectory(ENV.ConfigDirectory);
            }
            // 初始化服务
            await ENV.InitServices();
            Log("配置加载完毕!");
            // 如果启用服务则自动启用
            if (ENV.Config.PluginEnabled)
            {
                Start();
            }
        }

        public override void DeInit()
        {
            AdminWindow.Close();
        }

        public override void Admin()
        {
            AdminWindow.Show();
        }

        public override void Start()
        {
            base.Start();
            ENV.Config.PluginEnabled = true;
        }

        public override void Stop()
        {
            base.Stop();
            ENV.Config.PluginEnabled = false;
        }

        private async void OnConnected(object sender, ConnectedEvtArgs e)
        {
            IsConnected = true;
            if (ENV.Config.WebSocket.AutoStream && !ENV.BliveInfo.IsStart)
            {
                await ENV.BliveInfo.StartStreamLive();
            }
        }

        private async void OnDisconnected(object sender, DisconnectEvtArgs e)
        {
            IsConnected = false;
            if (ENV.Config.WebSocket.AutoStream && ENV.BliveInfo.IsStart)
            {
                await ENV.BliveInfo.StopStreamLive();
            }
        }

        private void OnReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            if (e.Danmaku.MsgType is MsgTypeEnum.Comment)
            {
                AdminWindow.Dispatcher.Invoke(() => ENV.AddDanmaku(e.Danmaku));
            }
        }

        private void OnReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {
        }
    }
}

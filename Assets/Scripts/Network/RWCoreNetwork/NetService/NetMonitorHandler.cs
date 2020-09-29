using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RWCoreLib.Log;


namespace RWCoreNetwork.NetService
{
    public class PacketStatus
    {
        public int ProtocolId { get; set; }
        public int SendCount { get; set; }
        public int SendUsage { get; set; }
        public int ReceiveCount { get; set; }
        public int ReceiveUsage { get; set; }
    }


    public class NetMonitorHandler
    {
        /// <summary>
        /// 핸재 접속 클라이언트 수
        /// </summary>
        public int ClientCount { get; set; }

        /// <summary>
        /// Async 수신이벤트 Args pool 갯수
        /// </summary>
        public int ReceiveEventPoolCount { get; set; }

        /// <summary>
        /// Async 송신이벤트 Args pool 갯수
        /// </summary>
        public int SendEventPoolCount { get; set; }

        /// <summary>
        /// 수신 큐 현재 갯수
        /// </summary>
        public int ReceiveQueueCount { get; set; }

        /// <summary>
        /// 클라이언트별 송신 큐 현재 갯수
        /// </summary>
        public Dictionary<int, int> SendQueueCount { get; set; }


        public Dictionary<int, Dictionary<int, PacketStatus>> _userPacketStatus { get; private set; }


        private object _lockObject;
        private long _monitorTick;
        private long _monitorInterval;



        public NetMonitorHandler(int interval)
        {
            _lockObject = new object();
            _monitorTick = DateTime.UtcNow.AddSeconds(interval).Ticks;
            _monitorInterval = interval;
            _userPacketStatus = new Dictionary<int, Dictionary<int, PacketStatus>>();
        }


        public void Clear()
        {
            lock (_lockObject)
            {
                ClientCount = 0;
                ReceiveEventPoolCount = 0;
                SendEventPoolCount = 0;
                ReceiveQueueCount = 0;

                SendQueueCount.Clear();
            }
        }


        public void Print(ILog logger)
        {
            DateTime now = DateTime.UtcNow;
            if (_monitorTick > now.Ticks)
            {
                return;
            }

            _monitorTick = now.AddSeconds(_monitorInterval).Ticks;

            logger.Info("----------------------------------------------------------------");
            logger.Info("Network monitoring...");

            foreach (var user in _userPacketStatus)
            {
                logger.Info(string.Format("Packet Status. UserId: {0}", user.Key));
                foreach (var s in user.Value)
                {
                    logger.Info(string.Format("Protocol: {0}, SendCount: {1:n0}, SendUsage: {2:n0}, RecvCount: {3:n0}, RecvUsage: {4:n0}", 
                        s.Value.ProtocolId, s.Value.SendCount, s.Value.SendUsage, s.Value.ReceiveCount, s.Value.ReceiveUsage));
                }
                logger.Info("");
            }
            logger.Info("----------------------------------------------------------------");
        }


        public void SetSendPacket(int id, byte[] buffer)
        {
            lock (_lockObject)
            {
                int protocolId = BitConverter.ToInt32(buffer, 0);
                if (_userPacketStatus.ContainsKey(id) == false)
                {
                    _userPacketStatus.Add(id, new Dictionary<int, PacketStatus> {
                            { protocolId, new PacketStatus {
                                ProtocolId = protocolId,
                                SendCount = 1,
                                SendUsage = buffer.Length
                            }}
                        });
                }
                else
                {
                    if (_userPacketStatus[id].ContainsKey(protocolId) == false)
                    {
                        _userPacketStatus[id].Add(protocolId, new PacketStatus
                        {
                            ProtocolId = protocolId,
                            SendCount = 1,
                            SendUsage = buffer.Length
                        });
                    }
                    else
                    {
                        _userPacketStatus[id][protocolId].SendCount += 1;
                        _userPacketStatus[id][protocolId].SendUsage += buffer.Length;
                    }
                }
                //_logger.Debug("[OnReceive] protocolId: " + protocolId + ", BytesTransferred : " + transfered);
            }
        }


        public void SetReceivePacket(int id, byte[] buffer)
        {
            lock (_lockObject)
            {
                int protocolId = BitConverter.ToInt32(buffer, 0);
                int length = BitConverter.ToInt32(buffer, Defines.PROTOCOL_ID);

                if (_userPacketStatus.ContainsKey(id) == false)
                {
                    _userPacketStatus.Add(id, new Dictionary<int, PacketStatus> {
                            { protocolId, new PacketStatus {
                                ProtocolId = protocolId,
                                ReceiveCount = 1,
                                ReceiveUsage = length
                            }}
                        });
                }
                else
                {
                    if (_userPacketStatus[id].ContainsKey(protocolId) == false)
                    {
                        _userPacketStatus[id].Add(protocolId, new PacketStatus
                        {
                            ProtocolId = protocolId,
                            ReceiveCount = 1,
                            ReceiveUsage = length
                        });
                    }
                    else
                    {
                        _userPacketStatus[id][protocolId].ReceiveCount += 1;
                        _userPacketStatus[id][protocolId].ReceiveUsage += length;
                    }
                }
                //_logger.Debug("[OnReceive] protocolId: " + protocolId + ", BytesTransferred : " + transfered);
            }
        }
    }
}

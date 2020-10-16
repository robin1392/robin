using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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


    public class MonitorHandler
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


        private ILog _logger;


        private Dictionary<string, Dictionary<int, PacketStatus>> _userPacketStatus;
        private int _totalSendPacketUsage;
        private int _totalRecvPacketUsage;
        

        private object _lockObject;

        private long _startTimeTick;
        private long _endTimeTick;


        public MonitorHandler(ILog logger, int interval)
        {
            _logger = logger;
            _lockObject = new object();
            _startTimeTick = 0;
            _endTimeTick = 0;
            _userPacketStatus = new Dictionary<string, Dictionary<int, PacketStatus>>();
        }


        public void Clear()
        {
            lock (_lockObject)
            {
                ClientCount = 0;
                ReceiveEventPoolCount = 0;
                SendEventPoolCount = 0;
                ReceiveQueueCount = 0;

                //SendQueueCount.Clear();
                _userPacketStatus.Clear();
                _totalSendPacketUsage = 0;
                _totalRecvPacketUsage = 0;
                _startTimeTick = 0;
                _endTimeTick = 0;

            }
        }


        public void Start()
        {
            Clear();
            _startTimeTick = DateTime.UtcNow.Ticks;
        }


        public void End()
        {
            _endTimeTick = DateTime.UtcNow.Ticks;
        }


        public void ShowResult()
        {
            if (_userPacketStatus.Count == 0)
            {
                return;
            }


            _logger.Info("----------------------------------------------------------------------------------");
            _logger.Info("Network monitoring");
            _logger.Info("----------------------------------------------------------------------------------");
            

            foreach (var user in _userPacketStatus)
            {
                _logger.Info(string.Format("Packet Status. UserId: {0}", user.Key));

                // 패킷 프로토콜 아이디별 패킷 송수신 횟수 및 송수신량
                foreach (var s in user.Value)
                {
                    _logger.Info(string.Format("Protocol: {0}, SendCount: {1:n0}, SendUsage: {2:n0}, RecvCount: {3:n0}, RecvUsage: {4:n0}", 
                        s.Value.ProtocolId, 
                        s.Value.SendCount, 
                        s.Value.SendUsage, 
                        s.Value.ReceiveCount, 
                        s.Value.ReceiveUsage));
                }
            }


            // 총 패킷 송수신량
            long elapsedSecond = (_endTimeTick - _startTimeTick) / TimeSpan.TicksPerSecond;
            _logger.Info(string.Format("PlayTime: {0} sec, TotalSendPacketUsage: {1:n0} bytes, TotalRecvPacketUsage: {2:n0} bytes,  TotalAllPacketUsage: {3:n0} bytes",
                elapsedSecond, 
                _totalSendPacketUsage, 
                _totalRecvPacketUsage, 
                _totalSendPacketUsage + _totalRecvPacketUsage));


            // 초당 평균 패킷 송수신량
            _logger.Info(string.Format("AvgSendPacketUsage: {0:n0} bytes/sec, AvgRecvPacketUsage: {1:n0} bytes/sec",
                (float)_totalSendPacketUsage / elapsedSecond,
                (float)_totalRecvPacketUsage / elapsedSecond));


            _logger.Info("----------------------------------------------------------------------------------");
        }


        public void SetSendPacket(string sessionId, byte[] buffer)
        {
            lock (_lockObject)
            {
                int protocolId = BitConverter.ToInt32(buffer, 0);
                int length = BitConverter.ToInt32(buffer, Defines.PROTOCOL_ID) + Defines.HEADER_SIZE;

                _totalSendPacketUsage += length;


                if (_userPacketStatus.ContainsKey(sessionId) == false)
                {
                    _userPacketStatus.Add(sessionId, new Dictionary<int, PacketStatus> {
                            { protocolId, new PacketStatus {
                                ProtocolId = protocolId,
                                SendCount = 1,
                                SendUsage = length
                            }}
                        });
                }
                else
                {
                    if (_userPacketStatus[sessionId].ContainsKey(protocolId) == false)
                    {
                        _userPacketStatus[sessionId].Add(protocolId, new PacketStatus
                        {
                            ProtocolId = protocolId,
                            SendCount = 1,
                            SendUsage = length
                        });
                    }
                    else
                    {
                        _userPacketStatus[sessionId][protocolId].SendCount += 1;
                        _userPacketStatus[sessionId][protocolId].SendUsage += length;
                    }
                }
                _logger.Debug(string.Format("[Monitor] SetSendPacket. sessionId: {0}, protocolId: {1}, length : {2}", sessionId, protocolId, length));
            }
        }


        public void SetReceivePacket(string sessionId, int protocolId, byte[] msg, int length)
        {
            lock (_lockObject)
            {
                _totalRecvPacketUsage += length;

                if (_userPacketStatus.ContainsKey(sessionId) == false)
                {
                    _userPacketStatus.Add(sessionId, new Dictionary<int, PacketStatus> {
                            { protocolId, new PacketStatus {
                                ProtocolId = protocolId,
                                ReceiveCount = 1,
                                ReceiveUsage = length
                            }}
                        });
                }
                else
                {
                    if (_userPacketStatus[sessionId].ContainsKey(protocolId) == false)
                    {
                        _userPacketStatus[sessionId].Add(protocolId, new PacketStatus
                        {
                            ProtocolId = protocolId,
                            ReceiveCount = 1,
                            ReceiveUsage = length
                        });
                    }
                    else
                    {
                        _userPacketStatus[sessionId][protocolId].ReceiveCount += 1;
                        _userPacketStatus[sessionId][protocolId].ReceiveUsage += length;
                    }
                }

                _logger.Debug(string.Format("[Monitor] SetReceivePacket. sessionId: {0}, protocolId: {1}, length : {2}", sessionId, protocolId, length));
            }
        }
    }
}

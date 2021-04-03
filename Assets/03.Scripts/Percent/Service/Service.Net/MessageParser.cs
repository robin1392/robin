using System;

namespace Service.Net
{
    public class Defines
	{
        public static readonly int PROTOCOL_ID = 4;
        public static readonly int BODY_SIZE = 4;
        public static readonly int HEADER_SIZE = PROTOCOL_ID + BODY_SIZE;
    }

    /// <summary>
    /// [header][body] 구조를 갖는 데이터를 파싱하는 클래스.
    /// - header : ProtocolId + 데이터 사이즈. Defines.HEADERSIZE에 정의된 타입만큼의 크기를 갖는다.
    /// - body : 메시지 본문.
    /// </summary>
    public class MessageParser
    {
        readonly int _bufferSize;

        // 수신 버퍼.
        byte[] _receiveBuffer;

        // 메세지 body 사이즈.
        int _bodySize;

   		// 현재 진행중인 버퍼의 인덱스를 가리키는 변수.
		// 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜줘야 한다.
        int _currentPosition;

        // 읽어와야 할 목표 위치.
        int _positionToRead;

        // 남은 사이즈.
        int _remainBytes;


        public MessageParser(int bufferSize)
        {
            _receiveBuffer = new byte[bufferSize];
            _bufferSize = bufferSize;
            _bodySize = 0;
            _currentPosition = 0;
            _positionToRead = 0;
            _remainBytes = 0;
        }


        /// <summary>
		/// 소켓 버퍼로부터 데이터를 수신할 때 마다 호출된다.
		/// 데이터가 남아 있을 때 까지 계속 패킷을 만들어 callback을 호출 해 준다.
		/// 하나의 패킷을 완성하지 못했다면 버퍼에 보관해 놓은 뒤 다음 수신을 기다린다.
		/// </summary>
        /// <param name="buffer">소켓으로부터  수신된 데이터가 들어 있는 바이트 배열</param>
        /// <param name="offset">데이터의 시작 위치를 나타내는 정수 값</param>
        /// <param name="transfered">수신된 데이터의 바이트 수</param>
		/// <param name="callback">패킷 완성시 호출할 콜백 함수</param>
        public void OnReceive(ClientSession clientSession, byte[] buffer, int offset, int transfered, CompletedMessageDelegate callback)
        {
            // 이번 receive로 읽어오게 될 바이트 수.
            _remainBytes = transfered;

            // 원본 버퍼의 포지션값.
			// 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.
            int srcPosition = offset;


            // 남은 데이터가 있다면 계속 반복한다.
            while (_remainBytes > 0)
            {
                bool completed = false;

                // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.
                if (_currentPosition < Defines.HEADER_SIZE)
                {
                    // 목표 지점 설정(헤더 위치까지 도달하도록 설정).
                    _positionToRead = Defines.HEADER_SIZE;


                    completed = ReadBuffer(buffer, ref srcPosition);
                    if (completed == false)
                    {
                        // 아직 다 못읽었으므로 다음 receive를 기다린다.
                        return;
                    }


                    // 메세지 사이즈를 구한다.
                    _bodySize = BitConverter.ToInt32(_receiveBuffer, Defines.PROTOCOL_ID);


                    // 다음 목표 지점
                    _positionToRead += _bodySize;
                }


                // 메시지를 읽는다.
                completed = ReadBuffer(buffer, ref srcPosition);
                if (completed == true)
                {
                    int protocolId = BitConverter.ToInt32(_receiveBuffer, 0);
                    int length = BitConverter.ToInt32(_receiveBuffer, Defines.PROTOCOL_ID);
                    byte[] msg = new byte[_bufferSize];
                    Array.Copy(_receiveBuffer, Defines.HEADER_SIZE, msg, 0, length);

                    callback(clientSession, protocolId, msg, length);
                    ClearBuffer();
                }
            }
        }


        /// <summary>
        /// 목표지점으로 설정된 위치까지의 바이트를 원본 버퍼로부터 복사한다.
        /// 데이터가 모자랄 경우 현재 남은 바이트 까지만 복사한다.
        /// </summary>
        /// <param name="buffer">소켓 버퍼</param>
        /// <param name="srcPosition">소켓 버퍼 현재 오프셋</param>
        /// <param name="transffered">전송량</param>
        /// <returns>다 읽었으면 true, 데이터가 모자라서 못 읽었으면 false를 리턴한다.</returns>
        private bool ReadBuffer(byte[] buffer, ref int srcPosition)
        {
            if (_remainBytes <= 0 && _bodySize > 0)
            {
                // 들어온 데이터 만큼 다 읽은 상태이므로 더이상 읽을 데이터가 없다.
                return false;
            }

			// 읽어와야 할 바이트.
			// 데이터가 분리되어 올 경우 이전에 읽어놓은 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해 준다.
            int copySize = _positionToRead - _currentPosition;

			// 남은 데이터가 더 적다면 가능한 만큼만 복사한다.
            if (_remainBytes < copySize)
            {
                copySize = _remainBytes;
            }

			// 버퍼에 복사.
            Array.Copy(buffer, srcPosition, _receiveBuffer, _currentPosition, copySize);

            // 원본 버퍼 포지션 이동.
            srcPosition += copySize;

            // 타겟 버퍼 포지션도 이동.
            _currentPosition += copySize;

            // 남은 바이트 수.
            _remainBytes -= copySize;

            // 목표지점에 도달 못했으면 false
            if (_currentPosition < _positionToRead)
            {
                return false;
            }
            
            return true;
        }


        public byte[] WriteBuffer(int protocolId, byte[] msg, int length)
        {
            byte[] buffer = new byte[Defines.HEADER_SIZE + length];

            // protocol id
            int offset = 0;
            byte[] tmpBuffer = BitConverter.GetBytes(protocolId);
            Array.Copy(tmpBuffer, 0, buffer, offset, Defines.PROTOCOL_ID);

            // body length
            offset = tmpBuffer.Length;
            tmpBuffer = BitConverter.GetBytes(length);
            Array.Copy(tmpBuffer, 0, buffer, offset, Defines.BODY_SIZE);

            // msg
            offset += tmpBuffer.Length;
            Array.Copy(msg, 0, buffer, offset, length);

            return buffer;
        }


        private void ClearBuffer()
        {
            Array.Clear(_receiveBuffer, 0, _receiveBuffer.Length);

            _currentPosition = 0;
            _bodySize = 0;
        }
    }    
}
using System;

namespace RWCoreNetwork
{
    class Defines
	{
        public static readonly short PROTOCOL_ID = 2;
		public static readonly short HEADERSIZE = 2;
	}

	/// <summary>
	/// [header][body] 구조를 갖는 데이터를 파싱하는 클래스.
	/// - header : 데이터 사이즈. Defines.HEADERSIZE에 정의된 타입만큼의 크기를 갖는다.
	///				2바이트일 경우 Int16, 4바이트는 Int32로 처리하면 된다.
	///				본문의 크기가 Int16.Max값을 넘지 않는다면 2바이트로 처리하는것이 좋을것 같다.
	/// - body : 메시지 본문.
	/// </summary>
    public class MessageHandler
    {
   		public delegate void CompletedMessageCallback(short protocolId, byte[] msg);

        // 진행중인 버퍼.
        byte[] m_messageBuffer;

        // 프로토콜 아이디
        short m_protocolId;

        // 메시지 사이즈.
        int m_messageSize;

   		// 현재 진행중인 버퍼의 인덱스를 가리키는 변수.
		// 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜줘야 한다.
        int m_currentPosition;

        // 읽어와야 할 목표 위치.
        int m_positionToRead;

        // 남은 사이즈.
        int m_remainBytes;


        public MessageHandler()
        {
            m_messageBuffer = new byte[1024];
            m_messageSize = 0;
            m_currentPosition = 0;
            m_positionToRead = 0;
            m_remainBytes = 0;
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
        public void OnReceive(byte[] buffer, int offset, int transfered, CompletedMessageCallback callback)
        {
   			// 이번 receive로 읽어오게 될 바이트 수.
            m_remainBytes = transfered;

            // 원본 버퍼의 포지션값.
			// 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.
            int srcPosition = offset;

            // 남은 데이터가 있다면 계속 반복한다.
            while (m_remainBytes > 0)
            {
                bool completed = false;

                // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.
                if (m_currentPosition < Defines.PROTOCOL_ID + Defines.HEADERSIZE)
                {
                    // 목표 지점 설정(헤더 위치까지 도달하도록 설정).
                    m_positionToRead = Defines.PROTOCOL_ID + Defines.HEADERSIZE;

                    // completed = ReadUntil(buffer, ref srcPosition, offset, transfered);
                    // if (completed == false)
                    // {
                    //     // 아직 다 못읽었으므로 다음 receive를 기다린다.
                    //     return;
                    // }

                    // // 프로토콜 아이디를 구한다.
                    // m_protocolId = GetProtocolId();

                    // // 헤더 하나를 온전히 읽어왔으므로 메시지 사이즈를 구한다.
                    // m_messageSize = GetBodySize();

                    m_protocolId = BitConverter.ToInt16(buffer, srcPosition);
                    m_messageSize = BitConverter.ToInt16(buffer, srcPosition + Defines.PROTOCOL_ID);

                    int copySize = m_positionToRead - m_currentPosition;

                    // 원본 버퍼 포지션 이동.
                    srcPosition += copySize;

                    // 타겟 버퍼 포지션도 이동.
                    //m_currentPosition += copySize;

                    // 남은 바이트 수.
                    m_remainBytes -= copySize;

                    // 다음 목표 지점(헤더 + 메시지 사이즈).
                    m_positionToRead = m_messageSize;
                }

                // 메시지를 읽는다.
                completed = ReadUntil(buffer, ref srcPosition, offset, transfered);
                if (completed == true)
                {
                    callback(m_protocolId, m_messageBuffer);
                    ClearBuffer();
                }
            }
        }

		/// <summary>
		/// 목표지점으로 설정된 위치까지의 바이트를 원본 버퍼로부터 복사한다.
		/// 데이터가 모자랄 경우 현재 남은 바이트 까지만 복사한다.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="srcPosition"></param>
		/// <param name="offset"></param>
		/// <param name="transffered"></param>
		/// <returns>다 읽었으면 true, 데이터가 모자라서 못 읽었으면 false를 리턴한다.</returns>
        private bool ReadUntil(byte[] buffer, ref int srcPosition, int offset, int transffered)
        {
            if (m_currentPosition >= offset + transffered)
            {
				// 들어온 데이터 만큼 다 읽은 상태이므로 더이상 읽을 데이터가 없다.
                return false;
            }

			// 읽어와야 할 바이트.
			// 데이터가 분리되어 올 경우 이전에 읽어놓은 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해 준다.
            int copySize = m_positionToRead - m_currentPosition;

			// 남은 데이터가 더 적다면 가능한 만큼만 복사한다.
            if (m_remainBytes < copySize)
            {
                copySize = m_remainBytes;
            }

			// 버퍼에 복사.
            Array.Copy(buffer, srcPosition, m_messageBuffer, m_currentPosition, copySize);

            // 원본 버퍼 포지션 이동.
            srcPosition += copySize;

            // 타겟 버퍼 포지션도 이동.
            m_currentPosition += copySize;

            // 남은 바이트 수.
            m_remainBytes -= copySize;

            // 목표지점에 도달 못했으면 false
            if (m_currentPosition < m_positionToRead)
            {
                return false;
            }
            
            return true;
        }

        private Int16 GetProtocolId()
        {
            Type type = Defines.PROTOCOL_ID.GetType();
            if (type.Equals(typeof(Int16)) == false)
            {
                throw new Exception("Invalid Protocol id type!");
            }

            return BitConverter.ToInt16(m_messageBuffer, 0);
        }

        /// <summary>
        /// 헤더 타입의 바이트만큼을 읽어와 메시지 사이즈를 리턴한다.
        /// </summary>
        private int GetBodySize()
        {
            Type type = Defines.HEADERSIZE.GetType();
            if (type.Equals(typeof(Int16)))
            {
                return BitConverter.ToInt16(m_messageBuffer, Defines.PROTOCOL_ID);
            }

            return BitConverter.ToInt32(m_messageBuffer, 0);
        }

        private void ClearBuffer()
        {
            Array.Clear(m_messageBuffer, 0, m_messageBuffer.Length);

            m_currentPosition = 0;
            m_messageSize = 0;
        }
    }
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RWCoreNetwork
{
    /// <summary>
    /// 
    /// </summary> 
    public class Listener
    {
        // 클라이언트의 접속을 처리할 소켓
        Socket _listenSocket;

        Thread _listenThread;

        // 비동기 Accept를 위한 EventArgs
        SocketAsyncEventArgs _acceptArgs;

        // Accept처리의 순서를 제어하기 위한 이벤트 변수
        AutoResetEvent _flowControlEvent;

        // 새로운 클라이언트가 접속했을 때 호출되는 콜백
        public delegate void NewClientHandler(Socket clientSocket, object token);
        public NewClientHandler OnNewClientCallback;


        public Listener()
        {
            OnNewClientCallback = null;

            _acceptArgs = new SocketAsyncEventArgs();
            _acceptArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            
            _listenThread = new Thread(DoListen);
        }

        public bool Start(string host, int port, int backlog)
        {
            IPAddress address;
            if (host == "0.0.0.0")
            {
                address = IPAddress.Any;
            }
            else
            {
                address = IPAddress.Parse(host);
            }
            IPEndPoint endpoint = new IPEndPoint(address, port);

            
            if (_listenSocket != null)
            {
                _listenSocket.Close();
                _listenSocket = null;
            }
            
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _listenSocket.Bind(endpoint);
            _listenSocket.Listen(backlog);
            _listenThread.Start();

            return true;
        }

        /// <summary>
        /// 루프를 돌며 클라이언트를 받아 들이고
        /// 하나의 접속 처리가 완료된 후 다음 accept를 수행하기 위해서 event객체를 통해 흐름을 제어함.
        /// </summary>
        private void DoListen()
        {
            _flowControlEvent = new AutoResetEvent(false);

            while (true)
            {
                // SocketAsyncEventArgs를 재사용 하기 위해서 null로 만들어 준다.
                _acceptArgs.AcceptSocket = null;

                bool pending = true;
                try
                {
                    // 비동기 accept를 호출하여 클라이언트의 접속을 받아들입니다.
					// 비동기 매소드 이지만 동기적으로 수행이 완료될 경우도 있으니
					// 리턴값을 확인하여 분기시켜야 합니다.
                    pending = _listenSocket.AcceptAsync(_acceptArgs);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

				// 즉시 완료 되면 이벤트가 발생하지 않으므로 리턴값이 false일 경우 콜백 매소드를 직접 호출해 줍니다.
				// pending상태라면 비동기 요청이 들어간 상태이므로 콜백 매소드를 기다리면 됩니다.
                if (pending == false)
                {
                    OnAcceptCompleted(null, _acceptArgs);
                }

				// 클라이언트 접속 처리가 완료되면 이벤트 객체의 신호를 전달받아 다시 루프를 수행하도록 합니다.
                _flowControlEvent.WaitOne();
            }
        }

        /// <summary>
        /// AcceptAsync의 콜백 매소드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">AcceptAsync 매소드 호출시 사용된 EventArgs</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // 새로 생긴 소켓 보관
                Socket clientSocket = e.AcceptSocket;

                // 다음 연결을 받아들인다.
                _flowControlEvent.Set();

                // 이 클래스에서는 accept까지의 역할만 수행하고 클라이언트의 접속 이후의 처리는
                // 외부로 넘기기 위해서 콜백 매소드를 호출해 주도록 합니다.
                // 이유는 소켓 처리부와 컨텐츠 구현부를 분리하기 위함입니다.
                // 컨텐츠 구현부분은 자주 바뀔 가능성이 있지만, 소켓 Accept부분은 상대적으로 변경이 적은 부분이기 때문에 양쪽을 분리시켜주는것이 좋습니다.
                if (OnNewClientCallback != null)
                {
                    OnNewClientCallback(clientSocket, e.UserToken);
                }

                return;
            }
            else
            {
                //todo:Accept 실패 처리.
                Console.WriteLine("Failed to accept client.");
            }

            // 다음 연결을 받아들인다.
            _flowControlEvent.Set();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            _listenSocket.Close();
        }
    }
}
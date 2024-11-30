using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// ������ ��� �Ͽ� ��,���ŵ��� �ϱ����� Ŭ����
/// </summary>
public class UserToken
{
    // ���Ͽ� ���� ����
    private enum EState
    {
        Idle,               // �����
        Connected,          // �����
        ReserveClosing,     // ���ᰡ �����, �����ִ� ��Ŷ�� ��� ���� �� ������ �ϱ� ���� ���°�
        Closed              // ������ ���� �����
    }

    private Socket _socket;
    private EState _curState = EState.Idle;

    private SocketAsyncEventArgs _receiveEventArgs;
    private SocketAsyncEventArgs _sendEventArgs;
    private MessageResolver _messageResolver = new MessageResolver(NetDefine.BUFFER_SIZE * 3);
    private IPeer _peer; // Peer ��ü. ���ø����̼ǿ��� �����Ͽ� ���. ��Ŷó���� �߰����� ������ �Ѵ�.
    private List<byte[]> _sendingList = new List<byte[]>();
    private IMessageDispatcher _dispatcher; // ��Ŷ �޽����� ���ν����忡�� ó���ϱ����� Dispatcher

    public Socket Socket => _socket;
    public SocketAsyncEventArgs ReceiveEventArgs => _receiveEventArgs;
    public SocketAsyncEventArgs SendEventArgs => _sendEventArgs;
    public bool IsConnected => _curState == EState.Connected;
    public IPeer Peer => _peer;

    public event Action<UserToken> onSessionClosed; // Close������ ȣ��Ǵ� event

    public UserToken(Socket socket, IMessageDispatcher dispatcher)
    {
        _socket = socket ?? throw new ArgumentNullException(nameof(socket), "Socket cannot be null");
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "Dispatcher cannot be null");

        // Receive�� SocketAsyncEventArgs ����
        _receiveEventArgs = new SocketAsyncEventArgs();
        _receiveEventArgs.UserToken = this;
        _receiveEventArgs.Completed += OnReceiveCompleted;
        _receiveEventArgs.SetBuffer(new byte[NetDefine.BUFFER_SIZE], 0, NetDefine.BUFFER_SIZE);
        // Send�� SocketAsyncEventArgs ����
        _sendEventArgs = new SocketAsyncEventArgs();
        _sendEventArgs.UserToken = this;
        _sendEventArgs.Completed += OnSendComplteted;
    }

    public void OnConnected()
    {
        _curState = EState.Connected;
        Console.WriteLine($"Connection established with {_socket.RemoteEndPoint}");

    }

    public void SetPeer(IPeer peer)
    {
        _peer = peer;
    }

    public void StartReceive()
    {


        Console.WriteLine($"Start receiving data from {_socket.RemoteEndPoint}");

        bool pending = false;
        try
        {
            // �񵿱� Receive
            pending = _socket.ReceiveAsync(_receiveEventArgs);
        }
        catch
        {
        }

        // ��������ʰ� �ٷ� Receive�� �Ǿ��ٸ� ����
        if (!pending)
        {
            OnReceiveCompleted(null, _receiveEventArgs);
        }
    }

    private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.LastOperation == SocketAsyncOperation.Receive)
        {
            _messageResolver.OnReceive(e.Buffer, e.Offset, e.BytesTransferred, OnMessageComplete);
            Console.WriteLine($"Received {e.BytesTransferred} bytes from {_socket.RemoteEndPoint}");

        }
        else
        {
            Console.WriteLine($"Connection closed by {_socket.RemoteEndPoint}");

            _socket.Close();
        }

        // �ٽ� Receive ����
        StartReceive();
    }

    private void OnMessageComplete(byte[] buffer)
    {
        // ���ν����忡�� ��Ŷ�� ó���ϰ� �Ѵ�.
        _dispatcher.OnMessage(this, buffer);
    }

    public void OnMessage(byte[] buffer)
    {
        if (_peer == null)
            return;

        // protocolID �����´�
        short protocolID = BitConverter.ToInt16(buffer, 2);
        Debug.Log($"Protocol ID : {protocolID}");
        _peer.ProcessMessage(protocolID, buffer);
    }

    public void Close()
    {
        Console.WriteLine($"Closing connection with {_socket.RemoteEndPoint}");

        if (_curState == EState.Closed)
        {
            return;
        }

        _curState = EState.Closed;
        _socket.Close();

        _socket = null;

        _sendingList.Clear();

        MainThreadDispatcher.Instance.Add(() =>
        {
            onSessionClosed?.Invoke(this);
        });

        _peer.Remove();

        Console.WriteLine("Close");
    }

    public void Send(byte[] data)
    {
        if (_socket == null)
        {
            Console.WriteLine("Error: Socket is null.");
            return;
        }

        Console.WriteLine($"Sending {data.Length} bytes to {_socket.RemoteEndPoint}");

        lock (_sendingList)
        {
            _sendingList.Add(data);

            if (_sendingList.Count > 1)
            {
                // ť�� ���𰡰� ��� �ִٸ� ���� ���� ������ �Ϸ���� ���� �����̹Ƿ� ť�� �߰��� �ϰ� �����Ѵ�.
                // ���� �������� SendAsync�� �Ϸ�� ���Ŀ� ť�� �˻��Ͽ� �����Ͱ� ������ SendAsync�� ȣ���Ͽ� �������� ���̴�.
                return;
            }

            StartSend();
        }
    }

    public void Send(Packet packet)
    {
        Send(packet.ToByte());
    }

    /// <summary>
    /// �񵿱� ������ �����Ѵ�.
    /// </summary>
    void StartSend()
    {
        try
        {
            // Send�� �̶� ���۸� ä���ش�.
            _sendEventArgs.SetBuffer(_sendingList[0], 0, _sendingList[0].Length);

            // �񵿱� ���� ����.
            bool pending = _socket.SendAsync(_sendEventArgs);
            if (!pending)
            {
                OnSendComplteted(null, _sendEventArgs);
            }
        }
        catch (Exception e)
        {
            if (_socket == null)
            {
                Close();
                return;
            }

            Console.WriteLine("send error!! close socket. " + e.Message);
        }
    }

    public void OnSendComplteted(object sender, SocketAsyncEventArgs e)
    {
        if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
        {
            // ������ ���ܼ� �̹� ������ ����� ����� ���̴�.
            return;
        }

        lock (_sendingList)
        {
            // ���� ���� ����
            _sendingList.RemoveAt(0);
            // �������� �����ִٸ� ������.
            if (_sendingList.Count > 0)
            {
                StartSend();
                return;
            }

            // ���ᰡ ����� ���, ������ �� �������� ��¥ ���� ó���� �����Ѵ�.
            if (_curState == EState.ReserveClosing)
            {
                _socket.Shutdown(SocketShutdown.Send);
            }
        }
    }

    /// <summary>
    /// Ŭ���̾�Ʈ���� ���� ����� ���.
    /// </summary>
    public void Disconnect()
    {
        try
        {
            if (_sendingList.Count <= 0)
            {
                _socket.Shutdown(SocketShutdown.Send);
                return;
            }

            // ������ �����ִٸ�
            _curState = EState.ReserveClosing;
        }
        catch (Exception)
        {
            Close();
        }
    }
}

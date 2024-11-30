using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// ���ŵ� �޽����� ����Ƽ�� ���ν����忡�� ó���ϰ� �Ѵ�.
/// </summary>
public class PacketMessageDispatcher : MonoBehaviour, IMessageDispatcher
{
    // �̱���(Singleton) ���� : �ν��Ͻ��� �ϳ��� ����, �������� ����
    private static PacketMessageDispatcher _instance;
    public static PacketMessageDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject container = new GameObject("PacketMessageDispatcher");
                container.name = "PacketMessageDispatcher";
                _instance = container.AddComponent<PacketMessageDispatcher>();
             //   _instance.Init();
            }

            return _instance;
        }
    }

    public struct PacketMessage
    {
        public UserToken token; // �ش� �����͸� ó���� ����
        public byte[] buffer;   // ���ŵ� ������

        public PacketMessage(UserToken token, byte[] buffer)
        {
            this.token = token;
            this.buffer = buffer;
        }
    }

    // �����忡 ������ Queue
    private ConcurrentQueue<PacketMessage> _messageQueue;

    public void Init()
    {
        _messageQueue = new ConcurrentQueue<PacketMessage>();
    }

    // ����Ƽ�� Update���� Queue�� ����ִ� ��Ŷ�� ��� ó���ϰ� �Ѵ�.
    private void Update()
    {
        // TryDequeue �����ϰ� ó�� ��Ҹ� �����´�.
        while (_messageQueue.TryDequeue(out PacketMessage msg))
        {

            if (!msg.token.IsConnected)
                continue;

            msg.token.OnMessage(msg.buffer);
        }
        
    }

    // ���� ��Ŷ�� �߰��Ѵ�.
    public void OnMessage(UserToken token, byte[] buffer)
    {
        _messageQueue.Enqueue(new PacketMessage(token, buffer));
    }
}
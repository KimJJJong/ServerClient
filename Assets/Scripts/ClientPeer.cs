/*using UnityEngine;

public class ClientPeer : IPeer
{
    private UserToken _userToken;

    public ClientPeer(UserToken userToken)
    {
        _userToken = userToken;
    }

    /// <summary>
    /// �����κ��� ���� �޽����� ó���մϴ�.
    /// </summary>
    public void ProcessMessage(short protocolID, byte[] buffer)
    {
        switch ((EProtocolID)protocolID)
        {
            case EProtocolID.SC_REQ_USERINFO:
                HandleServerRequestUserInfo(buffer);
                break;

            case EProtocolID.SC_GAME_START:
                HandleGameStart(buffer);
                break;

            default:
                Debug.LogWarning($"[ClientPeer] �� �� ���� ��������: {protocolID}");
                break;
        }
    }

    /// <summary>
    /// ������ ���� ������ ��û���� �� ó���մϴ�.
    /// </summary>
    private void HandleServerRequestUserInfo(byte[] buffer)
    {
        PacketReqUserInfo packet = new PacketReqUserInfo();
        packet.ToPacket(buffer);

        Debug.Log($"[ClientPeer] �������� ���� ���� ��û. UID: {packet.uid}");

        PacketAnsUserInfo response = new PacketAnsUserInfo
        {
            id = "Player1"
        };
        Send(response);

        Debug.Log("[ClientPeer] ���� ���� ���� ���� �Ϸ�.");
    }

    /// <summary>
    /// ���� ���� ��Ŷ ó��
    /// </summary>
    private void HandleGameStart(byte[] buffer)
    {
        PacketGameStart packet = new PacketGameStart();
        packet.ToPacket(buffer);

        Debug.Log("[ClientPeer] ���� ���� ����. �÷��̾� ������:");
        for (int i = 0; i < packet.userNum; i++)
        {
            var startInfo = packet.startInfos[i];
            Debug.Log($"UID: {startInfo.uid}, ID: {startInfo.id}");
        }

        // ���� �Ŵ��� �Ǵ� Ŭ���̾�Ʈ�� ���� ���� ó�� ���� ȣ��
        // Example: GameManager.Instance.OnGameStart(packet);
    }

    /// <summary>
    /// Ŭ���̾�Ʈ�� ��Ŷ ����
    /// </summary>
    public void Send(Packet packet)
    {
        _userToken.Send(packet);
    }

    /// <summary>
    /// ������ ����� �� ȣ��˴ϴ�.
    /// </summary>
    public void Remove()
    {
        Debug.Log("[ClientPeer] �������� ���� ����.");
        _userToken?.Close();
    }
}
*/
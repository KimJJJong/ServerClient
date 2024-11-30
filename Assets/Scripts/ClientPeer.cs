/*using UnityEngine;

public class ClientPeer : IPeer
{
    private UserToken _userToken;

    public ClientPeer(UserToken userToken)
    {
        _userToken = userToken;
    }

    /// <summary>
    /// 서버로부터 받은 메시지를 처리합니다.
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
                Debug.LogWarning($"[ClientPeer] 알 수 없는 프로토콜: {protocolID}");
                break;
        }
    }

    /// <summary>
    /// 서버가 유저 정보를 요청했을 때 처리합니다.
    /// </summary>
    private void HandleServerRequestUserInfo(byte[] buffer)
    {
        PacketReqUserInfo packet = new PacketReqUserInfo();
        packet.ToPacket(buffer);

        Debug.Log($"[ClientPeer] 서버에서 유저 정보 요청. UID: {packet.uid}");

        PacketAnsUserInfo response = new PacketAnsUserInfo
        {
            id = "Player1"
        };
        Send(response);

        Debug.Log("[ClientPeer] 유저 정보 응답 전송 완료.");
    }

    /// <summary>
    /// 게임 시작 패킷 처리
    /// </summary>
    private void HandleGameStart(byte[] buffer)
    {
        PacketGameStart packet = new PacketGameStart();
        packet.ToPacket(buffer);

        Debug.Log("[ClientPeer] 게임 시작 수신. 플레이어 데이터:");
        for (int i = 0; i < packet.userNum; i++)
        {
            var startInfo = packet.startInfos[i];
            Debug.Log($"UID: {startInfo.uid}, ID: {startInfo.id}");
        }

        // 게임 매니저 또는 클라이언트의 게임 시작 처리 로직 호출
        // Example: GameManager.Instance.OnGameStart(packet);
    }

    /// <summary>
    /// 클라이언트로 패킷 전송
    /// </summary>
    public void Send(Packet packet)
    {
        _userToken.Send(packet);
    }

    /// <summary>
    /// 연결이 종료될 때 호출됩니다.
    /// </summary>
    public void Remove()
    {
        Debug.Log("[ClientPeer] 서버와의 연결 종료.");
        _userToken?.Close();
    }
}
*/
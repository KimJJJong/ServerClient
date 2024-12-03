
using UnityEditor;
using UnityEngine;

public class Client: MonoBehaviour, IPeer
{
    private NetClient _client = new NetClient();
    private UserToken _userToken;
    private UIManager _ui;     //  UI ���� ��ũ���� ���� ������????����~~~~~~~

    public void StartClient(string ip)
    {
        MainThreadDispatcher.Instance.Init();
        PacketMessageDispatcher.Instance.Init();
        _client.onConnected += OnConnected;
        _client.Start(ip);

        _ui = FindObjectOfType<UIManager>();
    }

    private void OnConnected(bool connected, UserToken token)
    {
        if (connected)
        {
            Debug.Log("������ ���� �Ϸ�");
            _userToken = token;
            _userToken.SetPeer(this);
        }
    }

    public void ProcessMessage(short protocolID, byte[] buffer)
    {
        switch ((EProtocolID)protocolID)
        {
            case EProtocolID.SC_REQ_USERINFO:
                {
                    PacketReqUserInfo packet = new PacketReqUserInfo();
                    packet.ToPacket(buffer);
                    GameManager.Instance.UserUID = packet.uid;

                    PacketAnsUserInfo sendPacket = new PacketAnsUserInfo();
                    sendPacket.id = GameManager.Instance.UserID;

                    Send(sendPacket);
                }
                break;
            case EProtocolID.SC_ANS_USERLIST:
                {
                    // �������� ������ ��� ���� ������ �޾�����.
                    PacketAnsUserList packet = new PacketAnsUserList();
                    packet.ToPacket(buffer);

                     string strUserList = string.Empty;

                    for (int i = 0; i < packet.userNum; i++)
                    {
                        strUserList += $"ID:{packet.userInfos[i].id} UID:{packet.userInfos[i].uid} \n";

                    }

                    _ui.SetUIState(UIManager.EUIState.Lobby);
                    _ui.SetLobbyText(strUserList);
                }
                break;
            case EProtocolID.SC_ANS_JOIN_GAMEROOM:
                {
                    PacketAnsJoinRoom packet = new PacketAnsJoinRoom();
                    packet.ToPacket(buffer);
                    //���� �뿡 �����ߴٴ°� = Ŭ�� �κ� -> GameReady Panel
                    _ui.SetUIState(UIManager.EUIState.Ready);
                    _ui.ShowPlayer(packet.isFirst, packet.id);

                }
                break;

            case EProtocolID.SC_ANS_GAMEROOM_CREATE:
                {
                    PacketRoomInfo packet = new PacketRoomInfo();
                    packet.ToPacket(buffer);

                    _ui.CreateRoom(packet.roomId);
                    
                    // roomId��° ���� ���� ����� �����
                }
                break;

            case EProtocolID.REL_GAME_READY:
                {
                    GameManager.Instance.GameReady();
                }
                break;
            case EProtocolID.SC_GAME_START:
                {

                    PacketGameStart packet = new PacketGameStart();
                    packet.ToPacket(buffer);

                }
                break;

            case EProtocolID.SC_GAME_END:
                {
                    PacketGameEnd packet = new PacketGameEnd();
                    packet.ToPacket(buffer);

                }
                break;
        }
    }

    public void Remove()
    {
    }

    public void Send(Packet packet)
    {
        _userToken.Send(packet);
    }
}

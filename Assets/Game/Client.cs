/*using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Client : MonoBehaviour
{
    // TextMeshPro ���� UI ��� ���� ����
    public TMP_InputField IPInputField;   // ���� IP�� �Է¹޴� TMP_InputField
    public Button ConnectButton;          // ������ �����ϱ� ���� ��ư
    public TextMeshProUGUI StatusText;    // ���� ���¸� ǥ���ϴ� TextMeshProUGUI

    private NetClient _netClient;     // ��Ʈ��ũ Ŭ���̾�Ʈ �ν��Ͻ�
    private ClientPeer _clientPeer;   // �������� ��� ó�� ���� ��� �ν��Ͻ�

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �޼��� ����
        ConnectButton.onClick.AddListener(OnConnectButtonClicked);

        MainThreadDispatcher.Instance.Init();
        PacketMessageDispatcher.Instance.Init();

        // ��Ʈ��ũ Ŭ���̾�Ʈ �ʱ�ȭ
        _netClient = new NetClient();
        _netClient.onConnected += OnConnected; // ���� ���� �̺�Ʈ
    }

    private void OnConnectButtonClicked()
    {
        string serverIP = IPInputField.text;
        if (string.IsNullOrEmpty(serverIP))
        {
            StatusText.text = "��ȿ�� IP �ּҸ� �Է��ϼ���.";
            return;
        }

        // ������ ���� �õ�
        _netClient.Start(serverIP);
        StatusText.text = "������ ���� �õ� ��...";
    }

    private void OnConnected(bool connected, UserToken userToken)
    {
        if (connected)
        {
            // ���� ���� �� �޽��� ���
            StatusText.text = "������ ����Ǿ����ϴ�.";
            
            // UserToken�� �̿��Ͽ� ClientPeer ���� �� ����
            _clientPeer = new ClientPeer(userToken);
            userToken.SetPeer(_clientPeer);

            // ����� ���� ������ ���� ���� ����
            SendUserInfo();
        }
        else
        {
            // ���� ���� �� �޽��� ���
            StatusText.text = "���� ���� ����.";
        }
    }

    private void SendUserInfo()
    {
        // ������ ���� ������ �����ϴ� ��Ŷ ���� �� ����
        PacketAnsUserInfo userInfoPacket = new PacketAnsUserInfo
        {
            id = "Player1"
        };
        _clientPeer.Send(userInfoPacket);
        StatusText.text = "���� ������ ������ �����߽��ϴ�.";
    }
}
*/
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

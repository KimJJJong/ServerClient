/*using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Client : MonoBehaviour
{
    // TextMeshPro 전용 UI 요소 변수 선언
    public TMP_InputField IPInputField;   // 서버 IP를 입력받는 TMP_InputField
    public Button ConnectButton;          // 서버에 연결하기 위한 버튼
    public TextMeshProUGUI StatusText;    // 연결 상태를 표시하는 TextMeshProUGUI

    private NetClient _netClient;     // 네트워크 클라이언트 인스턴스
    private ClientPeer _clientPeer;   // 서버와의 통신 처리 로직 담당 인스턴스

    private void Start()
    {
        // 버튼 클릭 이벤트에 메서드 연결
        ConnectButton.onClick.AddListener(OnConnectButtonClicked);

        MainThreadDispatcher.Instance.Init();
        PacketMessageDispatcher.Instance.Init();

        // 네트워크 클라이언트 초기화
        _netClient = new NetClient();
        _netClient.onConnected += OnConnected; // 서버 연결 이벤트
    }

    private void OnConnectButtonClicked()
    {
        string serverIP = IPInputField.text;
        if (string.IsNullOrEmpty(serverIP))
        {
            StatusText.text = "유효한 IP 주소를 입력하세요.";
            return;
        }

        // 서버에 연결 시도
        _netClient.Start(serverIP);
        StatusText.text = "서버에 연결 시도 중...";
    }

    private void OnConnected(bool connected, UserToken userToken)
    {
        if (connected)
        {
            // 연결 성공 시 메시지 출력
            StatusText.text = "서버에 연결되었습니다.";
            
            // UserToken을 이용하여 ClientPeer 생성 및 설정
            _clientPeer = new ClientPeer(userToken);
            userToken.SetPeer(_clientPeer);

            // 연결된 이후 서버로 유저 정보 전송
            SendUserInfo();
        }
        else
        {
            // 연결 실패 시 메시지 출력
            StatusText.text = "서버 연결 실패.";
        }
    }

    private void SendUserInfo()
    {
        // 서버에 유저 정보를 전송하는 패킷 생성 및 전송
        PacketAnsUserInfo userInfoPacket = new PacketAnsUserInfo
        {
            id = "Player1"
        };
        _clientPeer.Send(userInfoPacket);
        StatusText.text = "유저 정보를 서버로 전송했습니다.";
    }
}
*/
using UnityEditor;
using UnityEngine;

public class Client: MonoBehaviour, IPeer
{
    private NetClient _client = new NetClient();
    private UserToken _userToken;
    private UIManager _ui;     //  UI 관리 스크립을 따로 뽑을까????몰라~~~~~~~

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
            Debug.Log("서버에 접속 완료");
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
                    // 서버에서 접속한 모든 유저 정보를 받았을시.
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

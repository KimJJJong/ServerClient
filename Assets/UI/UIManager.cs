using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 유니티에서 연결
    public GameObject startUI;
    public GameObject lobbyUI;

    public TMP_InputField inputID;
    public TMP_InputField inputIP;

    public TextMeshProUGUI textUserList;

    public Button testButtons;

   // public Button buttonID; //필요 없지 않을까?
    //public Button buttonIP; //필요 없지 않을까?

    public Button buttonStart;


    private Client _client;

    private void Awake()
    {

        _client = FindObjectOfType<Client>();

        testButtons.onClick.AddListener(() =>
        {
            PacketAnsUserInfo packet = new PacketAnsUserInfo();
            packet.id = GameManager.Instance.UserID;
            _client.Send(packet);
            Debug.Log((EProtocolID)packet.ProtocolID);
        }
        );
      
        buttonStart.onClick.AddListener(() =>
        {
            string id = inputID.text;
            string ip = inputIP.text;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(ip) )
            {
                Debug.Log("유효하지 않은 값입니다.");
                return;
            }
            GameManager.Instance.UserID = inputID.text;
            Debug.Log(GameManager.Instance.UserID);


            //for (int i = 0; i < 2; i++) 
            { 
            FindObjectOfType<Client>().StartClient(inputIP.text);
            
            }

        });

        SetUIState(EUIState.Start);
    }




    public enum EUIState
    {
        Start,      // 접속 하자 마자
        Lobby,      // IP, ID 입력 후 나오는 GameRoom이 있는 로비
        Game        // 찐으로 게임시작ㅇㅇ
    }

    public void SetUIState(EUIState state)
    {
        switch (state)
        {
            case EUIState.Start:
                startUI.SetActive(true);
                lobbyUI.SetActive(false);
                break;
            case EUIState.Lobby:
                startUI.SetActive(false);
                lobbyUI.SetActive(true);
                break;
            case EUIState.Game:
                startUI.SetActive(false);
                lobbyUI.SetActive(false);
                break;
        }
    }

    public void SetLobbyText(string UserList)
    {
        textUserList.text = UserList;
    }


}

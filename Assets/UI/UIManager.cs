using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Client _client;
    
    // 로그인
    public GameObject startUI;

    public TMP_InputField inputID;
    public TMP_InputField inputIP;
    
    public Button buttonStart;

    //로비
    public GameObject lobbyUI;

    public TextMeshProUGUI textUserList;

    public Button createRoom;
    
    public Transform roomListParent; // Room 버튼이 생성될 부모
    public Button roomButtonPrefab; // Room 버튼 프리팹

    private int roomButtonCounter = 1; // 고유 식별자 관리
    private Dictionary<Button, int> roomButtonIds = new Dictionary<Button, int>(); // 버튼과 식별자 매핑

    private Vector2 nextRoomButtonPosition = Vector2.zero; // 다음 버튼 위치
    private float roomButtonSpacing = 50f; // 버튼 간격


    //준비 창 
    public GameObject readyUI;

    public TextMeshProUGUI Player1; 
    public TextMeshProUGUI Player2; 


    private void Awake()
    {

        _client = FindObjectOfType<Client>();


      
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

            FindObjectOfType<Client>().StartClient(inputIP.text);

        });

        createRoom.onClick.AddListener(() =>
        {
            PacketReqCreateRoom packet = new PacketReqCreateRoom();
            _client.Send(packet);
        });

        SetUIState(EUIState.Start);
    }




    public enum EUIState
    {
        Start,      // 접속 하자 마자
        Lobby,      // IP, ID 입력 후 나오는 GameRoom이 있는 로비
        Ready,
        Game        // 찐으로 게임시작ㅇㅇ
    }

    public void SetUIState(EUIState state)
    {
        switch (state)
        {
            case EUIState.Start:
                startUI.SetActive(true);
                lobbyUI.SetActive(false);
                readyUI.SetActive(false);
                break;
            case EUIState.Lobby:
                startUI.SetActive(false);
                lobbyUI.SetActive(true);
                readyUI.SetActive(false);
                break;
            case EUIState.Ready:
                startUI.SetActive(false);
                lobbyUI.SetActive(false);
                readyUI.SetActive(true);
                break;
            case EUIState.Game:
                startUI.SetActive(false);
                lobbyUI.SetActive(false);
                readyUI.SetActive(false);

                break;
        }
    }

    public void SetLobbyText(string UserList)
    {
        textUserList.text = UserList;
    }

    public void CreateRoom(int roomId)
    {
        Button newRoomButton = Instantiate(roomButtonPrefab, roomListParent);
        newRoomButton.transform.localPosition = nextRoomButtonPosition;

        newRoomButton.name = $"{roomId}";

        int uniqueId = roomButtonCounter++;
        roomButtonIds.Add(newRoomButton, uniqueId);


        TextMeshProUGUI buttonText = newRoomButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = $"Room : {roomId}";

        newRoomButton.onClick.AddListener(() =>
        {
            if (roomButtonIds.TryGetValue(newRoomButton, out int assignedId))
            {
                PacketReqJoinRoom packet = new PacketReqJoinRoom
                {
                    roomId = roomId,
                    uid = GameManager.Instance.UserUID
                };

                Debug.Log($"Button Clicked - Room ID: {roomId}, Unique ID: {assignedId}");
                _client.Send(packet);
            }
            else
            {
                Debug.LogError("Button not found in dictionary!");
            }
         /*   PacketReqJoinRoom packet = new PacketReqJoinRoom();
            packet.roomId = int.Parse(newRoomButton.name);
            packet.uid = GameManager.Instance.UserUID;
        System.Console.WriteLine($"packetID :{packet.roomId} / packetUID : {packet.uid}");
            _client.Send(packet);*/
        });

        nextRoomButtonPosition.y -= roomButtonSpacing;
    }

    public void ShowPlayer(/*bool isFirst,*/ PacketAnsJoinRoom packet)
    {
        if (packet.isFirst)
        {
            Player1.text = packet.gameRoomInfo[0].id;
        }
        else
        {
            Player1.text = packet.gameRoomInfo[0].id;
            Player2.text = packet.gameRoomInfo[1].id; 
        }
    }

}

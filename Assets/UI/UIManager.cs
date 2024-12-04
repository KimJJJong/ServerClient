using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Client _client;
    
    // �α���
    public GameObject startUI;

    public TMP_InputField inputID;
    public TMP_InputField inputIP;
    
    public Button buttonStart;

    //�κ�
    public GameObject lobbyUI;

    public TextMeshProUGUI textUserList;

    public Button createRoom;
    
    public Transform roomListParent; // Room ��ư�� ������ �θ�
    public Button roomButtonPrefab; // Room ��ư ������

    private int roomButtonCounter = 1; // ���� �ĺ��� ����
    private Dictionary<Button, int> roomButtonIds = new Dictionary<Button, int>(); // ��ư�� �ĺ��� ����

    private Vector2 nextRoomButtonPosition = Vector2.zero; // ���� ��ư ��ġ
    private float roomButtonSpacing = 50f; // ��ư ����


    //�غ� â 
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
                Debug.Log("��ȿ���� ���� ���Դϴ�.");
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
        Start,      // ���� ���� ����
        Lobby,      // IP, ID �Է� �� ������ GameRoom�� �ִ� �κ�
        Ready,
        Game        // ������ ���ӽ��ۤ���
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

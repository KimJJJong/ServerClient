using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// ���ӿ� ���� �������� ����
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject container = new GameObject("GameManager");
                _instance = container.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    private UIManager _ui;
    private Client _client;
    private bool _startGame;

    public int UserUID {  get; set; }
    public string UserID {  get; set; }
    public bool IsGameStarted {  get; set; }
    public Client client => _client;

    private void Start()
    {
        _ui = FindObjectOfType<UIManager>();
        _client = FindObjectOfType<Client>();

    }


    public void GameReady()
    {
        _ui.SetUIState(UIManager.EUIState.Game);    //ui�� ġ��� ���� �غ� ����.

        PacketGameReadyOk packet = new PacketGameReadyOk();
    }

    public void GameStart(PacketGameStart packet)
    {
        for(int i=0; i< packet.userNum; i++)
        {
           //  ���� ���� ã�Ƽ� ��¥ ���� �������� �̵� �Ѵ�
        }
    }

}
// SC : ����->Ŭ��, CS : Ŭ��->����, REL : ������
//using UnityEngine;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
//using UnityEngine.Playables;

public enum EProtocolID
{
    SC_REQ_USERINFO,           // ���� -> Ŭ�� : ���� ���� ��û
    CS_ANS_USERINFO,           // Ŭ�� -> ���� : ���� ���� ����
    SC_ANS_USERLIST,           // ���� -> Ŭ�� : 
                               //    CS_REQ_CHANGE_TEAM,        // �� ������ �ʿ� ����








    SC_GAME_END,               // ���� -> Ŭ�� : ���� ����

    //Modified Packets
    SC_MANA_UPDATE,            // ���� -> Ŭ�� : ���� ���� ������Ʈ
    CS_SUMMON_UNIT,            // Ŭ�� -> ���� : ���� ��ȯ ��û
    REL_UNIT_SUMMONED,         // ���� -> Ŭ�� : ���� ��ȯ �Ϸ� ������
    SC_GAME_STATE_UPDATE,      // ���� -> Ŭ�� : ���� ���� ���� ������Ʈ


    SC_ERROR_MESSAGE,          // ���� -> Ŭ�� : ���� �޽��� ����

    //1203�߰�
    CS_REQ_GAMEROOM_CREATE,    // Ŭ�� -> ���� : ���ӷ� ���� ��û
    SC_ANS_GAMEROOM_CREATE,    // ���� -> Ŭ�� : ���ӷ� ���� ����
    CS_REQ_JOIN_GAMEROOM,      // Ŭ�� -> ���� : ���ӷ� ���� ��û
    SC_ANS_JOIN_GAMEROOM,      // ���� -> Ŭ�� : ���ӷ� ���� Ȯ��

    //1203�� 1900����
    CS_GAME_READY,             // Ŭ�� -> ���� : ���� �غ� ���� ��û
    REL_GAME_READY_OK,         // ���� -> Ŭ�� : ���� �غ� ����(����ȭ)
    SC_GAME_START,             // ���� -> Ŭ�� : ���� ����

    //1204�� 

    //InGame
    // Init
    SC_PVP_INIT_STATE,

    // �¾��� ������ �ٷ�����
    CS_PVP_UNIT_DAMAGED,
    REL_PVP_HANDLE_DAMAGE,

    // Grid����
    CS_PVP_GRID_UPDATE,
    REL_PVP_GRID_UPDATE,

    // Mana����
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGridRelease : Packet
{

    public PacketGridRelease()
        : base((short)EProtocolID.REL_PVP_GRID_UPDATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGridUpdate : Packet
{


    public PacketGridUpdate()
        : base((short)EProtocolID.CS_PVP_GRID_UPDATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketHandlingDmage : Packet
{
    public int damagedIndex; //���� ����
    public int attackIndex;  // ��� ����

    public PacketHandlingDmage()
        : base((short)EProtocolID.REL_PVP_HANDLE_DAMAGE)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketDamaged : Packet
{
    public UnitInfo damagedUnit; //���� ����
    public UnitInfo attackUnit;  // ��� ����

    public PacketDamaged()
        : base((short)EProtocolID.CS_PVP_UNIT_DAMAGED)
    {
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketInitState : Packet
{
    //�ʿ� ����
    //TEAM.(RED/BULE)
    //List<Unit>
    //Mana
    //Time
    //Grid
    //TowerHp

    public ETeam myTeam; //��
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Define.POOLING_SIZE)]
    public UnitInfo[] unitInfos = new UnitInfo[Define.POOLING_SIZE]; //Pooling
    public float currentMana;   //���� ����
    public float currentTime;   //���� �ð�
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public bool[][] grid;       //���ɵ�   --  Ȯ���ϰ� ������ �ʿ䰡 ����
    public float towerHp;       //ž ü��
    public PacketInitState()
        : base((short)EProtocolID.SC_PVP_INIT_STATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnitInfo
{
    public int unitUid;
    public int index;   //pooling Manager�� ��ġ
    public Vector2Float position;
    public float hp;
    public float damage;
    public float requireMana;
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketAnsJoinRoom : Packet
{
    public bool isFirst;
    //  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 2)]
    public GameRoomInfo[] gameRoomInfo = new GameRoomInfo[2];
    public PacketAnsJoinRoom()
        : base((short)EProtocolID.SC_ANS_JOIN_GAMEROOM)
    {
    }
}
// ���������� �迭�� ���� ��Ҵ� struct�� �ؾ� ������ �Ȼ����. 
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GameRoomInfo
{
    public int uid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;

}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketReqJoinRoom : Packet
{
    public int roomId;
    public int uid;
    public PacketReqJoinRoom()
        : base((short)EProtocolID.CS_REQ_JOIN_GAMEROOM)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketRoomInfo : Packet
{
    public int roomId;
    public PacketRoomInfo()
        : base((short)EProtocolID.SC_ANS_GAMEROOM_CREATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketReqCreateRoom : Packet
{

    public PacketReqCreateRoom()
        : base((short)EProtocolID.CS_REQ_GAMEROOM_CREATE)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketReqUserInfo : Packet
{
    //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public int uid;
    //public ETeam team;

    public PacketReqUserInfo()
        : base((short)EProtocolID.SC_REQ_USERINFO)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketAnsUserInfo : Packet
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    // public bool host;

    public PacketAnsUserInfo()
        : base((short)EProtocolID.CS_ANS_USERINFO)
    {
    }
}

// ���������� �迭�� ���� ��Ҵ� struct�� �ؾ� ������ �Ȼ����. 
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UserInfo
{
    public int uid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    // public ETeam team;
    //public bool host;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketAnsUserList : Packet
{
    public int userNum;
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 20)]
    public UserInfo[] userInfos = new UserInfo[20];
    public PacketAnsUserList()
        : base((short)EProtocolID.SC_ANS_USERLIST)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameReady : Packet
{
    public PacketGameReady()
        : base((short)EProtocolID.CS_GAME_READY)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameReadyOk : Packet
{
    public PacketGameReadyOk()
        : base((short)EProtocolID.REL_GAME_READY_OK)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GameStartInfo
{
    public int uid;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public string id;
    public int roomNum;
    // public ETeam team;
    // public Vector3Int position;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameStart : Packet
{
    public int userNum;
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 20)]
    public GameStartInfo[] startInfos = new GameStartInfo[20];

    public PacketGameStart()
        : base((short)EProtocolID.SC_GAME_START)
    {
    }
}






////////////Modefied Packet///////////////
///
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketManaUpdate : Packet
{
    public int currentMana;
    public int maxMana;
    public PacketManaUpdate()
        : base((short)EProtocolID.SC_MANA_UPDATE)
    {
    }
}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketSummonUnit : Packet
{
    public int cardID;
    public Vector3Int position;

    public PacketSummonUnit()
        : base((short)EProtocolID.CS_SUMMON_UNIT)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketUnitSummoned : Packet
{
    public int unitUID;         // ��ȯ�� ������ ���� ID
    public int cardID;          // ī�� ID
    public Vector3Int position;    // ��ȯ�� ��ġ
    public int ownerUID;        // ���� �������� UID

    public PacketUnitSummoned()
        : base((short)EProtocolID.REL_UNIT_SUMMONED)
    {
    }
}








[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketGameEnd : Packet
{
    //  public ETeam winningTeam;

    public PacketGameEnd()
        : base((short)EProtocolID.SC_GAME_END)
    {
    }
}


////////////////////
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector2Float
{
    public float x;
    public float y;

    public Vector2Float(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Vector3Int
{
    public int x;
    public int y;
    public int z;

    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}


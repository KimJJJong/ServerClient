// SC : 서버->클라, CS : 클라->서버, REL : 릴리스
//using UnityEngine;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
//using UnityEngine.Playables;

public enum EProtocolID
{
    SC_REQ_USERINFO,           // 서버 -> 클라 : 유저 정보 요청
    CS_ANS_USERINFO,           // 클라 -> 서버 : 유저 정보 응답
    SC_ANS_USERLIST,           // 서버 -> 클라 : 
                               //    CS_REQ_CHANGE_TEAM,        // 팀 변경이 필요 없음








    SC_GAME_END,               // 서버 -> 클라 : 게임 종료

    //Modified Packets
    SC_MANA_UPDATE,            // 서버 -> 클라 : 마나 상태 업데이트
    CS_SUMMON_UNIT,            // 클라 -> 서버 : 유닛 소환 요청
    REL_UNIT_SUMMONED,         // 서버 -> 클라 : 유닛 소환 완료 릴레이
    SC_GAME_STATE_UPDATE,      // 서버 -> 클라 : 게임 전반 상태 업데이트


    SC_ERROR_MESSAGE,          // 서버 -> 클라 : 에러 메시지 전달

    //1203추가
    CS_REQ_GAMEROOM_CREATE,    // 클라 -> 서버 : 게임룸 생성 요청
    SC_ANS_GAMEROOM_CREATE,    // 서버 -> 클라 : 게임룸 생성 응답
    CS_REQ_JOIN_GAMEROOM,      // 클라 -> 서버 : 게임룸 참여 요청
    SC_ANS_JOIN_GAMEROOM,      // 서버 -> 클라 : 게임룸 참여 확인

    //1203일 1900수정
    CS_GAME_READY,             // 클라 -> 서버 : 게임 준비 상태 요청
    REL_GAME_READY_OK,         // 서버 -> 클라 : 게임 준비 응답(동기화)
    SC_GAME_START,             // 서버 -> 클라 : 게임 시작

    //1204일 

    //InGame
    // Init
    SC_PVP_INIT_STATE,

    // 맞았으 아흑흑 줄래아파
    CS_PVP_UNIT_DAMAGED,
    REL_PVP_HANDLE_DAMAGE,

    // Grid업뎃
    CS_PVP_GRID_UPDATE,
    REL_PVP_GRID_UPDATE,

    // Mana관리
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
    public int damagedIndex; //본인 유닛
    public int attackIndex;  // 상대 유닛

    public PacketHandlingDmage()
        : base((short)EProtocolID.REL_PVP_HANDLE_DAMAGE)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketDamaged : Packet
{
    public UnitInfo damagedUnit; //본인 유닛
    public UnitInfo attackUnit;  // 상대 유닛

    public PacketDamaged()
        : base((short)EProtocolID.CS_PVP_UNIT_DAMAGED)
    {
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PacketInitState : Packet
{
    //필요 정보
    //TEAM.(RED/BULE)
    //List<Unit>
    //Mana
    //Time
    //Grid
    //TowerHp

    public ETeam myTeam; //팀
    [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = Define.POOLING_SIZE)]
    public UnitInfo[] unitInfos = new UnitInfo[Define.POOLING_SIZE]; //Pooling
    public float currentMana;   //현재 마나
    public float currentTime;   //현재 시간
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
    public bool[][] grid;       //점령도   --  확실하게 정의할 필요가 있음
    public float towerHp;       //탑 체력
    public PacketInitState()
        : base((short)EProtocolID.SC_PVP_INIT_STATE)
    {
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UnitInfo
{
    public int unitUid;
    public int index;   //pooling Manager의 위치
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
// 마샬링으로 배열에 들어가는 요소는 struct로 해야 문제가 안생긴다. 
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

// 마샬링으로 배열에 들어가는 요소는 struct로 해야 문제가 안생긴다. 
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
    public int unitUID;         // 소환된 유닛의 고유 ID
    public int cardID;          // 카드 ID
    public Vector3Int position;    // 소환된 위치
    public int ownerUID;        // 유닛 소유자의 UID

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


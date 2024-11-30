/*using System;
using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]     //이거 오류일수도 있다 잘 생각해라
public abstract class Packet
{
    public short ProtocolID { get; private set; }

    protected Packet(short protocolID)
    {
        ProtocolID = protocolID;
    }

    public byte[] ToByte()
    {
        int size = Marshal.SizeOf(this);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(this, ptr, false);
        byte[] buffer = new byte[size];
        Marshal.Copy(ptr, buffer, 0, size);
        Marshal.FreeHGlobal(ptr);
        return buffer;
    }
}
*/

using System.Runtime.InteropServices;
using System;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Packet
{
    private short _size;
    private short _protocolID;

    public short Size => _size;
    public short ProtocolID => _protocolID;

    public Packet(short protocolID)
    {
        _size = (short)Marshal.SizeOf(this);
        _protocolID = protocolID;
    }

    public byte[] ToByte()
    {
        // Send Packet;
        byte[] buffer = new byte[Marshal.SizeOf(this)];

        // unsafe 포인터 사용하기 위해
        // 빌드 설정 allow unsafe code, Unity : Player - OtherSetting
        // fixed unsafe 내에서만 사용가능 메모리를 안전하게 사용할수 있다
        unsafe
        {
            fixed (byte* fixed_buffer = buffer)
            {
                Marshal.StructureToPtr(this, (IntPtr)fixed_buffer, false);
            }
        }

        return buffer;
    }

    public void ToPacket(byte[] buffer)
    {
        unsafe
        {
            fixed (byte* fixed_buffer = buffer)
            {
                Marshal.PtrToStructure((IntPtr)fixed_buffer, this);
            }
        }
    }
}
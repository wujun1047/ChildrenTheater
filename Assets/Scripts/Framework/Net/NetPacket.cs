using System;
using System.Collections.Generic;
using System.Text;

public class NetPacket
{
    // 包体结构
    /***
     * 6字节包头标识，“vt2016”
     * 4字节数据长度
     * 包体数据
     ***/
    public static string s_HeadStr = "vt2016";
    public static char[] s_HeadArr = s_HeadStr.ToCharArray();
    public static int HEAD_SIZE = s_HeadStr.Length + sizeof(int);
    public class Packet
    {
        public string pktHead;  // 包头标识
        public int pktLen;      // 包体总长度（包头+数据）
        public byte[] data;     // 数据

        public Packet(byte[] bytes)
        {
            pktHead = string.Copy(s_HeadStr);
            data = bytes;
            pktLen = (data == null) ? 0 : data.Length;
        }
    }


    /// <summary>
    /// 从data里面提取出多个Packets，存入输出队列中，不足一个Packet的部分，留着等待下一次解析;调用者需保证输入参数的合法性；
    /// </summary>
    /// <param name="data">待解析的数据</param>
    /// <param name="dataLen">数据实际长度</param>
    /// <param name="pktQueue">解析后的数据队列</param>
    /// <returns>返回解析过的数据长度</returns>
    public static int ParseData(byte[] data, int dataLen, ref Queue<byte[]> pktQueue)
    {
        if (data == null)
            throw new Exception("InputError: data is NULL!");
        if (dataLen > data.Length)
            throw new Exception("InputError: dataLen is GREATER than data.Length!");

        byte[] packet = null;   // 解析出来的一个数据包
        byte[] lenBytes = new byte[sizeof(int)];
        int pktLen = 0;         // 解析出来的数据包总长度(包头+数据)
        int currParsePos = 0;   // 当前解析的数据位置
        bool bFindHead = false;
        while (currParsePos < dataLen)
        {
            bFindHead = _FindNextPacketcurrPos(data, dataLen, ref currParsePos);
            if (bFindHead)
            {
                // 找到包头
                if (currParsePos + HEAD_SIZE > dataLen)
                {
                    // 数据长度不足以解析出包头
                    break;
                }
                else
                {
                    // 取出数据包长度
                    Array.Clear(lenBytes, 0, lenBytes.Length);
                    Buffer.BlockCopy(data, currParsePos + s_HeadStr.Length, lenBytes, 0, sizeof(int));
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(lenBytes);
                    pktLen = BitConverter.ToInt32(lenBytes, 0);

                    if (currParsePos + pktLen > dataLen)
                    {
                        // 数据长度不足以解析出一个包
                        break;
                    }
                    else
                    {
                        // 可以解析出一个包
                        packet = new byte[pktLen];
                        Buffer.BlockCopy(data, currParsePos, packet, 0, pktLen);
                        pktQueue.Enqueue(packet);

                        // 修改位置信息
                        currParsePos += pktLen;                        
                    }
                }
            }
            else
            {
                // 没有找到包头
                break;
            }
        };
        return currParsePos;
    }


    /// <summary>
    /// 查找下一个包头位置，调用者要保证输入参数的合法性
    /// </summary>
    /// <param name="data">输入的数据</param>
    /// <param name="dataLen">数据长度</param>
    /// <param name="startPos">开始查找的位置，同时作为返回值，表示当前解析到的位置</param>
    /// <returns>返回是否找到包头</returns>
    static bool _FindNextPacketcurrPos(byte[] data, int dataLen, ref int startPos)
    {
        bool bFind = false;
        int end = dataLen - s_HeadStr.Length;
        
        while(startPos < end)
        {
            bool bMatch = true;
            for (int i = 0; i < s_HeadStr.Length; ++i)
            {
                if (data[startPos + i] != s_HeadArr[i])
                {
                    bMatch = false;
                    break;
                }
            }
            if (bMatch)
            {
                bFind = true;
                break;
            }
            else
            {
                ++startPos;
            }
        }
        if (!bFind)
            --startPos;
        
        return bFind;
    }

    public static byte[] PackData(byte[] bytes)
    {
        byte[] data = null;
        if (bytes == null || bytes.Length <= 0)
            return data;

        int pktLen = bytes.Length + HEAD_SIZE;
        data = new byte[pktLen];
        byte[] headBytes = Encoding.UTF8.GetBytes(s_HeadStr);
        byte[] lenBytes = BitConverter.GetBytes(pktLen);
        // 网络字节序为大端，需要先做个字节序转换
        if (BitConverter.IsLittleEndian)
            Array.Reverse(lenBytes);

        Buffer.BlockCopy(headBytes, 0, data, 0, headBytes.Length);
        Buffer.BlockCopy(lenBytes, 0, data, headBytes.Length, lenBytes.Length);
        Buffer.BlockCopy(bytes, 0, data, HEAD_SIZE, bytes.Length);

        return data;
    }

    public static byte[] UnpackData(byte[] packet)
    {
        byte[] data = null;
        if (packet.Length <= HEAD_SIZE)
            return data;

        byte[] lenBytes = new byte[sizeof(int)];
        Buffer.BlockCopy(packet, s_HeadStr.Length, lenBytes, 0, sizeof(int));
        if (BitConverter.IsLittleEndian)
            Array.Reverse(lenBytes);
        int len = BitConverter.ToInt32(lenBytes, 0);

        if (len != packet.Length)
        {
            Console.WriteLine("UnpackData: Len is ERROR!!!");
            return data;
        }

        data = new byte[len - HEAD_SIZE];
        Buffer.BlockCopy(packet, HEAD_SIZE, data, 0, len - HEAD_SIZE);
        return data;
    }
}

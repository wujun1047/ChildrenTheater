using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

/************************
 * 网络协议层
 * 负责解析网络连接层收发的数据，
 * 收数据时，将数据解析层一个事件，
 * 再通过事件系统将事件分发出去。
 * 发数据时，将应用层传过来的事件
 * 序列化成网络数据。
 ************************/

public enum ProtocolID
{
    ProtocolID_Begin,

    S2C_SvrIP,
    S2C_ChangeToAR,
    S2C_ChangeToVR,
    C2S_EnterAR,
    C2S_EnterVR,

    ProtocolID_End,
    ProtocolID_Invalid = ProtocolID_End,
}
	
/// 每个协议ID对应一个解析器
/// 负责将网络连接层收到的数据反序列化
public interface IParser
{
	NetworkEventArgs Deserialize(byte[] data);
}

public class Parser_S2C_SvrIP : IParser
{
	public NetworkEventArgs Deserialize(byte[] data)
	{
		string jsonStr = Encoding.UTF8.GetString(data);
		NetworkEventArgs_S2C_SvrIP args = JsonUtility.FromJson<NetworkEventArgs_S2C_SvrIP>(jsonStr);
		return args;
	}
}

public class Parser_NetworkEventArgs : IParser
{
    public NetworkEventArgs Deserialize(byte[] data)
    {
        string jsonStr = Encoding.UTF8.GetString(data);
        NetworkEventArgs args = JsonUtility.FromJson<NetworkEventArgs>(jsonStr);
        return args;
    }
}

public class Protocol : Singleton<Protocol>
{
	// 解析器映射表
	Dictionary<ProtocolID, IParser> _mapper = new Dictionary<ProtocolID, IParser>();

	public override void Init()
	{
        _mapper.Add(ProtocolID.S2C_SvrIP, new Parser_S2C_SvrIP());
        _mapper.Add(ProtocolID.S2C_ChangeToAR, new Parser_NetworkEventArgs());
        _mapper.Add(ProtocolID.S2C_ChangeToVR, new Parser_NetworkEventArgs());
        _mapper.Add(ProtocolID.C2S_EnterAR, new Parser_NetworkEventArgs());
        _mapper.Add(ProtocolID.C2S_EnterVR, new Parser_NetworkEventArgs());
	}

    ProtocolID _GetProtocolID(byte[] data)
    {
        ProtocolID id = ProtocolID.ProtocolID_Invalid;
        if (data.Length < sizeof(int))
            return id;

        byte[] idBytes = new byte[sizeof(int)];
        Buffer.BlockCopy(data, 0, idBytes, 0, idBytes.Length);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(idBytes);
        int nID = BitConverter.ToInt32(idBytes, 0);
        if (nID > (int)ProtocolID.ProtocolID_Begin && nID < (int)ProtocolID.ProtocolID_End)
        {
            id = (ProtocolID)nID;
        }
        return id;
    }

	IParser _GetParser(ProtocolID id)
	{
		if (_mapper.ContainsKey(id))
			return _mapper[id];
		else
			return null;
	}

	NetworkEventArgs _ParseData(byte[] data)
	{
		ProtocolID id = _GetProtocolID(data);
		IParser parser = _GetParser(id);
		if (parser == null)
			return null;
        
        byte[] content = new byte[data.Length - sizeof(int)];
        Buffer.BlockCopy(data, sizeof(int), content, 0, content.Length);
		NetworkEventArgs args = parser.Deserialize(content);
		return args;
	}

	public void ParseDataAndDispatchEvent(byte[] data)
    {
		NetworkEventArgs args = _ParseData(data);
		if (args != null)
			EventDispatcher.Instance.TriggerEvent(args);
    }

	public byte[] SerializeNetworkEventArgs(NetworkEventArgs args)
	{
		string jsonStr = JsonUtility.ToJson(args);
        byte[] idBytes = BitConverter.GetBytes((int)args.id);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(idBytes);
		byte[] data = Encoding.UTF8.GetBytes(jsonStr);
        return _CombomBinaryArray(idBytes, data);
	}

    /// <summary>
    /// C# byte数组合并(（二进制数组合并）
    /// </summary>
    /// <param name="srcArray1">待合并数组1</param>
    /// <param name="srcArray2">待合并数组2</param>
    /// <returns>合并后的数组</returns>
    private byte[] _CombomBinaryArray(byte[] srcArray1, byte[] srcArray2)
    {
        byte[] ret = new byte[srcArray1.Length + srcArray2.Length];
        Buffer.BlockCopy(srcArray1, 0, ret, 0, srcArray1.Length);
        Buffer.BlockCopy(srcArray2, 0, ret, srcArray1.Length, srcArray2.Length);
        return ret;
    }
}


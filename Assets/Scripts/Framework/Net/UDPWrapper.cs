using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

/******************************
 * 网络连接层
 * 封装UDP网络连接
 * 负责网络数据的发送和接收
 ******************************/

public class UDPWrapper
{
    public static int RECV_BUFFER_SIZE = 4 * 1024 * 1024;
    private static int MAX_SEND_QUEUE_SIZE = 64;
    
    IPEndPoint _sendIEP;
    IPEndPoint _recvIEP;
    public IPEndPoint RecvIEP
    {
        get { return _recvIEP; }
    }

    int _locolPort;
    UdpClient _udpClient;
    readonly object _udpClientLocker = new object();

    Thread _sendThread;
    Thread _recvThread;

    Queue<byte[]> _sendQueue = new Queue<byte[]>();
    Queue<byte[]> _recvQueue = new Queue<byte[]>();
    readonly object _sendQueueLocker = new object();
    readonly object _recvQueueLocker = new object();
    byte[] _sendBuff = null;
    byte[] _recvBuff = null;
    int _recvBuffOffset;

    public UDPWrapper()
    {
    }

    public void Init(IPEndPoint sendIEP, int locolPort)
    {
        _recvIEP = new IPEndPoint(IPAddress.Any, _locolPort);
        _sendIEP = sendIEP;
        _locolPort = locolPort;
        _ResetRecvBuff();
        _InitUdpClient();
        _InitThread();
    }

    public void Release()
    {
        _sendThread.Abort();
        _recvThread.Abort();
        lock (_sendQueueLocker)
            _sendQueue.Clear();
        lock (_recvQueueLocker)
            _recvQueue.Clear();
        lock (_udpClientLocker)
            _udpClient.Close();
    }

    void _ResetRecvBuff()
    {
        if (_recvBuff == null)
            _recvBuff = new byte[RECV_BUFFER_SIZE];

        Array.Clear(_recvBuff, 0, RECV_BUFFER_SIZE);
        _recvBuffOffset = 0;
    }

    void _InitUdpClient()
    {
        lock (_udpClientLocker)
        {
            _udpClient = new UdpClient(_locolPort);
        }
    }

    void _InitThread()
    {
        _sendThread = new Thread(new ThreadStart(_SendThread));
        _sendThread.IsBackground = true;
        _sendThread.Start();
        _recvThread = new Thread(new ThreadStart(_RecvThread));
        _recvThread.IsBackground = true;
        _recvThread.Start();
    }

    void _SendThread()
    {
        while (true)
        {
            _DoSend();
            Thread.Sleep(100);
        }
    }

    void _DoSend()
    {
        lock (_sendQueueLocker)
        {
            if (_sendQueue.Count <= 0)
                return;
            _sendBuff = _sendQueue.Dequeue();
        }

        if (_sendBuff == null || _sendBuff.Length <= 0)
            return;

        try
        {
            _sendBuff = NetPacket.PackData(_sendBuff);
            _udpClient.Send(_sendBuff, _sendBuff.Length, _sendIEP);
            _sendBuff = null;
        }
        catch (Exception e)
        {
            Console.WriteLine("DoSend Error:" + e.Message);
        }
    }

    void _RecvThread()
    {
        byte[] recvBytes = null;
        while (true)
        {
            // 接收数据
            try
            {
                recvBytes = _udpClient.Receive(ref _recvIEP);
            }
            catch (Exception e)
            {
                Console.WriteLine("Receive Error:" + e.Message);
            }

            if (recvBytes == null || recvBytes.Length <= 0)
            {
                Thread.Sleep(100);
                continue;
            }

            // 将数据缓存到buff
            if (recvBytes.Length + _recvBuffOffset <= RECV_BUFFER_SIZE)
            {
                _AddBytesToRecvBuff(recvBytes);
            }
            else
            {
                _ResetRecvBuff();
                Console.WriteLine("recvBuff is FULL!!!");
            }

            // 解析数据
            int parseLength = 0;
            lock (_recvQueueLocker)
            {
                parseLength = NetPacket.ParseData(_recvBuff, _recvBuffOffset, ref _recvQueue);
            }

            // 调整buff数据
            if (parseLength > 0 && parseLength <= _recvBuff.Length)
            {
                Buffer.BlockCopy(_recvBuff, parseLength, _recvBuff, 0, _recvBuff.Length - parseLength);
                Array.Clear(_recvBuff, _recvBuff.Length - parseLength, parseLength);
            }
        }
    }

    void _AddBytesToRecvBuff(byte[] bytes)
    {
        Buffer.BlockCopy(bytes, 0, _recvBuff, _recvBuffOffset, bytes.Length);
        _recvBuffOffset += bytes.Length;
    }

	public void Send(byte[] content, IPEndPoint iep)
    {
        _sendIEP = iep;
        if (_sendQueue.Count >= MAX_SEND_QUEUE_SIZE)
        {
            Console.WriteLine("Error: SendQueue is FULL !!!");
            return;
        }
        lock (_sendQueueLocker)
        {
            _sendQueue.Enqueue(content);
        }
    }

    public byte[] Recv()
    {
        byte[] data = null;
        lock (_recvQueueLocker)
        {
            if (_recvQueue.Count > 0)
            {
                byte[] bytes = _recvQueue.Dequeue();
                data = NetPacket.UnpackData(bytes);
            }
        }
        return data;
    }
}

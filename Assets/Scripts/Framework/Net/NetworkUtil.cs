using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class NetworkUtil 
{
    public static string GetLocalIPAddress()
    {
        // 获得本机局域网IP地址  
        IPAddress[] AddressList = Dns.GetHostAddresses(Dns.GetHostName());
        if (AddressList.Length < 1)
        {
            return "";
        }
        string addr = AddressList[0].ToString();
        Debug.Log("Local address:" + addr);
        return addr;  
    }
}

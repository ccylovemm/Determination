using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public class SocketClient 
{
    private TcpClient tcpClient;
    private MemoryStream memoryStream = new MemoryStream();
    private NetworkStream networkStream;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    static private SocketClient instance_;
    static public SocketClient Instance
    {
        get
        {
            if (instance_ == null)
            {
                instance_ = new SocketClient();
            }
            return instance_;
        }
    }

    static public void Connect()
    {
        Instance.Connect(AppConst.ip , AppConst.port);
    }

    public void Connect(string host, int port) 
    {
        tcpClient = null;
        tcpClient = new TcpClient();
        tcpClient.SendTimeout = 1000;
        tcpClient.ReceiveTimeout = 1000;
        tcpClient.NoDelay = true;
        try 
        {
            tcpClient.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e) 
        {
            Close(); 
            Debug.LogError(e.Message);
        }
    }

    void OnConnect(IAsyncResult asr) 
    {
        networkStream = tcpClient.GetStream();
        tcpClient.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
    }

    void OnRead(IAsyncResult asr) 
    {
        int bytesRead = 0;
        try 
        {
            lock (tcpClient.GetStream())
            {
                bytesRead = tcpClient.GetStream().EndRead(asr);
            }
            if (bytesRead < 1) 
            {              
                return;
            }
            OnReceive(byteBuffer, bytesRead);  
            lock (tcpClient.GetStream())
            {  
                Array.Clear(byteBuffer, 0, byteBuffer.Length); 
                tcpClient.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        } catch (Exception ex) {
        }
    }

    void OnReceive(byte[] bytes, int length)
    {
        memoryStream.Seek(0, SeekOrigin.End);
        memoryStream.Write(bytes, 0, length);
        BinaryReader binaryReader = new BinaryReader(memoryStream);
        
    }

    public void Send(byte[] message)
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            networkStream.BeginWrite(message , 0 , message.Length , new AsyncCallback(OnWrite) , null);
        }
        else
        {
            Debug.LogError("client.connected----->>false");
        }
    }

    void OnWrite(IAsyncResult r) 
    {
        try 
        {
            networkStream.EndWrite(r);
        } 
        catch (Exception ex) 
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    void Close()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
    }
}
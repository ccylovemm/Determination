using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

class SocketRecieve
{
    static public void Recieve(BinaryReader binaryReader)
    {
        int packetID = binaryReader.ReadByte();
        Packet response = (Packet)packetID;

        switch (response)
        {
            case Packet.S2C_SocketConnect:
                Prootocol.Login(Application.bundleIdentifier);
                break;
            case Packet.S2C_Login:
                bool hasCreate = binaryReader.ReadBoolean();
                if (!hasCreate)
                {
                    //showCreateView;
                }
                else
                {
                    ResourceManager.Instance.LoadScene("MainScene");
                }
                break;
            case Packet.S2C_Create:
                byte result = binaryReader.ReadByte();
                if (result == 0)
                {
                    Debug.Log("创建角色成功");
                    ResourceManager.Instance.LoadScene("MainScene");
                }
                else if (result == 1)
                {
                    Debug.Log("已有相同角色名称");
                }
                else
                {
                    Debug.Log("未知错误");
                }
                break;
            case Packet.S2C_SynPlayerData:
                string username = binaryReader.ReadString();
                Debug.Log(username);
                break;
            default:
                Debug.LogError("未知协议 " + packetID);
                break;
        }
    }
}

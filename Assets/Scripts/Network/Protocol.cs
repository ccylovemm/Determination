using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

class Prootocol
{
    static public void Login(string id)
    {
        Buffer buffer = Buffer.BeginPacket(Packet.C2S_Login);
        buffer.Writer(id);
        buffer.Flush();
        SocketClient.Instance.Send(buffer.ToArray());
    }
}

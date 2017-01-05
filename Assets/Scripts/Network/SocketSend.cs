using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

class SocketSend
{
    public void Send(uint id)
    {
        MemoryStream memoryStream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
        binaryWriter.Write(id);
        binaryWriter.Flush();
        SocketClient.Instance.Send(memoryStream.ToArray());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

class SocketRecieve
{
    static public void Recieve(BinaryReader binaryReader)
    {
        int packetID = binaryReader.ReadByte();
        Packet response = (Packet)packetID;

        switch (response)
        {
            case Packet.None:
                break;
        }
    }
}

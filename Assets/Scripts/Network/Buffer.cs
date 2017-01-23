using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Buffer
{
	public MemoryStream stream;
    public BinaryWriter writer;

	public Buffer ()
	{
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
	}

    public void Writer(string value)
    {
        writer.Write(value);
    }

    public void Writer(Byte value)
    {
        writer.Write(value);
    }

    public void Writer(Int16 value)
    {
        writer.Write(value);
    }

    public void Writer(UInt16 value)
    {
        writer.Write(value);
    }

    public void Writer(Int32 value)
    {
        writer.Write(value);
    }

    public void Writer(UInt32 value)
    {
        writer.Write(value);
    }

    public void Writer(Int64 value)
    {
        writer.Write(value);
    }

    public void Writer(UInt64 value)
    {
        writer.Write(value);
    }

    public void Flush()
    {
        writer.Flush();
    }

    public byte[] ToArray()
    {
        return stream.ToArray();
    }
   
    static public Buffer BeginPacket(Packet packet)
    {
        Buffer buffer = new Buffer() ;
        buffer.writer.Write((byte)packet);
        return buffer;
    }
}
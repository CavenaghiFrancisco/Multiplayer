using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PackageReceiver
{
    public static int CheckMessage(byte[] data)
    {
        int messageType = -20;
        try
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                messageType = (int)formatter.Deserialize(stream);
            }
        }
        catch
        {
            messageType = BitConverter.ToInt32(data, 0);
        }
        return messageType;
    }
}

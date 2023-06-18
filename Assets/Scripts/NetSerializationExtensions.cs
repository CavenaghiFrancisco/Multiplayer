using System;
using System.Collections.Generic;
using UnityEngine;

public static class NetSerializationExtensions
{
    static int vector3Instance = 1;

    public static byte[] ToMsg(this Vector3 vec3Value, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));
        outData.AddRange(BitConverter.GetBytes(vector3Instance++));
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));
        int asciiSize = 0;
        for (int i = 0; i < fieldName.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));
            asciiSize += fieldName[i];
        }
        outData.AddRange(BitConverter.GetBytes(vec3Value.x));
        outData.AddRange(BitConverter.GetBytes(vec3Value.y));
        outData.AddRange(BitConverter.GetBytes(vec3Value.z));
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3));
        return outData.ToArray();
    }

    //public static byte[] ToMsg(this Vector3 vec3Value, string fieldName)
    //{
    //   // return new NetVector3(vec3Value, fieldName).Serialize();
    //}
}

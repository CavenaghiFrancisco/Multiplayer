using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum ReflectionType
{
    Int = 0,
    Float = 1,
    Bool = 2,
    Char = 3,
    String = 4,
    Vector2 = 5,
    Vector3 = 6,
    Collection = 7,
    Color = 8,
    Transform = 9,
    Dictionary = 10,
    Quaternion = 11
}

public static class NetSerializationExtensions
{
    static int intInstance = 1;
    static int floatInstance = 1;
    static int boolInstance = 1;
    static int charInstance = 1;
    static int stringInstance = 1;
    static int vector2Instance = 1;
    static int vector3Instance = 1;
    static int quaternionInstance = 1;
    static int collectionInstance = 1;
    static int colorInstance = 1;
    static int transformInstance = 1;
    static int dictionaryInstance = 1;

    //int █
    //float█
    //bool█
    //char█
    //string█
    //UnityEngine.Vector2█
    //UnityEngine.Vector3█
    //UnityEngine.Quaternion█
    //UnityEngine.Color█
    //UnityEngine.Transform█
    //Colecciones(array, stack, queue, list) de los tipos antes mencionados█
    //Diccionarios de los tipos antes mencionados█

    public static byte[] ToMsg(this int intValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Int));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     
        outData.AddRange(BitConverter.GetBytes(intInstance++));                 
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  
        int asciiSize = 0;                                                          
        for (int i = 0; i < fieldName.Length; i++)                                  
        {                                                                           
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  
            asciiSize += fieldName[i];                                              
        }                                                                           
        outData.AddRange(BitConverter.GetBytes(intValue));                          
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));               
        return outData.ToArray();
    }

    public static byte[] ToMsg(this float floatValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Float));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));
        outData.AddRange(BitConverter.GetBytes(floatInstance++));
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));
        int asciiSize = 0;
        for (int i = 0; i < fieldName.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));
            asciiSize += fieldName[i];
        }
        outData.AddRange(BitConverter.GetBytes(floatValue));
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));
        return outData.ToArray();
    }

    public static byte[] ToMsg(this bool boolValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Bool));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));
        outData.AddRange(BitConverter.GetBytes(boolInstance++));
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));
        int asciiSize = 0;
        for (int i = 0; i < fieldName.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));
            asciiSize += fieldName[i];
        }
        outData.AddRange(BitConverter.GetBytes(boolValue));
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));
        return outData.ToArray();
    }

    public static byte[] ToMsg(this char charValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Char));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));
        outData.AddRange(BitConverter.GetBytes(charInstance++));
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));
        int asciiSize = 0;
        for (int i = 0; i < fieldName.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));
            asciiSize += fieldName[i];
        }
        outData.AddRange(BitConverter.GetBytes(charValue));
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));
        return outData.ToArray();
    }

    public static byte[] ToMsg(this string stringValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.String));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));
        outData.AddRange(BitConverter.GetBytes(stringInstance++));
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));
        int asciiSize = 0;
        for (int i = 0; i < fieldName.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));
            asciiSize += fieldName[i];
        }
        for (int i = 0; i < stringValue.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(stringValue[i]));
        }
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));
        return outData.ToArray();
    }

    public static byte[] ToMsg(this Vector2 vec2Value, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Vector2));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(vector2Instance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        outData.AddRange(BitConverter.GetBytes(vec2Value.x));                       //Body
        outData.AddRange(BitConverter.GetBytes(vec2Value.y));                       //Body
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg(this Vector3 vec3Value, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Vector3));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(vector3Instance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        outData.AddRange(BitConverter.GetBytes(vec3Value.x));                       //Body
        outData.AddRange(BitConverter.GetBytes(vec3Value.y));                       //Body
        outData.AddRange(BitConverter.GetBytes(vec3Value.z));                       //Body
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg(this Quaternion quatValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Quaternion));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(quaternionInstance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        outData.AddRange(BitConverter.GetBytes(quatValue.w));                       //Body
        outData.AddRange(BitConverter.GetBytes(quatValue.x));                       //Body
        outData.AddRange(BitConverter.GetBytes(quatValue.y));                       //Body
        outData.AddRange(BitConverter.GetBytes(quatValue.z));                       //Body
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg(this Color colorValue, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Color));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(colorInstance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        outData.AddRange(BitConverter.GetBytes(colorValue.r));                       //Body
        outData.AddRange(BitConverter.GetBytes(colorValue.g));                       //Body
        outData.AddRange(BitConverter.GetBytes(colorValue.b));                       //Body
        outData.AddRange(BitConverter.GetBytes(colorValue.a));                       //Body
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg(this Transform transformValue, string fieldName)
    {
        Vector3 rot = transformValue.rotation.eulerAngles;
        Vector3 tras = transformValue.position;
        Vector3 scale = transformValue.localScale;

        Matrix4x4 RTSMatrix = Matrix4x4.identity;

        RTSMatrix.SetTRS(Vector3.zero, Quaternion.Euler(rot), Vector3.one);

        RTSMatrix.SetColumn(3, new Vector4(tras.x, tras.y, tras.z, 1f));

        RTSMatrix *= Matrix4x4.Scale(scale);

        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Transform));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(transformInstance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, RTSMatrix);
            outData.AddRange(stream.ToArray());
        }                     
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Dictionary));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(dictionaryInstance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, dictionary);
            outData.AddRange(stream.ToArray());
        }
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }

    public static byte[] ToMsg<T>(this IEnumerable<T> collection, string fieldName)
    {
        List<byte> outData = new List<byte>();
        outData.AddRange(BitConverter.GetBytes((int)MessageType.Reflection));       //Header
        outData.AddRange(BitConverter.GetBytes((int)ReflectionType.Collection));
        outData.AddRange(BitConverter.GetBytes(NetworkManager.Instance.ownId));     //Header
        outData.AddRange(BitConverter.GetBytes(collectionInstance++));                 //Header
        outData.AddRange(BitConverter.GetBytes(fieldName.Length));                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            outData.AddRange(BitConverter.GetBytes(fieldName[i]));                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, collection);
            outData.AddRange(stream.ToArray());
        }
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3 + asciiSize));     //Tail            
        return outData.ToArray();
    }
}

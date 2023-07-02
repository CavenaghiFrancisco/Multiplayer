using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

    public static (string,object) MsgToInt(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        int intValue = BitConverter.ToInt32(data, data.Length - 8);
        return (fieldName.ToString(), intValue);
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

    public static (string, object) MsgToFloat(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        float floatVlue = BitConverter.ToSingle(data, data.Length - 8);
        return (fieldName.ToString(), floatVlue);
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

    public static (string, object) MsgToBool(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        bool boolValue = BitConverter.ToBoolean(data, data.Length - 4 - sizeof(bool));
        return (fieldName.ToString(), boolValue);
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

    public static (string, object) MsgToChar(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        bool charValue = BitConverter.ToBoolean(data, data.Length - 4 - sizeof(char));
        return (fieldName.ToString(), charValue);
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

    public static (string, object) MsgToString(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = 0; i < fieldNameLength; i++)
        {
            fieldName[i] = BitConverter.ToChar(data, dataIndex); 
            dataIndex += sizeof(char); 
        }

        int stringValueLength = (data.Length - dataIndex - sizeof(int)) / sizeof(char);

        char[] stringValue = new char[stringValueLength];

        for (int i = 0; i < stringValueLength; i++)
        {
            stringValue[i] = BitConverter.ToChar(data, dataIndex);
            dataIndex += sizeof(char);
        }
        return (fieldName.ToString(), stringValue.ToString());
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

    public static (string, object) MsgToVec2(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        Vector2 vec2Value = new Vector2(BitConverter.ToInt32(data, data.Length - 12), BitConverter.ToInt32(data, data.Length - 8));
        return (fieldName.ToString(), vec2Value);
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

    public static (string, object) MsgToVec3(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        Vector3 vec3Value = new Vector3(BitConverter.ToInt32(data, data.Length - 16), BitConverter.ToInt32(data, data.Length - 12), BitConverter.ToInt32(data, data.Length - 8));
        return (fieldName.ToString(), vec3Value);
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

    public static (string, object) MsgToQuat(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        Quaternion quatValue = new Quaternion(BitConverter.ToInt32(data, data.Length - 16), BitConverter.ToInt32(data, data.Length - 12), BitConverter.ToInt32(data, data.Length - 8), BitConverter.ToInt32(data, data.Length - 20));
        return (fieldName.ToString(), quatValue);
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

    public static (string, object) MsgToColor(byte[] data)
    {
        int fieldNameLength = BitConverter.ToInt32(data, 16);

        char[] fieldName = new char[fieldNameLength];

        int dataIndex = 20;

        for (int i = dataIndex; i < fieldNameLength; i += sizeof(char))
        {
            fieldName[i] = BitConverter.ToChar(data, i);
        }

        Color colorValue = new Color(BitConverter.ToInt32(data, data.Length - 20),BitConverter.ToInt32(data, data.Length - 16), BitConverter.ToInt32(data, data.Length - 12), BitConverter.ToInt32(data, data.Length - 8));
        return (fieldName.ToString(), colorValue);
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

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, (int)MessageType.Reflection);       //Header
        formatter.Serialize(stream, (int)ReflectionType.Transform);
        formatter.Serialize(stream, NetworkManager.Instance.ownId);     //Header
        formatter.Serialize(stream, transformInstance++);                 //Header
        formatter.Serialize(stream, fieldName.Length);                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            formatter.Serialize(stream, fieldName[i]);                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        formatter.Serialize(stream, RTSMatrix);
        formatter.Serialize(stream, (int)stream.Length * 3 + asciiSize);
        stream.Close();
        return stream.ToArray();
    }

    public static (string, object) MsgToTransform(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        MessageType messageType = (MessageType)formatter.Deserialize(stream);
        ReflectionType reflectionType = (ReflectionType)formatter.Deserialize(stream);
        int ownerId = (int)formatter.Deserialize(stream);
        int transformInstanceId = (int)formatter.Deserialize(stream);
        int fieldNameLength = (int)formatter.Deserialize(stream);

        StringBuilder fieldNameBuilder = new StringBuilder();
        int asciiSize = 0;
        for (int i = 0; i < fieldNameLength; i++)
        {
            char character = (char)formatter.Deserialize(stream);
            fieldNameBuilder.Append(character);
            asciiSize += character;
        }
        string fieldName = fieldNameBuilder.ToString();

        Matrix4x4 matrix = (Matrix4x4)formatter.Deserialize(stream);


        Vector3 position = matrix.GetColumn(3);
        Quaternion rotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        Vector3 scale = matrix.lossyScale;

        (Vector3, Vector3, Quaternion) transform;

        transform.Item1 = position;
        transform.Item2 = scale;
        transform.Item3 = rotation;

        stream.Close();
        return (fieldName.ToString(), transform);
    }

    public static byte[] ToMsg<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string fieldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, (int)MessageType.Reflection);       //Header
        formatter.Serialize(stream, (int)ReflectionType.Dictionary);
        formatter.Serialize(stream, NetworkManager.Instance.ownId);     //Header
        formatter.Serialize(stream, dictionaryInstance++);                 //Header
        formatter.Serialize(stream, fieldName.Length);                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            formatter.Serialize(stream, fieldName[i]);                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        formatter.Serialize(stream, dictionary);
        formatter.Serialize(stream, (int)stream.Length * 3 + asciiSize);     //Tail
        stream.Close();
        return stream.ToArray();
    }

    public static (string,object) MsgToDictionary<TKey, TValue>(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        MessageType messageType = (MessageType)formatter.Deserialize(stream);
        ReflectionType reflectionType = (ReflectionType)formatter.Deserialize(stream);
        int ownerId = (int)formatter.Deserialize(stream);
        int dictionaryInstanceId = (int)formatter.Deserialize(stream);
        int fieldNameLength = (int)formatter.Deserialize(stream);

        StringBuilder fieldNameBuilder = new StringBuilder();
        int asciiSize = 0;
        for (int i = 0; i < fieldNameLength; i++)
        {
            char character = (char)formatter.Deserialize(stream);
            fieldNameBuilder.Append(character);
            asciiSize += character;
        }
        string fieldName = fieldNameBuilder.ToString();

        IDictionary<TKey, TValue> dictionary = (IDictionary<TKey, TValue>)formatter.Deserialize(stream);

        stream.Close();

        return (fieldName.ToString(), dictionary);
    }

    public static byte[] ToMsg<T>(this ICollection<T> collection, string fieldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, (int)MessageType.Reflection);       //Header
        formatter.Serialize(stream, (int)ReflectionType.Collection);
        formatter.Serialize(stream, NetworkManager.Instance.ownId);     //Header
        formatter.Serialize(stream, collectionInstance++);                 //Header
        formatter.Serialize(stream, fieldName.Length);                  //Header
        int asciiSize = 0;                                                          //Header
        for (int i = 0; i < fieldName.Length; i++)                                  //Header
        {                                                                           //Header
            formatter.Serialize(stream, fieldName[i]);                  //Header
            asciiSize += fieldName[i];                                              //Header
        }                                                                           //Header
        formatter.Serialize(stream, collection);
        formatter.Serialize(stream, (int)stream.Length * 3 + asciiSize);     //Tail            
        stream.Close();
        return stream.ToArray();
    }

    public static (string, object) MsgToCollection<T>(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(data);

        MessageType messageType = (MessageType)formatter.Deserialize(stream);
        ReflectionType reflectionType = (ReflectionType)formatter.Deserialize(stream);
        int ownerId = (int)formatter.Deserialize(stream);
        int dictionaryInstanceId = (int)formatter.Deserialize(stream);
        int fieldNameLength = (int)formatter.Deserialize(stream);

        StringBuilder fieldNameBuilder = new StringBuilder();
        int asciiSize = 0;
        for (int i = 0; i < fieldNameLength; i++)
        {
            char character = (char)formatter.Deserialize(stream);
            fieldNameBuilder.Append(character);
            asciiSize += character;
        }
        string fieldName = fieldNameBuilder.ToString();

        ICollection<T> collection = (ICollection<T>)formatter.Deserialize(stream);

        stream.Close();

        return (fieldName.ToString(), collection);
    }
}

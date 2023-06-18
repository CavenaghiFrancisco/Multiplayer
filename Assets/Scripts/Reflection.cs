using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    private BindingFlags instanceDeclaratedOnlyFilter => (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    public GameManager gameManager;

    private void Start()
    {
        NetworkManager.Instance.OnReceiveDataFromReflection += OnReceiveData;
    }

    private void Update()
    {
        List<byte[]> gameManagerAsBytes = Inspect(gameManager, typeof(GameManager));

        foreach (byte[] msg in gameManagerAsBytes)
        {
            NetworkManager.Instance.SendToServer(msg);
        }
    }

    

    private List<byte[]> Inspect(object obj, Type type, string fieldName = "")
    {
        List<byte[]> output = new List<byte[]>();

        foreach (FieldInfo field in type.GetFields(instanceDeclaratedOnlyFilter))
        {
            IEnumerable<Attribute> attributes = field.GetCustomAttributes();
            foreach (Attribute attribute in attributes)
            {
                if (attribute is NetAttribute)
                {
                    object value = field.GetValue(obj);
                    if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                    {
                        if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            foreach (DictionaryEntry entry in (value as IDictionary))
                            {
                                ConvertToMsg(output, entry.Key, fieldName + field.Name + "Key [" + i.ToString() + "]");
                                ConvertToMsg(output, entry.Value, fieldName + field.Name + "Value [" + i.ToString() + "]");
                                i++;
                            }
                        }
                        else if (typeof(ICollection).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            foreach (object element in (value as ICollection))
                            {
                                ConvertToMsg(output, element, fieldName + field.Name + "[" + i.ToString() + "]");
                                i++;
                            }
                        }
                    }
                    else
                    {
                        ConvertToMsg(output, value, fieldName + field.Name);
                    }
                }
            }
            if (typeof(INet).IsAssignableFrom(type))
            {
                ConvertToMsg(output,(type as INet).Serialize(), fieldName + field.Name);
            }                
        }
        if (type.BaseType != null)
        {
            foreach (byte[] msg in Inspect(obj, type.BaseType, fieldName))
            {
                output.Add(msg);
            }
        }
        return output;
    }

    private void ConvertToMsg(List<byte[]> MsgStack, object obj, string fieldName)
    {
        if (obj is int)
        {
            Debug.Log(fieldName + " - Value: " + obj);
           
        }
        else if (obj is float)
        {
            Debug.Log(fieldName + " - Value: " + obj);
        }
        else if (obj is bool)
        {
            Debug.Log(fieldName + " - Value: " + obj);
        }
        else if (obj is Vector3)
        {
            Debug.Log(fieldName + " - Value: " + obj);
            MsgStack.Add(((Vector3)obj).ToMsg(fieldName));
        }
        else if (obj is Vector2)
        {

        }
        else if (obj is Color)
        {
            Debug.Log(fieldName + " - Value: " + obj);

        }
        else
        {
            foreach (byte[] msg in Inspect(obj, obj.GetType(), fieldName + "/"))
            {
                MsgStack.Add(msg);
            }
        }
    }

    private (string, object) ConvertToData(byte[] data)
    {
        (string, object) a = ("a", null);
        return a ;
    }

    private void OnReceiveData(byte[] data)
    {
        (string receivedFieldName, object value) obtainedData = ConvertToData(data);
        OverWrite(gameManager, typeof(GameManager), obtainedData.receivedFieldName, obtainedData.value);
    }

    private void OverWrite(object obj, Type type, string receivedFieldName, object newValue, string fieldName = "")
    {
        foreach (FieldInfo field in type.GetFields(instanceDeclaratedOnlyFilter))
        {
            IEnumerable<Attribute> attributes = field.GetCustomAttributes();
            foreach (Attribute attribute in attributes)
            {
                if (attribute is NetAttribute)
                {
                    object value = field.GetValue(obj);
                    if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                    {
                        if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            int iToModify = -1;
                            foreach (DictionaryEntry entry in (value as IDictionary))
                            {
                                fieldName += field.Name + "Value [" + i.ToString() + "] / ";
                                OverWrite((value as IDictionary)[i], (value as IDictionary)[i].GetType(), receivedFieldName, 5, fieldName);
                                i++;
                            }                            
                        }
                        else if (typeof(ICollection).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            int iToModify = -1;
                            foreach (object element in (value as ICollection))
                            {
                                fieldName += field.Name + "[" + i.ToString() + "]";
                                OverWrite((value as IDictionary)[i], (value as IDictionary)[i].GetType(), receivedFieldName, 5, fieldName);
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if (fieldName + field.Name == receivedFieldName)
                            field.SetValue(obj, newValue);
                    }
                }
            }
        }
        if (type.BaseType != null)
        {
            OverWrite(obj, type.BaseType, receivedFieldName, newValue, fieldName);
        }
    }
}

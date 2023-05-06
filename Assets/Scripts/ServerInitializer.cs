#if UNITY_SERVER
using System.Collections.Generic;
using UnityEngine;

public class ClientInitializer
{
    [RuntimeInitializeOnLoadMethod]
    public static void StartServer()
    {
        GameObject a = new GameObject();    
        a.AddComponent<NetworkManager>();        
    }
}
#endif
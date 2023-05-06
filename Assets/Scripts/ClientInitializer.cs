#if !UNITY_SERVER
using System.Collections.Generic;
using UnityEngine;

public class ClientInitializer
{
    [RuntimeInitializeOnLoadMethod]
    public static void InstanciarPrefab()
    {
        List<GameObject> prefabs = new List<GameObject>();
        prefabs.Add(Resources.Load<GameObject>("NetworkManager"));
        prefabs.Add(Resources.Load<GameObject>("Canvas"));
        prefabs.Add(Resources.Load<GameObject>("Main Camera"));
        prefabs.Add(Resources.Load<GameObject>("Directional Light"));
        prefabs.Add(Resources.Load<GameObject>("EventSystem"));
        prefabs.Add(Resources.Load<GameObject>("Plane"));
        
        foreach(GameObject go in prefabs)
        {
            GameObject instanciaPrefab = GameObject.Instantiate(go);
        }
        
    }
}
#endif
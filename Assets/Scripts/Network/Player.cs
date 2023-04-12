using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int id;
    public Vector3 pos;

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public void Move(Vector3 position)
    {
        transform.position = position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int id;
    public Vector3 pos;
    private bool firstTime = true;

    private void Start()
    {
    }

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public void Move(Vector3 position,Vector3 movement)
    {
        if (firstTime)
        {
            pos = position;
        }
        pos += movement;
        transform.position = pos;
    }
}

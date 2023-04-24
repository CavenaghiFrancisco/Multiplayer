using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int id = -1;
    [SerializeField] private GameObject mainPlayer;
    public Vector3 pos;
    public Color color;
    public Color[] colors;
    bool colorAssigned = false;
    private Material mat;

    private void Start()
    {
        colors = new Color[] { Color.black, Color.yellow, Color.green, Color.blue, Color.magenta, Color.cyan };
    }

    private void Update()
    {
        if(id != -1 && !colorAssigned)
        {
            colorAssigned = true;
            mat = new Material(GetComponent<Renderer>().material);
            int aux = id;
            while(aux > colors.Length-1)
            {
                aux -= colors.Length-1;
            }
            color = colors[aux];
            mat.color = color;
            GetComponent<Renderer>().material = mat;

            if (NetworkManager.Instance.ownId == id)
            {
                mainPlayer.SetActive(true);
            }
        }
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeHeader]
public class GameManager : MonoBehaviour
{
    public GameObject holder;

    [Net] private Dictionary<int, Player> players = new Dictionary<int, Player>();

    void Start()
    {
        players.Add(0, Instantiate(holder, transform.position, Quaternion.identity).AddComponent<Player>());
        players.Add(1, Instantiate(holder, transform.position, Quaternion.identity).AddComponent<Player>());
        players.Add(2, Instantiate(holder, transform.position, Quaternion.identity).AddComponent<Player>());
        players.Add(3, Instantiate(holder, transform.position, Quaternion.identity).AddComponent<Player>());
    }

    private void Update()
    {
    }
}

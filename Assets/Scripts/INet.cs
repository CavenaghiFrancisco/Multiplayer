using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INet
{
    public byte[] Serialize();

    public void Deserialize();
}

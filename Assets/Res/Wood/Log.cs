using UnityEngine;
using System;

public class Log : MapResource
{
    public Wood wood;
    public Transform[] woodSpawns;
    protected override void Gathered()
    {
        base.Gathered();
        for (int i = 0; i < woodSpawns.Length; i++)
        {
            Instantiate(wood, woodSpawns[i].position, woodSpawns[i].rotation, Game.Group("Woods"));
        }
        Destroy(gameObject);
    }
}

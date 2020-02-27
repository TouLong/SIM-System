using UnityEngine;
using System;

public class Wood : MapResource
{
    public Log log;
    public Transform[] logSpawns;
    protected override void Gathered()
    {
        base.Gathered();
        for (int i = 0; i < transform.childCount; i++)
        {
            Instantiate(log, transform.GetChild(i).position, Quaternion.identity, Game.Group("Logs"));
        }
        Destroy(gameObject);

    }
}

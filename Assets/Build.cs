using UnityEngine;
using System;
using System.Collections.Generic;

public class Build
{
    enum BuildState
    {
        WaitForResource,
        WaitForConstruct,
        Construct,
        Complete,
    }

    BuildState state = BuildState.WaitForResource;
    protected TextMesh info;
    float percent;

    public Build(Res res)
    {
        info = TextUI.Stay("", res.transform.position);
        UpdateMessage();
    }

    public bool IsComplete => state == BuildState.Complete;
    public bool IsConstruct => state == BuildState.Construct;
    public bool IsWaitForResource => state == BuildState.WaitForResource;
    public bool IsWaitForConstruct => state == BuildState.WaitForConstruct;


    public void UpdateMessage()
    {
        info.text = "";
        info.text += (percent * 100).ToString("0") + "%";
    }
}
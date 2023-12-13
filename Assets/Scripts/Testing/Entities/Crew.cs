using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew : ClickableObject
{
    public enum CrewRole {Builder, Researcher, Chef}
    public CrewRole role;

    [ExposeProperty] public string Role => role.ToString();
    [ExposeProperty] public bool Alive { get; set; }
}

[AddTypeMenu("Entity/Crew", 2)]
[Serializable]
public class CrewDefinition : Variable<Crew>
{
}

[Serializable]
public class CrewReference : VariableRef<Crew>
{
}

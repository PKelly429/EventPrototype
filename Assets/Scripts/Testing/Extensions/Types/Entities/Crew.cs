using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;

public class Crew : ClickableObject
{
    public enum CrewRole {Builder, Researcher, Chef}
    public CrewRole role;

    [ExposePropertyToBlackBoard] public string Role => role.ToString();
    [ExposePropertyToBlackBoard] public bool Alive { get; set; }
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

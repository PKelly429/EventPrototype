using System;
using System.Collections;
using System.Collections.Generic;
using GameEventSystem;
using UnityEngine;

public class Building : ClickableObject
{

}

[AddTypeMenu("Entity/Building", 2)]
[Serializable]
public class BuildingDefinition : Variable<Building>
{
}

[Serializable]
public class BuildingReference : VariableRef<Building>
{
}
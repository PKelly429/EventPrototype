using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Blackboard", menuName = "GameEvents/Blackboard", order = 2)]
[Serializable]
public class AssetBlackboard : ScriptableObject, IBlackboard
{
    [SerializeReference] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();
    public List<VariableDefinition> definedVariables => _definedVariables;
    public int uniqueID => GetInstanceID();
    
    public void AddVariable(VariableDefinition variable)
    {
        variable.GenerateId();
        definedVariables.Add(variable);
    }

    public VariableDefinition GetVariableByID(string id)
    {
        foreach (var variable in definedVariables)
        {
            if (variable.uniqueId.Equals(id))
            {
                return variable;
            }
        }

        return null;
    }

    public virtual void OnEnable()
    {
        RegisterBlackboard(this);
    }

    public virtual void OnDisable()
    {
        DeregisterBlackboard(this);
    }

    #region Lookup
    private static Dictionary<int, AssetBlackboard> blackboardLookup = new Dictionary<int, AssetBlackboard>();
    public static IBlackboard GetBlackboard(int uid)
    {
        #if UNITY_EDITOR
        if (!blackboardLookup.ContainsKey(uid))
        {
            LoadAllAssets();
        }
        #endif
        
        if (blackboardLookup.ContainsKey(uid))
        {
            return blackboardLookup[uid];
        }

        return null;
    }

    private static void RegisterBlackboard(AssetBlackboard blackboard)
    {
        if (blackboardLookup.ContainsKey(blackboard.uniqueID))
        {
            return;
        }
        blackboardLookup.Add(blackboard.uniqueID, blackboard);
    }
    
    private static void DeregisterBlackboard(AssetBlackboard blackboard)
    {
        if (!blackboardLookup.ContainsKey(blackboard.uniqueID))
        {
            return;
        }
        blackboardLookup.Remove(blackboard.uniqueID);
    }
    
    #if UNITY_EDITOR
    public static List<AssetBlackboard> GetAllBlackboards()
    {
        List<AssetBlackboard> allBlackboards = new List<AssetBlackboard>();
        string[] assetPaths = AssetDatabase.FindAssets("t:AssetBlackboard");
        foreach (var guid in assetPaths)
        {
            var blackboard = AssetDatabase.LoadAssetAtPath<AssetBlackboard>(AssetDatabase.GUIDToAssetPath(guid));
            if(blackboard == null) continue;
            allBlackboards.Add(blackboard);
        }

        return allBlackboards;
    }

    private static void LoadAllAssets()
    {
        string[] assetPaths = AssetDatabase.FindAssets("t:AssetBlackboard");
        foreach (var guid in assetPaths)
        {
            var blackboard = AssetDatabase.LoadAssetAtPath<AssetBlackboard>(AssetDatabase.GUIDToAssetPath(guid));
            if(blackboard == null) continue;
            RegisterBlackboard(blackboard);
        }
    }
    #endif
    #endregion
}

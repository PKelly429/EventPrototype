using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LogEffect : Effect
{
    public StringParameterBuilder logString;
    
    public override void SetBlackboards(IBlackboard blackboard)
    {
        base.SetBlackboards(blackboard);
        logString?.SetBlackboards(blackboard);
    }
    
    public override bool Execute()
    {
        Debug.Log(logString.GetValue());
        return true;
    }

#if UNITY_EDITOR
    public override void DrawEditorWindowUI(IBlackboard localBlackboard)
    {
        if (logString == null)
        {
            logString = new StringParameterBuilder();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Log string to console:");
        logString.DrawEditorWindowUI(localBlackboard);
        EditorGUILayout.EndVertical();
    }
    #endif
}

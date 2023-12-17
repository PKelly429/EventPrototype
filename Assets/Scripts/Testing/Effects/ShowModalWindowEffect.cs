using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ShowModalWindowEffect : Effect
{
    public StringParameterBuilder header;
    public StringParameterBuilder body;
    public List<ShowModalWindowOption> options;
    
    public override void SetBlackboards(IBlackboard blackboard)
    {
        base.SetBlackboards(blackboard);
        header?.SetBlackboards(blackboard);
        body?.SetBlackboards(blackboard);
        if (options != null)
        {
            foreach (var option in options)
            {
                option?.SetBlackboards(blackboard);
            }
        }
    }
    
    public override bool Execute()
    {
        ModalWindowController.ShowModalWindow(new ModalWindowSettings()
        {
            headerText = header.GetValue(),
            bodyText = body.GetValue(),
            optionSettings = GetOptionSettings()
        });
        
        return true;
    }

    private ModalWindowOptionSettings[] GetOptionSettings()
    {
        ModalWindowOptionSettings[] result = new ModalWindowOptionSettings[options.Count];
        for (int i = 0; i < options.Count; i++)
        {
            result[i] = new ModalWindowOptionSettings()
            {
                text = options[i].text.GetValue(),
                callback = options[i].OnSelected
            };
        }
        return result;
    }

#if UNITY_EDITOR
    [NonSerialized] private List<ShowModalWindowOption> removedOptions = new List<ShowModalWindowOption>();
    private static Texture2D delTexture;
    private bool isShowing;

    private Texture2D deleteButtonTexture
    {
        get
        {
            if (delTexture == null)
            {
                delTexture = EditorGUIUtility.FindTexture("Assets/Sprites/delete.png");
            }

            return delTexture;
        }
    }
    public override void DrawEditorWindowUI(IBlackboard localBlackboard)
    {
        if (header == null)
        {
            header = new StringParameterBuilder();
        }
        if (body == null)
        {
            body = new StringParameterBuilder();
        }

        if (options == null)
        {
            options = new List<ShowModalWindowOption>();
        }

        string prefix = isShowing ? "[-]" : "[+]";
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button($"{prefix} Show Modal Window", EditorUtils.UIHeaderStyle))
        {
            isShowing = !isShowing;
        }
        EditorGUILayout.Space(5);

        if (!isShowing)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField("Header:");
        header.DrawEditorWindowUI(localBlackboard);
        EditorGUILayout.LabelField("Body:");
        body.DrawEditorWindowUI(localBlackboard);
        EditorUtils.DrawUILine();
        foreach (var option in options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.BeginVertical();
            option.DrawEditorWindowUI(localBlackboard);
            EditorGUILayout.EndVertical();
            if (GUILayout.Button(deleteButtonTexture, EditorStyles.iconButton))
            {
                removedOptions.Add(option);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorUtils.DrawUILine();
        }
        
        foreach (var removed in removedOptions)
        {
            options.Remove(removed);
        }

        removedOptions.Clear();

        if (GUILayout.Button("Add Option", EditorStyles.popup))
        {
            var newOption = new ShowModalWindowOption();
            newOption.SetBlackboards(localBlackboard);
            options.Add(newOption);
        }

        EditorGUILayout.EndVertical();
    }
#endif
}

[System.Serializable]
public class ShowModalWindowOption : EventComponent
{
    public StringParameterBuilder text;
    [SerializeReference] public Effect effect;

    public void OnSelected()
    {
        effect?.Execute();
    }

    public override void SetBlackboards(IBlackboard blackboard)
    {
        text?.SetBlackboards(blackboard);
        effect?.SetBlackboards(blackboard);
    }

    public void AddEffect(Effect newEffect)
    {
        effect = newEffect;
        effect.SetBlackboards(localBlackboard);
    }
    
    #if UNITY_EDITOR
    private GUIStyle choiceHeaderStyle
    {
        get
        {
            return new GUIStyle()
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
    
    public override void DrawEditorWindowUI(IBlackboard localBlackboard)
    {
        if (text == null)
        {
            text = new StringParameterBuilder();
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Choice", choiceHeaderStyle);
        EditorGUILayout.LabelField("Text:");
        text.DrawEditorWindowUI(localBlackboard);
        if (effect != null)
        {
            effect.DrawEditorWindowUI(localBlackboard);
        }
        if (GUILayout.Button("Set Effect:", EditorStyles.popup))
        {
            var provider = new EffectSearchProvider((type) =>
            {
                AddEffect((Effect)Activator.CreateInstance(type));
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                provider);
        }
        EditorGUILayout.EndVertical();
    }
    #endif
}

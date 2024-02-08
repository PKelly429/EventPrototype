using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameEventSystem;
using UnityEngine;

[NodeInfo("Choice Window", "CHOICE", -1)]
[NodeDescription("Show a choice dialog")]
[AddNodeButton(typeof(ModalWindowChoiceNode))]
[NodeConnectionOutput(PortTypeDefinitions.PortTypes.Choice)]
public class ModalWindowNode : EffectNode
{
    public StringParameterBuilder header;
    public StringParameterBuilder body;

    protected override void OnStart()
    {
        var options = children.OfType<ModalWindowChoiceNode>().ToList();
        ModalWindowController.ShowModalWindow(new ModalWindowSettings()
        {
            headerText = header.GetValue(),
            bodyText = body.GetValue(),
            optionSettings = GetOptionSettings(options)
        });
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (state == State.Success) return State.Success;
        
        return State.Running;
    }

    private ModalWindowOptionSettings[] GetOptionSettings(List<ModalWindowChoiceNode> choiceNodes)
    {
        ModalWindowOptionSettings[] result = new ModalWindowOptionSettings[choiceNodes.Count];
        for (int i = 0; i < choiceNodes.Count; i++)
        {
            ModalWindowChoiceNode toSelect = choiceNodes[i];
            result[i] = new ModalWindowOptionSettings()
            {
                text = toSelect.text.GetValue(),
                active = toSelect.CheckConditions(),
                callback = () =>
                {
                    SelectNode(toSelect);
                }
            };
        }
        return result;
    }

    public void SelectNode(ModalWindowChoiceNode choice)
    {
        SetState(State.Success);
        choice.OnSelected();
        runtimeGameEvent.SetNodeComplete(this);
    }
}
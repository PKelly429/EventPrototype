///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
    public class GraphBlackboardView
    {
        private GameEvent gameEvent;
        public Blackboard blackboard { get; private set; }

        private Dictionary<System.Type, System.Type> blackboardFieldTypes = new Dictionary<Type, Type>();

        public GraphBlackboardView()
        {
            blackboard = new Blackboard();
            blackboard.scrollable = true;
            blackboard.windowed = false;
            blackboard.Add(new BlackboardSection { title = "Graph Properties" });
            blackboard.addItemRequested = AddItemRequested;
            blackboard.editTextRequested = EditTextRequested;
            
            InitBlackboardFields();
        }

        private void InitBlackboardFields()
        {
            if (blackboardFieldTypes == null)
            {
                blackboardFieldTypes = new Dictionary<Type, Type>();
            }
            blackboardFieldTypes.Clear();
            
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(BlackboardFieldView)) == true && type.IsAbstract == false)
                    {
                        BlackboardPropertyTypeAttribute fieldInfo = type.GetCustomAttribute<BlackboardPropertyTypeAttribute>();
                        if (fieldInfo == null) continue;
                        blackboardFieldTypes.Add(fieldInfo.type, type);
                    }
                }
            }
        }
        
        public void ClearBlackboard()
        {
            blackboard.Clear();
        }

        public void SetVisualGraph(GameEvent gameEvent)
        {
            blackboard.Clear();
            blackboard.title = gameEvent.name;
            this.gameEvent = gameEvent;
            if (blackboardFieldTypes == null)
            {
                InitBlackboardFields();
            }
            foreach (var property in gameEvent.definedVariables)
            { 
                AddBlackboardProperty(property);
            }
        }
        
        public void AddBlackboardProperty(VariableDefinition property)
        {
            // if (property == null) return;
            // if (!blackboardFieldTypes.ContainsKey(property.GetType())) return;
            // Type fieldType = blackboardFieldTypes[property.GetType()];
            // BlackboardFieldView propertyView = Activator.CreateInstance(fieldType) as BlackboardFieldView;
            // propertyView.graphBlackboardView = this;
            // propertyView.CreateView(gameEvent, property);
            // propertyView.onRemoveBlackboardProperty += OnRemoveBlackboardProperty;
            // blackboard.Add(propertyView);
        }
        
        void OnRemoveBlackboardProperty(BlackboardFieldView field)
        {
            Undo.RecordObject(gameEvent, "Remove Blackboard Property");
        
            gameEvent.RemoveVariable(field.property);
            blackboard.Remove(field);

            EditorUtility.SetDirty(gameEvent);
        }

        public void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            var field = (BlackboardField)visualElement;
            var property = (VariableDefinition)field.userData;

             if (!string.IsNullOrEmpty(newText) && newText != property.Name)
             {
                 Undo.RecordObject(gameEvent, "Edit Blackboard Text");
            
                 int count = 0;
                 string propertyName = newText;
                 foreach (var boardProperty in gameEvent.definedVariables)
                 {
                     if (boardProperty.Name == propertyName) count++;
                 }
                 if (count > 0) propertyName += $"({count})";
            
                 property.Name = propertyName;
                 field.text = property.Name;
                 
                 EditorUtility.SetDirty(gameEvent);
             }
        }

        void AddItemRequested(Blackboard blackboard)
        {
            var provider = new VariableDefinitionSearchProvider((type) =>
            {
                var newProperty = gameEvent.AddVariable(type);
                AddBlackboardProperty(newProperty);
            });
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                provider);
        }

        //void CreateBlackboardProperty(Type type)
        //{
            // BlackboardPropertyTypeAttribute attrib = type.GetCustomAttribute<BlackboardPropertyTypeAttribute>();
            //
            // Undo.RecordObject(visualGraph, "Add Blackboard Property");
            //
            // int count = 0;
            // string propertyName = attrib.menuName;
            // foreach (var boardProperty in visualGraph.BlackboardProperties)
            // {
            //     if (boardProperty.Name == propertyName) count++;
            // }
            // if (count > 0) propertyName += $"({count})";
            //
            // Type propertyType = attrib.type;
            // AbstractBlackboardProperty property = Activator.CreateInstance(propertyType) as AbstractBlackboardProperty;
            // property.name = attrib.type.Name;
            // property.Name = propertyName;
            // property.guid = System.Guid.NewGuid().ToString();
            // visualGraph.BlackboardProperties.Add(property);
            //
            // if (property.name == null || property.name.Trim() == "") property.name = ObjectNames.NicifyVariableName(property.name);
            // if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph))) AssetDatabase.AddObjectToAsset(property, visualGraph);
            //
            // AddBlackboardProperty(type, property);
            //
            // EditorUtility.SetDirty(visualGraph);
            // AssetDatabase.SaveAssets();
        //}
        

        // void AddBlackboardProperty(Type type, AbstractBlackboardProperty property)
        // {
        //     BlackboardFieldView propertyView = Activator.CreateInstance(type) as BlackboardFieldView;
        //     propertyView.blackboardView = this;
        //     propertyView.CreateView(visualGraph, property);
        //     propertyView.onRemoveBlackboardProperty += OnRemoveBlackboardProperty;
        //     blackboard.Add(propertyView);
        // }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem
{
    public class VariableSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private readonly IBlackboard blackboard;
        private Action<VariableSelection> onSelectCallback;
        private Type searchType;

        public VariableSearchProvider(IBlackboard blackboard, Action<VariableSelection> onSelectCallback, Type type)
        {
            this.blackboard = blackboard;
            this.onSelectCallback = onSelectCallback;
            searchType = type;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent("Matching Variable Definitions"), 0));

            foreach (var variableDefinition in blackboard.definedVariables)
            {
                if(variableDefinition == null) continue;
                if (!IsMatchingType(variableDefinition.type))
                {
                    AddExposedFields(searchList, blackboard, variableDefinition, 1);
                    continue;
                }

                var entry = new SearchTreeEntry(new GUIContent(variableDefinition.Name));
                entry.level = 1;
                entry.userData = new VariableSelection()
                {
                    blackboard = blackboard,
                    variable = variableDefinition
                };
                searchList.Add(entry);
            }

            // foreach (var attachedBlackboard in extraBlackboards)
            // {
            //     if (attachedBlackboard.uniqueID.Equals(blackboard.uniqueID))
            //     {
            //         continue;
            //     }
            //
            //     searchList.Add(new SearchTreeGroupEntry(new GUIContent(attachedBlackboard.name), 1));
            //
            //     foreach (var variableDefinition in attachedBlackboard.definedVariables)
            //     {
            //         if (!variableDefinition.type.IsAssignableFrom(searchType))
            //         {
            //             AddExposedFields(searchList, blackboard, variableDefinition, 2);
            //             continue;
            //         }
            //
            //         var entry = new SearchTreeEntry(new GUIContent(variableDefinition.Name));
            //         entry.level = 2;
            //         entry.userData = new VariableSelection()
            //         {
            //             blackboard = attachedBlackboard,
            //             variable = variableDefinition
            //         };
            //         searchList.Add(entry);
            //     }
            // }

            return searchList;
        }

        private void AddExposedFields(List<SearchTreeEntry> searchList, IBlackboard blackboard,
            VariableDefinition variableDefinition, int level)
        {
            //bool variableAdded = false;
            var matchingProperties = variableDefinition.type.GetProperties()
                .Where(prop => prop.IsDefined(typeof(ExposePropertyToBlackBoardAttribute), false));
            foreach (var property in matchingProperties)
            {
                if (!IsMatchingType(property.PropertyType)) continue;
                // if (!variableAdded)
                // {
                //     searchList.Add(new SearchTreeGroupEntry(new GUIContent(variableDefinition.name), level));
                //     variableAdded = true;
                // }
                var entry = new SearchTreeEntry(new GUIContent($"{variableDefinition.Name}.{property.Name}"));
                entry.level = level;
                entry.userData = new VariableSelection()
                {
                    blackboard = blackboard,
                    variable = variableDefinition,
                    propertyName = property.Name
                };
                searchList.Add(entry);
            }
        }

        private bool IsMatchingType(Type type)
        {
            if (searchType == null) return true;
            return type.IsAssignableFrom(searchType) || searchType.IsAssignableFrom(type);
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            VariableSelection selection = (VariableSelection)SearchTreeEntry.userData;
            onSelectCallback?.Invoke(selection);
            return true;
        }

        public struct VariableSelection
        {
            public IBlackboard blackboard;
            public VariableDefinition variable;
            public string propertyName;
        }
    }
}
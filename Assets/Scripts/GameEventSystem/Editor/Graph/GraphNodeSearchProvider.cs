using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem.Editor
{
    public class TriggerNodeSearchProvider : GraphNodeSearchProvider<TriggerNode>
    {
        public TriggerNodeSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }
    
    public class ConditionNodeSearchProvider : GraphNodeSearchProvider<ConditionNode>
    {
        public ConditionNodeSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }
    
    public class EffectNodeSearchProvider : GraphNodeSearchProvider<EffectNode>
    {
        public EffectNodeSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }
    
    public class GraphNodeSearchProvider<T> : ScriptableObject, ISearchWindowProvider
    {
        private Action<Type> selectCallback;

        public GraphNodeSearchProvider(Action<Type> callback)
        {
            selectCallback = callback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            List<string> groups = new List<string>();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent("Types")));
            foreach (var type in GetTypes<T>())
            {
                if (type == null) continue;
                NodeInfoAttribute nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
                if(nodeInfo == null) continue;
                
                string[] menuPath = nodeInfo.GetSplittedMenuName();
                string groupName = "";
                for (int i = 0; i < menuPath.Length - 1; i++)
                {
                    groupName += menuPath[i];
                    if (!groups.Contains(groupName))
                    {
                        searchList.Add(new SearchTreeGroupEntry(new GUIContent(groupName), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                var entry = new SearchTreeEntry(new GUIContent(menuPath.Last()));
                entry.level = menuPath.Length;
                entry.userData = type;
                searchList.Add(entry);
            }

            return searchList;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            selectCallback?.Invoke((Type)searchTreeEntry.userData);
            return true;
        }




        private IEnumerable<Type> cache;

        private IEnumerable<Type> GetTypes<T>()
        {
            if (cache != null)
            {
                return cache;
            }

            cache = FindAllClassesOfType<T>();
            cache = cache.OrderBy(type =>
            {
                if (type == null)
                {
                    return -999;
                }

                return type.GetCustomAttribute<NodeInfoAttribute>()?.Order ?? 0;
            });

            return cache;
        }

        private static IEnumerable<Type> FindAllClassesOfType<T>()
        {
            var baseType = typeof(T);
            var assembly = typeof(T).Assembly;
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }
    }


}
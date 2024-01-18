using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameEventSystem
{
    public class NodeSearchProvider : TypeSearchProvider<GameEventNode>
    {
        public Vector2 MousePosition;
        public NodeSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }
    
    public class TriggerSearchProvider : TypeSearchProvider<Trigger>
    {
        public TriggerSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }

    public class ConditionSearchProvider : TypeSearchProvider<Condition>
    {
        public ConditionSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }

    public class EffectSearchProvider : TypeSearchProvider<Effect>
    {
        public EffectSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }

    public class VariableDefinitionSearchProvider : TypeSearchProvider<VariableDefinition>
    {
        public VariableDefinitionSearchProvider(Action<Type> callback) : base(callback)
        {
        }
    }

    public class TypeSearchProvider<T> : ScriptableObject, ISearchWindowProvider
    {
        private Action<Type> selectCallback;

        public TypeSearchProvider(Action<Type> callback)
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
                AddTypeMenuAttribute typeMenu = type.GetCustomAttribute<AddTypeMenuAttribute>();
                if (typeMenu == null)
                {
                    var entry = new SearchTreeEntry(new GUIContent(type.ToString()));
                    entry.level = 1;
                    entry.userData = type;
                    searchList.Add(entry);
                }
                else
                {
                    string[] menuPath = typeMenu.GetSplittedMenuName();
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

                    if (menuPath.Last().Equals("skip"))
                    {
                        continue;
                    }

                    var entry = new SearchTreeEntry(new GUIContent(menuPath.Last()));
                    entry.level = menuPath.Length;
                    entry.userData = type;
                    searchList.Add(entry);
                }
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

                return type.GetCustomAttribute<AddTypeMenuAttribute>()?.Order ?? 0;
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
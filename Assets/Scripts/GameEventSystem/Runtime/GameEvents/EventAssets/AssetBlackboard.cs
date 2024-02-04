using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

namespace GameEventSystem
{
    [CreateAssetMenu(fileName = "Blackboard", menuName = "GameEvents/Blackboard", order = 2)]
    [Serializable]
    public class AssetBlackboard : ScriptableObject, IBlackboard
    {
        [SerializeReference] private List<VariableDefinition> _definedVariables = new List<VariableDefinition>();

        [ScriptableObjectIdAttribute] [SerializeField]
        private string _uniqueID;

        public List<VariableDefinition> definedVariables => _definedVariables;
        public string uniqueID => _uniqueID;

        public VariableDefinition AddVariable(Type type)
        {
            VariableDefinition variable = Activator.CreateInstance(type) as VariableDefinition;
            variable.Name = $"new{variable.type.Name}";
            
            variable.GenerateId();
            definedVariables.Add(variable);

            return variable;
        }

        public void RemoveVariable(VariableDefinition variable)
        {
            variable.Delete();
            definedVariables.Remove(variable);
        }
        
        public void RemoveVariableById(string id)
        {
            var variable = GetVariableByID(id);
            if (variable == null) return;
            RemoveVariable(variable);
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

        private static Dictionary<string, AssetBlackboard> blackboardLookup = new Dictionary<string, AssetBlackboard>();

        public static AssetBlackboard GetBlackboard(string uid)
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
            if (!Application.isPlaying)
            {
                return;
            }

            if (blackboardLookup.ContainsKey(blackboard.uniqueID))
            {
                return;
            }

            blackboardLookup.Add(blackboard.uniqueID, null); // prevent instantiate trying to re add and creating loop
            blackboardLookup[blackboard.uniqueID] = Instantiate(blackboard);
        }

        private static void DeregisterBlackboard(AssetBlackboard blackboard)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!blackboardLookup.ContainsKey(blackboard.uniqueID))
            {
                return;
            }

            blackboardLookup.Remove(blackboard.uniqueID);
        }

#if UNITY_EDITOR
        public VisualElement CreatePropertyField(VariableReference variableReference)
        {
            try
            {
                variableReference.ResolveRef(this);

                if (variableReference.variableDefinition == null)
                {
                    throw new Exception("Blackboard does not contain this reference");
                }
                
                return BlackboardFieldUtil.GetFieldForParameter(this, variableReference.variableDefinition);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return new Label("Could not find field info.");
        }

        public static BaseField<T> GetPropertyField<T, TU>()
        {
            Debug.Log($"Get: {typeof(T)} : {typeof(TU)}");
            return Activator.CreateInstance(typeof(TU)) as BaseField<T>;
        }

        public string GetBindingPath(VariableDefinition property)
        {
            string bindingPath = string.Empty;
            try
            {
                var serializedObject = new SerializedObject(this);
                var bindObj = serializedObject.FindProperty("_definedVariables");

                for (int i = 0; i < bindObj.arraySize; i++)
                {
                    var nextElement = bindObj.GetArrayElementAtIndex(i);
                    string id = nextElement.FindPropertyRelative("uniqueId").stringValue;
                    if (id.Equals(property.uniqueId))
                    {
                        var valueElement = nextElement.FindPropertyRelative("_value");
                        bindingPath = valueElement.propertyPath;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return bindingPath;
        }
        public static List<AssetBlackboard> GetAllBlackboards()
        {
            List<AssetBlackboard> allBlackboards = new List<AssetBlackboard>();
            string[] assetPaths = AssetDatabase.FindAssets("t:AssetBlackboard");
            foreach (var guid in assetPaths)
            {
                var blackboard = AssetDatabase.LoadAssetAtPath<AssetBlackboard>(AssetDatabase.GUIDToAssetPath(guid));
                if (blackboard == null) continue;
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
                if (blackboard == null) continue;
                RegisterBlackboard(blackboard);
            }
        }
#endif

        #endregion
    }
}
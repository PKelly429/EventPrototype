using System;
using System.Text;

namespace GameEventSystem
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
        AllowMultiple = false, Inherited = false)]
    public sealed class AddTypeMenuAttribute : Attribute
    {
        public string MenuName { get; }

        public int Order { get; }

        public AddTypeMenuAttribute(string menuName, int order = 0)
        {
            MenuName = menuName;
            Order = order;
        }

        static readonly char[] k_Separeters = new char[] { '/' };

        /// <summary>
        /// Returns the menu name split by the '/' separator.
        /// </summary>
        public string[] GetSplittedMenuName()
        {
            return !string.IsNullOrWhiteSpace(MenuName)
                ? MenuName.Split(k_Separeters, StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();
        }

        /// <summary>
        /// Returns the display name without the path.
        /// </summary>
        public string GetTypeNameWithoutPath()
        {
            string[] splittedDisplayName = GetSplittedMenuName();
            return (splittedDisplayName.Length != 0) ? splittedDisplayName[splittedDisplayName.Length - 1] : null;
        }
        
        public string GetMenuNameWithoutFinalItem()
        {
            string[] splittedDisplayName = GetSplittedMenuName();
            StringBuilder result = new StringBuilder();
            for(int i=0; i<splittedDisplayName.Length-1; i++)
            {
                result.Append(splittedDisplayName[i]);
            }
            return result.ToString();
        }

    }
}
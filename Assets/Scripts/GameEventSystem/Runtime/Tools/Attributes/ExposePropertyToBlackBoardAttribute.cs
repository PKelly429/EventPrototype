using System;

namespace GameEventSystem
{
    /// <summary>
    /// Allows BBParams to find references inside a class.
    /// e.g A string BBParam could find a reference to the name of a GameObject if name has this attribute. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExposePropertyToBlackBoardAttribute : Attribute
    {
        public ExposePropertyToBlackBoardAttribute()
        {
        }
    }
}
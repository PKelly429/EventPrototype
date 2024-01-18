using System;

namespace GameEventSystem
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExposePropertyAttribute : Attribute
    {
        public ExposePropertyAttribute()
        {
        }
    }
}
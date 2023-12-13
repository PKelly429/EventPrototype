using System;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ExposePropertyAttribute : Attribute
{
    public ExposePropertyAttribute () 
    {
    }
}
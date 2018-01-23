using System;

namespace SmartChord.Parser.Annotations
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
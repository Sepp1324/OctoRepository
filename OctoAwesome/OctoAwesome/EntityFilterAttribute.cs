using System;

namespace OctoAwesome
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EntityFilterAttribute : Attribute
    {
        public EntityFilterAttribute(params Type[] entityComponentTypes) => EntityComponentTypes = entityComponentTypes;

        public Type[] EntityComponentTypes { get; set; }
    }
}
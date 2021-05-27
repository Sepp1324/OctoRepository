using System;

namespace OctoAwesome
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EntityFilterAttribute : Attribute
    {
        public Type[] EntityComponentTypes { get; set; }

<<<<<<< HEAD
        public EntityFilterAttribute(params Type[] entityComponentTypes) => EntityComponentTypes = entityComponentTypes;
=======
        public EntityFilterAttribute(params Type[] entityComponentTypes) 
            => EntityComponentTypes = entityComponentTypes;
>>>>>>> feature/performance
    }
}

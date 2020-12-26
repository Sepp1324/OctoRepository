using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class EntityFilterAttribute : Attribute
    {
        public EntityFilterAttribute(params Type[] entityComponentTypes)
        {
            EntityComponentTypes = entityComponentTypes;
        }

        public Type[] EntityComponentTypes { get; set; }
    }
}
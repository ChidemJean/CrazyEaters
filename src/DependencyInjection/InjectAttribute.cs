using System;

namespace CrazyEaters.DependencyInjection 
{
    public enum InjectType {
        Singleton,
        Transient
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
        private InjectType type;
        public InjectAttribute(InjectType type = InjectType.Singleton) {
            this.type = type;
        }
        public InjectType GetInjectType()
        {
            return type;
        }
    }
}
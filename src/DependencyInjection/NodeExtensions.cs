using Godot;
using System;
using System.Reflection;
using System.Linq;
using SimpleInjector;

namespace CrazyEaters.DependencyInjection 
{
    public static class NodeExtensions
    {
        public static void ResolveDependencies(this Node node)
        {
            var disPath = "/root/DependencyInjectionSystem";
            DependencyInjectionSystem dis = node.GetNode<DependencyInjectionSystem>(disPath);
            var at = typeof(InjectAttribute);
            var fields = node.GetType()
                .GetRuntimeFields()
                .Where(f => f.GetCustomAttributes(at, true).Any());

            foreach (var field in fields)
            {

                var obj = dis.Resolve(field.FieldType);
                
                try
                {
                    field.SetValue(node, obj);
                }
                catch (InvalidCastException)
                {
                    GD.PrintErr($"Error converting value " +
                        $"{obj} ({obj.GetType()})" +
                        $" to {field.FieldType}");               
                    throw;
                }
            }
        }
    }
}
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
                .Where(f => f.GetCustomAttributes(at, true).Any() || f.Name.ToLower().Contains("path"));

            foreach (var field in fields)
            {
                if (field.Name.ToLower().Contains("path")) continue;

                var attr = field.GetCustomAttributes(at, true) as InjectAttribute[];
                var name = field.Name;
                Node obj = null;

                if (attr.Length > 0 && attr[0].GetInjectType() == InjectType.Transient) {
                    FieldInfo _fieldTarget = node.GetType().GetField(name + "Path", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (_fieldTarget != null) { 
                        object path = _fieldTarget.GetValue(node);
                        obj = path == null ? null : node.GetNode(path as NodePath);
                    }
                } else {
                    obj = dis.Resolve(field.FieldType);
                }
                
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
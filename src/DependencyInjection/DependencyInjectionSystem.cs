using Godot;
using System;
using SimpleInjector;
using CrazyEaters.Managers;
using CrazyEaters.Save;
using System.Collections.Generic;

namespace CrazyEaters.DependencyInjection 
{
    public class DependencyInjectionSystem : Node
    {
        private SimpleInjector.Container container;
        private Dictionary<string, string> dependencies;

        public override void _EnterTree()
        {
            base._EnterTree();
            // container = new SimpleInjector.Container();

            // // do registrations here
            // container.Register<SceneSwitcher>(Lifestyle.Singleton);
            dependencies = new Dictionary<string, string>();

            dependencies.Add(typeof(SceneSwitcher).ToString(), "/root/MainNode/SceneSwitcher");
            dependencies.Add(typeof(GameManager).ToString(), "/root/GameManager");
            dependencies.Add(typeof(SaveSystemNode).ToString(), "/root/MainNode/SaveSystem");
        }

        public Node Resolve<T>(T type) where T : class
        {
            string resolvedNodePath = null;

            dependencies.TryGetValue(type.ToString(), out resolvedNodePath);

            return GetNode(resolvedNodePath);
        }

    }
}
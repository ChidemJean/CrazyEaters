using Godot;
using System;

namespace CrazyEaters.UI
{
    public class QuestItem : Control
    {
        [Export] public NodePath animPlayerPath;

        private AnimationPlayer animPlayer;

        public override void _Ready()
        {
            animPlayer = GetNode<AnimationPlayer>(animPlayerPath);
            animPlayer.Play("full_bar_shining");
            // Connect("visibility_changed", this, nameof(OnVisibilityChanged));
        }

        // public void OnVisibilityChanged()
        // {
        //     if (Visible) {
        //         animPlayer.Play();
        //         return;
        //     }
        //     animPlayer.Stop();
        // }
    }
}
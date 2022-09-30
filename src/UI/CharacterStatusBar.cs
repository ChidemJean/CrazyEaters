
namespace CrazyEaters.UI
{
    using Godot;
    using System;
    using System.Threading.Tasks;
    using CrazyEaters.Resources;
    using CrazyEaters.Managers;
    using CrazyEaters.DependencyInjection;

    public class CharacterStatusBar : Control
    {
        [Export]
        private StatusesCharacter statusesCharacter;

        [Export]
        private PackedScene prefabStatus;

        private RandomNumberGenerator rnd;

        [Inject]
        GameManager gm;

        public override void _Ready()
        {
            this.ResolveDependencies();
            rnd = new RandomNumberGenerator();
            rnd.Randomize();
            Mount();
        }

        public void Mount() 
        {
            if (statusesCharacter != null && statusesCharacter.statuses != null) {
                foreach (string statusName in statusesCharacter.statuses.Keys) {
                    StatusCharacter status = statusesCharacter.statuses[statusName];
                    StatusCharacterUI statusUI = prefabStatus.Instance<StatusCharacterUI>();
                    statusUI.StatusData = status;
                    AddChild(statusUI);
                    // ChangeValue(statusUI);
                }
            }
        }

        // public async void ChangeValue(StatusCharacterUI statusUI) {
        //     await Task.Delay(TimeSpan.FromSeconds(2 + rnd.RandiRange(1, 2)));
        //     statusUI.UpdateValue(-rnd.RandiRange(20, 100));
        // }
    }
}
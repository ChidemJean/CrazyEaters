using Godot;
using System;
using System.Linq;
using CrazyEaters.UI.Generics;
using CrazyEaters.Save;
using CrazyEaters.Managers;
using CrazyEaters.DependencyInjection;

namespace CrazyEaters.UI
{
    public class SavesSelector : VBoxContainer
    {
        [Inject] private GameManager gameManager;
        [Inject] private SaveSystemNode saveSystemNode;
        [Export] private NodePath thumbPath;
        [Export] private NodePath btnJoinPath;
        [Export] private NodePath ctnSavesPath;
        [Export] private PackedScene saveDotPrefab;

        private TextureRect thumb;
        private BlueButtonNinePatch btnJoin;
        private Control ctnSaves;
        private HabitatsGameData habitatsGameData;
        private string uuidSaveSelected;
    
        public override void _Ready()
        {
            this.ResolveDependencies();
            thumb = GetNode<TextureRect>(thumbPath);
            btnJoin = GetNode<BlueButtonNinePatch>(btnJoinPath);
            ctnSaves = GetNode<Control>(ctnSavesPath);

            saveSystemNode.LoadHabitats(OnHabitatsLoaded);

            btnJoin.Connect("click", this, nameof(OnJoinClick));
        }

        public void OnJoinClick(bool pressed)
        {
            if (pressed) return;
            gameManager.TriggerEvent(GameEvent.ChangeScene, "habitat", uuidSaveSelected);
        }

        public void OnHabitatsLoaded(HabitatsGameData habitatsGameData)
        {
            this.habitatsGameData = habitatsGameData;
            SortHabitatsByLastDatePlay();
            PopulateSavesUI();
        }

        public void PopulateSavesUI()
        {
            CleanSavesUI();
            foreach (var (habitat, i) in habitatsGameData.habitats.Select((habitat, i) => (habitat, i))) {
                var newSave = saveDotPrefab.Instance<SaveDot>();
                newSave.SaveUUID = habitat.uuid;
                newSave.onReady = () => {
                    newSave.Idx = (i + 1).ToString();
                };
                newSave.Connect("click", this, nameof(OnSaveDotClick));
                ctnSaves.AddChild(newSave);
            }
            ChangeSelectedSave(0);
        }

        public void OnSaveDotClick(string idx)
        {
            int targetId = idx.ToInt() - 1;
            ChangeSelectedSave(targetId);
        }

        public void ChangeSelectedSave(int id) 
        {
            HabitatGameData habitatGame = habitatsGameData.habitats[id];
            uuidSaveSelected = habitatGame.uuid;
            foreach (Node saveNode in ctnSaves.GetChildren()) {
                var saveDot = (SaveDot) saveNode;
                saveDot.IsSelected = saveDot.Idx == (id + 1).ToString();
            }
        }

        public void CleanSavesUI()
        {
            foreach (Node child in ctnSaves.GetChildren()) {
                ctnSaves.RemoveChild(child);
                child.QueueFree();
            }
        }

        public void SortHabitatsByLastDatePlay()
        {
            DateTime dateTimeNow = DateTime.Now;
            this.habitatsGameData.habitats.Sort(delegate(HabitatGameData habitatA, HabitatGameData habitatB)
            {
                DateTime dateTimeA = DateTime.ParseExact(habitatA.datetimeLastPlay, saveSystemNode.DATETIME_FORMAT, null);
                DateTime dateTimeB = DateTime.ParseExact(habitatB.datetimeLastPlay, saveSystemNode.DATETIME_FORMAT, null);

                int resA = DateTime.Compare(dateTimeA, dateTimeNow);
                int resB = DateTime.Compare(dateTimeB, dateTimeNow);

                // mais próximos do dia atual
                if (resA < resB) {
                    return 1;
                } else {
                    return -1;
                }
            });
        }

    }
}

namespace CrazyEaters.Managers 
{
    using Godot;
    using System;
    using System.Collections.Generic;
    using CrazyEaters.Controllers;
    using CrazyEaters.Optimization;
    using CrazyEaters.Sandbox;
    using CrazyEaters.Save;

    public class GameManager : Spatial
    {
        public enum InputMode { SCENE, UI, LAUNCHER };
        public enum GameMode { DEFAULT, LAUNCHER, COOK, BUILD };
        public bool gameEventsActive = true;
        public Hud hud;
        public Godot.Object ResourceQueue;
        public InputMode inputMode = InputMode.SCENE;
        public GameMode gameMode = GameMode.LAUNCHER;

        public Vector3 gravityVector = (Vector3) ProjectSettings.GetSetting("physics/3d/default_gravity_vector");
        public int gravityMagnitude = Convert.ToInt32(ProjectSettings.GetSetting("physics/3d/default_gravity"));
        public bool inDebug = false;
        public CrazyEaters.Sandbox.World world;
        public GameData gameData = null;
        public Spatial currentMainNode3D = null;

        [Signal]
        public delegate void OnDebugModeChange(bool inDebug);

        public override void _Ready()
        {
            hud = GetNode<Hud>("/root/MainNode");
            if (eventDictionary == null) {
                eventDictionary = new Dictionary<GameEvent, Action<object>>();
            }
            StartListening(GameEvent.GameModeChange, OnGameModeChange);
        }

        public void OnGameModeChange(object mode)
        {
            if (mode is GameMode) {
                gameMode = (GameMode) mode;
            }
        }

        public void SetInDebug(bool inDebug) {
            this.inDebug = inDebug;
            EmitSignal(nameof(OnDebugModeChange), this.inDebug);
        }

        #region Events

        private Dictionary<GameEvent, Action<object>> eventDictionary;
        private Action<GameEvent, object> globalMidleware = null;

        public void StartListening(GameEvent gameEvent, Action<object> listener)
        {
            if (!gameEventsActive) return;
            Action<object> thisEvent;
            if (eventDictionary.TryGetValue(gameEvent, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                eventDictionary[gameEvent] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                eventDictionary.Add(gameEvent, thisEvent);
            }
        }

        public void StopListening(GameEvent gameEvent, Action<object> listener)
        {
            if (!gameEventsActive) return;
            Action<object> thisEvent;
            if (eventDictionary.TryGetValue(gameEvent, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                eventDictionary[gameEvent] = thisEvent;
            }
        }

        public void SetGlobalMidleware(Action<GameEvent, object> action)
        {
            if (!gameEventsActive) return;
            globalMidleware = action;
        }

        public void TriggerEvent(GameEvent gameEvent, params object[] eventParam)
        {
            if (!gameEventsActive) return;
            Action<object> thisEvent = null;
            if (eventDictionary.TryGetValue(gameEvent, out thisEvent))
            {
                if (globalMidleware != null) globalMidleware.Invoke(gameEvent, eventParam.Length == 1 ? eventParam[0] : eventParam);
                if (thisEvent != null) thisEvent.Invoke(eventParam.Length == 1 ? eventParam[0] : eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }

        public override void _EnterTree() {
            gameEventsActive = true;
        }

        public override void _ExitTree() {
            gameEventsActive = false;
            StopListening(GameEvent.GameModeChange, OnGameModeChange);
        }

        #endregion

    }
}
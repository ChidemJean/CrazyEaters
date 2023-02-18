using Godot;
using System;
using System.Collections.Generic;
using CrazyEaters.DependencyInjection;
using CrazyEaters.Managers;

namespace CrazyEaters.Debug
{
    public class CLI : VBoxContainer
    {

        [Export] public NodePath cmdLabelPath;
        [Export] public NodePath inputPath;
        [Export] public NodePath closePath;
        [Inject] public GameManager gameManager;

        public Label cmdLabel;
        public LineEdit input;
        public Control close;
        private bool isOpen = false;
        private SceneTreeTween tween;
        private string initialCmdText;

        public Dictionary<string, string> cmds = new Dictionary<string, string> 
        { 
            {"cs", "cs [scene_key] - Muda a cena atual"},  
            {"clear", "clear - Limpa cmd"},
            {"help", "help - Lista os comandos disponíveis"},
        };
        
        public override void _Ready()
        {
            this.ResolveDependencies();
            cmdLabel = GetNode<Label>(cmdLabelPath);
            input = GetNode<LineEdit>(inputPath);
            close = GetNode<Control>(closePath);
            initialCmdText = cmdLabel.Text;
            input.Connect("text_entered", this, nameof(OnTextEntered));
            close.Connect("gui_input", this, nameof(OnGuiInputClose));
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey) {
                InputEventKey _input = (InputEventKey) @event;
                if (!_input.IsPressed()) {
                    if (_input.Scancode == (uint) KeyList.Quoteleft) {
                        if (isOpen) {
                            Close();
                            return;
                        }
                        Open();
                    }
                    if (_input.Scancode == (uint) KeyList.Escape) {
                        Close();
                    }
                }
            }
        }

        public void OnTextEntered(string newText)
        {
            if (!isOpen) return;
            foreach (KeyValuePair<string, string> keyValueCmd in cmds) {
                if (newText.IndexOf(keyValueCmd.Key) != -1) {
                    ExecuteCmd(keyValueCmd.Key, newText);
                }
            }
        }

        public void ExecuteCmd(string cmdKey, string cmd)
        {
            if (cmdKey == "cls") {
                cmdLabel.Text = initialCmdText;
            } else {
                string[] cmdParams = cmd.Substr(cmd.IndexOf(cmdKey) + cmdKey.Length, cmd.Length-1).Trim().Split(" ");
                switch (cmdKey) {
                    case "cs":
                        gameManager.TriggerEvent(GameEvent.ChangeScene, cmdParams[0]);
                        cmdLabel.Text += "\n" + "'" + cmd + "' executado.";
                        Close();
                        break;
                    case "h":
                        string helpStr = "";
                        foreach (KeyValuePair<string, string> keyValueCmd in cmds) {
                            helpStr += "\n" + keyValueCmd.Value;
                        }
                        cmdLabel.Text += helpStr;
                        break;
                }
            }
            input.Text = "";
        }

        public void OnGuiInputClose(InputEvent @event)
        {

        }

        public void Open()
        {
            this.isOpen = true;
            Visible = true;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.TweenProperty(this, "rect_position:y", 0f, .4f);
            input.GrabFocus();
        }

        public async void Close()
        {
            this.isOpen = false;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.TweenProperty(this, "rect_position:y", -RectSize.y, .25f);
            await ToSignal(tween, "finished");
            Visible = false;
        }

    }
}
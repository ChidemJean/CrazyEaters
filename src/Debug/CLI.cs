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
        private List<string> history = new List<string>();
        private int selectedHistoryKey = 0;

        public Dictionary<string, string> cmds = new Dictionary<string, string> 
        { 
            {"cs", "cs [scene_key] - Muda a cena atual"},  
            {"clear", "clear - Limpa cmd"},
            {"help", "help - Lista os comandos disponíveis"},
            {"clr_hist", "clr_hist - Limpa o histórico de comandos"},
            {"list_hist", "list_hist - Lista o histórico de comandos"},
            {"inv", "inv - Solicita operação no inventário"},
        };
        
        public override void _Ready()
        {
            this.ResolveDependencies();
            cmdLabel = GetNode<Label>(cmdLabelPath);
            input = GetNode<LineEdit>(inputPath);
            close = GetNode<Control>(closePath);
            initialCmdText = cmdLabel.Text;
            input.Connect("text_entered", this, nameof(OnTextEntered));
            input.Connect("gui_input", this, nameof(OnGuiInput));
            close.Connect("gui_input", this, nameof(OnGuiInputClose));
        }

        public override void _Input(InputEvent @event)
        {
            if (isOpen) {
                if (@event is InputEventKey) {
                    InputEventKey _event = (InputEventKey) @event;
                    if (_event.IsPressed()) return; 
                    bool KeyArrowUp = _event.Scancode == (uint) KeyList.Up;
                    bool KeyArrowDown = _event.Scancode == (uint) KeyList.Down;
                    if (KeyArrowUp || KeyArrowDown) {
                        if (history.Count > 0) {
                            input.GrabFocus();
                            string cmdHistory = history[selectedHistoryKey];
                            input.Text = cmdHistory;
                            int newIdx = KeyArrowUp ? selectedHistoryKey + 1 : selectedHistoryKey - 1;
                            selectedHistoryKey = Mathf.Clamp(newIdx, 0, history.Count - 1);
                            input.Select(input.Text.Length);
                        }
                    }
                }
            }
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
                    return;
                }
            }
            cmdLabel.Text += "\n" + "Comando não encontrado";
            input.Text = "";
        }

        public void ExecuteCmd(string cmdKey, string cmd)
        {
            AddCmdToHistory(cmd);
            selectedHistoryKey = 0;
            if (cmdKey == "clear") {
                cmdLabel.Text = initialCmdText;
            } else {
                string[] cmdParams = cmd.Substr(cmd.IndexOf(cmdKey) + cmdKey.Length, cmd.Length-1).Trim().Split(" ");
                switch (cmdKey) {
                    case "cs":
                        gameManager.TriggerEvent(GameEvent.ChangeScene, cmdParams[0]);
                        cmdLabel.Text += "\n" + "'" + cmd + "' executado.";
                        Close();
                        break;
                    
                    case "help":
                        string helpStr = "";
                        foreach (KeyValuePair<string, string> keyValueCmd in cmds) {
                            helpStr += "\n" + keyValueCmd.Value;
                        }
                        cmdLabel.Text += helpStr;
                        break;

                    case "clr_hist":
                        history.Clear();
                        break;

                    case "list_hist":
                        string histStr = "\nHistórico de comandos: ";
                        if (history.Count == 0) {
                            histStr += "\n vazio";
                        }
                        foreach (string histItem in history) {
                            histStr += "\n ------ ["+(history.IndexOf(histItem)+1)+"]: " + histItem;
                        }
                        cmdLabel.Text += histStr;
                        break;

                    case "inv":
                        string entityKey = cmdParams[0];
                        int qtd = cmdParams[1].ToInt();
                        InventoryOperationRequest invRequest = new InventoryOperationRequest(entityKey, qtd);
                        gameManager.TriggerEvent(GameEvent.OnInventoryOperationRequest, invRequest);
                        Close();
                        break;
                }
            }
            input.Text = "";
        }

        public void AddCmdToHistory(string cmd)
        {
            if (cmd != "list_histr" && !history.Contains(cmd)) {
                history.Add(cmd);
            }
        }

        public void OnGuiInput(InputEvent @event)
        {
            
        }
        
        public void OnGuiInputClose(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;
                if (!_event.IsPressed() && _event.ButtonIndex == (int) ButtonList.Left) {
                    Close();
                }
            }
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
            selectedHistoryKey = 0;
            input.Text = "";
        }

    }
}
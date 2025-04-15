using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Infrastructure.Services.Debug.Commands;
using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Debug
{
    public class DebugController : MonoBehaviour, IService
    {
        [Inject] private UIFactory _uiFactory;
        
        private readonly Dictionary<string, DebugCommandBase> _commands = new();

        private bool _showConsole;
        private string _input;
        private Vector2 _scroll;

        [Header("Settings")]
        [SerializeField] private Font _consoleFont;

        [SerializeField] private Texture2D _backgroundTexture;
        [SerializeField] private Texture2D _buttonHoverTexture;

        [Header("History Settings")]
        [SerializeField] private int _maxHistoryItems = 20;

        private List<string> _commandHistory = new List<string>();
        private List<string> _displayHistory = new List<string>();

        private void Awake()
        {
            var commandList = new List<DebugCommandBase>
            {
                new HelpDebugCommand(),
                new JSONDebugCommand(),
                new SwitchDIBindingDebugCommand(),
                new SwitchUI(_uiFactory),
                new SwitchHUD(_uiFactory),
                new ChangeTimescale()
            };

            commandList.ForEach(c => _commands.Add(c.ID, c));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                _showConsole = !_showConsole;
                _input = "";
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    ProcessCommand();
                }
            }

            if (GUI.GetNameOfFocusedControl() != "ConsoleInput")
            {
                GUI.FocusControl("ConsoleInput");
            }
        }

        private const float CONSOLE_HEIGHT = 300f;
        private const float ANIM_SPEED = 8f;

        private float _currentHeight;
        private Vector2 _scrollPosition;

        private GUIStyle _windowStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _scrollStyle;
        private GUIStyle _textStyle;


        private void InitializeStyles()
        {
            // Window style
            _windowStyle = new GUIStyle();
            _windowStyle.normal.background = CreateTexture(new Color(0.08f, 0.08f, 0.08f, 0.98f));
            _windowStyle.border = new RectOffset(10, 10, 10, 10);
            _windowStyle.padding = new RectOffset(15, 15, 15, 15);

            // Input field style
            _inputStyle = new GUIStyle(GUI.skin.textField);
            _inputStyle.normal.background = CreateTexture(new Color(0.15f, 0.15f, 0.15f, 1f));
            _inputStyle.active.background = _inputStyle.normal.background;
            _inputStyle.focused.background = _inputStyle.normal.background;
            _inputStyle.normal.textColor = new Color(0.2f, 0.8f, 0.4f);
            _inputStyle.fontSize = 16;
            _inputStyle.padding = new RectOffset(12, 12, 8, 8);
            _inputStyle.font = _consoleFont;

            // Button style
            _buttonStyle = new GUIStyle();
            _buttonStyle.normal.background = CreateTexture(new Color(0.2f, 0.2f, 0.2f, 1f));
            _buttonStyle.hover.background = _buttonHoverTexture ?? CreateTexture(new Color(0.3f, 0.3f, 0.3f, 1f));
            _buttonStyle.active.background = CreateTexture(new Color(0.15f, 0.15f, 0.15f, 1f));
            _buttonStyle.normal.textColor = new Color(0.4f, 0.9f, 0.6f);
            _buttonStyle.fontSize = 14;
            _buttonStyle.padding = new RectOffset(20, 20, 10, 10);
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
            _buttonStyle.font = _consoleFont;

            // Scroll view style
            _scrollStyle = new GUIStyle();
            _scrollStyle.normal.background = CreateTexture(new Color(0.12f, 0.12f, 0.12f, 1f));

            // Text style
            _textStyle = new GUIStyle();
            _textStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            _textStyle.fontSize = 14;
            _textStyle.font = _consoleFont;
            _textStyle.richText = true;
        }

        private Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void AddToHistory(string message)
        {
            _commandHistory.Add(message);
            _displayHistory.Add(message);

            // Ограничиваем размер истории
            if (_displayHistory.Count > _maxHistoryItems)
            {
                _displayHistory.RemoveAt(0);
            }
        }

        private void ExecuteCommand()
        {
            string[] properties = _input.Split(" ");

            foreach (DebugCommandBase command in _commands.Values.Where(command => _input.Contains(command.ID)))
            {
                switch (command)
                {
                    case DebugCommand debugCommand:
                        debugCommand.Invoke();
                        break;
                    case DebugCommand<int> debugCommandInt:
                        debugCommandInt.Invoke(int.Parse(properties.Last()));
                        break;
                    case DebugCommand<bool> debugCommandInt:
                        debugCommandInt.Invoke(bool.Parse(properties.Last()));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(command));
                }

                AddToHistory($"<color=#888>Executed: {command}</color>");
            }
        }

        private void OnGUI()
        {
            if (!_showConsole) return;

            if (_windowStyle == null) InitializeStyles();

            // Smooth animation
            _currentHeight = Mathf.Lerp(_currentHeight, CONSOLE_HEIGHT, Time.deltaTime * ANIM_SPEED);

            GUI.color = Color.white;
            GUILayout.BeginVertical(_windowStyle, GUILayout.Height(_currentHeight));

            // Scroll view for history
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, _scrollStyle);

            GUILayout.Label("<b>Debug Console</b>", _textStyle);
            GUILayout.Space(10);

            // Example commands
            GUILayout.Label("<color=#4CAF50>> Welcome to Debug Console</color>", _textStyle);
            GUILayout.Label("<color=#2196F3>> Type 'help' for commands list</color>", _textStyle);

            GUILayout.EndScrollView();

            // Input area
            GUILayout.BeginHorizontal();

            {
                GUI.SetNextControlName("ConsoleInput");
                _input = GUILayout.TextField(_input, _inputStyle, GUILayout.Height(40));

                if (GUILayout.Button("Submit", _buttonStyle, GUILayout.Width(100), GUILayout.Height(40)))
                {
                    ProcessCommand();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            HandleInput();
        }

        private void ProcessCommand()
        {
            if (string.IsNullOrWhiteSpace(_input))
                return;

            // Добавляем команду в историю
            AddToHistory($"> {_input}");

            // Обработка команды (ваша логика)
            ExecuteCommand();

            _input = "";

            // Автоматическая прокрутка к новому элементу
            _scrollPosition.y = float.MaxValue;
        }

        /*
            private void OnGUI()
            {
                if (!_showConsole)
                    return;

                float y = 0f;

                bool showHelp =
                    ((HelpDebugCommand)_commands.Values.First(command => command is HelpDebugCommand))
                    .ShowHelp;

                if (showHelp)
                {
                    y = ShowHelp(y);
                }

                int width = Screen.width - 150;
                float height = 30;
                GUI.Box(new Rect(0, y, width, height), "");
                GUI.backgroundColor = new Color(0, 0, 0, 0);
                GUI.color = Color.green;
                _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 180, 20f), _input);

                bool sendButtonPressed = GUI.Button(new Rect(width + 10, y + height / 2, 140, height), "Send");

                if (sendButtonPressed)
                {
                    Send();
                }
            }*/

        private float ShowHelp(float y)
        {
            GUI.Box(new Rect(0, y, Screen.width - 200, 100), "");

            var viewport = new Rect(0, 0, Screen.width - 200, 20 * _commands.Count);
            _scroll = GUI.BeginScrollView(new Rect(0, y + 5, Screen.width - 200, 90), _scroll, viewport);

            for (int i = 0; i < _commands.Count; i++)
            {
                DebugCommandBase command = _commands.Values.ElementAt(i);
                string label = $"{command.Format} - {command.Description}";
                var labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();

            y += 100;
            return y;
        }
    }

    [Serializable]
    public class DebugJSONData<T>
    {
        public DateTime Time = DateTime.Now;
        public T Object;
    }
}
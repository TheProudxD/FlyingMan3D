using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.FPSCounter
{
    public class FPSDisplay2 : MonoBehaviour, IService
    {
        public bool ShowInfo = true;

        private Vector2 _resolution = new(1920, 1080);
        private int _fpsRange = 60;
        private float _deltaTime;
        private int[] _fpsBuffer;
        private int _fpsBufferIndex;
        private int AverageFPS { get; set; }
        private int HighestPfs { get; set; }
        private int LowersFPS { get; set; }

        private void Start()
        {
            Screen.SetResolution((int)_resolution.x, (int)_resolution.y, true);
            Application.targetFrameRate = _fpsRange;
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            if (_fpsBuffer == null || _fpsRange != _fpsBuffer.Length)
            {
                InitializeBuffer();
            }

            UpdateBuffer();
            CalculateFps();
        }

        private void InitializeBuffer()
        {
            _fpsBuffer = new int[_fpsRange];
            _fpsBufferIndex = 0;
        }

        private void UpdateBuffer()
        {
            _fpsBuffer[_fpsBufferIndex++] = (int)(1f / _deltaTime);

            if (_fpsBufferIndex >= _fpsRange)
            {
                _fpsBufferIndex = 0;
            }
        }

        private void CalculateFps()
        {
            int sum = 0;
            int lowest = int.MaxValue;
            int highest = 0;

            for (int i = 0; i < _fpsRange; i++)
            {
                int fps = _fpsBuffer[i];
                sum += fps;

                if (fps > highest)
                {
                    highest = fps;
                }

                if (fps < lowest)
                {
                    lowest = fps;
                }
            }

            HighestPfs = highest;
            LowersFPS = lowest;
            AverageFPS = sum / _fpsRange;
        }

        private void OnGUI()
        {
            if (!ShowInfo) return;

            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1.0f, 0.0f, 0.5f, 1.0f);
            float msec = _deltaTime * 1000.0f;

            string text =
                $"{msec:0.0} ms. Average FPS: {AverageFPS}. Highest FPS: {HighestPfs}. Lowers FPS: {LowersFPS}";

            GUI.Label(rect, text, style);

            //Rect rect2 = new Rect(250, -10, w, h * 2 / 100);
            //GUI.Label(rect2, Screen.currentResolution.width + "x" + Screen.currentResolution.height, style);
        }
    }
}
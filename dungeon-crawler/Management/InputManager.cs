using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.Management
{
    public class InputManager
    {
        public delegate void MethodDelegate();

        public class SingleShotInput
        {
            public Keys InputKey { get; set; }
            public MethodDelegate OutputFunc { get; }
            public bool KeyHeldDown { get; set; }

            public SingleShotInput(Keys inputKey, MethodDelegate outputFunc)
            {
                this.InputKey = inputKey;
                this.OutputFunc = outputFunc;
                KeyHeldDown = false;
            }
        }

        private readonly List<SingleShotInput> _singleShotInputs;

        public InputManager()
        {
            _singleShotInputs = new List<SingleShotInput>();
        }

        public void FrameTick()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            for (int idx = 0; idx < _singleShotInputs.Count; idx++)
            {
                if (keyboardState.IsKeyDown(_singleShotInputs[idx].InputKey))
                {
                    if (!_singleShotInputs[idx].KeyHeldDown)
                    {
                        _singleShotInputs[idx].OutputFunc();
                    }
                    _singleShotInputs[idx].KeyHeldDown = true;
                }
                else
                {
                    _singleShotInputs[idx].KeyHeldDown = false;
                }
            }
        }

        /// <summary>
        /// Adds a key that triggers a function only once while that key is held down.
        /// </summary>
        /// <param name="inputKey">The key to trigger the function.</param>
        /// <param name="func">The function that gets triggered.</param>
        public void AddSingleShotInput(Keys inputKey, MethodDelegate func)
        {
            _singleShotInputs.Add(new SingleShotInput(inputKey, func));
        }
    }
}

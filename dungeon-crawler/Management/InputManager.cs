using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace dungeoncrawler.Management
{
    public class InputManager
    {
        public delegate void MethodDelegate();

        public class SingleShotInput
        {
            public Keys inputKey { get; set; }
            public MethodDelegate outputFunc { get; }
            public bool keyHeldDown { get; set; }

            public SingleShotInput(Keys inputKey, MethodDelegate outputFunc)
            {
                this.inputKey = inputKey;
                this.outputFunc = outputFunc;
                keyHeldDown = false;
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
                if (keyboardState.IsKeyDown(_singleShotInputs[idx].inputKey))
                {
                    if (!_singleShotInputs[idx].keyHeldDown)
                    {
                        _singleShotInputs[idx].outputFunc();
                    }
                    _singleShotInputs[idx].keyHeldDown = true;
                }
                else
                {
                    _singleShotInputs[idx].keyHeldDown = false;
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

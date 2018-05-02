using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoIt;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NierEnhancedPCExperienceMacro
{
    class DodgeHelp : MacroFunction
    {


        public static readonly int TimerDelay = 50;

        public DodgeHelp(GlobalInputHook hk) : base(hk)
        {
            foreach (var key in _movementKeys)
            {
                hk.HookedKeys.Add(key);
                _keyStates[key.ToString()] = false;
            }

            hk.HookedKeys.Add(_dodgeButton);
            hk.HookedKeys.Add(Keys.Space);

            hk.KeyDown += HkOnKeyDown;
            hk.KeyUp += HkOnKeyUp;

            var timer1 = new System.Timers.Timer(TimerDelay);
            timer1.Elapsed += Timer1OnElapsed;
            timer1.Start();
        }

        private void Timer1OnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_sendQueue.Count > 0)
            {
                var x = _sendQueue.Dequeue();
                AutoItX.Send(x);
            }
            else
            {
                _canQueue = true;
            }
        }

        private void HkOnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (_movementKeys.Contains(keyEventArgs.KeyCode))
            {
                    _keyStates[keyEventArgs.KeyCode.ToString()] = false;
            }
        }

        private void HkOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (_movementKeys.Contains(keyEventArgs.KeyCode))
            {
                bool nextBoostKey = true;
                foreach (var key in _movementKeys)
                    if (_keyStates[key.ToString()])
                        nextBoostKey = false;

                if (nextBoostKey)
                    _boostMovementKey = keyEventArgs.KeyCode;


                _keyStates[keyEventArgs.KeyCode.ToString()] = true;

            }



            if (keyEventArgs.KeyCode == _dodgeButton && _canQueue)
            {
                string key = _boostMovementKey.ToString().ToLower();
                _canQueue = false;

                int offset = _keyStates[_boostMovementKey.ToString()] ? 1 : 0;

                for (int i = 0 + offset; i < 4 + offset; i++)
                {
                    var modifier = i % 2 == 0 ? "down" : "up";
                    _sendQueue.Enqueue($"{{{key} {modifier}}}");
                }

                keyEventArgs.Handled = true;
            }

        }


        private readonly Dictionary<string, bool> _keyStates = new Dictionary<string, bool>();

        private Keys _dodgeButton = Keys.XButton1;
        private Keys _boostMovementKey = Keys.W;
        private readonly Keys[] _movementKeys = {Keys.W, Keys.A, Keys.S, Keys.D};
        private readonly Queue<string> _sendQueue = new Queue<string>();
        private bool _canQueue = true;

    }
}

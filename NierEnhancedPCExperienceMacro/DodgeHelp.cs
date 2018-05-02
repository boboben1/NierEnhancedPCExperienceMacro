using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoIt;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NierEnhancedPCExperienceMacro
{
    class DodgeHelp : MacroFunction
    {
        public DodgeHelp(GlobalInputHook hk) : base(hk)
        {
            foreach (var key in _movementKeys)
                hk.HookedKeys.Add(key);
            hk.HookedKeys.Add(_dodgeButton);


            hk.KeyDown += HkOnKeyDown;
        }

        private void HkOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (_movementKeys.Contains(keyEventArgs.KeyCode))
            {
                _lastMovementKey = keyEventArgs.KeyCode;
            }

            if (keyEventArgs.KeyCode == _dodgeButton)
            {
                string key = _lastMovementKey.ToString().ToLower();
                AutoItX.Send($"{key}{key}");
            }

            //keyEventArgs.Handled = true;
        }

        private Keys _dodgeButton = Keys.XButton1;
        private Keys _lastMovementKey = Keys.W;
        private readonly Keys[] _movementKeys = {Keys.W, Keys.A, Keys.S, Keys.D};
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NierEnhancedPCExperienceMacro
{
    class MacroFunction
    {
        public MacroFunction(GlobalInputHook hk)
        {
            this._hk = hk;
        }

        protected GlobalInputHook _hk;
    }
}

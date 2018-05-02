using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoIt;

namespace NierEnhancedPCExperienceMacro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _globalInputHook = new GlobalInputHook();
            macroFunctions.Add(new DodgeHelp(_globalInputHook));
        }

        private GlobalInputHook _globalInputHook;
        private List<MacroFunction> macroFunctions = new List<MacroFunction>();
    }
}

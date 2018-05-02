using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NierEnhancedPCExperienceMacro
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WriteResourceToFile(Environment.Is64BitProcess ? "AutoItX3_x64.dll" : "AutoItX3.dll");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = new AssemblyName(args.Name).Name;
            if (assemblyName.EndsWith(".resources"))
                return null;


            string dllName = assemblyName + ".dll";


            return Assembly.LoadFrom(WriteResourceToFile(dllName));
        }

        static string WriteResourceToFile(string name)
        {
            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? throw new InvalidOperationException(), name);

            using (Stream s = Assembly.GetEntryAssembly().GetManifestResourceStream(typeof(Program).Namespace + @"." + name))
            {
                if (s != null)
                {
                    byte[] data = new BinaryReader(s).ReadBytes((int)s.Length);

                    File.WriteAllBytes(fullPath, data);
                }
            }

            return fullPath;
        }
    }
}

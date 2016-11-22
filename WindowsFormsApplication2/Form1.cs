using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Controls.Add(new MyButton());
        }
    }

    class MyButton : Button
    {
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_NCHITTEST = 0x0084;
        const int WM_SETCURSOR = 0x0020;
        const int WM_POINTERUPDATE = 0x0245;

        Dictionary<int, string> mCommandMap = new Dictionary<int, string>();

        public MyButton()
        {
            var t = typeof(Control).Assembly.GetType("System.Windows.Forms.NativeMethods", true);
            foreach (var f in t.GetFields().Where(f => f.Name.StartsWith("WM_")))
            {
                mCommandMap[(int)f.GetValue(null)] = f.Name;
            }

            getCustomMessage("System.Windows.Forms.Control", "WM_GETCONTROLNAME", null);
            getCustomMessage("System.Windows.Forms.Control", "WM_GETCONTROLTYPE", null);
            getCustomMessage("System.Windows.Forms.Control", "mouseWheelMessage", null);
            getCustomMessage("System.Windows.Forms.NativeMethods", "wmMouseEnterMessage", null);
            getCustomMessage("System.Windows.Forms.NativeMethods", "wmUnSubclass", null);

            addCustomMessage(0x0249, "WM_POINTERENTER");
            addCustomMessage(0x024A, "WM_POINTERLEAVE");
        }

        void getCustomMessage(string className, string fieldName, object instance)
        {
            var t = typeof(Control).Assembly.GetType(className, true);
            var f = t.GetField(fieldName, (instance == null ? BindingFlags.Static : BindingFlags.Default) | BindingFlags.NonPublic);
            int val = (int)f.GetValue(instance);
            if (val <= 0)
                return;
            mCommandMap[val] = fieldName + "^";
        }

        void addCustomMessage(int msg, string name)
        {
            mCommandMap.Add(msg, name + "#");
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_MOUSEMOVE:
                case WM_NCHITTEST:
                case WM_SETCURSOR:
                case WM_POINTERUPDATE:
                    break;
                default:
                    string name;
                    if (!mCommandMap.TryGetValue(m.Msg, out name))
                        name = $"0x{m.Msg:x}";
                    Console.WriteLine($"{DateTime.Now}: {name}");
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

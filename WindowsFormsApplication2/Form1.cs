using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        Dictionary<int, string> mCommandMap = new Dictionary<int, string>();

        public MyButton()
        {
            var t = typeof(Control).Assembly.GetType("System.Windows.Forms.NativeMethods", true);
            foreach (var f in t.GetFields().Where(f => f.Name.StartsWith("WM_")))
            {
                mCommandMap[(int)f.GetValue(null)] = f.Name;
            }

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != WM_MOUSEMOVE && m.Msg != WM_NCHITTEST && m.Msg != WM_SETCURSOR)
            {
                string name;
                if (!mCommandMap.TryGetValue(m.Msg, out name))
                    name = $"0x{m.Msg:x}";
                Console.WriteLine($"{DateTime.Now}: {name}");
            }
            base.WndProc(ref m);
        }
    }
}

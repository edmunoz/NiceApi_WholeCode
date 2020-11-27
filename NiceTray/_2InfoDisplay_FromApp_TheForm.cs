using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;

using NiceApiLibrary_low;
using System.Collections.Generic;

namespace NiceTray
{
    delegate void d_UserAction_DisplayText(Data_DisplayText data);
    delegate void d_SimpleKeyPress(char c);

    public class _2InfoDisplay_FromApp_TheForm : Form
    {
        private NotifyIcon trayIcon;
        private TextBox textBox_all;
        private ContextMenu trayMenu;
        private static ManualResetEvent readyEvent;
        public static Thread internalThread;

        #region startup
        public static _2InfoDisplay_FromApp_TheForm CreateAndStart(ManualResetEvent setOnGo)
        {
            readyEvent = setOnGo;
            _2InfoDisplay_FromApp_TheForm f = null;

            internalThread = new Thread(new ThreadStart(delegate()
                {
                    f = new _2InfoDisplay_FromApp_TheForm();
                    f.FormInit(
                        "_2InfoDisplay.FromApp.HaveFormOnTheEdge".IsConfiguredAndTRUE(),
                        "_2InfoDisplay.FromApp.BetterFormPosition".GetConfig()
                        );
                    Application.Run(f); 
                }));
            internalThread.Start();
            HideConsole.DoHide();
            readyEvent.WaitOne();
            return f;
        }

        private _2InfoDisplay_FromApp_TheForm()
        {

        }

        private void FormInit(bool onEdge, string betterFormPosition)
        {
            //            TheOnlyNiceAppForm = this;
            this.textBox_all = new System.Windows.Forms.TextBox();
            this.textBox_all = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            this.textBox_all.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_all.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox_all.Font = new System.Drawing.Font("Courier New", 8F);
            this.textBox_all.Location = new System.Drawing.Point(1, 1);
            this.textBox_all.Margin = new System.Windows.Forms.Padding(1);
            this.textBox_all.Multiline = true;
            this.textBox_all.Name = "textBox_all";
            this.textBox_all.Size = new System.Drawing.Size(10, 10);
            this.textBox_all.TabIndex = 0;
            this.textBox_all.Text = "";
            this.ClientSize = new Size(10, 10);
            this.Size = new Size(10, 10);
            this.ClientSize = new Size(10, 10);
            this.Size = new Size(10, 10);
            this.Width = 1000;
            this.Height = 500;
            FormWindowState s = this.WindowState;
            this.Controls.Add(this.textBox_all);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SysTrayApp";
            this.TopMost = true;
            this.Leave += new System.EventHandler(this.SysTrayApp_Leave);
            //this.Load += SysTrayApp_Load;
            this.ResumeLayout(false);
            this.PerformLayout();

            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "NiceApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            if (betterFormPosition != null)
            {
                string[] sp = betterFormPosition.Split(new char[] { ',' });
                SetSizeAndLocation(new Size(Int32.Parse(sp[0]), Int32.Parse(sp[1])), new Point(Int32.Parse(sp[2]), Int32.Parse(sp[3])), new Padding(2));
            }
            else if (onEdge)
            {
                SetSizeAndLocation(new Size(400, 150), new Point(0, 200), new Padding(2));
            }
            else
            {
                SetSizeAndLocation(new Size(400, 150), new Point(375, 200), new Padding(2));
            }
            OnShow(null, null);
        }

        private void SetSizeAndLocation(Size childSize, Size parentSize, Point parentLocation, Padding margine)
        {
            textBox_all.Location = new Point(margine.Top, margine.Left);
            textBox_all.Margin = margine;
            textBox_all.Size = childSize;
            Size = parentSize;
            Location = parentLocation;
            this.StartPosition = FormStartPosition.Manual;
        }
        private void SetSizeAndLocation(Size childSize, Point parentLocation, Padding margine)
        {
            SetSizeAndLocation(childSize, new Size(childSize.Width + margine.Horizontal, childSize.Height + margine.Vertical), parentLocation, margine);
        }

        #endregion

        public void UserAction_DisplayText(Data_DisplayText data)
        {
            if (InvokeRequired)
            {
                if (data.StopEvent.WaitOne(1, false) == false)
                {
                    Invoke(new d_UserAction_DisplayText(UserAction_DisplayText), data);
                }
            }
            else
            {
                if (data.ClearAll)
                {
                    this.textBox_all.Clear();
                }
                this.textBox_all.Text += data.Text;
            }
        }

        public void UserAction_SimpleKeyPress(char c)
        {
            if (InvokeRequired)
            {
                Invoke(new d_SimpleKeyPress(UserAction_SimpleKeyPress), c);
            }
            else
            {
                // special caracter handling s
                List<char> specialCharList = new List<char>() { '{', '}', '(', ')', '+', '^', '%', '~', '(', ')' };
                List<char> ignoredCharList = new List<char>() { '\t', '\v', '\r' };
                if (specialCharList.Contains(c))
                {
                    // this is a special character, so put it in brackets
                    SendKeys.SendWait("{" + c.ToString() + "}");
                }
                else if (ignoredCharList.Contains(c))
                {
                    // this is an ignored char, so do nothing
                }
                else if (c == '\n')
                {
                    SendKeys.SendWait("+{ENTER}");
                }
                else if (c == '\0')
                {
                    SendKeys.SendWait("{ENTER}");
                }
                else
                {
                    // a common case
                    SendKeys.SendWait(c.ToString());
                }
            }
        }

        private void OnShow(object sender, EventArgs e)
        {
            Visible = true;
            ShowInTaskbar = false;
            Focus();
            readyEvent.Set();
        }
        private void OnExit(object sender, EventArgs e)
        {
            //mg check this WorkerThread.Stop();
            Application.Exit();
        }
        private void SysTrayApp_Leave(object sender, EventArgs e)
        {
            Focus();
        }




    }

    public static class HideConsole
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void DoHide()
        {

            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            // Show
            //ShowWindow(handle, SW_SHOW);
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GreyMagic;
using WowWhisperer.Properties;

namespace WowWhisperer
{
    public partial class MainForm : Form
    {
        private static readonly IntPtr LocalPlayerKnownSpells = (IntPtr)0x010C8380 - 0x400000;

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const int VK_RETURN = 0x0D;
        private const int VK_TAB = 0x09;
        private const int VK_BACK = 0x08;

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [StructLayout(LayoutKind.Sequential)]
        private struct MarshalStruct
        {
            [MarshalAs(UnmanagedType.I4)]
            public readonly int Val1;
        }

        public int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }

        //public byte[] ReadMemory(int adress, int processSize, int processHandle)
        //{
        //    byte[] buffer = new byte[processSize];
        //    ReadProcessMemory(processHandle, adress, buffer, processSize, 0);
        //    return buffer;
        //}

        public Process process = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBoxWowDir.Text = Settings.Default.WorldOfWarcraftDir;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ////uint playerclass = wow.ReadUInt((uint)baseWoW + 0xDC9755);
            //ExternalProcessReader reader = new ExternalProcessReader(process);
            //IntPtr imgBase = reader.ImageBase;
            //SafeMemoryHandle handle = reader.ProcessHandle;
            ////reader.ReadBytes((uint)handle + 0xDC9755);
            //MarshalStruct _marshalStruct = reader.Read<MarshalStruct>(LocalPlayerKnownSpells, true);
            //MessageBox.Show(_marshalStruct.Val1.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            process = Process.Start(textBoxWowDir.Text);
            Thread.Sleep(300);

            ////ReadMemory(C79FD8, 50, process.MainWindowHandle);
            //uint baseAddress = (uint)process.MainModule.BaseAddress.ToInt32();
            //IntPtr readHandle = OpenProcess(0x0010, false, (uint)process.Id);
            //byte[] bytes = new byte[24];
            //uint rw = 0;
            //uint size = sizeof(int);

            //ReadProcessMemory(readHandle, ((IntPtr)baseAddress + 0x0C79FD8), bytes, (UIntPtr)24, ref rw);
            //string ownedcore = Encoding.UTF8.GetString(bytes);
            //ReadProcessMemory(readHandle, (IntPtr)baseAddress + 0x00528744, bytes, (UIntPtr)size, ref rw);
            //int someNumber = BitConverter.ToInt32(bytes, 0);
            //MessageBox.Show(ownedcore);
            //MessageBox.Show(someNumber.ToString());
            //return;

            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;

                    do
                    {
                        process.WaitForInputIdle();
                        process.Refresh();
                    }
                    while (process.MainWindowHandle.ToInt32() == 0);

                    Thread.Sleep(800);

                    for (int i = 0; i < textBoxAccountName.Text.Length; i++)
                    {
                        PostMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(textBoxAccountName.Text[i]), IntPtr.Zero);
                        Thread.Sleep(30);
                    }

                    PostMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_TAB), IntPtr.Zero);

                    for (int i = 0; i < textBoxAccountPassword.Text.Length; i++)
                    {
                        PostMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(textBoxAccountPassword.Text[i]), IntPtr.Zero);
                        Thread.Sleep(30);
                    }

                    for (int i = 0; i < 2; ++i)
                    {
                        //! Login to account and enter game on selected character
                        PostMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        PostMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                        Thread.Sleep(1800); //! Time it takes on average to connect
                    }

                    Thread.CurrentThread.Abort();
                }
                catch
                {
                    Thread.CurrentThread.Abort();
                }
            }).Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (process == null)
            {
                DialogResult result = MessageBox.Show("You did not launch an instance of World of Warcraft from the application. Do you wish to select a running instance?", "Do you want to select a process?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                using (SearchForProcessForm searchForProcessForm = new SearchForProcessForm())
                    if (searchForProcessForm.ShowDialog(this) != DialogResult.OK)
                        return;

                if (process == null)
                {
                    MessageBox.Show("The selected process could not be found!", "Process not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;

                    do
                    {
                        process.WaitForInputIdle();
                        process.Refresh();
                    }
                    while (process.MainWindowHandle.ToInt32() == 0);

                    Thread.Sleep(800);

                    PostMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                    PostMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                    Thread.Sleep(30);

                    for (int i = 0; i < 10; ++i)
                    {
                        string whoString = "/who 7" + i;

                        for (int j = 0; j < whoString.Length; ++j)
                        {
                            PostMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(whoString[j]), IntPtr.Zero);
                            Thread.Sleep(30);
                        }

                        PostMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        PostMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                        Thread.Sleep(1000);

                        Thread.Sleep(12000); //! Wait 12 seconds
                    }

                    Thread.CurrentThread.Abort();
                }
                catch
                {
                    Thread.CurrentThread.Abort();
                }
            }).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.WorldOfWarcraftDir = textBoxWowDir.Text;
            Settings.Default.AccountName = textBoxAccountName.Text;
            Settings.Default.AccountPassword = textBoxAccountPassword.Text;
            Settings.Default.Save();
        }
    }
}

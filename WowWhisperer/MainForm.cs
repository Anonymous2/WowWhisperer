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
using WowWhisperer.Properties;

namespace WowWhisperer
{
    public partial class MainForm : Form
    {
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_CHAR = 0x0102;
        private const int VK_RETURN = 0x0D;
        private const int VK_TAB = 0x09;
        private const int VK_BACK = 0x08;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, ref UInt32 lpNumberOfBytesRead);

        struct WhoResult
        {
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string Name;
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 96)]
            public string GuildName;
            public int Level;
            public int Race;
            public int Class;
            public int Gender;
            public int Area;
        }

        public Process process = null;
        private bool cancelWhispers = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBoxWowDir.Text = Settings.Default.WorldOfWarcraftDir;
            textBoxWhisperMessage.Text = Settings.Default.WhisperMessage;
            textBoxAccountName.Text = Settings.Default.AccountName;
            textBoxAccountPassword.Text = Settings.Default.AccountPassword;
        }

        public byte[] ReadBytes(IntPtr hnd, IntPtr Pointer, int Length)
        {
            byte[] Arr = new byte[Length];
            uint Bytes = 0;
            ReadProcessMemory(hnd, Pointer, Arr, (UIntPtr)Length, ref Bytes);
            return Arr;
        }

        T ByteArrayToStructure<T>(byte[] bytes) where T: struct 
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }

        private void buttonLaunchWow_Click(object sender, EventArgs e)
        {
            TryToStartWow();
        }

        private void TryToStartWow()
        {
            if (!File.Exists(textBoxWowDir.Text))
            {
                MessageBox.Show("The WoW directory could not be found!", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                process = Process.Start(textBoxWowDir.Text);
            }
            catch
            {
                return;
            }

            Thread.Sleep(300);

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
                        SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(textBoxAccountName.Text[i]), IntPtr.Zero);
                        Thread.Sleep(30);
                    }

                    SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_TAB), IntPtr.Zero);

                    for (int i = 0; i < textBoxAccountPassword.Text.Length; i++)
                    {
                        SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(textBoxAccountPassword.Text[i]), IntPtr.Zero);
                        Thread.Sleep(30);
                    }

                    for (int i = 0; i < 2; ++i)
                    {
                        //! Login to account and enter game on selected character
                        SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                    }

                    Thread.CurrentThread.Abort();
                }
                catch
                {
                    Thread.CurrentThread.Abort();
                }
            }).Start();
        }

        private void buttonCancelWhispers_Click(object sender, EventArgs e)
        {
            cancelWhispers = true;
            buttonCancelWhispers.Enabled = false;
            buttonPerformWhisperer.Enabled = true;
        }

        private void buttonPerformWhisperer_Click(object sender, EventArgs e)
        {
            if (process == null)
            {
                Process[] processes = Process.GetProcessesByName("Wow");

                if (processes.Length > 1)
                {
                    DialogResult result = MessageBox.Show("You did not launch an instance of World of Warcraft from the application. Do you wish to select a running instance?", "Do you want to select a process?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;

                    using (SearchForProcessForm searchForProcessForm = new SearchForProcessForm())
                        if (searchForProcessForm.ShowDialog(this) != DialogResult.OK)
                            return;
                }
                else if (processes.Length == 1)
                    process = processes[0];
                else
                {
                    DialogResult result = MessageBox.Show("There is no instance of Wow running at the given moment! Do you want to launch one?", "Process not found!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        return;

                    TryToStartWow();
                }

                if (process == null)
                {
                    MessageBox.Show("The selected process could not be found!", "Process not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            buttonCancelWhispers.Enabled = true;
            buttonPerformWhisperer.Enabled = false;

            new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;

                    SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                    SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                    Thread.Sleep(30);

                    for (int i = 0; i < 10; ++i)
                    {
                        string whoString = "/who 7" + i;

                        for (int j = 0; j < whoString.Length; ++j)
                        {
                            SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(whoString[j]), IntPtr.Zero);
                            Thread.Sleep(30);
                        }

                        if (cancelWhispers)
                            goto FinishedWhispering;

                        SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                        SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                        Thread.Sleep(500);

                        if (cancelWhispers)
                            goto FinishedWhispering;

                        byte[] numWhosBytes = ReadBytes(process.Handle, process.MainModule.BaseAddress + 0x87BFE0, 4);
                        uint numWhos = BitConverter.ToUInt32(numWhosBytes, 0);

                        int size = Marshal.SizeOf(typeof(WhoResult));
                        byte[] bytes = ReadBytes(process.Handle, ((IntPtr)process.MainModule.BaseAddress + 0x879FD8), ((int)numWhos * size));
                        List<byte> byteList = new List<byte>(bytes);

                        for (int j = 0; j < numWhos; ++j)
                        {
                            WhoResult whoResult = ByteArrayToStructure<WhoResult>(byteList.GetRange(j * size, size).ToArray());
                            string whisperStr = textBoxWhisperMessage.Text.Replace("_name_", whoResult.Name);

                            for (int x = 0; x < whisperStr.Length; ++x)
                            {
                                SendMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(whisperStr[x]), IntPtr.Zero);
                                Thread.Sleep(20);
                            }

                            if (cancelWhispers)
                                goto FinishedWhispering;

                            SendMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                            SendMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                            Thread.Sleep(20); //! 30 ms delay before we go to next whisper
                        }

                        Thread.Sleep(12000); //! Wait 12 seconds
                    }

                FinishedWhispering:
                    Thread.CurrentThread.Abort();
                    cancelWhispers = false;
                    buttonCancelWhispers.Enabled = false;
                    buttonPerformWhisperer.Enabled = true;
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
            Settings.Default.WhisperMessage = textBoxWhisperMessage.Text;
            Settings.Default.Save();
        }
    }
}

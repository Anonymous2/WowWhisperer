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

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }

        public struct MODULEENTRY32
        {
            public uint dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public IntPtr modBaseAddr;
            public uint modBaseSize;
            public IntPtr hModule;
            public string szModule;
            public string szExePath;
        }

        private const int TH32CS_SNAPPROCESS = 0x00000002;
        private const int TH32CS_SNAPMODULE = 0x00000008;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

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

        private void button1_Click(object sender, EventArgs e)
        {
            process = Process.Start(textBoxWowDir.Text);
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
                    MessageBox.Show("There is no instance of Wow running at the given moment!", "Process not found!", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                                PostMessage(process.MainWindowHandle, WM_CHAR, new IntPtr(whisperStr[x]), IntPtr.Zero);
                                Thread.Sleep(20);
                            }

                            PostMessage(process.MainWindowHandle, WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);
                            PostMessage(process.MainWindowHandle, WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
                            Thread.Sleep(20); //! 30 ms delay before we go to next whisper
                        }

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

using MouseCapture.Models;
using Serilog;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace MouseCapture.Services
{
    public sealed class MouseHookService : IDisposable
    {
        private IntPtr _hookHandle = IntPtr.Zero;
        private MouseHookMethods.LowLevelMouseProc _proc;
        private GCHandle _procHandle;
        private readonly Dispatcher _dispatcher;

        public event EventHandler<MouseEventInfo>? MouseEventReceived;

        public bool IsRunning => _hookHandle != IntPtr.Zero;

        public MouseHookService() : this(Dispatcher.CurrentDispatcher) { }

        public MouseHookService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _proc = HookCallback;
            _procHandle = GCHandle.Alloc(_proc);
        }

        public void Start()
        {
            try
            {
                if (IsRunning) return;

                var moduleName = Process.GetCurrentProcess().MainModule?.ModuleName;
                IntPtr hMod = IntPtr.Zero;
                if (!string.IsNullOrEmpty(moduleName))
                {
                    hMod = MouseHookMethods.GetModuleHandle(moduleName);
                }

                _hookHandle = MouseHookMethods.SetWindowsHookEx(MouseHookMethods.WH_MOUSE_LL, _proc, hMod, 0);
                if (_hookHandle == IntPtr.Zero)
                {
                    ThrowLastWin32Error("SetWindowsHookEx failed");
                }
                Log.Information("Mouse hook started.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to start mouse hook.");
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (!IsRunning) return;
                MouseHookMethods.UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;
                Log.Information("Mouse hook stopped.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to stop mouse hook.");
                throw;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && lParam != IntPtr.Zero)
                {
                    if (wParam == (IntPtr)MouseHookMethods.WM_LBUTTONDOWN)
                    {
                        var hookStruct = Marshal.PtrToStructure<MouseHookMethods.MOUSEHOOKSTRUCT>(lParam);

                        IntPtr hWnd = MouseHookMethods.WindowFromPoint(hookStruct.pt);

                        uint processId;
                        MouseHookMethods.GetWindowThreadProcessId(hWnd, out processId);

                        try
                        {
                            using (Process targetProcess = Process.GetProcessById((int)processId))
                            {
                                var info = new MouseEventInfo
                                {
                                    X = hookStruct.pt.X,
                                    Y = hookStruct.pt.Y,
                                    MouseData = hookStruct.mouseData,
                                    Flags = hookStruct.flags,
                                    Time = hookStruct.time,
                                    Message = $"{nameof(MouseHookMethods.WM_LBUTTONDOWN)}, " +
                                    $"ProcessName: {targetProcess.ProcessName}, PID: {processId} ",
                                };

                                // Raise on UI dispatcher so subscribers can update UI collections safely
                                _dispatcher.BeginInvoke(new Action(() =>
                                {
                                    MouseEventReceived?.Invoke(this, info);
                                }));
                                Log.Information($"Mouse Left button click captured on process: {targetProcess.ProcessName} (PID: {targetProcess.Id})");
                            }

                        }
                        catch (ArgumentException ex)
                        {
                            Log.Error(ex, "Could not find process with ID {processId}", processId);
                        }
                    }
                }
            }
            catch
            {
                // swallow to avoid breaking the hook chain
                Log.Error("Error in mouse hook callback.");
            }

            return MouseHookMethods.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        private static void ThrowLastWin32Error(string message)
        {
            var err = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"{message}: {err}");
        }

        public void Dispose()
        {
            Stop();
            if (_procHandle.IsAllocated) _procHandle.Free();
            GC.SuppressFinalize(this);
        }
    }
}
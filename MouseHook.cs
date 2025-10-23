using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Serilog;

namespace MouseCapture
{
    public class MouseHook
    {
        private static IntPtr _hookID = IntPtr.Zero;
        private static MouseHookMethods.LowLevelMouseProc _mouseHookProc;

        public static void StartMouseCapture()
        {
            try
            {
                _mouseHookProc = HookCallback;
                using (Process curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        _hookID = MouseHookMethods.SetWindowsHookEx(MouseHookMethods.WH_MOUSE_LL, _mouseHookProc, MouseHookMethods.GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while starting mouse capture.");
            }
        }

        public static void StopMouseCapture()
        {
            try
            {
                MouseHookMethods.UnhookWindowsHookEx(_hookID);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while stopping mouse capture.");
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr)MouseHookMethods.WM_LBUTTONDOWN) 
                {
                    MouseHookMethods.MOUSEHOOKSTRUCT hs = (MouseHookMethods.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MouseHookMethods.MOUSEHOOKSTRUCT));

                    // Get the window handle at the mouse click point  
                    IntPtr hWnd = MouseHookMethods.WindowFromPoint(hs.pt);

                    // Get the process ID associated with the window  
                    uint processId;
                    MouseHookMethods.GetWindowThreadProcessId(hWnd, out processId);

                    try
                    {
                        using (Process targetProcess = Process.GetProcessById((int)processId))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MainWindow.MouseActions.Add(new Models.MouseAction
                                {
                                    ActionOn = DateTime.Now.ToLocalTime().ToString(),
                                    LMouseAction = nameof(MouseHookMethods.WM_LBUTTONDOWN),
                                    ApplicationName = targetProcess.ProcessName,
                                    PID = targetProcess.Id
                                });
                                Log.Information($"Mouse Left button click captured on process: {targetProcess.ProcessName} (PID: {targetProcess.Id})");
                            });
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Log.Error($"Could not find process with ID {processId}: {ex.Message}");
                    }
                }
                return MouseHookMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Log.Error("Error in HookCallback: " + ex.Message);
                return MouseHookMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }
    }
}

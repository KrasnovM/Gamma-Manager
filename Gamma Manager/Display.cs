using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Gamma_Manager
{
    internal class Display
    {
        #region Classes
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        public class DisplayInfo
        {
            public int numDisplay;
            public string displayName;
            public string displayLink;
            public bool isExternal;
            public IntPtr PhysicalHandle;
            public float rGamma = 1.0f;
            public float gGamma = 1.0f;
            public float bGamma = 1.0f; 
            public float rContrast = 1.0f; 
            public float gContrast = 1.0f; 
            public float bContrast = 1.0f;
            public float rBright = 0.0f;
            public float gBright = 0.0f;
            public float bBright = 0.0f;
            public int monitorBrightness;
            public int monitorContrast;
        }
        #endregion

        #region DllImport
        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        #endregion

        public static List<DisplayInfo> QueryDisplayDevices()
        {
            List<DisplayInfo> monitors = new List<DisplayInfo>();

            WinApi.MonitorEnumDelegate MonitorEnumProc = new WinApi.MonitorEnumDelegate(
            (IntPtr hMonitor, IntPtr hdcMonitor, ref WinApi.RECT lprcMonitor, IntPtr dwData) =>
            {
                WinApi.MONITORINFOEX monitorInfo = new WinApi.MONITORINFOEX() { Size = Marshal.SizeOf(typeof(WinApi.MONITORINFOEX)) };

                DisplayInfo monitor = new DisplayInfo();
                if (WinApi.GetMonitorInfo(hMonitor, ref monitorInfo))
                {
                    WinApi.DISPLAY_DEVICE device = new WinApi.DISPLAY_DEVICE();
                    device.Initialize();
                    if (WinApi.EnumDisplayDevices(monitorInfo.DeviceName.ToLPTStr(), 0, ref device, 0))
                    {
                        monitor.displayLink = monitorInfo.DeviceName;

                    }
                    string DName = device.DeviceID;
                    DName = DName.Substring(DName.IndexOf("\\") + 1);
                    DName = DName.Substring(0, DName.IndexOf("\\"));
                    monitor.displayName = DName;

                    /*Console.WriteLine("Left: " + lprcMonitor.Left);
                    Console.WriteLine("Right: " + lprcMonitor.Right);
                    Console.WriteLine("Top: " + lprcMonitor.Top);
                    Console.WriteLine("Bottom: " + lprcMonitor.Bottom);*/

                }
                for (int i = 0; i < monitors.Count; i++)
                {

                }

                uint physicalMonitorsCount = 0;

                if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount))
                {
                    // Cannot get monitor count
                    return true;
                }

                var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
                if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors))
                {
                    // Cannot get physical monitor handle
                    return true;
                }

                foreach (PHYSICAL_MONITOR physicalMonitor in physicalMonitors)
                {

                    uint minValue = 0, currentValue = 0, maxValue = 0;
                    if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
                    {
                        monitor.isExternal = false;
                        monitor.PhysicalHandle = (IntPtr)(-1);
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                        continue;
                    }
                    else
                    {
                        monitor.isExternal = true;
                    }

                    monitor.PhysicalHandle = physicalMonitor.hPhysicalMonitor;

                }
                if (monitor.isExternal)
                {
                    monitor.monitorBrightness = ExternalBrightness.GetBrightness(monitor.PhysicalHandle);
                    monitor.monitorContrast = ExternalBrightness.GetContrast(monitor.PhysicalHandle);
                }
                else
                {
                    monitor.monitorBrightness = InternalBrightness.GetBrightness();
                    monitor.monitorContrast = -1;
                }
                monitors.Add(monitor);
                return true;
            });

            WinApi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
            return monitors;
        }

        /*public static void DisposeMonitors(IEnumerable<DisplayInfo> monitors)
        {
            if (monitors?.Any() == true)
            {
                PHYSICAL_MONITOR[] monitorArray = monitors.Select(m => new PHYSICAL_MONITOR { hPhysicalMonitor = m.PhysicalHandle }).ToArray();
                DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
            }
        }*/

    }
}

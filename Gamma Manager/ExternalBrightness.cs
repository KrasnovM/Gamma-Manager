using System;
using System.Runtime.InteropServices;

namespace Gamma_Manager
{
    internal class ExternalBrightness
    {
        #region DllImport
        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorContrast")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorContrast(IntPtr handle, ref uint minimumContrast, ref uint currentContrast, ref uint maxContrast);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorContrast")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorContrast(IntPtr handle, uint newContrast);
        #endregion

        #region Get & Set
        public static void SetBrightness(IntPtr hPhysicalMonitor, uint brightness)
        {
            uint realNewValue = 100 * brightness / 100;
            SetMonitorBrightness(hPhysicalMonitor, realNewValue);
        }

        public static int GetBrightness(IntPtr hPhysicalMonitor)
        {
            uint min = 0;
            uint cur = 0;
            uint max = 0;

            GetMonitorBrightness(hPhysicalMonitor, ref min, ref cur, ref max);

            return (int)cur;
        }

        public static void SetContrast(IntPtr hPhysicalMonitor, uint contrast)
        {
            uint realNewValue = 100 * contrast / 100;
            SetMonitorContrast(hPhysicalMonitor, realNewValue);
        }

        public static int GetContrast(IntPtr hPhysicalMonitor)
        {
            uint min = 0;
            uint cur = 0;
            uint max = 0;

            GetMonitorContrast(hPhysicalMonitor, ref min, ref cur, ref max);

            return (int)cur;
        }
        #endregion
    }
}

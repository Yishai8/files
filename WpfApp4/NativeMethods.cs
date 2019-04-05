using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    class NativeMethods
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "CreateFileW")]
        public static extern System.IntPtr CreateFileW(
      [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPWStr)] string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      [InAttribute()] System.IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      [InAttribute()] System.IntPtr hTemplateFile
  );

        public const int GENERIC_WRITE = 1073741824;

        /// FILE_SHARE_DELETE -> 0x00000004
        public const int FILE_SHARE_DELETE = 4;

        /// FILE_SHARE_WRITE -> 0x00000002
        public const int FILE_SHARE_WRITE = 2;

        /// FILE_SHARE_READ -> 0x00000001
        public const int FILE_SHARE_READ = 1;

        /// OPEN_ALWAYS -> 4
        public const int OPEN_ALWAYS = 4;
    
}
}

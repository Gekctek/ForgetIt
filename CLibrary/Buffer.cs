using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CLibrary
{
    [StructLayout(LayoutKind.Sequential)]
	public struct Buffer
    {   
        /// <summary>
        /// A pointer to the bytes
        /// </summary>
        public UIntPtr data;
        /// <summary>
        /// The amount of meaningful bytes
        /// </summary>
		[MarshalAs(UnmanagedType.U4)]
        public uint len;
        /// <summary>
        /// The total allocated memory `data` points to
        /// This is needed so Rust can free `data`
        /// </summary>
		[MarshalAs(UnmanagedType.U4)]
        public uint cap;
	}
}

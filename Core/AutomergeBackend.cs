using CLibrary;
using System;
using System.Text;

namespace Automerge
{

    public class AutomergeBackend : IDisposable
    {
        private IntPtr _backend;
        private IntPtr _buffer;
        private AutomergeBackend(IntPtr backend, IntPtr buffer)
        {
            _backend = backend;
            _buffer = buffer;
        }

        public int ApplyLocalChange(string request)
        {
            byte[] changes = Encoding.UTF8.GetBytes(request);
            UIntPtr changesLength = new UIntPtr(Convert.ToUInt32(changes.Length));
            return AutomergeLib.ApplyLocalChange(this._backend, this._buffer, changes, changesLength);
        }

        public void Dispose()
        {
            // TODO
        }

        public static AutomergeBackend Init()
        {
            IntPtr backend = AutomergeLib.Init();
            IntPtr buffer = AutomergeLib.CreateBuffer();
            return new AutomergeBackend(backend, buffer);
        }
    }
}
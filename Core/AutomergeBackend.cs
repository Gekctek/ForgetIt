using CLibrary;
using System;
using System.Text;
using Buffer = CLibrary.Buffer;

namespace Automerge
{

    public class AutomergeBackend : IDisposable, ICloneable
    {
        private IntPtr _backend;
        private Buffer _buffer;
        private AutomergeBackend(IntPtr backend, Buffer buffer)
        {
            _backend = backend;
            _buffer = buffer;
        }

        public void ApplyLocalChange(string request)
        {
            byte[] changes = Encoding.UTF8.GetBytes(request);
            UIntPtr changesLength = new UIntPtr(Convert.ToUInt32(changes.Length));
            IntPtr ptr = AutomergeLib.ApplyLocalChange(this._backend, this._buffer, changes, changesLength);
        }

        public void Dispose()
        {
            // TODO
        }

        public static AutomergeBackend Init()
        {
            IntPtr backend = AutomergeLib.Init();
            Buffer buffer = AutomergeLib.CreateBuffer();
            return new AutomergeBackend(backend, buffer);
        }

		public AutomergeBackend Clone()
		{
            AutomergeLib.Clone(this._backend, out IntPtr newBackend);
            Buffer buffer = AutomergeLib.CreateBuffer();
            return new AutomergeBackend(newBackend, buffer);
        }

		object ICloneable.Clone()
		{
            return Clone();
		}
	}
}
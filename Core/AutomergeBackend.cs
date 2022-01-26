using CLibrary;
using System;
using System.Text;
using ForgetIt.Core;
using Buffer = CLibrary.Buffer;
using System.Text.Json;

namespace Automerge
{

    public class AutomergeBackend : IDisposable, ICloneable
    {
        private IntPtr _backend;
        private Buffer _buffer;
        private IntPtr _syncState;
        private AutomergeBackend(IntPtr backend, Buffer buffer, IntPtr syncState)
        {
            _backend = backend;
            _buffer = buffer;
            _syncState = syncState;
        }

        public void ApplyLocalChange(Change change)
        {
            byte[] changeBytes = Serialize(change);
            UIntPtr changesLength = new (Convert.ToUInt32(changeBytes.Length));
            IntPtr ptr = AutomergeLib.ApplyLocalChange(this._backend, this._buffer, changeBytes, changesLength);
            CheckError(ptr);
        }

        public byte[] Serialize<T>(T obj)
        {
            var options = new JsonSerializerOptions(); // TODO
            return JsonSerializer.SerializeToUtf8Bytes(obj, options);
        }

        public void Dispose()
        {
            AutomergeLib.DisposeBuffer(this._buffer);
            AutomergeLib.DisposeSyncState(this._syncState);
            AutomergeLib.DisposeBackend(this._backend);
        }

        private void CheckError(IntPtr intPtr)
        {
            if (intPtr.ToInt32() <= -1)
            {
                string errorMessage = AutomergeLib.GetLastErrorMessage(this._backend);
                throw new Exception(errorMessage);
            }
        }

        public static AutomergeBackend Init()
        {
            IntPtr backend = AutomergeLib.Init();
            Buffer buffer = AutomergeLib.CreateBuffer();
            IntPtr syncState = AutomergeLib.SyncStateInit();
            return new AutomergeBackend(backend, buffer, syncState);
        }

		public AutomergeBackend Clone()
		{
            AutomergeLib.Clone(this._backend, out IntPtr newBackend);
            Buffer buffer = AutomergeLib.CreateBuffer();
            IntPtr syncState = AutomergeLib.SyncStateInit();
            return new AutomergeBackend(newBackend, buffer, syncState);
        }

		object ICloneable.Clone()
		{
            return Clone();
		}
	}
}
using System;
using System.Runtime.InteropServices;

namespace CLibrary
{
	public static class AutomergeLib
	{
#if _WINDOWS
		private const string automergeLibPath = "libautomerge.dll";
#else
		private const string automergeLibPath = "libautomerge.so";
#endif
		/// <summary>
		/// Initialized the backend instance
		/// </summary>
		/// <returns>Instance of a backend</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_init")]
		public unsafe static extern IntPtr Init();

		/// <summary>
		/// Applies the supplied change locally
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>Length of the result written to the buffer</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_apply_local_change", CharSet = CharSet.Ansi)]
		public static extern IntPtr ApplyLocalChange(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			byte[] changes,
			UIntPtr changeLength);

		/// <summary>
		/// Clones the automerge backend
		/// </summary>
		/// <param name="backend">Current instance of the automerge backend</param>
		/// <param name="newBackend">The cloned automerge backend</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_clone")]
		public static extern IntPtr Clone(IntPtr backend, out IntPtr newBackend);

		/// <summary>
		/// Creates a buffer to store return values
		/// </summary>
		/// <returns>The buffer to use for requests</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_create_buff")]
		[return: MarshalAs(UnmanagedType.Struct)]
		public static extern Buffer CreateBuffer();

		/// <summary>
		/// Decodes the change TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_decode_change")]
		public static extern IntPtr DecodeChange(IntPtr backend, Buffer buffer, byte[] change, UIntPtr changeLength);

		/// <summary>
		/// Decodes the Sync State TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="encodedState">Encoded state bytes TODO</param>
		/// <param name="encodedStateLength">Length of the encoded state bytes</param>
		/// <param name="syncState">TODO</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_decode_sync_state")]
		public static extern IntPtr DecodeSyncState(
			IntPtr backend,
			IntPtr encodedState,
			UIntPtr encodedStateLength,
		    out IntPtr syncState);

		/// <summary>
		/// Encodes specified json change
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_encode_change")]
		public static extern IntPtr EncodeChange(
			IntPtr backend,
			Buffer buffer,
			byte[] change,
			UIntPtr changeLength);



		/// <summary>
		/// Encodes the Sync State TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <param name="syncState">TODO</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_encode_sync_state")]
		public static extern IntPtr EncodeSyncState(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			IntPtr syncState);


		/// <summary>
		/// Gets the last error message
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <returns>Last error message</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_error", CharSet = CharSet.Auto)]
		public static extern string GetLastErrorMessage(IntPtr backend);

		/// <summary>
		/// Dipose of the backend and frees its resources
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		[DllImport(automergeLibPath, EntryPoint = "automerge_free")]
		public static extern void DisposeBackend(IntPtr backend);

		/// <summary>
		/// Dipose of the buffer and frees its resources
		/// </summary>
		/// <param name="buffer">Buffer to dispose</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_free_buff")]
		public static extern IntPtr DisposeBuffer([MarshalAs(UnmanagedType.Struct)]Buffer backend);


		/// <summary>
		/// Generates sync message TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <param name="syncState">TODO</param>
		/// <returns>Length of bytes written</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_generate_sync_message")]
		public static extern IntPtr GenerateSyncMessage(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			IntPtr syncState);

		/// <summary>
		/// Get changes TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <param name="bytes">TODO</param>
		/// <param name="hashes">TODO</param>
		/// <returns>Length of bytes written</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_changes")]
		public static extern IntPtr GetChanges(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			byte[] bytes,
			IntPtr hashes);

		/// <summary>
		/// Get changes for a sepcified actor TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <param name="actor">The id of the actor to get changes for</param>
		/// <returns>Length of bytes written</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_changes_for_actor")]
		public static extern IntPtr GetChangesForActor(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			string actor);

		/// <summary>
		/// Get heads TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <returns>Length of bytes written</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_heads")]
		public static extern IntPtr GetHeads(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer);

		/// <summary>
		/// Get last local change TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write results to</param>
		/// <returns>Length of bytes written</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_last_local_change")]
		public static extern IntPtr GetLastLocalChange(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer);

		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_missing_deps")]
		public static extern IntPtr GetMissingDeps(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer,
			byte[] change,
			UIntPtr changeLength);


		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <returns>0 if successful, -1 if error</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_get_patch")]
		public static extern IntPtr GetPatch(
			IntPtr backend,
			[MarshalAs(UnmanagedType.Struct)] Buffer buffer);


		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="bytes">TODO</param>
		/// <param name="bytesLength">Length of the bytes</param>
		/// <returns>Backend instance</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_load")]
		public static extern IntPtr Load(byte[] bytes, UIntPtr bytesLength);


		/// <summary>
		/// Load changes from bytes TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="changes">TODO</param>
		/// <param name="changesLength">Length of the changes bytes</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_load_changes")]
		public static extern IntPtr LoadChanges(IntPtr backend, byte[] changes, UIntPtr changesLength);

		/// <summary>
		/// Receive sync message TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="syncState">TODO</param>
		/// <param name="encodedMessage">Bytes of the encoded message</param>
		/// <param name="changesLength">Length of the encoded message bytes</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_receive_sync_message")]
		public static extern IntPtr ReceiveSyncMessage(
			IntPtr backend, 
			[MarshalAs(UnmanagedType.Struct)]Buffer buffer,
			IntPtr syncState,
			byte[] encodedMessage,
			UIntPtr encodedMessageLength);


		/// <summary>
		/// Save TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_save")]
		public static extern IntPtr Save(
			IntPtr backend, 
			[MarshalAs(UnmanagedType.Struct)]Buffer buffer);

		/// <summary>
		/// Dispose sync state and free its resources
		/// </summary>
		/// <param name="syncState">Sync state to dispose</param>
		[DllImport(automergeLibPath, EntryPoint = "automerge_sync_state_free")]
		public static extern void DisposeSyncState(IntPtr syncState);
		
		/// <summary>
		/// Initialize new sync state to use
		/// </summary>
		/// <param name="syncState">Sync state to dispose</param>
		/// <returns>New sync state instace</returns>
		[DllImport(automergeLibPath, EntryPoint = "automerge_sync_state_init")]
		public static extern IntPtr SyncStateInit();
				
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="change">Json change</param>
		/// <param name="msgpack">Msgpack converted from json</param>
		/// <param name="msgpackLength">Msgpack byte length</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "debug_json_change_to_msgpack")]
		public static extern IntPtr DebugJsonChangeToMsgpack(
			string change,
			out byte[] msgpack,
			out UIntPtr msgpackLength
		);	
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="msgpack">TODO</param>
		/// <param name="msgpackLength">Msgpack byte length</param>
		/// <param name="json">Json converted from from msgpack</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "debug_msgpack_change_to_json")]
		public static extern IntPtr DebugMsgpackChangeToJson(
			out byte[] msgpack,
			out UIntPtr msgpackLength,
			out byte[] json
		);
		
		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="prefix">TODO</param>
		/// <param name="buffer">TODO</param>
		/// <param name="bufferLength">Length of the buffer in bytes</param>
		/// <returns>TODO</returns>
		[DllImport(automergeLibPath, EntryPoint = "debug_print_msgpack_patch")]
		public static extern void DebugPrintMsgpackPatch(
			string[] prefix,
			byte[] buffer,
			UIntPtr bufferLength
		);
	}
}
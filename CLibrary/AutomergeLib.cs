using System;
using System.Runtime.InteropServices;

namespace CLibrary
{
	public static class AutomergeLib
	{
		private const string autmergeLibPath = "automerge/libautomerge.so";

		/// <summary>
		/// Initialized the backend instance
		/// </summary>
		/// <returns>Instance of a backend</returns>
		[DllImport(autmergeLibPath, EntryPoint = "automerge_init")]
		public unsafe static extern IntPtr Init();

		/// <summary>
		/// Applies the supplied change locally
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffer">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>Length of the result written to the buffer</returns>
		[DllImport(autmergeLibPath, EntryPoint = "automerge_apply_local_change", CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.SysInt)] // Convert isize to int
		public static extern int ApplyLocalChange(IntPtr backend, IntPtr buffer, byte[] changes, UIntPtr changeLength);

		/// <summary>
		/// Clones the automerge backend
		/// </summary>
		/// <param name="backend">Current instance of the automerge backend</param>
		/// <param name="newBackend">The cloned automerge backend</param>
		/// <returns>0</returns>
		[DllImport(autmergeLibPath, EntryPoint = "automerge_clone")]
		[return: MarshalAs(UnmanagedType.SysInt)] // Convert isize to int
		public static extern int automerge_clone(IntPtr backend, out IntPtr newBackend);

		/// <summary>
		/// Creates a buffer to store return values
		/// </summary>
		/// <returns>The buffer to use for requests</returns>
		[DllImport("automerge/libautomerge.so", EntryPoint = "automerge_create_buff")]
		public static extern IntPtr CreateBuffer();

		/// <summary>
		/// Decodes the change TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="buffs">Buffer to write the response to</param>
		/// <param name="changes">Bytes of the changes in json</param>
		/// <param name="changeLength">Length of the changes bytes</param>
		/// <returns>0</returns>
		[DllImport("automerge/libautomerge.so", EntryPoint = "automerge_decode_change")]
		public static extern IntPtr DecodeChange(IntPtr backend, IntPtr buffs, byte[] changes, UIntPtr changeLength);

		/// <summary>
		/// Decodes the Sync State TODO
		/// </summary>
		/// <param name="backend">Current backend instance</param>
		/// <param name="encodedState">Encoded state bytes TODO</param>
		/// <param name="encodedStateLength">Length of the encoded state bytes</param>
		/// <param name="syncState">TODO</param>
		/// <returns></returns>
		[DllImport("automerge/libautomerge.so", EntryPoint = "automerge_decode_sync_state")]
		public static extern IntPtr DecodeSyncState(
			IntPtr backend,
			IntPtr encodedState,
			UIntPtr encodedStateLength,
		    out IntPtr syncState);

		//        /**
		//         * # Safety
		//         * This must me called with a valid pointer to a JSON string of a change
		//         */
		//        intptr_t automerge_encode_change(Backend* backend, Buffer* buffs, const uint8_t* change, uintptr_t len);

		///**
		// * # Safety
		// * Must be called with a pointer to a valid Backend, sync_state, and buffs
		// */
		//intptr_t automerge_encode_sync_state(Backend* backend, Buffer* buffs, SyncState* sync_state);

		//        /**
		//         * # Safety
		//         * This must be called with a valid backend pointer
		//         */
		//        const char* automerge_error(Backend * backend);

		//        /**
		//         * # Safety
		//         * This must be called with a valid backend pointer
		//         */
		//        void automerge_free(Backend* backend);

		//        /**
		//         * # Safety
		//         * Must point to a valid `Buffers` struct
		//         * Free the memory a `Buffers` struct points to
		//         */
		//        intptr_t automerge_free_buff(Buffer* buffs);

		//        /**
		//         * # Safety
		//         * Must be called with a valid backend pointer
		//         * sync_state must be a valid pointer to a SyncState
		//         * Returns an `isize` indicating the length of the binary message
		//         * (-1 if there was an error, 0 if there is no message)
		//         */
		//        intptr_t automerge_generate_sync_message(Backend* backend, Buffer* buffs, SyncState* sync_state);

		//        /**
		//         * # Safety
		//         * This must be called with a valid backend pointer,
		//         * binary must be a valid pointer to `hashes` hashes
		//         */
		//        intptr_t automerge_get_changes(Backend* backend, Buffer* buffs, const uint8_t* bin, uintptr_t hashes);

		///**
		// * # Safety
		// * This must be called with a valid pointer to a `Backend`
		// * and a valid C String
		// */
		//intptr_t automerge_get_changes_for_actor(Backend* backend, Buffer* buffs, const char* actor);

		//        /**
		//         * # Safety
		//         * This must be called with a valid backend pointer
		//         */
		//        intptr_t automerge_get_heads(Backend* backend, Buffer* buffs);

		//        /**
		//         * # Safety
		//         */
		//        intptr_t automerge_get_last_local_change(Backend* backend, Buffer* buffs);

		//        /**
		//         * # Safety
		//         * This must be called with a valid backend pointer,
		//         * binary must be a valid pointer to len bytes
		//         */
		//        intptr_t automerge_get_missing_deps(Backend* backend, Buffer* buffs, const uint8_t* bin, uintptr_t len);

		///**
		// * # Safety
		// * This should be called with a valid pointer to a `Backend`
		// * and a valid pointer to a `Buffers``
		// */
		//intptr_t automerge_get_patch(Backend* backend, Buffer* buffs);

		//        /**
		//         * # Safety
		//         * This must be called with a valid pointer to len bytes
		//         */
		//        Backend* automerge_load(const uint8_t* data, uintptr_t len);

		///**
		// * # Safety
		// * This should be called with a valid pointer to a `Backend`
		// * and a valid pointers to a `CBuffers`
		// */
		//intptr_t automerge_load_changes(Backend* backend, const uint8_t* changes, uintptr_t changes_len);

		///**
		// * # Safety
		// * Must be called with a valid backend pointer
		// * sync_state must be a valid pointer to a SyncState
		// * `encoded_msg_[ptr|len]` must be the address & length of a byte array
		// */
		//intptr_t automerge_receive_sync_message(Backend* backend,
		//                                        Buffer* buffs,
		//                                        SyncState* sync_state,
		//                                        const uint8_t* encoded_msg_ptr,
		//                                        uintptr_t encoded_msg_len);

		///**
		// * # Safety
		// * This should be called with a valid pointer to a `Backend`
		// */
		//intptr_t automerge_save(Backend* backend, Buffer* buffs);

		//        /**
		//         * # Safety
		//         * sync_state must be a valid pointer to a SyncState
		//         */
		//        void automerge_sync_state_free(SyncState* sync_state);

		//        SyncState* automerge_sync_state_init(void);

		//        /**
		//         * # Safety
		//         * This must be called with a valid C-string
		//         */
		//        intptr_t debug_json_change_to_msgpack(const char* change, uint8_t **out_msgpack, uintptr_t* out_len);

		//        /**
		//         * # Safety
		//         * This must be called with a valid pointer to len bytes
		//         */
		//        intptr_t debug_msgpack_change_to_json(const uint8_t* msgpack, uintptr_t len, uint8_t* out_json);

		//        /**
		//         * # Safety
		//         * `prefix` & `buff` must be valid pointers
		//         */
		//        void debug_print_msgpack_patch(const char* prefix, const uint8_t* buff, uintptr_t len);

	}
}
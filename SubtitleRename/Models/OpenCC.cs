using System.Runtime.InteropServices;


namespace SubtitleRename.Models
{
    internal partial class OpenCC : SafeHandle
    {
        /// <summary>
        /// Makes an instance of opencc
        /// </summary>
        /// <param name="configFileName">configuration file</param>
        /// <returns>
        /// A description pointer of the newly allocated instance of opencc.
        /// On error the  return value will be -1.
        /// </returns>
        [LibraryImport(
            libraryName: "opencc.dll",
            StringMarshalling = StringMarshalling.Utf8)]
        private static partial IntPtr opencc_open(string configFileName);

        /// <summary>
        /// Destroys an instance of opencc
        /// </summary>
        /// <param name="opencc">The description pointer.</param>
        /// <returns>
        /// 0 on success or non-zero number on failure.
        /// </returns>
        [LibraryImport("opencc.dll")]
        private static partial int opencc_close(IntPtr opencc);

        /// <summary>
        /// Converts UTF-8 string
        /// </summary>
        /// <param name="opencc">The opencc description pointer.</param>
        /// <param name="length">
        /// The maximum length in byte to convert. If value is -1,
        /// the whole string (terminated by '\0') will be converted.
        /// </param>
        /// <param name="utf8Buffer">
        /// The buffer to store converted text. You MUST make sure this
        /// buffer has sufficient space.
        /// </param>
        /// <returns>
        /// The length of converted string or -1 on error.
        /// </returns>
        [LibraryImport(
            libraryName: "opencc.dll",
            StringMarshalling = StringMarshalling.Utf8)]
        private static partial int opencc_convert_utf8_to_buffer(
            IntPtr opencc,
            string input,
            IntPtr length,
            [Out] byte[] utf8Buffer);

        /// <summary>
        /// You MUST call opencc_convert_utf8_free() to release allocated memory.
        /// </summary>
        /// <param name="opencc">The opencc description pointer.</param>
        /// <param name="input">The UTF-8 encoded string.</param>
        /// <param name="length">
        /// The maximum length in byte to convert. If length is -1,
        /// the whole string (terminated by '\0') will be converted.
        /// </param>
        /// <returns>
        /// The newly allocated UTF-8 string that stores text converted,
        /// or <see cref="IntPtr.Zero"> on error.
        /// </returns>
        [LibraryImport(libraryName: "opencc.dll",
            StringMarshalling = StringMarshalling.Utf8)]
        private static partial IntPtr opencc_convert_utf8(
            IntPtr opencc,
            string input,
            IntPtr length);

        /// <summary>
        /// Releases allocated buffer by opencc_convert_utf8
        /// </summary>
        /// <param name="text">
        /// Pointer to the allocated string buffer by opencc_convert_utf8.
        /// </param>
        [LibraryImport("opencc.dll")]
        private static partial void opencc_convert_utf8_free(IntPtr text);

        /// <summary>
        /// Returns the last error message
        /// </summary>
        [LibraryImport("opencc.dll")]
        public static partial IntPtr opencc_error();

        public string TraditionalToSimplify(string text)
        {
            using Result result = new(opencc_convert_utf8(handle, text, -1));
            return result.ToString();
        }

        public OpenCC() : base(-1, true) =>
            SetHandle(opencc_open("t2s.json"));
        public override bool IsInvalid => handle == -1;
        protected override bool ReleaseHandle() => opencc_close(handle) == 0;
        private class Result : IDisposable
        {
            private readonly IntPtr result;
            public Result(IntPtr ptr)
            {
                if (ptr == IntPtr.Zero) { throw new Exception("converter err"); }
                result = ptr;
            }
            public override string ToString() => Marshal.PtrToStringUTF8(result) ?? "";
            public void Dispose() => opencc_convert_utf8_free(result);
        }
    }
}

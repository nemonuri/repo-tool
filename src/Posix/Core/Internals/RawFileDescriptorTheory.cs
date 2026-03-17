/**
    ## References

    - [stdio.h](https://pubs.opengroup.org/onlinepubs/9799919799/basedefs/stdio.h.html)
    - [unistd.h](https://pubs.opengroup.org/onlinepubs/9799919799/basedefs/unistd.h.html)
    - [File descriptor](https://en.wikipedia.org/wiki/File_descriptor)
*/

namespace Nemonuri.Posix.Internals;

internal static class RawFileDescriptorTheory
{
    public static class Unistd
    {
#if MARKDOWN
        /**
            > The <unistd.h> header shall define the following symbolic constants for file streams:
        */
#endif

        /// <summary> File number of stderr; 2. </summary>
        public const nint STDERR_FILENO = 2;

        /// <summary> File number of stdin; 0. </summary>
        public const nint STDIN_FILENO = 0;

        /// <summary> File number of stdout; 1. </summary>
        public const nint STDOUT_FILENO = 1;

        /// <summary>
        /// isatty — test for a terminal device <br/>
        /// The isatty() function shall test whether fildes, an open file descriptor, is associated with a terminal device.
        /// </summary>
        /// <returns>
        /// The isatty() function shall return 1 if fildes is associated with a terminal; otherwise, it shall return 0 and may set errno to indicate the error.
        /// </returns>
        /// <remarks><a href="https://pubs.opengroup.org/onlinepubs/9799919799/functions/isatty.html">(doc)</a> </remarks>
        public static nint isatty(nint fildes)
        {
            return fildes switch
            {
                STDERR_FILENO or STDIN_FILENO or STDOUT_FILENO => 1,
                _ => 0
            };
        }
    }
}
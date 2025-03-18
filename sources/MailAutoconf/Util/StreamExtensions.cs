using System.Text;

namespace MailAutoconf.Util
{
    /// <summary>
    /// This class contains some extension methods for MemoryStream.
    /// </summary>
	public static class StreamExtensions
    {
        /// <summary>
        /// Returns the memory stream as string encoded using ASCII encoding.
        /// </summary>
        public static string GetString(this MemoryStream stream) => stream.GetString(Encoding.ASCII);

        /// <summary>
        /// Returns the content of the whole memory stream as string using the passed encoding.
        /// </summary>
        public static string GetString(this MemoryStream stream, Encoding encoding)
        {
            long positionBefore = stream.Position;
            stream.Position = 0;
            string result = encoding.GetString(stream.ToArray());
            stream.Position = positionBefore;
            return result;
        }

		/// <summary>
		/// Copies the content of the memory stream to the text writer. Processes the memory stream line
		/// per line and only passes lines to the text writer, if lineAccepted() returns true.
		/// </summary>
		/// <param name="stream">of the memory stream to be copied the text writer</param>
		/// <param name="writer">the text writer that is filled with the filtered content of the memory stream</param>
		/// <param name="lineAccepted">a predicate that returns true for all lines, that have to be copied from
        /// the memory stream to the text writer</param>
		public static void CopyTo(this MemoryStream stream, TextWriter writer, Predicate<string> lineAccepted)
        {
            long positionBefore = stream.Position;
            stream.Position = 0;

            using MemoryStream streamCopy = new();
            stream.CopyTo(streamCopy);
            streamCopy.Position = 0;

            using StreamReader reader = new(streamCopy);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (lineAccepted(line))
                {
                    writer.WriteLine(line);
                }
            }

            stream.Position = positionBefore;
        }
    }
}

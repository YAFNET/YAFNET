using System.IO;
using System.Text;

namespace ServiceStack.Text
{
    public class DirectStreamWriter : TextWriter
    {
        private const int optimizedBufferLength = 256;
        private const int maxBufferLength = 1024;
        
        private Stream stream;
        private StreamWriter writer;
        private byte[] curChar = new byte[1];
        private bool needFlush;

        public override Encoding Encoding { get; }

        public DirectStreamWriter(Stream stream, Encoding encoding)
        {
            this.stream = stream;
            this.Encoding = encoding;
        }

        public override void Write(string s)
        {
            if (s.IsNullOrEmpty())
                return;

            if (s.Length <= optimizedBufferLength)
            {
                if (this.needFlush) 
                {
                    this.writer.Flush();
                    this.needFlush = false;
                }

                var buffer = this.Encoding.GetBytes(s);
                this.stream.Write(buffer, 0, buffer.Length);
            } else 
            {
                if (this.writer == null) this.writer = new StreamWriter(this.stream, this.Encoding, s.Length < maxBufferLength ? s.Length : maxBufferLength);

                this.writer.Write(s);
                this.needFlush = true;
            }
        }

        public override void Write(char c)
        {
            if ((int)c < 128)
            {
                if (this.needFlush)
                {
                    this.writer.Flush();
                    this.needFlush = false;
                }

                this.curChar[0] = (byte)c;
                this.stream.Write(this.curChar, 0, 1);
            } else
            {
                if (this.writer == null) this.writer = new StreamWriter(this.stream, this.Encoding, optimizedBufferLength);

                this.writer.Write(c);
                this.needFlush = true;
            }
        }

        public override void Flush()
        {
            if (this.writer != null)
            {
                this.writer.Flush();
            }
            else
            {
                this.stream.Flush();
            }
        }
    }
}
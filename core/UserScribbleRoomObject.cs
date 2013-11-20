using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    class UserScribbleRoomObject
    {
        public uint Size { get; set; }
        public ushort Chunks { get; set; }
        public ushort ChunkCount { get; set; }
        public List<byte> DataIn { get; set; }
        public bool IsReceiving { get; set; }

        public void Reset()
        {
            this.Size = 0;
            this.Chunks = 0;
            this.ChunkCount = 0;

            if (this.DataIn != null)
                this.DataIn.Clear();

            this.DataIn = new List<byte>();
            this.IsReceiving = false;
        }

        public uint ReceivedCount
        {
            get { return (uint)this.DataIn.Count; }
        }
    }
}

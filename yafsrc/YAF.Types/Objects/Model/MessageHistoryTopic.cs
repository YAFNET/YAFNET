using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAF.Types.Objects.Model
{
    public class MessageHistoryTopic
    {
        public string EditReason { get; set; }

        public DateTime Edited { get; set; }

        public int EditedBy { get; set; }

        public int Flags { get; set; }

        public string IP { get; set; }

        public bool? IsModeratorChanged { get; set; }

        public int MessageID { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string UserStyle { get; set; }

        public DateTime? Suspended { get; set; }

        public int ForumID { get; set; }

        public int TopicID { get; set; }

        public string Topic { get; set; }

        public DateTime Posted { get; set; }

        public string MessageIP { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.Chat
{
    public class SendChatToUser
    {
        public string Content { get; set; }
        public int ToUserId { get; set; }
    }
}

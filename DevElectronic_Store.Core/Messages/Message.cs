﻿using System;

namespace DevElectronic_Store.Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; set; }
        public Guid AggregateId { get; set; }
        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}

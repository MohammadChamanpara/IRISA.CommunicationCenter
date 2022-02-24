﻿using IRISA.CommunicationCenter.Library.Adapters;
using System.Collections.Generic;

namespace IRISA.CommunicationCenter.Core
{
    public interface IIccCore
    {
        bool Started { get; }
        IIccQueue IccQueue { get; }
        string PersianDescription { get; }
        List<IIccAdapter> ConnectedAdapters { get; }
        void Stop();
        void Start();
    }
}

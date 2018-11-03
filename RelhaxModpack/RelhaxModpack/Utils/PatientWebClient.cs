﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RelhaxModpack
{
    public class PatientWebClient : WebClient
    {
        public const int TO_SECONDS = 1000;
        public const int TO_MINUETS = 60;
        public int Timeout { get; set; } = 5 * TO_SECONDS * TO_MINUETS;
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout;
            return w;
        }
    }
}
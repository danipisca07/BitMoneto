﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GestoriAPI
{
    public class EccezioneApi : Exception
    {
        public EccezioneApi()
        {

        }

        public EccezioneApi(String messaggio) : base(messaggio)
        {

        }
    }
}

﻿using System;
using System.Threading.Tasks;

namespace Terryberry.Http
{
    internal static class ExponentialBackoff
    {
        public static Task Sleep(double backoffFactor, int attempt)
        {
            return Task.Delay(TimeSpan.FromMilliseconds(Math.Pow(backoffFactor, attempt)));
        }
    }
}
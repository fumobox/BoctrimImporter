using UnityEngine;
using System.Collections;
using System;

namespace Boctrim.Domain
{

    public class BoctException : Exception
    {
        public int Code { get; private set;}

        public BoctException()
        {
        }

        public BoctException(string message) : base(message)
        {
        }

        public BoctException(string message, int code) : base(message)
        {
            Code = code;
        }
    }
}
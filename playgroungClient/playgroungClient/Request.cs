using System;
using System.Collections.Generic;
using System.Text;

namespace serverProject
{
    public static class Request
    {
        public const string REGISTR = "0000000";
        public const string LOGIN = "0000001";
        public const string OK = "0000010";
        public const string USER_ALREADY_EXIST = "0000011";
        public const string USER_DOES_NOT_EXIST = "0000100";
        public const string INVALID_PASSWORD = "0000101";
        public const string SENDING_FILE = "0000110";
    }
}

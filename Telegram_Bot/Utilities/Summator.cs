using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_Bot_Practice.Utilities
{
    public static class Summator
    {
        public static int TrySum(string str)
        {
            string[] substr = str.Split();
            int[] ints = new int[substr.Length];
            for (int i = 0; i < substr.Length; ++i)
            {
                ints[i] = int.Parse(substr[i]);
            }
            return ints.Sum();
        }
    }
}

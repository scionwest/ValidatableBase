using System;
using System.Collections.Generic;
using System.Text;

namespace SampleUniversalApp.Models
{
    public class Bank
    {
        public const decimal MinimumBalance = 2000M;

        public bool IsOpen { get; set; }
    }
}

using System;
using OpenQA.Selenium.Chrome;

namespace Instagram_Automat.Utils
{
    public class YKNChromeDriver : ChromeDriver
    {
        private static readonly Random Random = new Random();

        public YKNChromeDriver(ChromeOptions options) : base(options)
        {            
        }

        public void Scroll(int from, int to)
        {
            ExecuteScript($"window.scrollBy(0,{Random.Next(from, to)})");
        }
    }
}

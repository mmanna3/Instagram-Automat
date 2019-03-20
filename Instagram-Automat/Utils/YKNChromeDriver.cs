using System;
using System.Threading;
using Instagram_Automat.ExtensionMethods;
using OpenQA.Selenium;
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

	    public void ClickIfDisplayed(By by)
	    {
			if (this.IsElementDisplayed(by))
				FindElement(by).Click();
		}

	    public void ClickIfDisplayedAndWait(By by, int minSeconds, int maxSeconds)
	    {
		    if (this.IsElementDisplayed(by))
		    {
				FindElement(by).Click();
			    EsperarEntre(minSeconds, maxSeconds);
		    }			    
	    }

	    private static void EsperarEntre(int minSegundos, int maxSegundos)
	    {
		    Thread.Sleep(Random.Next(minSegundos, maxSegundos)*1000);
	    }
	}
}

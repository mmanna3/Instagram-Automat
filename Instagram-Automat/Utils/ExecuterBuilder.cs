using System;
using System.Threading;
using OpenQA.Selenium;

namespace Instagram_Automat.Utils
{
    public class ExecuterBuilder
    {
        private Action _methodToExecute;
        private Action _ifExceptionMethod;

        private static int _attemptsNumberBeforeCancel = 1;

        private static int _minWaitTimeBetweenAttemptsInSeconds = 3;
        private static int _maxWaitTimeBetweenAttemptsInSeconds = 6;

        private static int _minWaitTimeBeforeStart = 0;
        private static int _maxWaitTimeBeforeStart = 0;

        private int _minWaitTimeAfterExecution = 0;
        private int _maxWaitTimeAfterExecution = 0;

        private int _minWaitTimeIfException = 0;
        private int _maxWaitTimeIfException = 0;

        private static readonly Random Random = new Random();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ExecuterBuilder(Action methodToExecute, object[] parameters)
		{
            _methodToExecute = methodToExecute;
		}

        public ExecuterBuilder(Action methodToExecute)
        {
            _methodToExecute = methodToExecute;
        }

        public ExecuterBuilder IfException(Action methodToExecute)
        {
            _ifExceptionMethod = methodToExecute;
            return this;
        }

        public void Execute()
        {
            WaitBetween(_minWaitTimeBeforeStart, _maxWaitTimeBeforeStart);

            var seEjecutoCorrectamente = false;
            while (_attemptsNumberBeforeCancel > 0 && !seEjecutoCorrectamente)
            {
                try
                {
                    _methodToExecute.Invoke();
                    seEjecutoCorrectamente = true;
                }
                catch (Exception e)
                {
                    log.Warn($"Exception: {e.Message}");
                    log.Warn($"Attempt number: {_attemptsNumberBeforeCancel}");

                    _attemptsNumberBeforeCancel--;
                    WaitBetween(_minWaitTimeIfException, _maxWaitTimeIfException);

                    if (_ifExceptionMethod != null)
                        _ifExceptionMethod.Invoke();

                    WaitBetween(_minWaitTimeBetweenAttemptsInSeconds, _maxWaitTimeBetweenAttemptsInSeconds);
                }
            }

            WaitBetween(_minWaitTimeAfterExecution, _maxWaitTimeAfterExecution);
        }

        private void WaitBetween(int minWaitTimeInSeconds, int maxWaitTimeInSeconds)
        {
            Thread.Sleep(Random.Next(minWaitTimeInSeconds*1000, maxWaitTimeInSeconds*1000));
        }

        public ExecuterBuilder WaitTimeBetweenAttempts(int minInSeconds, int maxInSeconds)
        {
            _minWaitTimeBetweenAttemptsInSeconds = minInSeconds;
            _maxWaitTimeBetweenAttemptsInSeconds = maxInSeconds;
            return this;
        }

        public ExecuterBuilder WaitTimeBeforeStart(int minInSeconds, int maxInSeconds)
        {
            _minWaitTimeBeforeStart = minInSeconds;
            _maxWaitTimeBeforeStart = maxInSeconds;
            return this;
        }

        public ExecuterBuilder WaitTimeAfterExecution(int minInSeconds, int maxInSeconds)
        {
            _minWaitTimeAfterExecution = minInSeconds;
            _maxWaitTimeAfterExecution = maxInSeconds;
            return this;
        }

        public ExecuterBuilder WaitTimeIfException(int minInSeconds, int maxInSeconds)
        {
            _minWaitTimeIfException = minInSeconds;
            _maxWaitTimeIfException = maxInSeconds;
            return this;
        }

        public ExecuterBuilder AttemptsNumberBeforeCancel(int attemptsNumber)
        {
            _attemptsNumberBeforeCancel = attemptsNumber;
            return this;
        }
    }
}
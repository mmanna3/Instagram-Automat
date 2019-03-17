using System;
using System.Threading;

namespace Instagram_Automat.Utils
{
    public class ExecuterBuilder
    {
        private Action _methodToExecute;
        private object[] _methodToExecuteParams;

        private Action _ifExceptionMethod;
        private object[] _ifExceptionMethodParams;

        private static int _cantidadDeVecesAntesDeAbortar = 1;
        private static int _esperaMinimaEntreIntentosEnSegundos = 3;
        private static int _esperaMaximaEntreIntentosEnSegundos = 6;

        private static readonly Random Random = new Random();

        public ExecuterBuilder(Action methodToExecute, object[] parameters)
		{
            _methodToExecute = methodToExecute;
		}

        public ExecuterBuilder IfException(Action methodToExecute, object[] parameters)
        {
            _ifExceptionMethod = methodToExecute;
            _ifExceptionMethodParams = parameters;
            return this;
        }

        public void Execute()
        {
            var seEjecutoCorrectamente = false;
            while (_cantidadDeVecesAntesDeAbortar > 0 && seEjecutoCorrectamente)
            {
                try
                {
                    _methodToExecute.DynamicInvoke(_methodToExecuteParams);
                    seEjecutoCorrectamente = true;
                }
                catch (Exception)
                {
                    _cantidadDeVecesAntesDeAbortar--;
                    Thread.Sleep(Random.Next(_esperaMinimaEntreIntentosEnSegundos, _esperaMaximaEntreIntentosEnSegundos));
                    _ifExceptionMethod.DynamicInvoke(_ifExceptionMethodParams);
                }
            }            
        }

        public ExecuterBuilder TiempoDeEsperaEntreIntentosEnSegundos(int randomFrom, int randomTo)
        {
            _esperaMinimaEntreIntentosEnSegundos = randomFrom;
            _esperaMaximaEntreIntentosEnSegundos = randomTo;
            return this;
        }

        public ExecuterBuilder CantidadDeVecesAntesDeAbortar(int cantidadDeVecesAntesDeAbortar)
        {
            _cantidadDeVecesAntesDeAbortar = cantidadDeVecesAntesDeAbortar;
            return this;
        }
    }
}
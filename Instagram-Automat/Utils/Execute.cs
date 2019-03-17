using System;

namespace Instagram_Automat.Utils
{
    public class ExecuterBuilder
    {
        private Delegate _methodToExecute;
        private object[] _methodToExecuteParams;

        private Delegate _ifExceptionMethod;
        private object[] _ifExceptionMethodParams;

        private static int _cantidadDeVecesAntesDeAbortar = 1;
        private static int _esperaMinimaEntreIntentosEnSegundos = 3;
        private static int _esperaMaximaEntreIntentosEnSegundos = 6;

        public ExecuterBuilder(Delegate methodToExecute, object[] parameters)
		{
            _methodToExecute = methodToExecute;
		}

        public ExecuterBuilder IfException(Delegate methodToExecute, object[] parameters)
        {
            _ifExceptionMethod = methodToExecute;
            _ifExceptionMethodParams = parameters;
            return this;
        }

        public void Execute()
        {
            _methodToExecute.DynamicInvoke(_methodToExecuteParams);
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
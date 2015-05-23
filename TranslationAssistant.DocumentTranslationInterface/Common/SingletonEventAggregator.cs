using System;
using Microsoft.Practices.Prism.Events;

namespace TranslationAssistant.DocumentTranslationInterface.Common
{
    public sealed class SingletonEventAggregator : EventAggregator
    {
        private static volatile EventAggregator instance;
        private static object syncRoot = new Object();

        private SingletonEventAggregator() { }

        public static IEventAggregator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new EventAggregator();
                    }
                }

                return instance;
            }
        }
    }
}

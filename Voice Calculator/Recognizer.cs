using System;
using System.Speech.Recognition;

namespace Voice_Calculator
{
    public abstract class Recognizer
    {
        public abstract event EventHandler<EventArgs> Recognized;

        protected const int ELEMENTS_COUNT = 1000;

        public string[] numbers;
        public string[] operations = new string[] { "+", "-", "*", "/", "=" };
        public string[] commands = new string[] { ".", "point", "result", "clear", "cancel", "delete", "exit", "shut down", "say my name" };

        protected void GenerateNumbers()
        {
            numbers = new string[ELEMENTS_COUNT];

            for (int i = 0; i < ELEMENTS_COUNT; i++)
            {
                numbers[i] = i.ToString();
            }
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
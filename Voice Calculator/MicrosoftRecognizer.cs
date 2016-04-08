using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Voice_Calculator
{
    public class MicrosoftRecognizer : Recognizer
    {
        private SpeechRecognitionEngine recEngine;

        public override event EventHandler<EventArgs> Recognized;

        public MicrosoftRecognizer()
        {
            recEngine = new SpeechRecognitionEngine();

            GenerateNumbers();

            Choices allCommands = new Choices();
            allCommands.Add(numbers);
            allCommands.Add(commands);
            allCommands.Add(operations);

            GrammarBuilder grammaBuilder = new GrammarBuilder();
            grammaBuilder.Append(allCommands);

            Grammar grammar = new Grammar(grammaBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
        }

        public override void Start()
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        public override void Stop()
        {
            recEngine.RecognizeAsyncStop();
        }

        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Recognized != null)
            {
                Recognized.Invoke(sender, e);
            }
        }
    }
}

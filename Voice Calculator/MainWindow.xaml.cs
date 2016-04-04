using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace Voice_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int ELEMENTS_COUNT = 1000;

        private bool operationUsed = false;
        private bool isRecognizing = true;

        private string[] operations = new string[] { "+", "-", "*", "/", "=" };

        SpeechRecognitionEngine recEngine;
        SpeechSynthesizer speechSynth;

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;

            speechSynth = new SpeechSynthesizer();
            speechSynth.Volume = 100;
            speechSynth.Rate = 1;

            recEngine = new SpeechRecognitionEngine();

            Choices commands = new Choices();
            string[] numbers = new string[ELEMENTS_COUNT];

            for (int i = 0; i < ELEMENTS_COUNT; i++)
            {
                numbers[i] = i.ToString();
            }

            commands.Add(numbers);
            commands.Add(new string[] { ".", "point", "result", "clear", "cancel", "delete", "exit", "shut down", "say my name" });
            commands.Add(operations);

            GrammarBuilder grammaBuilder = new GrammarBuilder();
            grammaBuilder.Append(commands);

            Grammar grammar = new Grammar(grammaBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized;
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void EnterCommand(string command)
        {
            command = command.ToLower();
            string res;
            switch (command)
            {
                case "=":
                case "result":
                    operationUsed = false;

                    res = Calculate(tbExpression.Text);
                    tbExpression.Text = res;

                    speechSynth.SpeakAsync("Result is " + res);
                    break;
                case "delete":
                case "cancel":

                    if (tbExpression.Text.Length > 2 && operations.Contains((tbExpression.Text[tbExpression.Text.Length - 2]).ToString()))
                    {
                        tbExpression.Text = tbExpression.Text.Substring(0, tbExpression.Text.Length - 3);
                    }
                    else if(tbExpression.Text.Length > 0)
                    {
                        tbExpression.Text = tbExpression.Text.Substring(0, tbExpression.Text.Length - 1);
                    }
                    speechSynth.SpeakAsync("Removed");
                    break;
                case "clear":
                    tbExpression.Text = string.Empty;
                    speechSynth.SpeakAsync("Cleared");
                    break;
                case "shut down":
                case "exit":
                    this.Close();
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                    if (!(tbExpression.Text == string.Empty && (command == "+" || command == "-" || command == "*" || command == "/")))
                    {
                        if (operationUsed)
                        {
                            operationUsed = false;

                            res = Calculate(tbExpression.Text);
                            tbExpression.Text = res;
                        }
                        speechSynth.SpeakAsync(command);
                        tbExpression.Text += " " + command + " ";
                        operationUsed = true;
                    }
                    break;
                case "point":
                    speechSynth.SpeakAsync(command);
                    tbExpression.Text += ".";
                    break;
                case "say my name":
                    speechSynth.SpeakAsync("Heisenberg");
                    break;
                default:
                    speechSynth.SpeakAsync(command);
                    tbExpression.Text += command;
                    break;
            }
        }
        private string Calculate(string text)
        {
            string[] tokens;
            double a, b;
            try
            {
                tokens = text.Split(' ');
                a = double.Parse(tokens[0]);
                b = double.Parse(tokens[2]);
            }
            catch (Exception)
            {
                lbMessanges.Content = "Error operation";
                return text;
            }

            switch (tokens[1])
            {
                case "+":
                    return (a + b).ToString();
                case "-":
                    return (a - b).ToString();
                case "*":
                    return (a * b).ToString();
                case "/":
                    return (a / b).ToString();
                default:
                    return text;
            }
        }

        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text;
            EnterCommand(command);
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            EnterCommand((sender as Button).Content.ToString());
        }
        private void btnMute_Click(object sender, RoutedEventArgs e)
        {
            speechSynth.Volume = 0;
        }
        private void btnRec_Click(object sender, RoutedEventArgs e)
        {
            if(isRecognizing)
            {
                recEngine.RecognizeAsyncStop();
            }
            else
            {
                recEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
        }
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace Voice_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool operationUsed = false;
        private bool isRecognizing = true;

        SpeechSynthesizer speechSynth;
        MicrosoftRecognizer recognizer;

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;

            speechSynth = new SpeechSynthesizer();
            speechSynth.Volume = 100;
            speechSynth.Rate = 1;

            recognizer = new MicrosoftRecognizer();
            recognizer.Recognized += Recognizer_Recognized;
            recognizer.Start();
        }

        private void Recognizer_Recognized(object sender, EventArgs e)
        {
            string command = (e as SpeechRecognizedEventArgs).Result.Text;
            EnterCommand(command);
        }

        private void EnterCommand(string command)
        {
            //TODO: That's so bad code. I know it)
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

                    if (tbExpression.Text.Length > 2 && recognizer.operations.Contains((tbExpression.Text[tbExpression.Text.Length - 2]).ToString()))
                    {
                        tbExpression.Text = tbExpression.Text.Substring(0, tbExpression.Text.Length - 3);
                    }
                    else if (tbExpression.Text.Length > 0)
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
                    if (tbExpression.Text == string.Empty && (command == "+" || command == "-" || command == "*" || command == "/"))
                    {
                        break;
                    }
                    if(tbExpression.Text.Length>2 && recognizer.operations.Contains(tbExpression.Text[tbExpression.Text.Length - 2].ToString()))
                    {
                        break;
                    }
                    if (operationUsed)
                    {
                        operationUsed = false;

                        res = Calculate(tbExpression.Text);
                        tbExpression.Text = res;
                    }
                    speechSynth.SpeakAsync(command);
                    tbExpression.Text += " " + command + " ";
                    operationUsed = true;
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
            if (isRecognizing)
            {
                recognizer.Stop();
            }
            else
            {
                recognizer.Start();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            recognizer.Stop();
        }
    }
}

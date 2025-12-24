using System;
using System.Threading;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task1Form : Form
    {
        private Thread? numberThread;
        private Thread? letterThread;
        private Thread? symbolThread;
        private bool isRunning = false;
        private TextBox outputTextBox;
        private ComboBox numberPriorityCombo;
        private ComboBox letterPriorityCombo;
        private ComboBox symbolPriorityCombo;
        private Button startButton;
        private Button stopButton;

        public Task1Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 1: Генерация чисел, букв и символов";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label numberLabel = new Label
            {
                Text = "Приоритет потока чисел:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(150, 20)
            };

            numberPriorityCombo = new ComboBox
            {
                Location = new System.Drawing.Point(180, 18),
                Size = new System.Drawing.Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            numberPriorityCombo.Items.AddRange(new object[] { "Lowest", "BelowNormal", "Normal", "AboveNormal", "Highest" });
            numberPriorityCombo.SelectedIndex = 2;

            Label letterLabel = new Label
            {
                Text = "Приоритет потока букв:",
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(150, 20)
            };

            letterPriorityCombo = new ComboBox
            {
                Location = new System.Drawing.Point(180, 48),
                Size = new System.Drawing.Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            letterPriorityCombo.Items.AddRange(new object[] { "Lowest", "BelowNormal", "Normal", "AboveNormal", "Highest" });
            letterPriorityCombo.SelectedIndex = 2;

            Label symbolLabel = new Label
            {
                Text = "Приоритет потока символов:",
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(150, 20)
            };

            symbolPriorityCombo = new ComboBox
            {
                Location = new System.Drawing.Point(180, 78),
                Size = new System.Drawing.Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            symbolPriorityCombo.Items.AddRange(new object[] { "Lowest", "BelowNormal", "Normal", "AboveNormal", "Highest" });
            symbolPriorityCombo.SelectedIndex = 2;

            startButton = new Button
            {
                Text = "Старт",
                Location = new System.Drawing.Point(20, 110),
                Size = new System.Drawing.Size(100, 30)
            };
            startButton.Click += StartButton_Click;

            stopButton = new Button
            {
                Text = "Стоп",
                Location = new System.Drawing.Point(130, 110),
                Size = new System.Drawing.Size(100, 30),
                Enabled = false
            };
            stopButton.Click += StopButton_Click;

            outputTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 150),
                Size = new System.Drawing.Size(550, 300),
                ReadOnly = true
            };

            this.Controls.AddRange(new Control[] {
                numberLabel, numberPriorityCombo,
                letterLabel, letterPriorityCombo,
                symbolLabel, symbolPriorityCombo,
                startButton, stopButton,
                outputTextBox
            });
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            if (isRunning) return;

            isRunning = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;
            numberPriorityCombo.Enabled = false;
            letterPriorityCombo.Enabled = false;
            symbolPriorityCombo.Enabled = false;
            outputTextBox.Clear();

            ThreadPriority GetPriority(ComboBox combo)
            {
                return combo.SelectedIndex switch
                {
                    0 => ThreadPriority.Lowest,
                    1 => ThreadPriority.BelowNormal,
                    2 => ThreadPriority.Normal,
                    3 => ThreadPriority.AboveNormal,
                    4 => ThreadPriority.Highest,
                    _ => ThreadPriority.Normal
                };
            }

            numberThread = new Thread(() => GenerateNumbers(GetPriority(numberPriorityCombo)))
            {
                Priority = GetPriority(numberPriorityCombo),
                IsBackground = true
            };

            letterThread = new Thread(() => GenerateLetters(GetPriority(letterPriorityCombo)))
            {
                Priority = GetPriority(letterPriorityCombo),
                IsBackground = true
            };

            symbolThread = new Thread(() => GenerateSymbols(GetPriority(symbolPriorityCombo)))
            {
                Priority = GetPriority(symbolPriorityCombo),
                IsBackground = true
            };

            numberThread.Start();
            letterThread.Start();
            symbolThread.Start();
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            isRunning = false;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            numberPriorityCombo.Enabled = true;
            letterPriorityCombo.Enabled = true;
            symbolPriorityCombo.Enabled = true;
        }

        private void GenerateNumbers(ThreadPriority priority)
        {
            Random random = new Random();
            while (isRunning)
            {
                int number = random.Next(0, 100);
                AppendText($"Число: {number} (приоритет: {priority})\r\n");
                Thread.Sleep(100);
            }
        }

        private void GenerateLetters(ThreadPriority priority)
        {
            Random random = new Random();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            while (isRunning)
            {
                char letter = letters[random.Next(letters.Length)];
                AppendText($"Буква: {letter} (приоритет: {priority})\r\n");
                Thread.Sleep(100);
            }
        }

        private void GenerateSymbols(ThreadPriority priority)
        {
            Random random = new Random();
            string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            while (isRunning)
            {
                char symbol = symbols[random.Next(symbols.Length)];
                AppendText($"Символ: {symbol} (приоритет: {priority})\r\n");
                Thread.Sleep(100);
            }
        }

        private void AppendText(string text)
        {
            if (outputTextBox.InvokeRequired)
            {
                outputTextBox.Invoke(new Action<string>(AppendText), text);
            }
            else
            {
                outputTextBox.AppendText(text);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isRunning = false;
            Thread.Sleep(200);
            base.OnFormClosing(e);
        }
    }
}


using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task8Form : Form
    {
        private TextBox inputTextBox;
        private Button calculateButton;
        private Label vowelsLabel;
        private Label consonantsLabel;
        private Label symbolsLabel;
        private Label statusLabel;
        private bool isCalculating = false;

        private readonly char[] vowels = { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 
                                          'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я',
                                          'a', 'e', 'i', 'o', 'u', 'y',
                                          'A', 'E', 'I', 'O', 'U', 'Y' };

        public Task8Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 8: Подсчет символов в тексте";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label inputLabel = new Label
            {
                Text = "Введите текст:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            inputTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(540, 150)
            };

            calculateButton = new Button
            {
                Text = "Подсчитать",
                Location = new System.Drawing.Point(20, 205),
                Size = new System.Drawing.Size(150, 30)
            };
            calculateButton.Click += CalculateButton_Click;

            vowelsLabel = new Label
            {
                Text = "Гласные: 0",
                Location = new System.Drawing.Point(20, 250),
                Size = new System.Drawing.Size(540, 25),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            consonantsLabel = new Label
            {
                Text = "Согласные: 0",
                Location = new System.Drawing.Point(20, 280),
                Size = new System.Drawing.Size(540, 25),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            symbolsLabel = new Label
            {
                Text = "Всего символов: 0",
                Location = new System.Drawing.Point(20, 310),
                Size = new System.Drawing.Size(540, 25),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F)
            };

            statusLabel = new Label
            {
                Text = "Готово к подсчету",
                Location = new System.Drawing.Point(20, 345),
                Size = new System.Drawing.Size(540, 20)
            };

            this.Controls.AddRange(new Control[] {
                inputLabel, inputTextBox, calculateButton,
                vowelsLabel, consonantsLabel, symbolsLabel, statusLabel
            });
        }

        private async void CalculateButton_Click(object? sender, EventArgs e)
        {
            if (isCalculating)
            {
                MessageBox.Show("Подсчет уже выполняется. Пожалуйста, подождите.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string text = inputTextBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Пожалуйста, введите текст для анализа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isCalculating = true;
            calculateButton.Enabled = false;
            vowelsLabel.Text = "Гласные: вычисление...";
            consonantsLabel.Text = "Согласные: вычисление...";
            symbolsLabel.Text = "Всего символов: вычисление...";
            statusLabel.Text = "Подсчет символов...";

            try
            {
                var result = await CountCharactersAsync(text);
                
                vowelsLabel.Text = $"Гласные: {result.Vowels}";
                consonantsLabel.Text = $"Согласные: {result.Consonants}";
                symbolsLabel.Text = $"Всего символов: {result.TotalSymbols}";
                statusLabel.Text = "Подсчет завершен успешно!";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка при подсчете: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isCalculating = false;
                calculateButton.Enabled = true;
            }
        }

        private Task<(int Vowels, int Consonants, int TotalSymbols)> CountCharactersAsync(string text)
        {
            return Task.Run(() =>
            {
                int vowels = 0;
                int consonants = 0;
                int totalSymbols = text.Length;

                foreach (char c in text)
                {
                    if (char.IsLetter(c))
                    {
                        if (this.vowels.Contains(c))
                        {
                            vowels++;
                        }
                        else
                        {
                            consonants++;
                        }
                    }
                }

                return (vowels, consonants, totalSymbols);
            });
        }
    }
}


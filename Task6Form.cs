using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task6Form : Form
    {
        private TextBox inputTextBox;
        private Button calculateButton;
        private TextBox resultTextBox;
        private Label statusLabel;
        private bool isCalculating = false;

        public Task6Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 6: Подсчет факториала";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label inputLabel = new Label
            {
                Text = "Введите число:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            inputTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(200, 25)
            };

            calculateButton = new Button
            {
                Text = "Вычислить",
                Location = new System.Drawing.Point(230, 43),
                Size = new System.Drawing.Size(100, 30)
            };
            calculateButton.Click += CalculateButton_Click;

            Label resultLabel = new Label
            {
                Text = "Результат:",
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(100, 20)
            };

            resultTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 105),
                Size = new System.Drawing.Size(440, 120),
                ReadOnly = true
            };

            statusLabel = new Label
            {
                Text = "Готово к вычислению",
                Location = new System.Drawing.Point(20, 235),
                Size = new System.Drawing.Size(440, 20)
            };

            this.Controls.AddRange(new Control[] {
                inputLabel, inputTextBox, calculateButton,
                resultLabel, resultTextBox, statusLabel
            });
        }

        private async void CalculateButton_Click(object? sender, EventArgs e)
        {
            if (isCalculating)
            {
                MessageBox.Show("Вычисление уже выполняется. Пожалуйста, подождите.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(inputTextBox.Text, out int number) || number < 0)
            {
                MessageBox.Show("Пожалуйста, введите неотрицательное целое число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (number > 10000)
            {
                MessageBox.Show("Число слишком большое. Введите число не более 10000.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isCalculating = true;
            calculateButton.Enabled = false;
            resultTextBox.Clear();
            statusLabel.Text = "Вычисление факториала...";

            try
            {
                BigInteger result = await CalculateFactorialAsync(number);
                resultTextBox.Text = result.ToString();
                statusLabel.Text = $"Факториал {number} вычислен успешно!";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка при вычислении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isCalculating = false;
                calculateButton.Enabled = true;
            }
        }

        private Task<BigInteger> CalculateFactorialAsync(int n)
        {
            return Task.Run(() =>
            {
                if (n == 0 || n == 1)
                    return BigInteger.One;

                BigInteger result = BigInteger.One;
                for (int i = 2; i <= n; i++)
                {
                    result *= i;
                    
                    if (i % 100 == 0)
                    {
                        UpdateStatus($"Вычисление... {i} / {n}");
                    }
                }

                return result;
            });
        }

        private void UpdateStatus(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), text);
            }
            else
            {
                statusLabel.Text = text;
            }
        }
    }
}


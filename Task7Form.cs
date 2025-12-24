using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task7Form : Form
    {
        private TextBox numberTextBox;
        private TextBox powerTextBox;
        private Button calculateButton;
        private TextBox resultTextBox;
        private Label statusLabel;
        private bool isCalculating = false;

        public Task7Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 7: Подсчет степени числа";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label numberLabel = new Label
            {
                Text = "Введите число:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
            };

            numberTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(200, 25)
            };

            Label powerLabel = new Label
            {
                Text = "Введите степень:",
                Location = new System.Drawing.Point(20, 75),
                Size = new System.Drawing.Size(100, 20)
            };

            powerTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 100),
                Size = new System.Drawing.Size(200, 25)
            };

            calculateButton = new Button
            {
                Text = "Вычислить",
                Location = new System.Drawing.Point(230, 70),
                Size = new System.Drawing.Size(100, 30)
            };
            calculateButton.Click += CalculateButton_Click;

            Label resultLabel = new Label
            {
                Text = "Результат:",
                Location = new System.Drawing.Point(20, 135),
                Size = new System.Drawing.Size(100, 20)
            };

            resultTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(440, 80),
                ReadOnly = true
            };

            statusLabel = new Label
            {
                Text = "Готово к вычислению",
                Location = new System.Drawing.Point(20, 250),
                Size = new System.Drawing.Size(440, 20)
            };

            this.Controls.AddRange(new Control[] {
                numberLabel, numberTextBox,
                powerLabel, powerTextBox,
                calculateButton,
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

            if (!double.TryParse(numberTextBox.Text, out double number))
            {
                MessageBox.Show("Пожалуйста, введите корректное число.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(powerTextBox.Text, out int power) || power < 0)
            {
                MessageBox.Show("Пожалуйста, введите неотрицательное целое число для степени.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (power > 10000)
            {
                MessageBox.Show("Степень слишком большая. Введите степень не более 10000.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isCalculating = true;
            calculateButton.Enabled = false;
            resultTextBox.Clear();
            statusLabel.Text = "Вычисление степени...";

            try
            {
                double result = await CalculatePowerAsync(number, power);
                resultTextBox.Text = result.ToString("F10").TrimEnd('0').TrimEnd('.');
                statusLabel.Text = $"{number}^{power} вычислено успешно!";
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

        private Task<double> CalculatePowerAsync(double number, int power)
        {
            return Task.Run(() =>
            {
                if (power == 0)
                    return 1.0;

                if (number == 0)
                    return 0.0;

                double result = 1.0;
                for (int i = 0; i < power; i++)
                {
                    result *= number;
                    
                    if (i % 1000 == 0 && i > 0)
                    {
                        UpdateStatus($"Вычисление... {i} / {power}");
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


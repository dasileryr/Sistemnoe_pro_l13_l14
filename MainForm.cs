using System;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Text = "Практические задания по многопоточности";
            this.Size = new System.Drawing.Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            int buttonHeight = 40;
            int buttonWidth = 300;
            int spacing = 10;
            int startY = 20;
            
            Button btnTask1 = new Button
            {
                Text = "Задание 1: Генерация чисел, букв, символов",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY),
                UseVisualStyleBackColor = true
            };
            btnTask1.Click += (s, e) => new Task1Form().ShowDialog();
            
            Button btnTask2 = new Button
            {
                Text = "Задание 2: Копирование файлов",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 1),
                UseVisualStyleBackColor = true
            };
            btnTask2.Click += (s, e) => new Task2Form().ShowDialog();
            
            Button btnTask3 = new Button
            {
                Text = "Задание 3: Копирование файлов (с паузой)",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 2),
                UseVisualStyleBackColor = true
            };
            btnTask3.Click += (s, e) => new Task3Form().ShowDialog();
            
            Button btnTask4 = new Button
            {
                Text = "Задание 4: Копирование директорий",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 3),
                UseVisualStyleBackColor = true
            };
            btnTask4.Click += (s, e) => new Task4Form().ShowDialog();
            
            Button btnTask5 = new Button
            {
                Text = "Задание 5: Копирование директорий (с паузой)",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 4),
                UseVisualStyleBackColor = true
            };
            btnTask5.Click += (s, e) => new Task5Form().ShowDialog();
            
            Button btnTask6 = new Button
            {
                Text = "Задание 6: Подсчет факториала",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 5),
                UseVisualStyleBackColor = true
            };
            btnTask6.Click += (s, e) => new Task6Form().ShowDialog();
            
            Button btnTask7 = new Button
            {
                Text = "Задание 7: Подсчет степени числа",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 6),
                UseVisualStyleBackColor = true
            };
            btnTask7.Click += (s, e) => new Task7Form().ShowDialog();
            
            Button btnTask8 = new Button
            {
                Text = "Задание 8: Подсчет символов в тексте",
                Size = new System.Drawing.Size(buttonWidth, buttonHeight),
                Location = new System.Drawing.Point(50, startY + (buttonHeight + spacing) * 7),
                UseVisualStyleBackColor = true
            };
            btnTask8.Click += (s, e) => new Task8Form().ShowDialog();
            
            this.Controls.AddRange(new Control[] {
                btnTask1, btnTask2, btnTask3, btnTask4, btnTask5, btnTask6, btnTask7, btnTask8
            });
            
            this.ResumeLayout(false);
        }
    }
}


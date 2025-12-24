using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task4Form : Form
    {
        private TextBox sourcePathTextBox;
        private TextBox destPathTextBox;
        private NumericUpDown threadCountNumeric;
        private Button browseSourceButton;
        private Button browseDestButton;
        private Button startButton;
        private ProgressBar progressBar;
        private Label statusLabel;
        private bool isCopying = false;
        private int totalFiles = 0;
        private int copiedFiles = 0;

        public Task4Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 4: Копирование директорий";
            this.Size = new System.Drawing.Size(600, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label sourceLabel = new Label
            {
                Text = "Исходная директория:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(150, 20)
            };

            sourcePathTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(400, 25),
                ReadOnly = true
            };

            browseSourceButton = new Button
            {
                Text = "Обзор...",
                Location = new System.Drawing.Point(430, 43),
                Size = new System.Drawing.Size(100, 30)
            };
            browseSourceButton.Click += BrowseSourceButton_Click;

            Label destLabel = new Label
            {
                Text = "Путь для копирования:",
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(150, 20)
            };

            destPathTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 105),
                Size = new System.Drawing.Size(400, 25),
                ReadOnly = true
            };

            browseDestButton = new Button
            {
                Text = "Обзор...",
                Location = new System.Drawing.Point(430, 103),
                Size = new System.Drawing.Size(100, 30)
            };
            browseDestButton.Click += BrowseDestButton_Click;

            Label threadLabel = new Label
            {
                Text = "Количество потоков:",
                Location = new System.Drawing.Point(20, 140),
                Size = new System.Drawing.Size(150, 20)
            };

            threadCountNumeric = new NumericUpDown
            {
                Location = new System.Drawing.Point(180, 138),
                Size = new System.Drawing.Size(100, 25),
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };

            startButton = new Button
            {
                Text = "Старт",
                Location = new System.Drawing.Point(20, 170),
                Size = new System.Drawing.Size(100, 30)
            };
            startButton.Click += StartButton_Click;

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 210),
                Size = new System.Drawing.Size(510, 25)
            };

            statusLabel = new Label
            {
                Text = "Готово к копированию",
                Location = new System.Drawing.Point(20, 245),
                Size = new System.Drawing.Size(510, 20)
            };

            this.Controls.AddRange(new Control[] {
                sourceLabel, sourcePathTextBox, browseSourceButton,
                destLabel, destPathTextBox, browseDestButton,
                threadLabel, threadCountNumeric,
                startButton, progressBar, statusLabel
            });
        }

        private void BrowseSourceButton_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sourcePathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void BrowseDestButton_Click(object? sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    destPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sourcePathTextBox.Text) || string.IsNullOrEmpty(destPathTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, выберите исходную директорию и путь для копирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(sourcePathTextBox.Text))
            {
                MessageBox.Show("Исходная директория не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isCopying = true;
            copiedFiles = 0;
            startButton.Enabled = false;
            browseSourceButton.Enabled = false;
            browseDestButton.Enabled = false;
            threadCountNumeric.Enabled = false;

            Thread copyThread = new Thread(() => CopyDirectory(sourcePathTextBox.Text, destPathTextBox.Text, (int)threadCountNumeric.Value))
            {
                IsBackground = true
            };
            copyThread.Start();
        }

        private void CopyDirectory(string sourceDir, string destDir, int threadCount)
        {
            try
            {
                string[] allFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
                totalFiles = allFiles.Length;

                if (totalFiles == 0)
                {
                    UpdateStatus("Директория пуста");
                    UpdateProgress(100);
                    UpdateUI(() =>
                    {
                        startButton.Enabled = true;
                        browseSourceButton.Enabled = true;
                        browseDestButton.Enabled = true;
                        threadCountNumeric.Enabled = true;
                    });
                    return;
                }

                UpdateStatus($"Найдено файлов: {totalFiles}. Начало копирования...");
                UpdateProgress(0);

                Directory.CreateDirectory(destDir);

                foreach (string sourceFile in allFiles)
                {
                    if (!isCopying) break;

                    string relativePath = Path.GetRelativePath(sourceDir, sourceFile);
                    string destFile = Path.Combine(destDir, relativePath);
                    string destFileDir = Path.GetDirectoryName(destFile)!;

                    Directory.CreateDirectory(destFileDir);

                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                        copiedFiles++;
                        int progress = (int)((copiedFiles * 100) / totalFiles);
                        UpdateProgress(progress);
                        UpdateStatus($"Копирование: {copiedFiles} / {totalFiles} файлов ({progress}%)");
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"Ошибка при копировании {Path.GetFileName(sourceFile)}: {ex.Message}");
                    }
                }

                if (isCopying)
                {
                    UpdateStatus($"Копирование завершено успешно! Скопировано {copiedFiles} файлов.");
                    UpdateProgress(100);
                    MessageBox.Show($"Директория успешно скопирована! Скопировано {copiedFiles} файлов.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateStatus("Копирование прервано.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при копировании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateUI(() =>
                {
                    startButton.Enabled = true;
                    browseSourceButton.Enabled = true;
                    browseDestButton.Enabled = true;
                    threadCountNumeric.Enabled = true;
                });
                isCopying = false;
            }
        }

        private void UpdateProgress(int value)
        {
            UpdateUI(() => progressBar.Value = Math.Min(100, Math.Max(0, value)));
        }

        private void UpdateStatus(string text)
        {
            UpdateUI(() => statusLabel.Text = text);
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isCopying = false;
            Thread.Sleep(200);
            base.OnFormClosing(e);
        }
    }
}


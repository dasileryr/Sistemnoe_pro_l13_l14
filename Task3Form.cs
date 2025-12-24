using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ThreadingTasks
{
    public partial class Task3Form : Form
    {
        private TextBox sourcePathTextBox;
        private TextBox destPathTextBox;
        private NumericUpDown threadCountNumeric;
        private Button browseSourceButton;
        private Button browseDestButton;
        private Button startButton;
        private Button pauseButton;
        private Button stopButton;
        private ProgressBar progressBar;
        private Label statusLabel;
        private bool isCopying = false;
        private bool isPaused = false;
        private ManualResetEvent pauseEvent = new ManualResetEvent(true);
        private Thread? copyThread;

        public Task3Form()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Задание 3: Копирование файлов с паузой и остановкой";
            this.Size = new System.Drawing.Size(600, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label sourceLabel = new Label
            {
                Text = "Исходный файл:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(100, 20)
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

            pauseButton = new Button
            {
                Text = "Пауза",
                Location = new System.Drawing.Point(130, 170),
                Size = new System.Drawing.Size(100, 30),
                Enabled = false
            };
            pauseButton.Click += PauseButton_Click;

            stopButton = new Button
            {
                Text = "Стоп",
                Location = new System.Drawing.Point(240, 170),
                Size = new System.Drawing.Size(100, 30),
                Enabled = false
            };
            stopButton.Click += StopButton_Click;

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
                startButton, pauseButton, stopButton,
                progressBar, statusLabel
            });
        }

        private void BrowseSourceButton_Click(object? sender, EventArgs e)
        {
            if (isCopying)
            {
                MessageBox.Show("Сначала остановите копирование.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sourcePathTextBox.Text = dialog.FileName;
                }
            }
        }

        private void BrowseDestButton_Click(object? sender, EventArgs e)
        {
            if (isCopying)
            {
                MessageBox.Show("Сначала остановите копирование.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    destPathTextBox.Text = dialog.FileName;
                }
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sourcePathTextBox.Text) || string.IsNullOrEmpty(destPathTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, выберите исходный файл и путь для копирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(sourcePathTextBox.Text))
            {
                MessageBox.Show("Исходный файл не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isCopying = true;
            isPaused = false;
            pauseEvent.Set();
            startButton.Enabled = false;
            pauseButton.Enabled = true;
            stopButton.Enabled = true;
            browseSourceButton.Enabled = false;
            browseDestButton.Enabled = false;
            threadCountNumeric.Enabled = false;

            copyThread = new Thread(() => CopyFile(sourcePathTextBox.Text, destPathTextBox.Text, (int)threadCountNumeric.Value))
            {
                IsBackground = true
            };
            copyThread.Start();
        }

        private void PauseButton_Click(object? sender, EventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                pauseEvent.Set();
                pauseButton.Text = "Пауза";
                UpdateStatus("Копирование возобновлено");
            }
            else
            {
                isPaused = true;
                pauseEvent.Reset();
                pauseButton.Text = "Продолжить";
                UpdateStatus("Копирование приостановлено");
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            isCopying = false;
            isPaused = false;
            pauseEvent.Set();
            UpdateStatus("Остановка копирования...");
        }

        private void CopyFile(string sourcePath, string destPath, int threadCount)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(sourcePath);
                long fileSize = fileInfo.Length;

                UpdateStatus($"Начало копирования файла размером {fileSize} байт...");
                UpdateProgress(0);

                using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[8192];
                    long totalCopied = 0;

                    while (totalCopied < fileSize && isCopying)
                    {
                        pauseEvent.WaitOne();

                        int bytesRead = sourceStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        destStream.Write(buffer, 0, bytesRead);
                        totalCopied += bytesRead;

                        int progress = (int)((totalCopied * 100) / fileSize);
                        UpdateProgress(progress);
                        UpdateStatus($"Копирование: {totalCopied} / {fileSize} байт ({progress}%)");
                    }
                }

                if (isCopying)
                {
                    UpdateStatus("Копирование завершено успешно!");
                    UpdateProgress(100);
                    MessageBox.Show("Файл успешно скопирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UpdateStatus("Копирование остановлено пользователем.");
                    if (File.Exists(destPath))
                    {
                        try
                        {
                            File.Delete(destPath);
                        }
                        catch { }
                    }
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
                    pauseButton.Enabled = false;
                    pauseButton.Text = "Пауза";
                    stopButton.Enabled = false;
                    browseSourceButton.Enabled = true;
                    browseDestButton.Enabled = true;
                    threadCountNumeric.Enabled = true;
                });
                isCopying = false;
                isPaused = false;
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
            pauseEvent.Set();
            Thread.Sleep(200);
            base.OnFormClosing(e);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace FullTrade
{
    public partial class InvestmentQuestionnaireWindow : Window
    {
        private readonly InvestmentQuestionnaire _questionnaire;
        private readonly InvestmentDisclaimer _disclaimer;
        private readonly List<int> _answers;
        private readonly List<CheckBox> _disclaimerCheckboxes;
        private int _currentQuestionIndex;
        private StackPanel _mainPanel;
        private TextBlock _questionText;
        private StackPanel _answersPanel;
        private Button _nextButton;
        private Button _backButton;
        private Button _skipButton;
        private ProgressBar _progressBar;

        public bool IsCompleted { get; private set; }
        public bool WasSkipped { get; private set; }
        public InvestorProfile UserProfile { get; private set; }

        public InvestmentQuestionnaireWindow()
        {
            InitializeComponent();
            _questionnaire = new InvestmentQuestionnaire();
            _disclaimer = new InvestmentDisclaimer();
            _answers = new List<int>();
            _disclaimerCheckboxes = new List<CheckBox>();

            Title = "Investiční dotazník";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            CreateUI();
            ShowWelcomeScreen();
        }


        private void InitializeComponent()
        {
            _mainPanel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            _progressBar = new ProgressBar
            {
                Height = 10,
                Margin = new Thickness(20, 10, 20, 10)
            };

            _questionText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20),
                FontSize = 16
            };

            _answersPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 20)
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            _backButton = new Button
            {
                Content = "Zpět",
                Width = 100,
                Margin = new Thickness(5),
                IsEnabled = false
            };
            _backButton.Click += BackButton_Click;

            _nextButton = new Button
            {
                Content = "Další",
                Width = 100,
                Margin = new Thickness(5)
            };
            _nextButton.Click += NextButton_Click;

            _skipButton = new Button
            {
                Content = "Přeskočit dotazník",
                Width = 150,
                Margin = new Thickness(5),
                Background = Brushes.Orange,
                Foreground = Brushes.White
            };
            _skipButton.Click += SkipButton_Click;

            buttonPanel.Children.Add(_backButton);
            buttonPanel.Children.Add(_nextButton);
            buttonPanel.Children.Add(_skipButton);

            _mainPanel.Children.Add(_progressBar);
            _mainPanel.Children.Add(_questionText);
            _mainPanel.Children.Add(_answersPanel);
            _mainPanel.Children.Add(buttonPanel);

            Content = new ScrollViewer { Content = _mainPanel };
        }

        private void ShowWelcomeScreen()
        {
            _mainPanel.Children.Clear();

            var welcomePanel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = "Vítejte v investičním dotazníku",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20),
                TextAlignment = TextAlignment.Center
            };

            var description = new TextBlock
            {
                Text = "Tento dotazník vám pomůže určit váš investiční profil a nastavit " +
                      "obchodní platformu podle vašich potřeb a zkušeností. Dotazník obsahuje " +
                      "sérii otázek týkajících se vašich investičních cílů, zkušeností a " +
                      "tolerance k riziku.\n\n" +
                      "Doporučujeme dotazník vyplnit pro získání personalizovaných doporučení, " +
                      "ale můžete jej také přeskočit a používat aplikaci s výchozím nastavením.",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 30),
                TextAlignment = TextAlignment.Left
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var startButton = new Button
            {
                Content = "Začít dotazník",
                Width = 150,
                Height = 40,
                Margin = new Thickness(10),
                Background = Brushes.Green,
                Foreground = Brushes.White
            };
            startButton.Click += (s, e) => ShowDisclaimer();

            var skipButton = new Button
            {
                Content = "Přeskočit dotazník",
                Width = 150,
                Height = 40,
                Margin = new Thickness(10),
                Background = Brushes.Orange,
                Foreground = Brushes.White
            };
            skipButton.Click += SkipButton_Click;

            buttonPanel.Children.Add(startButton);
            buttonPanel.Children.Add(skipButton);

            welcomePanel.Children.Add(title);
            welcomePanel.Children.Add(description);
            welcomePanel.Children.Add(buttonPanel);

            _mainPanel.Children.Add(welcomePanel);
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Opravdu chcete přeskočit investiční dotazník?\n\n" +
                "Aplikace bude nastavena na konzervativní profil s omezenými funkcemi.",
                "Potvrzení přeskočení",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                WasSkipped = true;
                UserProfile = new InvestorProfile
                {
                    RiskLevel = "Nízké",
                    Description = "Výchozí konzervativní profil",
                    AssetAllocation = new Dictionary<string, double>
                    {
                        { "Hotovost a krátkodobé instrumenty", 0.40 },
                        { "Dluhopisy", 0.40 },
                        { "Akcie", 0.15 },
                        { "Alternativní investice", 0.05 }
                    }
                };
                IsCompleted = true;
                Close();
            }
        }

        private void CreateUI()
        {
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(20)
            };

            _mainPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            scrollViewer.Content = _mainPanel;
            Content = scrollViewer;
        }

        private void ShowDisclaimer()
        {
            _mainPanel.Children.Clear();
            _disclaimerCheckboxes.Clear();

            var disclaimerTitle = new TextBlock
            {
                Text = "Důležité upozornění o rizicích investování",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            _mainPanel.Children.Add(disclaimerTitle);

            // Přidání úvodního potvrzení
            var generalConfirmation = new CheckBox
            {
                Content = "Potvrzuji, že jsem starší 18 let a budu jednat svým vlastním jménem",
                Margin = new Thickness(0, 0, 0, 15),
                FontWeight = FontWeights.Bold
            };
            _disclaimerCheckboxes.Add(generalConfirmation);
            _mainPanel.Children.Add(generalConfirmation);

            foreach (var statement in _disclaimer.GetRiskStatements())
            {
                var statementPanel = new StackPanel
                {
                    Margin = new Thickness(0, 0, 0, 15)
                };

                var titleBlock = new TextBlock
                {
                    Text = statement.Title,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                statementPanel.Children.Add(titleBlock);

                var descriptionBlock = new TextBlock
                {
                    Text = statement.Description,
                    TextWrapping = TextWrapping.Wrap
                };
                statementPanel.Children.Add(descriptionBlock);

                if (statement.RequiresAcknowledgment)
                {
                    var checkbox = new CheckBox
                    {
                        Content = "Rozumím a souhlasím s tímto rizikem",
                        Margin = new Thickness(0, 5, 0, 0)
                    };
                    _disclaimerCheckboxes.Add(checkbox);
                    statementPanel.Children.Add(checkbox);
                }

                _mainPanel.Children.Add(statementPanel);
            }

            // Přidání závěrečného potvrzení
            var finalConfirmation = new CheckBox
            {
                Content = "Potvrzuji, že jsem si přečetl(a) všechna upozornění a souhlasím s nimi",
                Margin = new Thickness(0, 15, 0, 0),
                FontWeight = FontWeights.Bold
            };
            _disclaimerCheckboxes.Add(finalConfirmation);
            _mainPanel.Children.Add(finalConfirmation);

            var confirmButton = new Button
            {
                Content = "Pokračovat na investiční dotazník",
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(20, 10, 20, 10)
            };
            confirmButton.Click += ConfirmButton_Click;
            _mainPanel.Children.Add(confirmButton);

            // Přidání vysvětlujícího textu
            var explanationText = new TextBlock
            {
                Text = "Pro pokračování musíte potvrdit všechna povinná prohlášení zaškrtnutím příslušných políček.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 20, 0, 0),
                Foreground = Brushes.Gray,
                FontStyle = System.Windows.FontStyles.Italic
            };
            _mainPanel.Children.Add(explanationText);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_disclaimerCheckboxes.All(cb => cb.IsChecked == true))
            {
                StartQuestionnaire();
            }
            else
            {
                var uncheckedCount = _disclaimerCheckboxes.Count(cb => cb.IsChecked != true);
                MessageBox.Show(
                    $"Prosím potvrďte všechna prohlášení zaškrtnutím příslušných políček.\n\n" +
                    $"Zbývá potvrdit: {uncheckedCount} {(uncheckedCount == 1 ? "položka" : uncheckedCount < 5 ? "položky" : "položek")}",
                    "Nepotvrzená prohlášení",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                // Zvýraznění nezaškrtnutých políček
                foreach (var checkbox in _disclaimerCheckboxes)
                {
                    if (checkbox.IsChecked != true)
                    {
                        checkbox.Foreground = Brushes.Red;
                        checkbox.FontWeight = FontWeights.Bold;
                    }
                    else
                    {
                        checkbox.Foreground = Brushes.Black;
                        checkbox.FontWeight = FontWeights.Normal;
                    }
                }
            }
        }

        private void StartQuestionnaire()
        {
            _currentQuestionIndex = 0;
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {

            _progressBar.Maximum = _questionnaire.GetQuestions().Count;
            _progressBar.Value = _currentQuestionIndex + 1;
            _mainPanel.Children.Clear();

            var questions = _questionnaire.GetQuestions();
            if (_currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            var currentQuestion = questions[_currentQuestionIndex];

            var progressText = new TextBlock
            {
                Text = $"Otázka {_currentQuestionIndex + 1} z {questions.Count}",
                Margin = new Thickness(0, 0, 0, 20)
            };
            _mainPanel.Children.Add(progressText);

            var questionText = new TextBlock
            {
                Text = currentQuestion.QuestionText,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 20)
            };
            _mainPanel.Children.Add(questionText);

            foreach (var answer in currentQuestion.PossibleAnswers)
            {
                var radioButton = new RadioButton
                {
                    Content = answer.Text,
                    Margin = new Thickness(0, 5, 0, 5),
                    Tag = answer.Score
                };
                _mainPanel.Children.Add(radioButton);
            }

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var backButton = new Button
            {
                Content = "Zpět",
                Width = 100,
                Margin = new Thickness(5),
                IsEnabled = _currentQuestionIndex > 0
            };
            backButton.Click += (s, e) => {
                _currentQuestionIndex--;
                _answers.RemoveAt(_answers.Count - 1);
                ShowCurrentQuestion();
            };

            var nextButton = new Button
            {
                Content = _currentQuestionIndex == questions.Count - 1 ? "Dokončit" : "Další",
                Width = 100,
                Margin = new Thickness(5)
            };
            nextButton.Click += (s, e) => {
                var selectedRadio = _mainPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
                if (selectedRadio != null)
                {
                    _answers.Add((int)selectedRadio.Tag);
                    _currentQuestionIndex++;
                    ShowCurrentQuestion();
                }
                else
                {
                    MessageBox.Show("Prosím vyberte odpověď", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };

            buttonPanel.Children.Add(backButton);
            buttonPanel.Children.Add(nextButton);
            _mainPanel.Children.Add(buttonPanel);
        }

        private void ShowResults()
        {
            _mainPanel.Children.Clear();

            UserProfile = _questionnaire.EvaluateAnswers(_answers);
            var strategy = new InvestmentStrategy(UserProfile);
            var recommendations = strategy.GenerateRecommendations();

            var resultTitle = new TextBlock
            {
                Text = "Výsledky investičního dotazníku",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            _mainPanel.Children.Add(resultTitle);

            var resultText = new TextBlock
            {
                Text = recommendations,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };
            _mainPanel.Children.Add(resultText);

            var closeButton = new Button
            {
                Content = "Dokončit",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            closeButton.Click += (s, e) => {
                IsCompleted = true;
                Close();
            };
            _mainPanel.Children.Add(closeButton);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                _answers.RemoveAt(_answers.Count - 1);
                ShowCurrentQuestion();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRadio = _mainPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
            if (selectedRadio != null)
            {
                _answers.Add((int)selectedRadio.Tag);
                _currentQuestionIndex++;
                ShowCurrentQuestion();
            }
            else
            {
                MessageBox.Show("Prosím vyberte odpověď", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
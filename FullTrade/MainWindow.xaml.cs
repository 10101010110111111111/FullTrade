using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FullTrade
{
    public partial class MainWindow : Window
    {
        private readonly BinanceDataService _binanceService;
        private List<CandleData> _currentData;
        private const double CANDLE_WIDTH = 8;
        private const double CANDLE_SPACING = 2;
        private InvestorProfile _userProfile;

        private int _displayStartIndex = 0;
        private int _visibleCandleCount = 100;
        private double _zoomLevel = 1.0;
        private Point? _lastMousePosition;
        private bool _isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            _binanceService = new BinanceDataService();
            _currentData = new List<CandleData>();

            SymbolTextBox.ToolTip = "Zadejte symbol z Binance (např. BTCUSDT, ETHUSDT, BNBUSDT)";

            ChartCanvas.MouseWheel += ChartCanvas_MouseWheel;
            ChartCanvas.MouseLeftButtonDown += ChartCanvas_MouseLeftButtonDown;
            ChartCanvas.MouseLeftButtonUp += ChartCanvas_MouseLeftButtonUp;
            ChartCanvas.MouseMove += ChartCanvas_MouseMove;
            this.KeyDown += MainWindow_KeyDown;

            Loaded += MainWindow_Loaded;
            ShowInitialText();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var questionnaireWindow = new InvestmentQuestionnaireWindow();
            questionnaireWindow.ShowDialog();

            if (questionnaireWindow.IsCompleted)
            {
                _userProfile = questionnaireWindow.UserProfile;
                Title = $"FullTrade - {_userProfile.RiskLevel} profil";

                // Můžeme přizpůsobit UI podle profilu investora
                AdjustUIBasedOnProfile(_userProfile);
            }
            else
            {
                MessageBox.Show("Pro používání aplikace je nutné dokončit investiční dotazník.",
                              "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
            }
        }

        private void AdjustUIBasedOnProfile(InvestorProfile profile)
        {
            // Zde můžeme přizpůsobit UI podle profilu investora
            // Například skrýt nebo zobrazit určité prvky, upravit limity atd.
            switch (profile.RiskLevel)
            {
                case "Nízké":
                    // Například omezit páku, zobrazit více varování atd.
                    break;
                case "Střední":
                    // Standardní nastavení
                    break;
                case "Vyšší":
                    // Povolit více pokročilých funkcí
                    break;
                case "Vysoké":
                    // Zpřístupnit všechny funkce
                    break;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MoveChart(-10);
                    break;
                case Key.Right:
                    MoveChart(10);
                    break;
                case Key.Add:
                case Key.OemPlus:
                    ZoomChart(1.1);
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    ZoomChart(0.9);
                    break;
                case Key.Home:
                    _displayStartIndex = 0;
                    DrawChart();
                    break;
                case Key.End:
                    _displayStartIndex = Math.Max(0, _currentData.Count - _visibleCandleCount);
                    DrawChart();
                    break;
                    
            }
        }

        private void ChartCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Zoom
                double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
                ZoomChart(zoomFactor, e.GetPosition(ChartCanvas));
            }
            else
            {
                // Horizontální scroll
                int scrollAmount = -(e.Delta / 120) * 3;
                MoveChart(scrollAmount);
            }
        }

        private void ChartCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(ChartCanvas);
            _isDragging = true;
            ChartCanvas.CaptureMouse();
        }

        private void ChartCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _lastMousePosition = null;
            ChartCanvas.ReleaseMouseCapture();
        }

        private void ChartCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _lastMousePosition.HasValue)
            {
                Point currentPosition = e.GetPosition(ChartCanvas);
                double deltaX = currentPosition.X - _lastMousePosition.Value.X;

                // Převod deltaX na počet svíček
                int candleShift = -(int)(deltaX / (CANDLE_WIDTH + CANDLE_SPACING));
                if (candleShift != 0)
                {
                    MoveChart(candleShift);
                    _lastMousePosition = currentPosition;
                }
            }
        }

        private void ZoomChart(double factor, Point? zoomCenter = null)
        {
            double oldVisibleCount = _visibleCandleCount;
            _visibleCandleCount = Math.Max(10, Math.Min(_currentData.Count, (int)(_visibleCandleCount / factor)));

            if (zoomCenter.HasValue)
            {
                // Vypočítat index svíčky pod kurzorem myši
                double candleWidth = (CANDLE_WIDTH + CANDLE_SPACING) * _zoomLevel;
                int candleIndex = _displayStartIndex + (int)(zoomCenter.Value.X / candleWidth);

                // Upravit počáteční index tak, aby se zachovala pozice kurzoru
                double relativePosition = zoomCenter.Value.X / ChartCanvas.ActualWidth;
                _displayStartIndex = Math.Max(0, candleIndex - (int)(_visibleCandleCount * relativePosition));
            }

            _zoomLevel = _zoomLevel * factor;
            DrawChart();
        }

        private void MoveChart(int candleShift)
        {
            int newStartIndex = _displayStartIndex + candleShift;
            if (newStartIndex >= 0 && newStartIndex <= _currentData.Count - _visibleCandleCount)
            {
                _displayStartIndex = newStartIndex;
                DrawChart();
            }
        }

        private async void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataButton.IsEnabled = false;
                Mouse.OverrideCursor = Cursors.Wait;

                if (IntervalComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Prosím vyberte interval", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string symbol = SymbolTextBox.Text.Trim().ToUpper();
                if (string.IsNullOrEmpty(symbol))
                {
                    MessageBox.Show("Prosím zadejte symbol", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string interval = ((ComboBoxItem)IntervalComboBox.SelectedItem).Content.ToString();
                ShowLoadingStatus();

                try
                {
                    _currentData = await _binanceService.GetSpotCandleData(symbol, interval);
                    if (_currentData?.Any() == true)
                    {
                        _displayStartIndex = Math.Max(0, _currentData.Count - _visibleCandleCount);
                        DrawChart();
                        Title = $"{symbol} Chart ({interval}) - {_currentData.Count} candles";
                    }
                    else
                    {
                        MessageBox.Show("Nebyla načtena žádná data. Ujistěte se, že symbol existuje na Binance.",
                                      "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show($"Symbol '{symbol}' nebyl nalezen na Binance nebo není momentálně dostupný.",
                                  "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                LoadDataButton.IsEnabled = true;
                Mouse.OverrideCursor = null;
            }
        }

        private void ShowLoadingStatus()
        {
            ChartCanvas.Children.Clear();
            var statusText = new TextBlock
            {
                Text = "Načítám data...",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(statusText, ChartCanvas.ActualWidth / 2 - 50);
            Canvas.SetTop(statusText, ChartCanvas.ActualHeight / 2);
            ChartCanvas.Children.Add(statusText);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Chyba při načítání dat",
                           MessageBoxButton.OK, MessageBoxImage.Error);

            ChartCanvas.Children.Clear();
            var errorText = new TextBlock
            {
                Text = "Chyba při načítání dat. Zkuste to prosím znovu.",
                Foreground = Brushes.Red,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(errorText, ChartCanvas.ActualWidth / 2 - 150);
            Canvas.SetTop(errorText, ChartCanvas.ActualHeight / 2);
            ChartCanvas.Children.Add(errorText);
        }

        private void DrawChart()
        {
            ChartCanvas.Children.Clear();

            if (_currentData.Count == 0) return;

            var visibleData = _currentData
                .Skip(_displayStartIndex)
                .Take(_visibleCandleCount)
                .ToList();

            if (!visibleData.Any()) return;

            double minPrice = visibleData.Min(c => c.Low);
            double maxPrice = visibleData.Max(c => c.High);
            double priceRange = maxPrice - minPrice;

            minPrice -= priceRange * 0.02;
            maxPrice += priceRange * 0.02;
            priceRange = maxPrice - minPrice;

            double scaleFactor = (ChartCanvas.ActualHeight - 40) / priceRange;
            double effectiveCandleWidth = CANDLE_WIDTH * _zoomLevel;
            double effectiveSpacing = CANDLE_SPACING * _zoomLevel;

            DrawPriceGrid(minPrice, maxPrice, scaleFactor);

            for (int i = 0; i < visibleData.Count; i++)
            {
                var candle = visibleData[i];
                double x = i * (effectiveCandleWidth + effectiveSpacing);

                double highY = ChartCanvas.ActualHeight - 20 - (candle.High - minPrice) * scaleFactor;
                double lowY = ChartCanvas.ActualHeight - 20 - (candle.Low - minPrice) * scaleFactor;
                double openY = ChartCanvas.ActualHeight - 20 - (candle.Open - minPrice) * scaleFactor;
                double closeY = ChartCanvas.ActualHeight - 20 - (candle.Close - minPrice) * scaleFactor;

                var wickLine = new Line
                {
                    X1 = x + effectiveCandleWidth / 2,
                    X2 = x + effectiveCandleWidth / 2,
                    Y1 = highY,
                    Y2 = lowY,
                    Stroke = Brushes.Black,
                    StrokeThickness = Math.Max(1, _zoomLevel * 0.5)
                };
                ChartCanvas.Children.Add(wickLine);

                var candleRect = new Rectangle
                {
                    Width = effectiveCandleWidth,
                    Height = Math.Max(Math.Abs(closeY - openY), 1),
                    Fill = candle.Close >= candle.Open ? Brushes.Green : Brushes.Red
                };

                Canvas.SetLeft(candleRect, x);
                Canvas.SetTop(candleRect, Math.Min(openY, closeY));
                ChartCanvas.Children.Add(candleRect);

                // Časové popisky - zobrazení se přizpůsobí zoomu
                if (i % Math.Max(1, (int)(10 / _zoomLevel)) == 0)
                {
                    var timeLabel = new TextBlock
                    {
                        Text = candle.Timestamp.ToString("HH:mm"),
                        FontSize = Math.Max(8, 8 * _zoomLevel),
                        RenderTransform = new RotateTransform(45)
                    };
                    Canvas.SetLeft(timeLabel, x);
                    Canvas.SetTop(timeLabel, ChartCanvas.ActualHeight - 15);
                    ChartCanvas.Children.Add(timeLabel);
                }
            }

            // Přidat informaci o zobrazeném rozsahu
            var rangeInfo = new TextBlock
            {
                Text = $"Zobrazeno: {_displayStartIndex + 1} - {_displayStartIndex + visibleData.Count} z {_currentData.Count}",
                FontSize = 10,
                Foreground = Brushes.DarkGray
            };
            Canvas.SetLeft(rangeInfo, 10);
            Canvas.SetTop(rangeInfo, 10);
            ChartCanvas.Children.Add(rangeInfo);
        }

        private void DrawPriceGrid(double minPrice, double maxPrice, double scaleFactor)
        {
            int priceLines = 10;
            double priceStep = (maxPrice - minPrice) / priceLines;

            for (int i = 0; i <= priceLines; i++)
            {
                double price = minPrice + (i * priceStep);
                double yPosition = ChartCanvas.ActualHeight - 20 - (price - minPrice) * scaleFactor;

                var gridLine = new Line
                {
                    X1 = 0,
                    X2 = ChartCanvas.ActualWidth,
                    Y1 = yPosition,
                    Y2 = yPosition,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 0.5,
                    StrokeDashArray = new DoubleCollection(new[] { 4.0, 4.0 })
                };
                ChartCanvas.Children.Add(gridLine);

                var priceLabel = new TextBlock
                {
                    Text = price.ToString("F2"),
                    FontSize = 10 * Math.Max(1, _zoomLevel * 0.7),
                    Foreground = Brushes.DarkGray
                };
                Canvas.SetRight(priceLabel, 5);
                Canvas.SetTop(priceLabel, yPosition - 7);
                ChartCanvas.Children.Add(priceLabel);
            }
        }

        private void ShowInitialText()
        {
            ChartCanvas.Children.Clear();

            var mainPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20)
            };

            // Kryptoměny
            var cryptoPanel = CreateSymbolPanel("Kryptoměny", new Dictionary<string, string>
    {
        { "BTCUSDT", "Bitcoin" },
        { "ETHUSDT", "Ethereum" },
        { "BNBUSDT", "Binance Coin" },
        { "DOGEUSDT", "Dogecoin" },
        { "ADAUSDT", "Cardano" }
    });
            mainPanel.Children.Add(cryptoPanel);

            // Akcie
            var stocksPanel = CreateSymbolPanel("Akcie (tokenizované)", new Dictionary<string, string>
    {
        { "TSLUSDT", "Tesla" },
        { "GOOGUSDT", "Google" },
        { "AAPLUSDT", "Apple" },
        { "MSFUSDT", "Microsoft" }
    });
            mainPanel.Children.Add(stocksPanel);

            // Komodity
            var commoditiesPanel = CreateSymbolPanel("Komodity", new Dictionary<string, string>
    {
        { "GOLDUSDT", "Zlato" },
        { "XAGUSDT", "Stříbro" },
        { "WTIUSDT", "Ropa" }
    });
            mainPanel.Children.Add(commoditiesPanel);

            // Přidání instrukce pod seznam
            var instructionBlock = new TextBlock
            {
                Text = "Pro zobrazení grafu zadejte symbol a stiskněte 'Načíst data'",
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var containerPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            containerPanel.Children.Add(mainPanel);
            containerPanel.Children.Add(instructionBlock);

            ChartCanvas.Children.Add(containerPanel);
        }



        private StackPanel CreateSymbolPanel(string title, Dictionary<string, string> symbols)
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0))
            };

            var titleBlock = new TextBlock
            {
                Text = title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            panel.Children.Add(titleBlock);

            foreach (var symbol in symbols)
            {
                var symbolBlock = new TextBlock
                {
                    Text = $"{symbol.Key} - {symbol.Value}",
                    TextAlignment = TextAlignment.Left,
                    Margin = new Thickness(5),
                    FontSize = 14
                };
                panel.Children.Add(symbolBlock);
            }

            return panel;
        }
    }
}
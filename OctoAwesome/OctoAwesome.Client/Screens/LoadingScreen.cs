using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Screens
{
    internal sealed class LoadingScreen : BaseScreen
    {
        private static readonly QuoteProvider _loadingQuoteProvider;
        static LoadingScreen()
        {
            var settings = TypeContainer.Get<ISettings>();
            _loadingQuoteProvider = new QuoteProvider(new FileInfo(Path.Combine(settings.Get<string>("LoadingScreenQuotesPath"))));
        }

        private readonly GameScreen _gameScreen;
        private readonly CancellationTokenSource _tokenSource;
        private readonly Task _quoteUpdate;

        public LoadingScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);
            _tokenSource = new CancellationTokenSource();

            Title = "Loading";

            SetDefaultBackground();

            //Main Panel
            var mainStack = new Grid(manager);
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 4 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            var backgroundStack = new Panel(manager)
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10)
            };
            mainStack.AddControl(backgroundStack, 0, 0, 1, 1);

            var mainGrid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainGrid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 4 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 4 });

            backgroundStack.Controls.Add(mainGrid);

            var text = new Label(manager)
            {
                Text = "Konfuzius sagt: Das hier lädt...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = Border.All(10),
            };

            _quoteUpdate = Task.Run(async () => await UpdateLabel(text, _loadingQuoteProvider, TimeSpan.FromSeconds(1), _tokenSource.Token));
            mainGrid.AddControl(text, 1, 1);


            //Buttons
            var buttonStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            mainGrid.AddControl(buttonStack, 1, 2);

            var cancelButton = GetButton(Languages.OctoClient.Cancel);
            buttonStack.Controls.Add(cancelButton);

            Debug.WriteLine("Create GameScreen");
            _gameScreen = new GameScreen(manager);
            _gameScreen.Update(new GameTime());
            _gameScreen.OnCenterChanged += SwitchToGame;

            cancelButton.LeftMouseClick += (s, e) =>
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                manager.Player.SetEntity(null);
                manager.Game.Simulation.ExitGame();
                _gameScreen.Unload();
                manager.NavigateBack();
            };
        }

        private void SwitchToGame(object sender, System.EventArgs args)
        {
            Manager.Invoke(() =>
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                Manager.NavigateToScreen(_gameScreen);
                _gameScreen.OnCenterChanged -= SwitchToGame;
            });
        }

        private static async Task UpdateLabel(Label label, QuoteProvider quoteProvider, TimeSpan timeSpan, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                var text = quoteProvider.GetRandomQuote();

                label.ScreenManager.Invoke(() => label.Text = text + "...");

                await Task.Delay(timeSpan, token);
            }
        }
    }
}

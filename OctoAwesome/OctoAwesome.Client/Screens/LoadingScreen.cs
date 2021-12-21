using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Languages;
using EventArgs = System.EventArgs;

namespace OctoAwesome.Client.Screens
{
    internal sealed class LoadingScreen : BaseScreen
    {
        private static readonly QuoteProvider LoadingQuoteProvider;

        private readonly GameScreen _gameScreen;
        private readonly Task _quoteUpdate;
        private readonly CancellationTokenSource _tokenSource;

        static LoadingScreen()
        {
            var settings = TypeContainer.Get<ISettings>();
            LoadingQuoteProvider = new(new(Path.Combine(settings.Get<string>("LoadingScreenQuotesPath"))));
        }

        public LoadingScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new(0, 0, 0, 0);
            _tokenSource = new();

            Title = "Loading";

            SetDefaultBackground();

            //Main Panel
            var mainStack = new Grid(manager);
            mainStack.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 4 });
            mainStack.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
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
            mainStack.AddControl(backgroundStack, 0, 0);

            var mainGrid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            mainGrid.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainGrid.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainGrid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 4 });
            mainGrid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainGrid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 4 });

            backgroundStack.Controls.Add(mainGrid);

            var text = new Label(manager)
            {
                Text = "Konfuzius sagt: Das hier lädt...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = Border.All(10)
            };

            _quoteUpdate = Task.Run(async () =>
                await UpdateLabel(text, LoadingQuoteProvider, TimeSpan.FromSeconds(1.5), _tokenSource.Token));
            mainGrid.AddControl(text, 1, 1);


            //Buttons
            var buttonStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            mainGrid.AddControl(buttonStack, 1, 2);

            var cancelButton = GetButton(OctoClient.Cancel);
            buttonStack.Controls.Add(cancelButton);

            Debug.WriteLine("Create GameScreen");
            _gameScreen = new(manager);
            _gameScreen.Update(new());
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

        private void SwitchToGame(object sender, EventArgs args)
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
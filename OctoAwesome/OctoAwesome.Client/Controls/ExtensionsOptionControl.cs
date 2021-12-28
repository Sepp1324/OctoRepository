using System;
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.UI.Languages;

namespace OctoAwesome.Client.Controls
{
    internal sealed class ExtensionsOptionControl : Panel
    {
        private readonly Listbox<IExtension> _activeExtensionsList;
        private readonly Button _disableButton;
        private readonly Button _enableButton;
        private readonly Label _infoLabel;
        private readonly Listbox<IExtension> _loadedExtensionsList;

        public ExtensionsOptionControl(BaseScreenComponent manager, IExtensionLoader extensionLoader) : base(manager)
        {
            var grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = Border.All(15)
            };
            Controls.Add(grid);

            grid.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Columns.Add(new() { ResizeMode = ResizeMode.Fixed, Width = 100 });
            grid.Columns.Add(new() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new() { ResizeMode = ResizeMode.Auto, Height = 1 });

            var buttons = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.AddControl(buttons, 1, 0);

            #region Manipulationsbuttons

            _enableButton = new TextButton(manager, OctoClient.Enable);
            _enableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _enableButton.Visible = false;
            buttons.Controls.Add(_enableButton);

            _disableButton = new TextButton(manager, OctoClient.Disable);
            _disableButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _disableButton.Visible = false;
            buttons.Controls.Add(_disableButton);

            #endregion

            Button applyButton = new TextButton(manager, OctoClient.Apply);
            applyButton.HorizontalAlignment = HorizontalAlignment.Right;
            applyButton.VerticalAlignment = VerticalAlignment.Bottom;
            grid.AddControl(applyButton, 0, 2, 3);

            _infoLabel = new(ScreenManager)
            {
                HorizontalTextAlignment = HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                WordWrap = true
            };
            grid.AddControl(_infoLabel, 0, 1, 3);

            #region Listen

            _loadedExtensionsList = new Listbox<IExtension>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator
            };

            grid.AddControl(_loadedExtensionsList, 0, 0);

            _activeExtensionsList = new Listbox<IExtension>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator
            };

            grid.AddControl(_activeExtensionsList, 2, 0);

            #endregion

            _loadedExtensionsList.SelectedItemChanged += loadedList_SelectedItemChanged;
            _activeExtensionsList.SelectedItemChanged += activeList_SelectedItemChanged;

            _enableButton.LeftMouseClick += (s, e) =>
            {
                var ext = _loadedExtensionsList.SelectedItem;
                _loadedExtensionsList.Items.Remove(ext!);
                _activeExtensionsList.Items.Add(ext!);
                _activeExtensionsList.SelectedItem = ext;
            };

            _disableButton.LeftMouseClick += (s, e) =>
            {
                var ext = _activeExtensionsList.SelectedItem;
                _activeExtensionsList.Items.Remove(ext!);
                _loadedExtensionsList.Items.Add(ext!);
                _loadedExtensionsList.SelectedItem = ext;
            };

            applyButton.LeftMouseClick += (s, e) =>
            {
                //TODO: Apply
                extensionLoader.ApplyExtensions(_loadedExtensionsList.Items);
                Program.Restart();
            };

            // Daten laden
            var loader = extensionLoader;
            foreach (var item in loader.LoadedExtensions)
                _loadedExtensionsList.Items.Add(item);

            foreach (var item in loader.ActiveExtensions)
            {
                _activeExtensionsList.Items.Add(item);
                if (_loadedExtensionsList.Items.Contains(item))
                    _loadedExtensionsList.Items.Remove(item);
            }
        }

        private Control ListTemplateGenerator(IExtension ext)
        {
            return new Label(ScreenManager)
            {
                Text = ext.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalTextAlignment = HorizontalAlignment.Left
            };
        }

        private void loadedList_SelectedItemChanged(Control control, SelectionEventArgs<IExtension> e)
        {
            e.Handled = true;
            _enableButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                _activeExtensionsList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (_activeExtensionsList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void activeList_SelectedItemChanged(Control control, SelectionEventArgs<IExtension> e)
        {
            e.Handled = true;
            _disableButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                _loadedExtensionsList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (_loadedExtensionsList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void SetPackInfo(IExtension ext)
        {
            if (ext != null)
                _infoLabel.Text = string.Format("{0}{1}{2}", ext.Name, Environment.NewLine, ext.Description);
            else
                _infoLabel.Text = string.Empty;
        }
    }
}
using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    internal sealed class ResourcePacksOptionControl : Panel
    {
        private readonly Button _addButton;
        private readonly Button _removeButton;
        private readonly Button _moveUpButton;
        private readonly Button _moveDownButton;
        private readonly Button _applyButton;
        private readonly Listbox<ResourcePack> _loadedPacksList;
        private readonly Listbox<ResourcePack> _activePacksList;
        private readonly Label _infoLabel;

        public ResourcePacksOptionControl(ScreenComponent manager) : base(manager)
        {
            var grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = Border.All(15),
            };
            Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 100 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });

            var buttons = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            grid.AddControl(buttons, 1, 0);

            #region Manipulationsbuttons

            _addButton = new TextButton(manager, Languages.OctoClient.Add);
            _addButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _addButton.Visible = false;
            buttons.Controls.Add(_addButton);

            _removeButton = new TextButton(manager, Languages.OctoClient.Remove);
            _removeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _removeButton.Visible = false;
            buttons.Controls.Add(_removeButton);

            _moveUpButton = new TextButton(manager, Languages.OctoClient.Up);
            _moveUpButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _moveUpButton.Visible = false;
            buttons.Controls.Add(_moveUpButton);

            _moveDownButton = new TextButton(manager, Languages.OctoClient.Down);
            _moveDownButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            _moveDownButton.Visible = false;
            buttons.Controls.Add(_moveDownButton);

            #endregion

            _applyButton = new TextButton(manager, Languages.OctoClient.Apply)
            {
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom
            };
            grid.AddControl(_applyButton, 0, 2, 3);

            _infoLabel = new Label(ScreenManager)
            {
                HorizontalTextAlignment = HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                WordWrap = true,
            };
            grid.AddControl(_infoLabel, 0, 1, 3);

            #region Listen

            _loadedPacksList = new Listbox<ResourcePack>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(_loadedPacksList, 0, 0);

            _activePacksList = new Listbox<ResourcePack>(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f),
                TemplateGenerator = ListTemplateGenerator,
            };

            grid.AddControl(_activePacksList, 2, 0);

            #endregion

            _loadedPacksList.SelectedItemChanged += loadedList_SelectedItemChanged;
            _activePacksList.SelectedItemChanged += activeList_SelectedItemChanged;

            _addButton.LeftMouseClick += (s, e) =>
            {
                var pack = _loadedPacksList.SelectedItem;
                _loadedPacksList.Items.Remove(pack);
                _activePacksList.Items.Add(pack);
                _activePacksList.SelectedItem = pack;
            };

            _removeButton.LeftMouseClick += (s, e) =>
            {
                var pack = _activePacksList.SelectedItem;
                _activePacksList.Items.Remove(pack);
                _loadedPacksList.Items.Add(pack);
                _loadedPacksList.SelectedItem = pack;
            };

            _moveUpButton.LeftMouseClick += (s, e) =>
            {
                var pack = _activePacksList.SelectedItem;
                if (pack == null)
                    return;

                var index = _activePacksList.Items.IndexOf(pack);
                
                if (index > 0)
                {
                    _activePacksList.Items.Remove(pack);
                    _activePacksList.Items.Insert(index - 1, pack);
                    _activePacksList.SelectedItem = pack;
                }
            };

            _moveDownButton.LeftMouseClick += (s, e) =>
            {
                var pack = _activePacksList.SelectedItem;
                if (pack == null) return;

                var index = _activePacksList.Items.IndexOf(pack);
                
                if (index < _activePacksList.Items.Count - 1)
                {
                    _activePacksList.Items.Remove(pack);
                    _activePacksList.Items.Insert(index + 1, pack);
                    _activePacksList.SelectedItem = pack;
                }
            };

            _applyButton.LeftMouseClick += (s, e) =>
            {
                manager.Game.Assets.ApplyResourcePacks(_activePacksList.Items);
                Program.Restart();
            };

            var assets = manager.Game.Assets;

            foreach (var item in assets.LoadedResourcePacks)
                _loadedPacksList.Items.Add(item);

            foreach (var item in manager.Game.Assets.ActiveResourcePacks)
            {
                _activePacksList.Items.Add(item);
            
                if (_loadedPacksList.Items.Contains(item))
                    _loadedPacksList.Items.Remove(item);
            }
        }

        private Control ListTemplateGenerator(ResourcePack pack)
        {
            return new Label(ScreenManager)
            {
                Text = pack.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalTextAlignment = HorizontalAlignment.Left
            };
        }

        private void loadedList_SelectedItemChanged(Control control, SelectionEventArgs<ResourcePack> e)
        {
            e.Handled = true;
            _addButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                _activePacksList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (_activePacksList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void activeList_SelectedItemChanged(Control control, SelectionEventArgs<ResourcePack> e)
        {
            e.Handled = true;
            _removeButton.Visible = e.NewItem != null;
            _moveUpButton.Visible = e.NewItem != null;
            _moveDownButton.Visible = e.NewItem != null;

            if (e.NewItem != null)
            {
                _loadedPacksList.SelectedItem = null;
                SetPackInfo(e.NewItem);
            }
            else
            {
                if (_loadedPacksList.SelectedItem == null)
                    SetPackInfo(null);
            }
        }

        private void SetPackInfo(ResourcePack pack) => _infoLabel.Text = pack != null ? $"{pack.Name} ({pack.Version})\r\n{pack.Author}\r\n{pack.Description}" : string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using engenious.Input;
using engenious.UI;

namespace OctoAwesome.Client.Components
{
    internal class KeyMapper
    {
        public enum KeyType
        {
            Down,
            Up,
            Pressed
        }

        private readonly ISettings _settings;

        public KeyMapper(BaseScreenComponent manager, ISettings settings)
        {
            manager.KeyDown += KeyDown;
            manager.KeyUp += KeyUp;
            manager.KeyPress += KeyPressed;

            _settings = settings;

            Bindings = new Dictionary<string, Binding>();
        }

        private Dictionary<string, Binding> Bindings { get; }

        /// <summary>
        ///     Registers a new Binding
        /// </summary>
        /// <param name="id">The ID - guideline: ModName:Action</param>
        /// <param name="displayName">The Displayname</param>
        public void RegisterBinding(string id, string displayName)
        {
            if (Bindings.ContainsKey(id))
                return;
            Bindings.Add(id, new Binding {Id = id, DisplayName = displayName});
        }

        /// <summary>
        ///     Removes a Binding
        /// </summary>
        /// <param name="id">The ID</param>
        public void UnregisterBinding(string id)
        {
            if (Bindings.ContainsKey(id))
                Bindings.Remove(id);
        }

        /// <summary>
        ///     Adds a Key to a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="key">The Key</param>
        public void AddKey(string id, Keys key)
        {
            if (Bindings.TryGetValue(id, out var binding))
                if (!binding.Keys.Contains(key))
                    binding.Keys.Add(key);
        }

        /// <summary>
        ///     Removes a Key from a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="key">The Key</param>
        public void RemoveKey(string id, Keys key)
        {
            if (Bindings.TryGetValue(id, out var binding))
                if (binding.Keys.Contains(key))
                    binding.Keys.Remove(key);
        }

        /// <summary>
        ///     Adds an Action to a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="action">The Action</param>
        public void AddAction(string id, Action<KeyType> action)
        {
            if (Bindings.TryGetValue(id, out var binding))
                if (!binding.Actions.Contains(action))
                    binding.Actions.Add(action);
        }

        /// <summary>
        ///     Removes an Action from a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="action">The Action</param>
        public void RemoveAction(string id, Action<KeyType> action)
        {
            if (Bindings.TryGetValue(id, out var binding))
                if (binding.Actions.Contains(action))
                    binding.Actions.Remove(action);
        }

        /// <summary>
        ///     Sets the DisplayName of a Binding
        /// </summary>
        /// <param name="id">The ID of the Binding</param>
        /// <param name="displayName">The new DisplayName</param>
        public void SetDisplayName(string id, string displayName)
        {
            if (Bindings.TryGetValue(id, out var binding))
                binding.DisplayName = displayName;
        }

        /// <summary>
        ///     Lädt KeyBindings aus der App.config-Datei und greift, wenn kein Wert vorhanden ist, auf die angegebenen
        ///     Standardwerte aus.
        /// </summary>
        /// <param name="standardKeys"></param>
        public void LoadFromConfig(Dictionary<string, Keys> standardKeys)
        {
            foreach (var id in standardKeys.Keys)
                if (_settings.KeyExists("KeyMapper-" + id))
                    try
                    {
                        var val = _settings.Get<string>("KeyMapper-" + id);
                        var key = (Keys) Enum.Parse(typeof(Keys), val);
                        AddKey(id, key);
                    }
                    catch
                    {
                        AddKey(id, standardKeys[id]);
                    }
                else
                    AddKey(id, standardKeys[id]);
        }

        public IEnumerable<Binding> GetBindings() => Bindings.Select(binding => binding.Value).ToList();

        public class Binding
        {
            public Binding()
            {
                Keys = new List<Keys>();
                Actions = new List<Action<KeyType>>();
            }

            public string Id { get; set; }

            public string DisplayName { get; set; }

            public List<Keys> Keys { get; set; }

            public List<Action<KeyType>> Actions { get; set; }
        }


        #region KeyEvents

        private void KeyPressed(KeyEventArgs args)
        {
            var result = Bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            foreach (var action in binding.Actions)
                action(KeyType.Pressed);
        }

        private void KeyDown(KeyEventArgs args)
        {
            var result = Bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            foreach (var action in binding.Actions)
                action(KeyType.Down);
        }

        private void KeyUp(KeyEventArgs args)
        {
            var result = Bindings.Values.Where(b => b.Keys.Contains(args.Key));
            foreach (var binding in result)
            foreach (var action in binding.Actions)
                action(KeyType.Up);
        }

        #endregion
    }
}
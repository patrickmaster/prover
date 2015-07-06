using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Prover.Engine.Types;

namespace Prover.UI.ViewModel
{
    class SettingsViewModel : ObservableBase
    {
        private string _filename;

        readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof (OperatorsConfig));
        private OperatorsConfig _config;

        public SettingsViewModel(OperatorsConfig config)
        {
            Config = new OperatorsConfig(config);
        }

        public OperatorsConfig Config
        {
            get { return _config; }
            private set
            {
                if (Equals(value, _config)) return;
                _config = value;
                OnPropertyChanged();
            }
        }

        public string Filename
        {
            get { return _filename; }
            private set
            {
                if (_filename != value)
                {
                    _filename = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadFromFile(string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    OperatorsConfig loadedConfig = (OperatorsConfig) _xmlSerializer.Deserialize(stream);
                    Config = loadedConfig;
                    Filename = filename;
                }
            }
            catch
            {
                MessageBox.Show("Nie udało sie załadować ustawień", "Błąd", MessageBoxButton.OK);
            }
        }

        public void SaveToFile(string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create))
                {
                    _xmlSerializer.Serialize(stream, Config);
                    Filename = filename;
                }
            }
            catch
            {
                MessageBox.Show("Nie udało sie zapisać ustawień", "Błąd", MessageBoxButton.OK);
            }
        }
    }
}

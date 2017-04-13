using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;
using Saltuk.Nsudotnet.Enigma;

namespace Saltuk.Nsudotnet.EnigmaWPFWrapper.ViewModels
{
    class InputViewModel : PropertyChangedBase
    {
        private bool _isEncrypt;

        private string _inputFile;
        private string _outputFile;
        private string _keyFile;

        private string _algorithm;

        private bool _autofill = true;

        public bool IsEncryptChecked
        {
            get
            {
                return _isEncrypt;
            }
            set
            {
                _isEncrypt = value;
                NotifyOfPropertyChange(() => IsEncryptChecked);
            }
        }
        public bool IsDecryptChecked
        {
            get
            {
                return !_isEncrypt;
            }
            set
            {
                _isEncrypt = !value;
                NotifyOfPropertyChange(() => IsDecryptChecked);
            }
        }

        public string InputFile
        {
            get
            {
                return _inputFile;
            }
            set
            {
                _inputFile = value;
                if (_autofill)
                {

                    if (_isEncrypt)
                    {
                        _outputFile = _inputFile + ".crypted";
                        _keyFile = _inputFile + ".key";
                    }
                    else
                    {
                        if (_inputFile.EndsWith(".crypted"))
                        {
                            _outputFile = _inputFile.Remove(_inputFile.Length - 8);
                            _keyFile = _outputFile + ".key";
                        }
                        else
                        {
                            _outputFile = "";
                            _keyFile = "";
                        }
                    }

                    NotifyOfPropertyChange(() => OutputFile);
                    NotifyOfPropertyChange(() => KeyFile);
                }
                NotifyOfPropertyChange(() => InputFile);
                NotifyOfPropertyChange(() => CanDo);
            }
        }
        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            set
            {
                _autofill = false;
                _outputFile = value;
                NotifyOfPropertyChange(() => OutputFile);
                NotifyOfPropertyChange(() => CanDo);
            }
        }
        public string KeyFile
        {
            get
            {
                return _keyFile;
            }
            set
            {
                _autofill = false;
                _keyFile = value;
                NotifyOfPropertyChange(() => KeyFile);
                NotifyOfPropertyChange(() => CanDo);
            }
        }

        public string SelectedAlgorithm
        {
            get { return _algorithm; }
            set
            {
                _algorithm = value;
                NotifyOfPropertyChange(() => SelectedAlgorithm);
                NotifyOfPropertyChange(() => CanDo);
            }
        }

        public List<string> Algorithm => new List<string>() { "AES", "DES", "RC2", "Rijndael" };

        public bool CanDo => !string.IsNullOrEmpty(_inputFile) &&
                             !string.IsNullOrEmpty(_outputFile) &&
                             !string.IsNullOrEmpty(_keyFile) &&
                             !string.IsNullOrEmpty(_algorithm);


        public void BrowseInput()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                InputFile = fileDialog.FileName;
            }
        }

        public void BrowseOutput()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                OutputFile = fileDialog.FileName;
            }
        }

        public void BrowseKey()
        {
            FileDialog fileDialog = _isEncrypt ? (FileDialog)new SaveFileDialog() : (FileDialog)new OpenFileDialog();

            if (fileDialog.ShowDialog() == true)
            {
                KeyFile = fileDialog.FileName;
            }
        }

        public void Do()
        {
            try
            {
                using (var inFile = new FileStream(_inputFile, FileMode.Open))
                using (var outFile = new FileStream(_outputFile, FileMode.Create))
                using (var key = _isEncrypt
                    ? new FileStream(_keyFile, FileMode.Create)
                    : new FileStream(_keyFile, FileMode.Open))
                {

                    if (_isEncrypt)
                        Cryptor.Encrypt(Cryptor.ByName(_algorithm), inFile, outFile, key);
                    else
                        Cryptor.Decrypt(Cryptor.ByName(_algorithm), inFile, outFile, key);

                    MessageBox.Show("Operation completed", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message}", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

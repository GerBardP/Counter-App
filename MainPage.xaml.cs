using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using Counter.Models;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private int lastCounterNumber = 0;
        private const string FileName = "counters.xml";
        public ObservableCollection<CounterModel> Counters { get; set; } = new ObservableCollection<CounterModel>();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadCountersFromXml();
        }

        private void OnAddCounterClicked(object sender, EventArgs e)
        {
            int initialValue = 0;
            if (!int.TryParse(InitialValueEntry.Text, out int parsedValue))
            {
                ShowValidate.Text = "Wartość początkowa ma być wprowadzona i całkowita.";
                return;
            }
            initialValue = parsedValue;

            lastCounterNumber++;

            Color selectedColor = Colors.White;
            if (!string.IsNullOrEmpty(ColorEntry.Text) && ColorEntry.Text.StartsWith("#"))
            {
                try
                {
                    selectedColor = Color.FromArgb(ColorEntry.Text);
                    
                }
                catch
                {
                    Console.WriteLine("Niepoprawnie wprowadzony kolor");
                }
            }
            if (selectedColor == Colors.White)
            {
                ShowValidate.Text = "Kolor nie został wprowadzony, albo został wprowadzony niepoprawnie.";
                return;
            }

            var newCounter = new CounterModel
            {
                Name = $"Licznik {lastCounterNumber}",
                Value = initialValue,
                InitialValue = initialValue,
                CounterColor = selectedColor
            };

            Counters.Add(newCounter);
            SaveCountersToXml();

            InitialValueEntry.Text = string.Empty;
            ColorEntry.Text = string.Empty;
        }


        public Command<CounterModel> IncrementCommand => new Command<CounterModel>((counter) =>
        {
            counter.Value++;
            SaveCountersToXml();
        });

        public Command<CounterModel> DecrementCommand => new Command<CounterModel>((counter) =>
        {
            counter.Value--;
            SaveCountersToXml();
        });

        public Command<CounterModel> ResetCommand => new Command<CounterModel>((counter) =>
        {
            counter.ResetValue();
            SaveCountersToXml();
        });

        public Command<CounterModel> DeleteCommand => new Command<CounterModel>((counter) =>
        {
            Counters.Remove(counter);
            SaveCountersToXml();
        });

        private void SaveCountersToXml()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<CounterModel>));
                using (var writer = new StreamWriter(Path.Combine(FileSystem.AppDataDirectory, FileName)))
                {
                    serializer.Serialize(writer, Counters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisu liczników: {ex.Message}");
            }
        }

        private void LoadCountersFromXml()
        {
            try
            {
                var filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
                if (File.Exists(filePath))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<CounterModel>));
                    using (var reader = new StreamReader(filePath))
                    {
                        var loadedCounters = (ObservableCollection<CounterModel>)serializer.Deserialize(reader);
                        Counters.Clear();
                        int maxCounterNumber = 0;

                        foreach (var counter in loadedCounters)
                        {
                            Counters.Add(counter);

                            if (int.TryParse(counter.Name.Replace("Licznik ", ""), out int counterNumber))
                            {
                                if (counterNumber > maxCounterNumber)
                                {
                                    maxCounterNumber = counterNumber;
                                }
                            }
                        }

                        lastCounterNumber = maxCounterNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania liczników: {ex.Message}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimSort.Algorithms;
using TimSort.Models;
using TimSortAlgorithm = TimSort.Algorithms.TimSort;

namespace TimSort.Services
{
    /// <summary>
    /// Interfaz que abstrae la gestión de datos para diferentes tipos.
    /// Implementa el patrón Strategy para permitir que la interfaz de usuario interactúe
    /// con cualquier tipo de dato de forma polimórfica y desacoplada.
    /// </summary>
    public interface IDataStrategy
    {
        string DisplayName { get; }
        bool HasData { get; }
        int Count { get; }
        SortMetrics? LastMetrics { get; }

        void Generate(int count);
        SortMetrics Sort(bool ascending, bool recordSteps = true, int maxTraceSteps = 2000);
        Task<SortMetrics> SortVisualAsync(bool ascending, Func<SortVisualUpdate, Task>? onVisualUpdate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene la altura relativa normalizada (0.05 a 1.0) del elemento en el índice indicado
        /// para el visualizador gráfico de barras, sincronizado con las reglas de normalización alfabética.
        /// </summary>
        double GetNormalizedHeight(int index);

        void Save(string filePath);
        void Load(string filePath);
        IReadOnlyList<object> GetItems();
        void Clear();
    }

    /// <summary>
    /// Estrategia genérica para manejar colecciones de elementos.
    /// </summary>
    public class DataStrategy<T> : IDataStrategy
    {
        private List<T> _data = new();
        private readonly Func<int, List<T>> _generator;
        private readonly IComparer<T> _comparer;

        // Límites en caché para renderizado de barras rápido y proporcional
        private double _minStringWeight;
        private double _maxStringWeight;
        private int _minCharVal;
        private int _maxCharVal;
        private int _minInt;
        private int _maxInt;
        private double _minDouble;
        private double _maxDouble;
        private int _minCustomId;
        private int _maxCustomId;

        public string DisplayName { get; }
        public bool HasData => _data.Count > 0;
        public int Count => _data.Count;
        public List<T> RawData => _data;
        public SortMetrics? LastMetrics { get; private set; }

        public DataStrategy(string displayName, Func<int, List<T>> generator, IComparer<T>? comparer = null)
        {
            DisplayName = displayName;
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _comparer = comparer ?? Comparer<T>.Default;
        }

        public void Generate(int count)
        {
            _data = _generator(count) ?? new List<T>();
            LastMetrics = null;
            RefreshBounds();
        }

        public SortMetrics Sort(bool ascending, bool recordSteps = true, int maxTraceSteps = 2000)
        {
            var sw = Stopwatch.StartNew();
            var metrics = TimSortAlgorithm.Sort(_data, ascending, _comparer, recordSteps, maxTraceSteps);
            sw.Stop();

            metrics.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            metrics.ElapsedTicks = sw.ElapsedTicks;
            LastMetrics = metrics;

            return metrics;
        }

        public async Task<SortMetrics> SortVisualAsync(bool ascending, Func<SortVisualUpdate, Task>? onVisualUpdate, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var metrics = await TimSortAlgorithm.SortVisualAsync(_data, ascending, _comparer, onVisualUpdate, cancellationToken);
            sw.Stop();

            metrics.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            metrics.ElapsedTicks = sw.ElapsedTicks;
            LastMetrics = metrics;

            return metrics;
        }

        /// <summary>
        /// Recalcula los valores mínimos y máximos de la colección actual para que la escala
        /// visual de las barras aproveche toda la altura de la gráfica de manera proporcional.
        /// </summary>
        private void RefreshBounds()
        {
            if (_data.Count == 0) return;

            try
            {
                if (typeof(T) == typeof(int))
                {
                    var list = (List<int>)(object)_data;
                    _minInt = list.Min();
                    _maxInt = list.Max();
                }
                else if (typeof(T) == typeof(double))
                {
                    var list = (List<double>)(object)_data;
                    _minDouble = list.Min();
                    _maxDouble = list.Max();
                }
                else if (typeof(T) == typeof(CustomItem))
                {
                    var list = (List<CustomItem>)(object)_data;
                    _minCustomId = list.Min(x => x.Id);
                    _maxCustomId = list.Max(x => x.Id);
                }
                else if (typeof(T) == typeof(char))
                {
                    var list = (List<char>)(object)_data;
                    _minCharVal = list.Min(c => NormalizedStringComparer.GetNormalizedCharValue(c));
                    _maxCharVal = list.Max(c => NormalizedStringComparer.GetNormalizedCharValue(c));
                }
                else if (typeof(T) == typeof(string))
                {
                    var list = (List<string>)(object)_data;
                    _minStringWeight = list.Min(s => NormalizedStringComparer.GetNormalizedWordWeight(s));
                    _maxStringWeight = list.Max(s => NormalizedStringComparer.GetNormalizedWordWeight(s));
                }
            }
            catch
            {
                // Fallback silencioso
            }
        }

        public double GetNormalizedHeight(int index)
        {
            if (index < 0 || index >= _data.Count) return 0.5;

            try
            {
                if (typeof(T) == typeof(int))
                {
                    var list = (List<int>)(object)_data;
                    if (_maxInt > _minInt)
                    {
                        return 0.05 + 0.95 * ((double)(list[index] - _minInt) / (_maxInt - _minInt));
                    }
                    return 0.5;
                }

                if (typeof(T) == typeof(double))
                {
                    var list = (List<double>)(object)_data;
                    if (_maxDouble > _minDouble)
                    {
                        return 0.05 + 0.95 * ((list[index] - _minDouble) / (_maxDouble - _minDouble));
                    }
                    return 0.5;
                }

                if (typeof(T) == typeof(CustomItem))
                {
                    var list = (List<CustomItem>)(object)_data;
                    if (_maxCustomId > _minCustomId)
                    {
                        return 0.05 + 0.95 * ((double)(list[index].Id - _minCustomId) / (_maxCustomId - _minCustomId));
                    }
                    return 0.5;
                }

                // Normalización sincronizada para caracteres individuales
                if (typeof(T) == typeof(char))
                {
                    var list = (List<char>)(object)_data;
                    char ch = list[index];
                    int charVal = NormalizedStringComparer.GetNormalizedCharValue(ch);

                    // Escalar proporcionalmente al rango presente en la colección
                    if (_maxCharVal > _minCharVal)
                    {
                        return 0.05 + 0.95 * ((double)(charVal - _minCharVal) / (_maxCharVal - _minCharVal));
                    }

                    // Escala absoluta sobre las 27 letras del alfabeto español (1 a 27)
                    return 0.05 + 0.95 * ((double)(Math.Max(1, charVal) - 1) / 26.0);
                }

                // Normalización sincronizada para palabras con peso posicional en base 28
                if (typeof(T) == typeof(string))
                {
                    var list = (List<string>)(object)_data;
                    string word = list[index];
                    double weight = NormalizedStringComparer.GetNormalizedWordWeight(word);

                    // Escalar proporcionalmente al rango presente en la colección
                    if (_maxStringWeight > _minStringWeight)
                    {
                        return 0.05 + 0.95 * ((weight - _minStringWeight) / (_maxStringWeight - _minStringWeight));
                    }

                    const double MaxTheoreticalWeight = 17210368.0; // 28^5
                    return 0.05 + 0.95 * (weight / MaxTheoreticalWeight);
                }
            }
            catch
            {
                // Fallback seguro
            }

            return 0.5;
        }

        public void Save(string filePath)
        {
            FileHandler.SaveToFile(_data, filePath);
        }

        public void Load(string filePath)
        {
            var loaded = FileHandler.LoadFromFile<T>(filePath);
            if (loaded.Count == 0)
            {
                throw new InvalidOperationException($"No se encontraron elementos válidos de tipo '{DisplayName}' en el archivo.");
            }
            _data = loaded;
            LastMetrics = null;
            RefreshBounds();
        }

        public IReadOnlyList<object> GetItems()
        {
            return _data.Cast<object>().ToList();
        }

        public void Clear()
        {
            _data.Clear();
            LastMetrics = null;
            _minStringWeight = _maxStringWeight = 0;
            _minCharVal = _maxCharVal = 0;
            _minInt = _maxInt = 0;
            _minDouble = _maxDouble = 0;
            _minCustomId = _maxCustomId = 0;
        }

        public override string ToString() => DisplayName;
    }
}

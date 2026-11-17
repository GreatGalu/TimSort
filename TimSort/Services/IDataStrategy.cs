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
        /// para el visualizador gráfico de barras.
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

        public double GetNormalizedHeight(int index)
        {
            if (index < 0 || index >= _data.Count) return 0.5;

            try
            {
                if (typeof(T) == typeof(int))
                {
                    var list = (List<int>)(object)_data;
                    int min = list.Min();
                    int max = list.Max();
                    if (max == min) return 0.5;
                    return 0.05 + 0.95 * ((double)(list[index] - min) / (max - min));
                }

                if (typeof(T) == typeof(double))
                {
                    var list = (List<double>)(object)_data;
                    double min = list.Min();
                    double max = list.Max();
                    if (Math.Abs(max - min) < 0.0001) return 0.5;
                    return 0.05 + 0.95 * ((list[index] - min) / (max - min));
                }

                if (typeof(T) == typeof(CustomItem))
                {
                    var list = (List<CustomItem>)(object)_data;
                    int min = list.Min(x => x.Id);
                    int max = list.Max(x => x.Id);
                    if (max == min) return 0.5;
                    return 0.05 + 0.95 * ((double)(list[index].Id - min) / (max - min));
                }

                if (typeof(T) == typeof(char))
                {
                    var list = (List<char>)(object)_data;
                    char ch = list[index];
                    string s = ch.ToString().ToUpperInvariant();
                    char baseChar = s.Length > 0 ? s[0] : ch;
                    int val = baseChar - 'A';
                    val = Math.Clamp(val, 0, 26);
                    return 0.05 + 0.95 * (val / 26.0);
                }

                if (typeof(T) == typeof(string))
                {
                    // Rango proporcional a la primera letra / longitud
                    var str = _data[index]?.ToString() ?? "";
                    if (str.Length == 0) return 0.05;
                    char first = char.ToUpperInvariant(str[0]);
                    int val = Math.Clamp(first - 'A', 0, 26);
                    return 0.05 + 0.95 * (val / 26.0);
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
        }

        public IReadOnlyList<object> GetItems()
        {
            return _data.Cast<object>().ToList();
        }

        public void Clear()
        {
            _data.Clear();
            LastMetrics = null;
        }

        public override string ToString() => DisplayName;
    }
}

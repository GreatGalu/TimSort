using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TimSort.Algorithms;
using TimSort.Models;
using TimSortAlgorithm = TimSort.Algorithms.TimSort;

namespace TimSort.Services
{
    /// <summary>
    /// Interfaz que abstrae la gestión de datos para diferentes tipos.
    /// Implementa el patrón Strategy para permitir que la interfaz de usuario interactúe
    /// con cualquier tipo de dato (int, double, char, string, CustomItem) de forma polimórfica y desacoplada.
    /// </summary>
    public interface IDataStrategy
    {
        string DisplayName { get; }
        bool HasData { get; }
        int Count { get; }
        SortMetrics? LastMetrics { get; }
        void Generate(int count);
        SortMetrics Sort(bool ascending, bool recordSteps = true, int maxTraceSteps = 1000);
        void Save(string filePath);
        void Load(string filePath);
        IReadOnlyList<object> GetItems();
        void Clear();
    }

    /// <summary>
    /// Estrategia genérica para manejar colecciones de elementos que implementan IComparable.
    /// </summary>
    public class DataStrategy<T> : IDataStrategy where T : IComparable<T>
    {
        private List<T> _data = new();
        private readonly Func<int, List<T>> _generator;

        public string DisplayName { get; }
        public bool HasData => _data.Count > 0;
        public int Count => _data.Count;
        public List<T> RawData => _data;
        public SortMetrics? LastMetrics { get; private set; }

        public DataStrategy(string displayName, Func<int, List<T>> generator)
        {
            DisplayName = displayName;
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public void Generate(int count)
        {
            _data = _generator(count) ?? new List<T>();
            LastMetrics = null;
        }

        public SortMetrics Sort(bool ascending, bool recordSteps = true, int maxTraceSteps = 1000)
        {
            var sw = Stopwatch.StartNew();
            var metrics = TimSortAlgorithm.Sort(_data, ascending, recordSteps, maxTraceSteps);
            sw.Stop();

            metrics.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            metrics.ElapsedTicks = sw.ElapsedTicks;
            LastMetrics = metrics;

            return metrics;
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

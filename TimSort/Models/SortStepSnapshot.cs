using System;
using System.Collections.Generic;

namespace TimSort.Models
{
    /// <summary>
    /// Representa una instantánea (snapshot) del arreglo en un paso o pasada clave del algoritmo.
    /// Utilizado para renderizar la visualización horizontal por filas (Matriz de Pasos).
    /// </summary>
    public class SortStepSnapshot
    {
        public int StepIndex { get; set; }
        public string Phase { get; set; } = string.Empty;
        public object[] Data { get; set; } = Array.Empty<object>();
        public HashSet<int> ChangedIndices { get; set; } = new();

        public SortStepSnapshot() { }

        public SortStepSnapshot(int stepIndex, string phase, IEnumerable<object> data, IEnumerable<int>? changedIndices = null)
        {
            StepIndex = stepIndex;
            Phase = phase;
            Data = System.Linq.Enumerable.ToArray(data);
            if (changedIndices != null)
            {
                ChangedIndices = new HashSet<int>(changedIndices);
            }
        }
    }
}

using System.Collections.Generic;

namespace TimSort.Models
{
    /// <summary>
    /// Almacena las métricas de rendimiento y la trazabilidad (paso a paso) del algoritmo TimSort.
    /// </summary>
    public class SortMetrics
    {
        public long ComparisonCount { get; set; }
        public long MovementCount { get; set; }
        public int MinRun { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public long ElapsedTicks { get; set; }

        /// <summary>
        /// Registro textual de cada comparación, desplazamiento, inserción y fusión realizada.
        /// </summary>
        public List<string> TraceSteps { get; } = new();

        /// <summary>
        /// Límite máximo de pasos a guardar en memoria/interfaz para evitar saturación de la UI.
        /// </summary>
        public int MaxTraceSteps { get; set; } = 1000;

        /// <summary>
        /// Indica si se debe registrar el detalle paso a paso.
        /// </summary>
        public bool RecordSteps { get; set; } = true;

        public void AddStep(string step)
        {
            if (!RecordSteps) return;

            if (TraceSteps.Count < MaxTraceSteps)
            {
                TraceSteps.Add(step);
            }
            else if (TraceSteps.Count == MaxTraceSteps)
            {
                TraceSteps.Add($"... [Límite de {MaxTraceSteps} pasos alcanzado en la vista previa para proteger el rendimiento de la interfaz gráfica] ...");
            }
        }
    }
}

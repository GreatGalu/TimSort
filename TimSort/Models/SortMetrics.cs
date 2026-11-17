using System.Collections.Generic;

namespace TimSort.Models
{
    /// <summary>
    /// Almacena las métricas de rendimiento, bitácora de movimientos y trazabilidad del algoritmo TimSort.
    /// </summary>
    public class SortMetrics
    {
        public long ComparisonCount { get; set; }
        public long MovementCount { get; set; }
        public int MinRun { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public long ElapsedTicks { get; set; }

        /// <summary>
        /// Indica si el ordenamiento final fue verificado y validado con éxito.
        /// </summary>
        public bool IsSortedSuccessfully { get; set; }
        public string VerificationMessage { get; set; } = string.Empty;

        /// <summary>
        /// Registro textual detallado de todas las operaciones (comparaciones, fusiones, runs).
        /// </summary>
        public List<string> TraceSteps { get; } = new();

        /// <summary>
        /// Bitácora específica de movimientos e intercambios (swaps) realizados.
        /// </summary>
        public List<string> MovementsLog { get; } = new();

        /// <summary>
        /// Instantáneas del arreglo tomadas en cada fase/pasada para la matriz visual horizontal.
        /// </summary>
        public List<SortStepSnapshot> Snapshots { get; } = new();

        public int MaxTraceSteps { get; set; } = 2000;
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
                TraceSteps.Add($"... [Límite de {MaxTraceSteps} pasos alcanzado en la vista previa] ...");
            }
        }

        public void AddMovement(string movement)
        {
            if (!RecordSteps) return;

            if (MovementsLog.Count < MaxTraceSteps)
            {
                MovementsLog.Add(movement);
            }
            else if (MovementsLog.Count == MaxTraceSteps)
            {
                MovementsLog.Add($"... [Límite de {MaxTraceSteps} movimientos alcanzado en la bitácora visual] ...");
            }
        }
    }
}

namespace TimSort.Models
{
    public enum VisualUpdateType
    {
        Compare,
        Move,
        Swap,
        Write,
        RunComplete,
        Complete
    }

    /// <summary>
    /// Evento emitido en tiempo real para actualizar la animación gráfica de barras y emitir sonidos.
    /// </summary>
    public class SortVisualUpdate
    {
        public VisualUpdateType Type { get; set; }
        public int Index1 { get; set; } = -1;
        public int Index2 { get; set; } = -1;
        public string Description { get; set; } = string.Empty;
        public long CurrentComparisons { get; set; }
        public long CurrentMovements { get; set; }
    }
}

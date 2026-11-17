using System;
using System.Threading.Tasks;

namespace TimSort.Services
{
    /// <summary>
    /// Servicio de audio para retroalimentación sonora durante los intercambios del algoritmo.
    /// Ejecuta tonos en frecuencias dependientes del valor relativo del elemento.
    /// </summary>
    public static class SoundService
    {
        private static DateTime _lastToneTime = DateTime.MinValue;
        private static readonly object _lock = new();

        /// <summary>
        /// Reproduce un tono cuya frecuencia varía según el valor normalizado (0.0 a 1.0).
        /// </summary>
        public static void PlayFrequency(double normalizedValue)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                // Limitar la cadencia de emisión para no saturar el buffer de audio ni retrasar la UI
                if ((now - _lastToneTime).TotalMilliseconds < 35) return;
                _lastToneTime = now;
            }

            double clamped = Math.Clamp(normalizedValue, 0.0, 1.0);
            int frequency = 220 + (int)(clamped * 1050); // 220 Hz a 1270 Hz

            Task.Run(() =>
            {
                try
                {
                    Console.Beep(frequency, 25);
                }
                catch
                {
                    // Si el sistema no soporta Console.Beep, no interrumpe el flujo
                }
            });
        }

        /// <summary>
        /// Reproduce una melodía corta al finalizar el ordenamiento exitosamente.
        /// </summary>
        public static void PlaySuccessChord()
        {
            Task.Run(() =>
            {
                try
                {
                    Console.Beep(523, 60); // C5
                    Console.Beep(659, 60); // E5
                    Console.Beep(784, 80); // G5
                    Console.Beep(1046, 120); // C6
                }
                catch
                {
                    try { System.Media.SystemSounds.Asterisk.Play(); } catch { }
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using TimSort.Models;

namespace TimSort.Algorithms
{
    public static class TimSort
    {
        private const int MIN_MERGE = 32;

        public static int GetMinRun(int n)
        {
            int r = 0;
            while (n >= MIN_MERGE)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }

        private static void InsertionSort<T>(List<T> array, int left, int right, bool ascending, SortMetrics metrics) where T : IComparable<T>
        {
            for (int i = left + 1; i <= right; i++)
            {
                T key = array[i];
                int j = i - 1;

                metrics.AddStep($"[Inserción] Tomando elemento clave '{key}' en pos {i}");

                while (j >= left)
                {
                    metrics.ComparisonCount++;
                    bool condition = ascending ? array[j].CompareTo(key) > 0 : array[j].CompareTo(key) < 0;

                    metrics.AddStep($"  [Comparación #{metrics.ComparisonCount}] '{array[j]}' vs clave '{key}' -> {(condition ? "Requiere desplazar" : "Posición correcta")}");

                    if (!condition) break;

                    array[j + 1] = array[j];
                    metrics.MovementCount++;
                    metrics.AddStep($"  [Desplazamiento #{metrics.MovementCount}] Moviendo '{array[j]}' de pos {j} a {j + 1}");
                    j--;
                }

                if (j + 1 != i)
                {
                    array[j + 1] = key;
                    metrics.MovementCount++;
                    metrics.AddStep($"[Inserción Final] Colocando clave '{key}' en pos {j + 1}");
                }
            }
        }

        private static void Merge<T>(List<T> array, int l, int m, int r, bool ascending, SortMetrics metrics) where T : IComparable<T>
        {
            int len1 = m - l + 1;
            int len2 = r - m;

            metrics.AddStep($"[Fusión / Merge] Fusionando bloque [{l}..{m}] ({len1} elems) con [{m + 1}..{r}] ({len2} elems)");

            List<T> leftList = new(len1);
            List<T> rightList = new(len2);

            for (int x = 0; x < len1; x++) leftList.Add(array[l + x]);
            for (int y = 0; y < len2; y++) rightList.Add(array[m + 1 + y]);

            int i = 0, j = 0, k = l;

            while (i < len1 && j < len2)
            {
                metrics.ComparisonCount++;
                bool condition = ascending
                    ? (leftList[i].CompareTo(rightList[j]) <= 0)
                    : (leftList[i].CompareTo(rightList[j]) >= 0);

                metrics.AddStep($"  [Comparación Merge #{metrics.ComparisonCount}] '{leftList[i]}' vs '{rightList[j]}'");

                if (condition)
                {
                    array[k] = leftList[i];
                    metrics.MovementCount++;
                    metrics.AddStep($"  [Colocación #{metrics.MovementCount}] Poniendo '{leftList[i]}' en índice {k}");
                    i++;
                }
                else
                {
                    array[k] = rightList[j];
                    metrics.MovementCount++;
                    metrics.AddStep($"  [Colocación #{metrics.MovementCount}] Poniendo '{rightList[j]}' en índice {k}");
                    j++;
                }
                k++;
            }

            while (i < len1)
            {
                array[k] = leftList[i];
                metrics.MovementCount++;
                metrics.AddStep($"  [Restante Izq #{metrics.MovementCount}] Poniendo '{leftList[i]}' en índice {k}");
                i++;
                k++;
            }

            while (j < len2)
            {
                array[k] = rightList[j];
                metrics.MovementCount++;
                metrics.AddStep($"  [Restante Der #{metrics.MovementCount}] Poniendo '{rightList[j]}' en índice {k}");
                j++;
                k++;
            }
        }

        /// <summary>
        /// Ejecuta el algoritmo TimSort midiendo comparaciones, movimientos y registrando la traza de pasos.
        /// </summary>
        public static SortMetrics Sort<T>(List<T> array, bool ascending = true, bool recordSteps = true, int maxTraceSteps = 1000) where T : IComparable<T>
        {
            var metrics = new SortMetrics
            {
                RecordSteps = recordSteps,
                MaxTraceSteps = maxTraceSteps
            };

            if (array == null || array.Count <= 1)
            {
                metrics.AddStep("El arreglo contiene 0 o 1 elementos. Ya se encuentra ordenado.");
                return metrics;
            }

            int n = array.Count;
            int minRun = GetMinRun(n);
            if (minRun <= 0) minRun = 1;
            metrics.MinRun = minRun;

            metrics.AddStep($"[Inicio TimSort] Total elementos: {n}, MinRun calculado: {minRun}, Dirección: {(ascending ? "Ascendente" : "Descendente")}");

            // 1. Ordenar runs individuales con Insertion Sort
            for (int start = 0; start < n; start += minRun)
            {
                int end = Math.Min(start + minRun - 1, n - 1);
                metrics.AddStep($"--- [Run] Ordenando sub-run rango [{start}..{end}] con InsertionSort ---");
                InsertionSort(array, start, end, ascending, metrics);
            }

            // 2. Fusionar runs de forma iterativa
            for (int size = minRun; size < n; size *= 2)
            {
                for (int left = 0; left < n; left += 2 * size)
                {
                    int mid = Math.Min(left + size - 1, n - 1);
                    int right = Math.Min(left + 2 * size - 1, n - 1);
                    if (mid < right)
                    {
                        Merge(array, left, mid, right, ascending, metrics);
                    }
                }
            }

            metrics.AddStep($"[Fin TimSort] Proceso completado. Total comparaciones: {metrics.ComparisonCount:N0}, Total movimientos: {metrics.MovementCount:N0}");

            return metrics;
        }
    }
}

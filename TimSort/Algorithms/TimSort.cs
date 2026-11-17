using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        #region Verificación de Ordenamiento

        /// <summary>
        /// Verifica si todos los elementos del arreglo se encuentran en el orden especificado.
        /// </summary>
        public static bool VerifySorted<T>(IReadOnlyList<T> array, bool ascending, IComparer<T>? comparer, out string message)
        {
            if (array == null || array.Count <= 1)
            {
                message = "✔ Verificado: Arreglo ordenado correctamente (0 o 1 elementos).";
                return true;
            }

            comparer ??= Comparer<T>.Default;

            for (int i = 0; i < array.Count - 1; i++)
            {
                int cmp = comparer.Compare(array[i], array[i + 1]);
                bool valid = ascending ? cmp <= 0 : cmp >= 0;

                if (!valid)
                {
                    string dirText = ascending ? "ascendente" : "descendente";
                    message = $"❌ Error de orden en índice {i} y {i + 1}: '{array[i]}' y '{array[i + 1]}' no cumplen orden {dirText}.";
                    return false;
                }
            }

            message = $"✔ ¡Ordenamiento Correcto Verificado! (100% de los {array.Count} elementos en orden {(ascending ? "ascendente" : "descendente")}).";
            return true;
        }

        #endregion

        #region TimSort Sincrónico (Instantáneo con Métricas y Snapshots)

        private static void InsertionSort<T>(List<T> array, int left, int right, bool ascending, IComparer<T> comparer, SortMetrics metrics, HashSet<int> modifiedIndices)
        {
            for (int i = left + 1; i <= right; i++)
            {
                T key = array[i];
                int j = i - 1;

                metrics.AddStep($"[Inserción] Tomando elemento clave '{key}' en posición {i}");

                while (j >= left)
                {
                    metrics.ComparisonCount++;
                    int cmp = comparer.Compare(array[j], key);
                    bool condition = ascending ? cmp > 0 : cmp < 0;

                    metrics.AddStep($"  [Comparación #{metrics.ComparisonCount}] '{array[j]}' vs clave '{key}' -> {(condition ? "Requiere desplazar" : "Posición correcta")}");

                    if (!condition) break;

                    array[j + 1] = array[j];
                    metrics.MovementCount++;
                    modifiedIndices.Add(j + 1);
                    metrics.AddMovement($"[Desplazamiento #{metrics.MovementCount}] Mover '{array[j]}' de pos {j} a {j + 1}");
                    metrics.AddStep($"  [Desplazamiento #{metrics.MovementCount}] Moviendo '{array[j]}' de pos {j} a {j + 1}");
                    j--;
                }

                if (j + 1 != i)
                {
                    array[j + 1] = key;
                    metrics.MovementCount++;
                    modifiedIndices.Add(j + 1);
                    metrics.AddMovement($"[Inserción Clave #{metrics.MovementCount}] Colocar '{key}' en pos {j + 1}");
                    metrics.AddStep($"[Inserción Final] Colocando clave '{key}' en pos {j + 1}");
                }
            }
        }

        private static void Merge<T>(List<T> array, int l, int m, int r, bool ascending, IComparer<T> comparer, SortMetrics metrics, HashSet<int> modifiedIndices)
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
                int cmp = comparer.Compare(leftList[i], rightList[j]);
                bool condition = ascending ? cmp <= 0 : cmp >= 0;

                metrics.AddStep($"  [Comparación Merge #{metrics.ComparisonCount}] '{leftList[i]}' vs '{rightList[j]}'");

                if (condition)
                {
                    array[k] = leftList[i];
                    metrics.MovementCount++;
                    modifiedIndices.Add(k);
                    metrics.AddMovement($"[Fusión #{metrics.MovementCount}] Colocar '{leftList[i]}' en pos {k}");
                    metrics.AddStep($"  [Colocación #{metrics.MovementCount}] Poniendo '{leftList[i]}' en índice {k}");
                    i++;
                }
                else
                {
                    array[k] = rightList[j];
                    metrics.MovementCount++;
                    modifiedIndices.Add(k);
                    metrics.AddMovement($"[Fusión #{metrics.MovementCount}] Colocar '{rightList[j]}' en pos {k}");
                    metrics.AddStep($"  [Colocación #{metrics.MovementCount}] Poniendo '{rightList[j]}' en índice {k}");
                    j++;
                }
                k++;
            }

            while (i < len1)
            {
                array[k] = leftList[i];
                metrics.MovementCount++;
                modifiedIndices.Add(k);
                metrics.AddMovement($"[Fusión Restante Izq #{metrics.MovementCount}] Colocar '{leftList[i]}' en pos {k}");
                metrics.AddStep($"  [Restante Izq #{metrics.MovementCount}] Poniendo '{leftList[i]}' en índice {k}");
                i++;
                k++;
            }

            while (j < len2)
            {
                array[k] = rightList[j];
                metrics.MovementCount++;
                modifiedIndices.Add(k);
                metrics.AddMovement($"[Fusión Restante Der #{metrics.MovementCount}] Colocar '{rightList[j]}' en pos {k}");
                metrics.AddStep($"  [Restante Der #{metrics.MovementCount}] Poniendo '{rightList[j]}' en índice {k}");
                j++;
                k++;
            }
        }

        /// <summary>
        /// Ejecuta TimSort sincrónicamente generando bitácora, métricas e instantáneas (snapshots)
        /// para la matriz visual horizontal por filas.
        /// </summary>
        public static SortMetrics Sort<T>(
            List<T> array,
            bool ascending = true,
            IComparer<T>? comparer = null,
            bool recordSteps = true,
            int maxTraceSteps = 2000)
        {
            comparer ??= Comparer<T>.Default;

            var metrics = new SortMetrics
            {
                RecordSteps = recordSteps,
                MaxTraceSteps = maxTraceSteps
            };

            if (array == null || array.Count <= 1)
            {
                metrics.AddStep("El arreglo contiene 0 o 1 elementos. Ya se encuentra ordenado.");
                VerifySorted(array ?? new List<T>(), ascending, comparer, out string singleMsg);
                metrics.IsSortedSuccessfully = true;
                metrics.VerificationMessage = singleMsg;
                if (array != null && array.Count > 0)
                {
                    metrics.Snapshots.Add(new SortStepSnapshot(0, "0 - Inicial / Final", array.Cast<object>()));
                }
                return metrics;
            }

            int n = array.Count;
            int minRun = GetMinRun(n);
            if (minRun <= 0) minRun = 1;
            metrics.MinRun = minRun;

            int snapshotIndex = 0;
            // Instantánea Inicial (Paso 0)
            metrics.Snapshots.Add(new SortStepSnapshot(snapshotIndex++, "0 - Estado Inicial", array.Cast<object>()));

            metrics.AddStep($"[Inicio TimSort] Total elementos: {n}, MinRun calculado: {minRun}, Dirección: {(ascending ? "Ascendente" : "Descendente")}");

            // 1. Ordenar runs individuales con InsertionSort
            int runNumber = 1;
            for (int start = 0; start < n; start += minRun)
            {
                int end = Math.Min(start + minRun - 1, n - 1);
                var modifiedInRun = new HashSet<int>();
                metrics.AddStep($"--- [Run {runNumber}] Rango [{start}..{end}] con InsertionSort ---");

                InsertionSort(array, start, end, ascending, comparer, metrics, modifiedInRun);

                // Registrar snapshot de este run
                metrics.Snapshots.Add(new SortStepSnapshot(
                    snapshotIndex++,
                    $"Run {runNumber} [{start}..{end}]",
                    array.Cast<object>(),
                    modifiedInRun.Count > 0 ? modifiedInRun : Enumerable.Range(start, end - start + 1)
                ));
                runNumber++;
            }

            // 2. Fusionar runs de forma iterativa
            int mergeNumber = 1;
            for (int size = minRun; size < n; size *= 2)
            {
                for (int left = 0; left < n; left += 2 * size)
                {
                    int mid = Math.Min(left + size - 1, n - 1);
                    int right = Math.Min(left + 2 * size - 1, n - 1);
                    if (mid < right)
                    {
                        var modifiedInMerge = new HashSet<int>();
                        Merge(array, left, mid, right, ascending, comparer, metrics, modifiedInMerge);

                        metrics.Snapshots.Add(new SortStepSnapshot(
                            snapshotIndex++,
                            $"Merge {mergeNumber} [{left}..{mid}] con [{mid + 1}..{right}]",
                            array.Cast<object>(),
                            modifiedInMerge.Count > 0 ? modifiedInMerge : Enumerable.Range(left, right - left + 1)
                        ));
                        mergeNumber++;
                    }
                }
            }

            // Instantánea Final
            metrics.Snapshots.Add(new SortStepSnapshot(snapshotIndex, "Fin - Arreglo Ordenado", array.Cast<object>()));

            // Verificación del orden final
            metrics.IsSortedSuccessfully = VerifySorted(array, ascending, comparer, out string verifyMsg);
            metrics.VerificationMessage = verifyMsg;

            metrics.AddStep($"[Fin TimSort] Proceso completado. Total comparaciones: {metrics.ComparisonCount:N0}, Total movimientos: {metrics.MovementCount:N0}");

            return metrics;
        }

        #endregion

        #region TimSort Asincrónico para el Visualizador Gráfico en Tiempo Real

        /// <summary>
        /// Ejecuta TimSort emitiendo actualizaciones visuales en tiempo real para el panel de barras y sonidos.
        /// </summary>
        public static async Task<SortMetrics> SortVisualAsync<T>(
            List<T> array,
            bool ascending,
            IComparer<T>? comparer,
            Func<SortVisualUpdate, Task>? onVisualUpdate,
            CancellationToken cancellationToken = default)
        {
            comparer ??= Comparer<T>.Default;

            var metrics = new SortMetrics
            {
                RecordSteps = true,
                MaxTraceSteps = 2000
            };

            if (array == null || array.Count <= 1)
            {
                VerifySorted(array ?? new List<T>(), ascending, comparer, out string singleMsg);
                metrics.IsSortedSuccessfully = true;
                metrics.VerificationMessage = singleMsg;
                if (onVisualUpdate != null)
                {
                    await onVisualUpdate(new SortVisualUpdate
                    {
                        Type = VisualUpdateType.Complete,
                        Description = "Arreglo de 0 o 1 elemento."
                    });
                }
                return metrics;
            }

            int n = array.Count;
            int minRun = GetMinRun(n);
            if (minRun <= 0) minRun = 1;
            metrics.MinRun = minRun;

            // 1. InsertionSort sobre runs
            for (int start = 0; start < n; start += minRun)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int end = Math.Min(start + minRun - 1, n - 1);

                for (int i = start + 1; i <= end; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    T key = array[i];
                    int j = i - 1;

                    while (j >= start)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        metrics.ComparisonCount++;

                        if (onVisualUpdate != null)
                        {
                            await onVisualUpdate(new SortVisualUpdate
                            {
                                Type = VisualUpdateType.Compare,
                                Index1 = j,
                                Index2 = i,
                                CurrentComparisons = metrics.ComparisonCount,
                                CurrentMovements = metrics.MovementCount,
                                Description = $"Comparando '{array[j]}' (pos {j}) con '{key}'"
                            });
                        }

                        int cmp = comparer.Compare(array[j], key);
                        bool condition = ascending ? cmp > 0 : cmp < 0;

                        if (!condition) break;

                        array[j + 1] = array[j];
                        metrics.MovementCount++;
                        metrics.AddMovement($"[Desplazamiento #{metrics.MovementCount}] Mover '{array[j]}' de pos {j} a {j + 1}");

                        if (onVisualUpdate != null)
                        {
                            await onVisualUpdate(new SortVisualUpdate
                            {
                                Type = VisualUpdateType.Move,
                                Index1 = j,
                                Index2 = j + 1,
                                CurrentComparisons = metrics.ComparisonCount,
                                CurrentMovements = metrics.MovementCount,
                                Description = $"Moviendo '{array[j]}' a pos {j + 1}"
                            });
                        }

                        j--;
                    }

                    if (j + 1 != i)
                    {
                        array[j + 1] = key;
                        metrics.MovementCount++;
                        metrics.AddMovement($"[Inserción #{metrics.MovementCount}] Colocar '{key}' en pos {j + 1}");

                        if (onVisualUpdate != null)
                        {
                            await onVisualUpdate(new SortVisualUpdate
                            {
                                Type = VisualUpdateType.Write,
                                Index1 = j + 1,
                                CurrentComparisons = metrics.ComparisonCount,
                                CurrentMovements = metrics.MovementCount,
                                Description = $"Insertando clave '{key}' en pos {j + 1}"
                            });
                        }
                    }
                }

                if (onVisualUpdate != null)
                {
                    await onVisualUpdate(new SortVisualUpdate
                    {
                        Type = VisualUpdateType.RunComplete,
                        Index1 = start,
                        Index2 = end,
                        CurrentComparisons = metrics.ComparisonCount,
                        CurrentMovements = metrics.MovementCount,
                        Description = $"Sub-run [{start}..{end}] completado."
                    });
                }
            }

            // 2. Fusionar runs
            for (int size = minRun; size < n; size *= 2)
            {
                for (int left = 0; left < n; left += 2 * size)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int mid = Math.Min(left + size - 1, n - 1);
                    int right = Math.Min(left + 2 * size - 1, n - 1);

                    if (mid < right)
                    {
                        int len1 = mid - left + 1;
                        int len2 = right - mid;

                        List<T> leftList = new(len1);
                        List<T> rightList = new(len2);

                        for (int x = 0; x < len1; x++) leftList.Add(array[left + x]);
                        for (int y = 0; y < len2; y++) rightList.Add(array[mid + 1 + y]);

                        int i = 0, j = 0, k = left;

                        while (i < len1 && j < len2)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            metrics.ComparisonCount++;

                            if (onVisualUpdate != null)
                            {
                                await onVisualUpdate(new SortVisualUpdate
                                {
                                    Type = VisualUpdateType.Compare,
                                    Index1 = left + i,
                                    Index2 = mid + 1 + j,
                                    CurrentComparisons = metrics.ComparisonCount,
                                    CurrentMovements = metrics.MovementCount,
                                    Description = $"Comparando izquierda '{leftList[i]}' con derecha '{rightList[j]}'"
                                });
                            }

                            int cmp = comparer.Compare(leftList[i], rightList[j]);
                            bool condition = ascending ? cmp <= 0 : cmp >= 0;

                            if (condition)
                            {
                                array[k] = leftList[i];
                                metrics.MovementCount++;
                                metrics.AddMovement($"[Merge #{metrics.MovementCount}] Colocar '{leftList[i]}' en pos {k}");

                                if (onVisualUpdate != null)
                                {
                                    await onVisualUpdate(new SortVisualUpdate
                                    {
                                        Type = VisualUpdateType.Write,
                                        Index1 = k,
                                        CurrentComparisons = metrics.ComparisonCount,
                                        CurrentMovements = metrics.MovementCount,
                                        Description = $"Colocando '{leftList[i]}' en índice {k}"
                                    });
                                }
                                i++;
                            }
                            else
                            {
                                array[k] = rightList[j];
                                metrics.MovementCount++;
                                metrics.AddMovement($"[Merge #{metrics.MovementCount}] Colocar '{rightList[j]}' en pos {k}");

                                if (onVisualUpdate != null)
                                {
                                    await onVisualUpdate(new SortVisualUpdate
                                    {
                                        Type = VisualUpdateType.Write,
                                        Index1 = k,
                                        CurrentComparisons = metrics.ComparisonCount,
                                        CurrentMovements = metrics.MovementCount,
                                        Description = $"Colocando '{rightList[j]}' en índice {k}"
                                    });
                                }
                                j++;
                            }
                            k++;
                        }

                        while (i < len1)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            array[k] = leftList[i];
                            metrics.MovementCount++;
                            metrics.AddMovement($"[Merge Izq #{metrics.MovementCount}] Colocar '{leftList[i]}' en pos {k}");

                            if (onVisualUpdate != null)
                            {
                                await onVisualUpdate(new SortVisualUpdate
                                {
                                    Type = VisualUpdateType.Write,
                                    Index1 = k,
                                    CurrentComparisons = metrics.ComparisonCount,
                                    CurrentMovements = metrics.MovementCount,
                                    Description = $"Restante izq '{leftList[i]}' en índice {k}"
                                });
                            }
                            i++;
                            k++;
                        }

                        while (j < len2)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            array[k] = rightList[j];
                            metrics.MovementCount++;
                            metrics.AddMovement($"[Merge Der #{metrics.MovementCount}] Colocar '{rightList[j]}' en pos {k}");

                            if (onVisualUpdate != null)
                            {
                                await onVisualUpdate(new SortVisualUpdate
                                {
                                    Type = VisualUpdateType.Write,
                                    Index1 = k,
                                    CurrentComparisons = metrics.ComparisonCount,
                                    CurrentMovements = metrics.MovementCount,
                                    Description = $"Restante der '{rightList[j]}' en índice {k}"
                                });
                            }
                            j++;
                            k++;
                        }
                    }
                }
            }

            metrics.IsSortedSuccessfully = VerifySorted(array, ascending, comparer, out string verifyMsg);
            metrics.VerificationMessage = verifyMsg;

            if (onVisualUpdate != null)
            {
                await onVisualUpdate(new SortVisualUpdate
                {
                    Type = VisualUpdateType.Complete,
                    CurrentComparisons = metrics.ComparisonCount,
                    CurrentMovements = metrics.MovementCount,
                    Description = "Ordenamiento completado."
                });
            }

            return metrics;
        }

        #endregion
    }
}

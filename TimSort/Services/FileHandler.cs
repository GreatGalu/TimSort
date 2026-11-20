using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using TimSort.Models;

namespace TimSort.Services
{
    public static class FileHandler
    {
        private static readonly Random Rand = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Arreglo de respaldo con más de 120 palabras reales variadas en español,
        /// con acentos, diéresis, letra 'ñ' y combinación de mayúsculas/minúsculas.
        /// </summary>
        private static readonly string[] FallbackRealWords = new string[]
        {
            "Úrsula", "último", "pingüino", "árbol", "Ángel", "alga", "niño", "Ñandú", "Éxito", "elefante",
            "Óscar", "ozono", "cigüeña", "vergüenza", "paragüero", "bilingüe", "agüita", "pedigüeño", "ungüento", "argüir",
            "año", "otoño", "mañana", "leña", "caña", "piña", "señal", "muñeca", "añejo", "campaña",
            "sueño", "daño", "África", "América", "ávido", "ágil", "átomo", "ámbar", "árbitro", "álbum",
            "ánfora", "áspero", "Época", "éxtasis", "ético", "ébano", "élite", "épico", "éter", "éxodo",
            "Índice", "ídolo", "íntimo", "ítem", "ígneo", "ícono", "Ópera", "ómnibus", "óptimo", "órbita",
            "óxido", "óvulo", "órgano", "óleo", "Único", "útil", "úlcera", "útero", "uranio", "universo",
            "algoritmo", "estructura", "memoria", "variable", "función", "interfaz", "arreglo", "pila", "cola", "grafo",
            "recursión", "búsqueda", "binaria", "diccionario", "cadena", "proceso", "bloqueo", "servidor", "cliente", "consulta",
            "transacción", "cifrado", "autenticación", "sesión", "módulo", "paquete", "terminal", "consola", "excepción", "delegado",
            "expresión", "archivo", "directorio", "diseño", "patrón", "sistema", "núcleo", "rendimiento", "prueba", "despliegue",
            "nube", "servicio", "código", "fuente", "mantenibilidad", "escalabilidad", "arquitectura", "controlador", "modelo", "vista",
            "componente", "método", "parámetro", "condicional", "modularidad", "abstracción", "encapsulamiento", "barco", "cielo", "fuego",
            "tierra", "viento", "sol", "luna", "estrella", "montaña", "río", "bosque", "puente", "camino", "playa", "selva"
        };

        /// <summary>
        /// Carga el conjunto de palabras reales desde el archivo "DatosPrueba/palabras_200.txt".
        /// Si no se encuentra, recurre al catálogo de respaldo en memoria.
        /// </summary>
        public static List<string> LoadRealWordsPool()
        {
            string[] candidatePaths = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosPrueba", "palabras_200.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "DatosPrueba", "palabras_200.txt"),
                Path.Combine(Directory.GetCurrentDirectory(), "DatosPrueba", "palabras_200.txt"),
                Path.Combine("DatosPrueba", "palabras_200.txt")
            };

            foreach (string path in candidatePaths)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        var lines = File.ReadAllLines(path, Encoding.UTF8)
                            .Select(line => line.Trim())
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();

                        if (lines.Count > 0)
                        {
                            return lines;
                        }
                    }
                }
                catch
                {
                    // Intentar con la siguiente ruta candidata
                }
            }

            return new List<string>(FallbackRealWords);
        }

        /// <summary>
        /// Guarda los datos en el formato adecuado según la extensión del archivo (.json, .xml, .txt, .csv).
        /// </summary>
        public static void SaveToFile<T>(IEnumerable<T> data, string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

                switch (extension)
                {
                    case ".json":
                        SaveAsJson(data, filePath);
                        break;

                    case ".xml":
                        SaveAsXml(data, filePath);
                        break;

                    case ".csv":
                    case ".txt":
                    default:
                        SaveAsPlainText(data, filePath);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Error al guardar el archivo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Carga los datos automáticamente reconociendo el formato (.json, .xml, .txt, .csv)
        /// y convirtiéndolos al tipo genérico T (int, double, char, string, CustomItem).
        /// </summary>
        public static List<T> LoadFromFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("El archivo especificado no fue encontrado.");
            }

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            try
            {
                return extension switch
                {
                    ".json" => LoadFromJson<T>(filePath),
                    ".xml" => LoadFromXml<T>(filePath),
                    _ => LoadFromPlainText<T>(filePath)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer o interpretar el archivo '{Path.GetFileName(filePath)}': {ex.Message}", ex);
            }
        }

        #region Guardado por Formato

        private static void SaveAsJson<T>(IEnumerable<T> data, string filePath)
        {
            string json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        private static void SaveAsXml<T>(IEnumerable<T> data, string filePath)
        {
            var root = new XElement("Items");

            foreach (var item in data)
            {
                if (item is CustomItem ci)
                {
                    root.Add(new XElement("Item",
                        new XElement("Id", ci.Id),
                        new XElement("Name", ci.Name)
                    ));
                }
                else
                {
                    root.Add(new XElement("Item", item?.ToString() ?? string.Empty));
                }
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            doc.Save(filePath);
        }

        private static void SaveAsPlainText<T>(IEnumerable<T> data, string filePath)
        {
            var lines = data.Select(item =>
            {
                if (item is CustomItem ci)
                {
                    return $"{ci.Id},{ci.Name}";
                }
                return item?.ToString() ?? string.Empty;
            });

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        #endregion

        #region Carga por Formato

        private static List<T> LoadFromJson<T>(string filePath)
        {
            string json = File.ReadAllText(filePath, Encoding.UTF8);
            var result = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
            return result ?? new List<T>();
        }

        private static List<T> LoadFromXml<T>(string filePath)
        {
            var list = new List<T>();
            var doc = XDocument.Load(filePath);
            var items = doc.Root?.Elements() ?? Enumerable.Empty<XElement>();

            Type targetType = typeof(T);

            foreach (var el in items)
            {
                if (targetType == typeof(CustomItem))
                {
                    var idEl = el.Element("Id");
                    var nameEl = el.Element("Name");
                    if (idEl != null && int.TryParse(idEl.Value, out int id))
                    {
                        string name = nameEl?.Value ?? string.Empty;
                        list.Add((T)(object)new CustomItem(id, name));
                    }
                }
                else
                {
                    if (TryParseValue<T>(el.Value, out T? val) && val is not null)
                    {
                        list.Add(val);
                    }
                }
            }

            return list;
        }

        private static List<T> LoadFromPlainText<T>(string filePath)
        {
            var list = new List<T>();
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            Type targetType = typeof(T);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (targetType == typeof(CustomItem))
                {
                    var parts = line.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[0].Trim(), out int id))
                    {
                        list.Add((T)(object)new CustomItem(id, parts[1].Trim()));
                    }
                    else if (line.Contains("[ID:") && line.Contains("]"))
                    {
                        int start = line.IndexOf("[ID:", StringComparison.OrdinalIgnoreCase) + 4;
                        int end = line.IndexOf(']', start);
                        if (start > 3 && end > start && int.TryParse(line[start..end].Trim(), out int parsedId))
                        {
                            string parsedName = line[(end + 1)..].Trim();
                            list.Add((T)(object)new CustomItem(parsedId, parsedName));
                        }
                    }
                    else if (int.TryParse(line, out int singleId))
                    {
                        list.Add((T)(object)new CustomItem(singleId, $"Item_{singleId}"));
                    }
                }
                else
                {
                    string[] tokens = line.Split(new[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string token in tokens)
                    {
                        if (TryParseValue<T>(token.Trim(), out T? val) && val is not null)
                        {
                            list.Add(val);
                        }
                    }
                }
            }

            return list;
        }

        private static bool TryParseValue<T>(string input, out T? result)
        {
            result = default;
            Type type = typeof(T);

            try
            {
                if (type == typeof(int))
                {
                    if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iVal) ||
                        int.TryParse(input, NumberStyles.Integer, CultureInfo.CurrentCulture, out iVal))
                    {
                        result = (T)(object)iVal;
                        return true;
                    }
                    return false;
                }

                if (type == typeof(double))
                {
                    string normalized = input.Replace(',', '.');
                    if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double dVal))
                    {
                        result = (T)(object)dVal;
                        return true;
                    }
                    return false;
                }

                if (type == typeof(char))
                {
                    if (!string.IsNullOrEmpty(input))
                    {
                        result = (T)(object)input[0];
                        return true;
                    }
                    return false;
                }

                if (type == typeof(string))
                {
                    result = (T)(object)input;
                    return true;
                }

                var converted = Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                if (converted is not null)
                {
                    result = (T)converted;
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Genera elementos aleatorios según la cantidad y el tipo de dato.
        /// Para cadenas, utiliza palabras reales del archivo DatosPrueba/palabras_200.txt (o catálogo de respaldo)
        /// sin sufijos numéricos.
        /// </summary>
        public static List<T> GenerateRandomData<T>(int count, int minValue = 1, int maxValue = 1000)
        {
            List<T> list = new(count);
            Type type = typeof(T);

            char[] sampleChars = {
                'A', 'a', 'á', 'Á', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'é', 'É',
                'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i', 'í', 'Í', 'J', 'j', 'L', 'l',
                'M', 'm', 'N', 'n', 'ñ', 'Ñ', 'O', 'o', 'ó', 'Ó', 'P', 'p', 'Q', 'q',
                'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'ú', 'Ú', 'ü', 'Ü', 'V', 'v',
                'Z', 'z'
            };

            string[] customItemNames = {
                "Laptop", "Teclado", "Mouse", "Monitor", "Servidor", "Router", "Switch", "Tablet", "Memoria", "Disco"
            };

            if (type == typeof(string))
            {
                var pool = LoadRealWordsPool();
                // Mezcla aleatoria de palabras reales
                var shuffled = pool.OrderBy(_ => Rand.Next()).ToList();

                for (int i = 0; i < count; i++)
                {
                    string word = shuffled[i % shuffled.Count];
                    list.Add((T)(object)word);
                }
                return list;
            }

            for (int i = 0; i < count; i++)
            {
                object value;

                if (type == typeof(int))
                {
                    value = Rand.Next(minValue, maxValue + 1);
                }
                else if (type == typeof(double))
                {
                    value = Math.Round(Rand.NextDouble() * (maxValue - minValue) + minValue, 2);
                }
                else if (type == typeof(char))
                {
                    value = sampleChars[Rand.Next(sampleChars.Length)];
                }
                else if (type == typeof(CustomItem))
                {
                    int id = Rand.Next(minValue, maxValue + 1);
                    string name = customItemNames[Rand.Next(customItemNames.Length)] + " " + id;
                    value = new CustomItem(id, name);
                }
                else
                {
                    value = default(T)!;
                }

                list.Add((T)value);
            }

            return list;
        }
    }
}

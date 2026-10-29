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
                    // Soporta formatos "1,Nombre", "1;Nombre" o formato de ToString "[ID: 1] Nombre"
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
                    // Puede venir un solo elemento por línea o múltiples separados por comas/espacios/puntos y comas
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
                    // Soporta separador decimal de punto '.' o de coma ','
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
        /// Genera elementos aleatorios de forma dinámica según la cantidad y el tipo de dato.
        /// </summary>
        public static List<T> GenerateRandomData<T>(int count, int minValue = 1, int maxValue = 1000) where T : IComparable<T>
        {
            List<T> list = new(count);
            Type type = typeof(T);

            string[] sampleWords = { "Alpha", "Beta", "Gamma", "Delta", "TimSort", "Code", "Data", "Struct", "Node", "List", "Array", "Buffer" };
            string[] customItemNames = { "Laptop", "Teclado", "Mouse", "Monitor", "Servidor", "Router", "Switch", "Tablet", "Memoria", "Disco" };

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
                    value = (char)Rand.Next('A', 'Z' + 1);
                }
                else if (type == typeof(string))
                {
                    value = sampleWords[Rand.Next(sampleWords.Length)] + "_" + Rand.Next(1, 999);
                }
                else if (type == typeof(CustomItem))
                {
                    int id = Rand.Next(minValue, maxValue + 1);
                    string name = customItemNames[Rand.Next(customItemNames.Length)] + "_" + (i + 1);
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
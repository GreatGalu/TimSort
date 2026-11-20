using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TimSort.Services
{
    /// <summary>
    /// Comparador y utilidades de normalización para cadenas y caracteres.
    /// Unifica mayúsculas/minúsculas y acentos/diacríticos respetando el alfabeto en español ('ñ').
    /// </summary>
    public class NormalizedStringComparer : IComparer<string>
    {
        public static readonly NormalizedStringComparer Instance = new();
        private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            // Comparación primaria: ignora mayúsculas/minúsculas y acentos/diéresis usando reglas del español (para 'ñ')
            int result = SpanishCulture.CompareInfo.Compare(
                x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);

            if (result != 0) return result;

            // Desempate determinista secundario
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Remueve diacríticos de una cadena usando normalización Unicode FormD, preservando la 'ñ' y 'Ñ'.
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var sb = new StringBuilder(text.Length);
            foreach (char ch in text)
            {
                if (ch == 'ñ' || ch == 'Ñ')
                {
                    sb.Append(ch);
                    continue;
                }

                string normalized = ch.ToString().Normalize(NormalizationForm.FormD);
                foreach (char nch in normalized)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(nch) != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(nch);
                    }
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Mapea un carácter normalizado al rango 1..27 en base al alfabeto español:
        /// 'a'..'n' -> 1..14, 'ñ' -> 15, 'o'..'z' -> 16..27.
        /// Caracteres no reconocidos o vacíos retornan 0.
        /// </summary>
        public static int GetNormalizedCharValue(char c)
        {
            // Conservar la 'ñ' / 'Ñ' entre 'n' y 'o'
            if (c == 'ñ' || c == 'Ñ')
            {
                return 15;
            }

            // Normalización FormD para quitar diacríticos (ej. 'á', 'Á', 'à' -> 'a')
            string normalized = c.ToString().Normalize(NormalizationForm.FormD);
            char baseChar = char.ToLowerInvariant(normalized[0]);

            if (baseChar >= 'a' && baseChar <= 'n')
            {
                return (baseChar - 'a') + 1; // 1 a 14
            }
            if (baseChar >= 'o' && baseChar <= 'z')
            {
                return (baseChar - 'o') + 16; // 16 a 27
            }

            return 0;
        }

        /// <summary>
        /// Calcula el peso posicional normalizado de una palabra considerando sus primeras 5 letras en base 28:
        /// Weight = c0 * 28^4 + c1 * 28^3 + c2 * 28^2 + c3 * 28 + c4
        /// Incluye una fracción residual para letras posteriores para evitar empates idénticos.
        /// </summary>
        public static double GetNormalizedWordWeight(string? word)
        {
            if (string.IsNullOrEmpty(word)) return 0.0;

            const double BaseVal = 28.0;
            double weight = 0.0;

            // Primeras 5 letras en base 28
            for (int i = 0; i < 5; i++)
            {
                int charVal = (i < word.Length) ? GetNormalizedCharValue(word[i]) : 0;
                weight = weight * BaseVal + charVal;
            }

            // Fracción residual para letras posteriores (desempate progresivo)
            double factor = 1.0 / BaseVal;
            for (int i = 5; i < Math.Min(word.Length, 10); i++)
            {
                int charVal = GetNormalizedCharValue(word[i]);
                weight += charVal * factor;
                factor /= BaseVal;
            }

            return weight;
        }
    }

    /// <summary>
    /// Comparador de caracteres individuales insensible a mayúsculas/minúsculas y acentos/diéresis.
    /// </summary>
    public class NormalizedCharComparer : IComparer<char>
    {
        public static readonly NormalizedCharComparer Instance = new();
        private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");

        public int Compare(char x, char y)
        {
            string sx = x.ToString();
            string sy = y.ToString();

            int result = SpanishCulture.CompareInfo.Compare(
                sx, sy, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);

            if (result != 0) return result;

            return char.ToUpperInvariant(x).CompareTo(char.ToUpperInvariant(y));
        }
    }
}

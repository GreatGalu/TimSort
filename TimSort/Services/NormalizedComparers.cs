using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TimSort.Services
{
    /// <summary>
    /// Comparador de cadenas insensible a mayúsculas/minúsculas y acentos/diacríticos.
    /// Unifica letras con tilde, diéresis o variaciones (ej: u, U, ú, ü).
    /// </summary>
    public class NormalizedStringComparer : IComparer<string>
    {
        public static readonly NormalizedStringComparer Instance = new();

        public int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            // Comparación primaria: ignora mayúsculas/minúsculas y acentos/diéresis
            int result = CultureInfo.InvariantCulture.CompareInfo.Compare(
                x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);

            if (result != 0) return result;

            // Desempate determinista secundario
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Remueve diacríticos de una cadena usando normalización Unicode FormD.
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);
            foreach (char ch in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }

    /// <summary>
    /// Comparador de caracteres individuales insensible a mayúsculas/minúsculas y acentos/diéresis.
    /// </summary>
    public class NormalizedCharComparer : IComparer<char>
    {
        public static readonly NormalizedCharComparer Instance = new();

        public int Compare(char x, char y)
        {
            string sx = x.ToString();
            string sy = y.ToString();

            int result = CultureInfo.InvariantCulture.CompareInfo.Compare(
                sx, sy, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);

            if (result != 0) return result;

            return char.ToUpperInvariant(x).CompareTo(char.ToUpperInvariant(y));
        }
    }
}

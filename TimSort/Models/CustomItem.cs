using System;

namespace TimSort.Models
{
    /// <summary>
    /// Representa una entidad de objeto personalizado con un identificador numérico y un nombre.
    /// Implementa IComparable para permitir su ordenamiento con TimSort.
    /// </summary>
    public class CustomItem : IComparable<CustomItem>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CustomItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Compara dos objetos CustomItem basándose en su Id.
        /// </summary>
        public int CompareTo(CustomItem? other)
        {
            if (other is null) return 1;
            return this.Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return $"[ID: {Id,4}] {Name}";
        }
    }
}
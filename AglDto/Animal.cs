namespace AglDto
{
    /// <summary>
    /// The Animal class.
    /// </summary>

    public class Animal
    {
        /// <summary>
        /// Gets or sets a value of the animal name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value of the animal type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the animal is a cat
        /// </summary>
        public bool? IsCat => !string.IsNullOrEmpty(this.Type) ?
                    string.CompareOrdinal(this.Type, Properties.Settings.Default.CatAnimalTypeKey) == 0 :
                    (bool?)null;
    }
}

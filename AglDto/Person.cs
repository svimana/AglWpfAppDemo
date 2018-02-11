namespace AglDto
{
    using System.Collections.Generic;

    /// <summary>
    /// The Person class.
    /// </summary>

    public class Person
    {
        /// <summary>
        /// Gets or sets a value of the person name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value of the person gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets a value of the person age
        /// </summary>
        public int Age { get; set; }

        public IList<Animal> Pets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the person is male
        /// </summary>
        public bool IsMale
        {
            get
            {
                return string.CompareOrdinal(this.Gender, Properties.Settings.Default.MaleGenderKey) == 0;
            }
        }

        public bool IsFemale
        {
            get
            {
                return string.CompareOrdinal(this.Gender, Properties.Settings.Default.FemaleGenderKey) == 0;
            }
        }
    }
}

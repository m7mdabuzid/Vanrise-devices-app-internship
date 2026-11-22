using System.ComponentModel.DataAnnotations;

namespace Devices_tasks1_2_3.Models
{
    public class Client : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage =
                "Client name must contain between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public ClientType Type { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool HasActiveReservation { get; set; }

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (!Enum.IsDefined(typeof(ClientType), Type))
            {
                yield return new ValidationResult(
                    "Client type must be Individual or Organization.",
                    new[] { nameof(Type) });

                yield break;
            }

            if (Type != ClientType.Individual)
            {
                yield break;
            }

            if (!BirthDate.HasValue)
            {
                yield return new ValidationResult(
                    "Birth date is required for an Individual client.",
                    new[] { nameof(BirthDate) });

                yield break;
            }

            DateTime eighteenYearsAgo =
                DateTime.Today.AddYears(-18);

            if (BirthDate.Value.Date >= eighteenYearsAgo)
            {
                yield return new ValidationResult(
                    "The client must be older than 18 years.",
                    new[] { nameof(BirthDate) });
            }
        }
    }
}
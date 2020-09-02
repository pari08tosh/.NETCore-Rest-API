using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MoviesAPI.Validations
{
    public class FileSizeValidator : ValidationAttribute
    {
        private readonly int _maxFileSizeInMB;

        public FileSizeValidator(int maxFileSizeInMB)
        {
            _maxFileSizeInMB = maxFileSizeInMB;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (formFile.Length > _maxFileSizeInMB * 1024 * 1024)
            {
                return new ValidationResult($"File size cannot be greater than {_maxFileSizeInMB}MB");
            }

            return ValidationResult.Success;
        }
    }

}
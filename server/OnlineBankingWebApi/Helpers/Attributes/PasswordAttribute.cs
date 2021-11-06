using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static OnlineBankingWebApi.Helpers.Constants.ErrorConstants;

namespace OnlineBankingWebApi.Helpers.Attributes
{
	public class PasswordAttribute : ValidationAttribute
	{
		public int MinLength { get; set; } = 0;

		public bool MustHaveUpper { get; set; }

		public bool MustHaveLower { get; set; }

		public bool MustHaveDigit { get; set; }

		public bool MustHaveSpecial { get; set; }

		public char[] AllowedSpecialCharacters { get; set; }

		public PasswordAttribute(int minLength = 0,
			bool mustHaveUpper = false,
			bool mustHaveLower = false, 
			bool mustHaveDigit = false,
			bool mustHaveSpecial = false, 
			char[] allowedSpecialCharacters = null)
		{
			MinLength = minLength;
			MustHaveUpper = mustHaveUpper;
			MustHaveLower = mustHaveLower;
			MustHaveDigit = mustHaveDigit;
			MustHaveSpecial = mustHaveSpecial;

			if (allowedSpecialCharacters is null)
			{
				AllowedSpecialCharacters = new[] { '!', '?', ':', ',', '.' };
			}
			else {
				AllowedSpecialCharacters = allowedSpecialCharacters;
			}


		}



		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			try
			{
				var password = value.ToString();

				if (password.Length < MinLength)
				{
					return new ValidationResult(PasswordErrors.PasswordTooSmallError);
				}


				var isFormatValid = !MustHaveUpper || password.Any(char.IsUpper);
				if (!isFormatValid)
				{
					return new ValidationResult(PasswordErrors.PasswordFormatNotCorrectError);
				}

				isFormatValid = !MustHaveLower || password.Any(char.IsLower);
				if (!isFormatValid)
				{
					return new ValidationResult(PasswordErrors.PasswordFormatNotCorrectError);
				}

				isFormatValid = !MustHaveDigit || password.Any(char.IsDigit);
				if (!isFormatValid)
				{
					return new ValidationResult(PasswordErrors.PasswordFormatNotCorrectError);
				}

				bool isSpecialSign(char c) { return AllowedSpecialCharacters.Contains(c); }
				isFormatValid = !MustHaveSpecial || password.Any(isSpecialSign);
				if (!isFormatValid)
				{
					return new ValidationResult(PasswordErrors.PasswordFormatNotCorrectError);
				}

				bool isInvalidSign(char c) { return !char.IsLetter(c) && !char.IsDigit(c) && !AllowedSpecialCharacters.Contains(c); };
				isFormatValid = !password.Any(isInvalidSign);
				if (!isFormatValid)
				{
					return new ValidationResult(PasswordErrors.PasswordFormatNotCorrectError);
				}
			}
			catch (Exception) {
				return new ValidationResult(PasswordErrors.PasswordNotInValidFormat);
			}

			return ValidationResult.Success;

		}
	}
}

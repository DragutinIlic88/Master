using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using  OnlineBankingWebApi.Helpers.Constants;

namespace OnlineBankingWebApi.Models
{
	public class VerificationData : IValidatableObject
	{
		[Required(ErrorMessage = nameof(ErrorConstants.VerificationType.VerificationTypeRequiredError))]
		public string VerificationType { get; set; }


		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var verificationData = (VerificationData)validationContext.ObjectInstance;
			if (verificationData.VerificationType.ToUpper() != "OTP" && verificationData.VerificationType.ToUpper() != "EMAIL") {
				yield return new ValidationResult(ErrorConstants.VerificationType.VerificationTypeValueError); 
			}
		}
	}
}

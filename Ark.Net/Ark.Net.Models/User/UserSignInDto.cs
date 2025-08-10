using System;

namespace Ark.Net.Models
{
    /// <summary>
    /// DTO used when a user signs in the portal.
    /// </summary>
    public class UserSignInDto
    {
        /// <summary>
        /// Primary email used for authentication.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional secondary contact email.
        /// </summary>
        public string SecondaryEmail { get; set; }

        /// <summary>
        /// Email used for account recovery.
        /// </summary>
        public string RecoveryEmail { get; set; }

        /// <summary>
        /// Country of residence.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Mailing address of the user.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// User's gender.
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// Courtesy title such as Mr or Ms.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Date of birth.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Nationality of the user.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// GPS location string captured at sign in.
        /// </summary>
        public string GpsLocation { get; set; }

        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Expiration date of the password, if any.
        /// </summary>
        public DateTime? PasswordExpiration { get; set; }

        /// <summary>
        /// Indicates whether the account is verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Additional metadata in JSON format.
        /// </summary>
        public string MetadataJson { get; set; }
    }
}

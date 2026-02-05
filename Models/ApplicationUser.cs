using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EcommerceWebsite.Models
{
    /// <summary>
    /// Custom user model extending ASP.NET Core Identity
    /// Adds marketplace-specific properties
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(500)]
        public string ProfileImageUrl { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        // Verification status
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsDocumentVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }

        // Account metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSuspended { get; set; }
        public string SuspensionReason { get; set; }

        // Address information
        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string AddressLine2 { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Province { get; set; }

        [StringLength(20)]
        public string PostalCode { get; set; }

        [StringLength(100)]
        public string Country { get; set; } = "South Africa";

        // Seller-specific
        public bool IsSeller { get; set; }
        public decimal SellerRating { get; set; }
        public int TotalSales { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";

        public bool IsFullyVerified => IsEmailVerified && IsDocumentVerified;

        public VerificationStatus VerificationStatus
        {
            get
            {
                if (IsFullyVerified) return VerificationStatus.Verified;
                if (IsEmailVerified) return VerificationStatus.PartiallyVerified;
                return VerificationStatus.Unverified;
            }
        }

        // Navigation properties
        public ICollection<UserDocument> Documents { get; set; }
        public ICollection<Listing> Listings { get; set; }
        public ICollection<Order> OrdersAsBuyer { get; set; }
        public ICollection<Order> OrdersAsSeller { get; set; }
    }

    /// <summary>
    /// User verification status
    /// </summary>
    public enum VerificationStatus
    {
        Unverified = 0,
        PartiallyVerified = 1,
        Verified = 2
    }
}

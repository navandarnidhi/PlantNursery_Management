using System.ComponentModel.DataAnnotations;

namespace PlantNurseryManagement.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot be longer than 500 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "PIN Code is required")]
        [StringLength(10, ErrorMessage = "PIN Code cannot be longer than 10 characters")]
        [Display(Name = "PIN Code")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select user type")]
        [Display(Name = "Register as")]
        public string UserType { get; set; } = "User"; // Default to User
    }
} 
namespace backend.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a user.
    /// Used to return user information from APIs or services.
    /// </summary>
    public class GetUserDto
    {
        /// <summary>
        /// Unique identifier of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the user.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;
    }
}


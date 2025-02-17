namespace Business.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? FaceBase64 { get; set; }
        public List<AccountDto> Accounts { get; set; } = [];
    }
}
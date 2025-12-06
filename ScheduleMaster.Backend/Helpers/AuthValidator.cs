using System.Text.RegularExpressions;
using ScheduleMaster.DTOs;

namespace ScheduleMaster.Helpers
{

    public static class AuthValidator
    {
        private static readonly string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public static void ValidateRegisterDTO(RegisterRequestDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (!IsValidEmail(dto.Email))
                throw new Exception("Некорректный Email");

            if (!IsValidPassword(dto.Password))
                throw new Exception("Пароль должен содержать минимум 8 символов, хотя бы одну букву и цифру, разрешённые спецсимволы");
        }


        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;
            if (!Regex.IsMatch(password, @"[A-Za-z]"))
                return false;
            if (!Regex.IsMatch(password, @"\d"))
                return false;
            if (Regex.IsMatch(password, @"[А-Яа-я\s]"))
                return false;
            if (Regex.IsMatch(password, @"[^A-Za-z0-9!@#]")) // только допустимые символы
                return false;
            return true;
        }
    }
}
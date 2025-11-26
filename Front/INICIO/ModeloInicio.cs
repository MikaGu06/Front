using System;
using System.Collections.Generic;
using System.Text;

namespace Front.INICIO
{
    internal class ModeloInicio
    {
        private string username;
        private string password;
        private string confirmPassword;

        public string Username
        {
            get { return username; } 
            set { username = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { confirmPassword = value; }
        }
        public ModeloInicio (string pUsername, string pPassword, string pConfirmPassword)
        {
            username = pUsername;
            password = pPassword;
            confirmPassword = pConfirmPassword;
        }
    }
}

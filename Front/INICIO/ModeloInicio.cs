using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Front.Data__bd_;
using Front.Helpers;

namespace Front.MODELO
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

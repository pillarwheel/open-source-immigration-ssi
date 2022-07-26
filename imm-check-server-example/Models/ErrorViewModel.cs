using System;

namespace imm_check_server_example.Models
{
    public class ErrorViewModel
    {
        public string idnumber { get; set; }

        public bool ShowIdNumber => !string.IsNullOrEmpty(idnumber);
    }
}

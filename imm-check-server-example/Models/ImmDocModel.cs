namespace imm_check_server_example.Models {
    public class ImmDocCollection {
        public List<ImmDocI20>? immDocI20 { get; set; }
        public List<ImmDocDS2019info>? immDocDS2019info { get; set; }
        public List<ImmDocI94>? immDocI94 { get; set; }
        public List<ImmDocPassport>? immDocPassport { get; set; }
        public List<ImmDocVisaInfo>? immDocVisaInfo { get; set; }
        public List<SponsoredStudentInformation>? sponsoredStudentInformation { get; set; }

    }
}
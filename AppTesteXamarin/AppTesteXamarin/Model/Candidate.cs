using AppTesteXamarin.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppTesteXamarin.Model
{
    public class Candidate
    {
        public Guid candidateId { get; set; }
        public String fullName { get; set; }
        public string profilePicture { get; set; }
        public EStatus status { get; set; }
        public List<CandidateExperience> experience { get; set; }

        public Candidate()
        {
            this.status = EStatus.Undefined;
        }
    }
}

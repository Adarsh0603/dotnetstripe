using System.ComponentModel.DataAnnotations;

namespace dotnetstripe.Models
{

    public class Plan
    {
        [Key]
        public int Id { get; set; }

        public string PlanName { get; set; } = String.Empty;
        public float Price { get; set; }
        public int Space { get; set; }
        public int Domains { get; set; }
        public int Emails { get; set; }
        public bool LiveSupport { get; set; }

    }

}
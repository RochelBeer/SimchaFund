using hmwk50.data;

namespace hmwk50.web.Models
{
    public class ViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public List<Simcha> Simchos { get; set; }
        public List<Deposit> Actions { get; set; }
        public string Message { get; set; }
        public string ContributorName { get; set; }
        public decimal Balance { get; set; }
        public string SimchaName { get; set; }
        public int ContributorTotal { get; set; }
        public int ContributorCount { get; set; }
        public decimal GrandTotal { get; set; }
        public int SimchaId { get; set; }
    }
}
